// gist by Roystan (IronWarrior): https://gist.github.com/IronWarrior/005f649e443bf51b656729231d0b8af4
//
// CONTRIBUTIONS:
// Mac and Linux support added by Creature Coding: https://creaturecoding.com/
//
// PURPOSE:
// Unity does not permit a project to be open in two different editor instances.
// This can be frustrating when building projects with multiplayer networking,
// as it would require you to create a build every time you wish you test your netcode.
// A workaround would be to duplicate the project, but this would require the duplicated
// project to be updated each time a change is made.
//
// Instead of duplicating the project, we create a new directory and populate it with
// junction links to our existing project's Assets, ProjectSettings and Packages directories.
// https://docs.microsoft.com/en-us/windows/win32/fileio/hard-links-and-junctions#junctions
// The contents of these junctions are identical, but Unity will treat the directory containing
// them as a new project, allowing us to have multiple instances of our project open.
//
//
// TO USE THIS SCRIPT: 
// Place it in your Unity project in a directory called "Editor". A new menu will appear
// on the top bar called "Unity Project Junction". Click it and Create Unity Junction Project.
// You can now open this "new" project in a separate Unity Editor instance.

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

public static class UnityProjectJunctionTool
{
	[MenuItem("Unity Project Junction/Create Junction Unity Project")]
	private static void CreateJunctionUnityProject()
	{
		DirectoryInfo projectDirectoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

		string junctionDirectory = $"{projectDirectoryInfo.Name}-Junction";
		string junctionPath = Path.Combine(projectDirectoryInfo.Parent.FullName, junctionDirectory);

		if (Directory.Exists(junctionPath))
		{
			if (EditorUtility.DisplayDialog("Junction already exists",
				$"Junction project directory already exists at {junctionPath}.\n\nWould you like to create another junction project?",
				"Cancel", "Yes, create another"))
			{
				return;
			}
			else
			{
				int junctionIndex = 1;

				do
				{
					junctionDirectory = $"{projectDirectoryInfo.Name}-Junction-{junctionIndex}";
					junctionPath = Path.Combine(projectDirectoryInfo.Parent.FullName, junctionDirectory);

					junctionIndex++;
				}
				while (Directory.Exists(junctionPath));
			}
		}

		DirectoryInfo junctionDirectoryInfo = Directory.CreateDirectory(junctionPath);

		string linkAssets = Path.Combine(junctionDirectoryInfo.FullName, "Assets");
		string linkProjectSettings = Path.Combine(junctionDirectoryInfo.FullName, "ProjectSettings");
		string linkPackages = Path.Combine(junctionDirectoryInfo.FullName, "Packages");

		string targetAssets = Path.Combine(projectDirectoryInfo.FullName, "Assets");
		string targetProjectSettings = Path.Combine(projectDirectoryInfo.FullName, "ProjectSettings");
		string targetPackages = Path.Combine(projectDirectoryInfo.FullName, "Packages");

		CreateJunction(linkAssets, targetAssets);
		CreateJunction(linkProjectSettings, targetProjectSettings);
		CreateJunction(linkPackages, targetPackages);

		EditorUtility.DisplayDialog("Complete", $"Created junction project at {junctionPath}.\n\nYou may now open this new project in a separate Unity Editor instance.", "Ok");

		EditorUtility.RevealInFinder(junctionDirectoryInfo.FullName);
	}

	private static void CreateJunction(string link, string target)
	{
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			LinkWinOS(link, target);
		}
		else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor)
		{
			LinkMacLinOS(link, target);
		}
	}

	public static void LinkWinOS(string link, string target)
	{
		string command = $"/C mklink /J \"{link}\" \"{target}\"";
		Process.Start("cmd.exe", command);
	}

	public static void LinkMacLinOS(string link, string target)
	{
		string command = $"ln -s \"{target}\" \"{link}\"";

		var proc = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "/bin/bash",
				Arguments = "-c \"" + command.Replace("\"", "\"\"") + "\"",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
			}
		};

		proc.Start();
		proc.WaitForExit();
	}

}