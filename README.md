:warning: **This project is a work-in-progress and is not production ready** :warning:

This repository uses the [Parsec C# SDK](https://github.com/parsec-cloud/parsec-sdk) to allow hosting and joining Parsec sessions within Unity. The Parsec C# SDK provides interoperability with the [native Parsec SDK](https://parsecgaming.com/docs/sdk/). [Parsec](https://parsecgaming.com/) is an interactive video streaming application that is commonly used to play local multiplayer games over a network. This allows a developer to add networked multiplayer to their project without needing to write any netcode. **This repository is not affiliated with the Parsec team.**

![Screenshot of both local and remote views of the demo game](https://i.imgur.com/TBphcwK.png)

[Demo video](https://gfycat.com/specificuncomfortableargentinehornedfrog)

# Getting Started

This repository does not contain the Parsec SDK libraries. You will need to copy the following into this repo from the [Parsec SDK](https://github.com/parsec-cloud/parsec-sdk):
- [parsec.dll](https://github.com/parsec-cloud/parsec-sdk/blob/master/sdk/windows/parsec.dll)
- [parsec32.dll](https://github.com/parsec-cloud/parsec-sdk/blob/master/sdk/windows/parsec32.dll)
- [ParsecUnity46.dll](https://github.com/parsec-cloud/parsec-sdk/blob/master/sdk/ParsecUnity/ParsecUnity46.dll)
- [libparsec.dylib](https://github.com/parsec-cloud/parsec-sdk/blob/master/sdk/macos/libparsec.dylib) (rename it to `parsec.bundle`)

You can also run the setup scripts for the Unity SDK ([described on this page](https://github.com/parsec-cloud/parsec-sdk/tree/master/sdk/ParsecUnity)) to have these libraries set up in the Unity examples included in the Parsec SDK, and then copy them from there into this repository.

Unity does not allow a single project to be open in multiple Editor instances. This can be frustrating when working with networked projects, as it would require a build to be created each time you wish to test both host and client. A workaround to this is to use the [Unity Project Junction tool](https://gist.github.com/IronWarrior/005f649e443bf51b656729231d0b8af4) included with this repository to create a second dummy project.

To start a host and connect a client, you will need a Parsec account (created on their website) and a valid `sessionId` and `peerId` [retrieved using the Parsec API](https://github.com/parsec-cloud/parsec-sdk/tree/master/api/personal) (the examples do come included with a valid `session`/`peerId` at the time of writing, but this may change. Better to request your own).

# Examples

Run the scene [Assets/ParsecUnity/Experiments/ParsecTest](Assets/ParsecUnity/Experiments/ParsecTest.unity) to get started. The status will be displayed on screen if the connection is successful. Tick the **Try Make Texture** toggle on the client to start polling for video frames.

If you have successfully run the above scene, you can try the demo project in [Assets/Demo/Main](Assets/Demo/Main.unity). This uses the Unity Input System to handle input. Note that in the Editor, to allow guest inputs to be correctly read on the host, you'll need to lock input in the host Editor instance (`Window > Analysis > Input Debugger > Options > Lock Input to Game View`).

# TODO

- Unity sometimes crashes when attempting to connect a client. Unknown why.
- Parsec supports [many different color formats](https://parsecgaming.com/docs/sdk/enum/ParsecColorFormat/) for streaming. Currently `FORMAT_I420` is the only supported one in this package (not sure how to force the host to use a specific format to test the others). The color in the guest view is also slightly off right now.
- ~Audio is not currently supported. The [Parsec Unity SDK](https://github.com/parsec-cloud/parsec-sdk/tree/master/sdk/ParsecUnity) does have an example that supports outgoing audio from a single AudioSource. Currently the exception *`MarshalDirectiveException: [MarshalAs] attribute required to marshal arrays to managed code.`* is thrown when calling `Parsec.ClientPollAudio`.~ This is fixed on the `audio` branch. However, it uses an edited version of the ParsecUnity dll, which is not included in this package. This branch will be merged when the fix propogates to the official distribution of the ParsecUnity dll.
- Unity Input System is the only supported input on both the host and client. The Parsec Unity SDK does have input for both the legacy input and Rewired, so adding in more input systems should not be challenging.
- ...and lots more! Search for `// TODO` in the comments.
