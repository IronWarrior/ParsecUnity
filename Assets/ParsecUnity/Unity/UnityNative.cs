using System;
using System.Runtime.InteropServices;

public class UnityNative
{
	[DllImport("parsec")]
	public static extern IntPtr UnityGetRenderEventFunction();

	[DllImport("parsec", CallingConvention = CallingConvention.Cdecl)]
	public static extern void UnitySubmitFrame(IntPtr parsec, IntPtr source);
}
