using UnityEngine;
using ParsecGaming;

/// <summary>
/// Transforms raw parsec frame data into YUV textures.
/// </summary>
public class ParsecFrameDecoder
{
    public Texture2D Y { get; private set; }
    public Texture2D U { get; private set; }
    public Texture2D V { get; private set; }

    public Vector2 Padding { get; private set; }
    public float AspectRatio { get; private set; }

    // TODO: I don't know for certain, but this is probably not the most efficient way of
    // getting an image to Unity. I'm guessing that ClientPollFrame pulls the texture
    // from GPU memory (where it was initially decoded) into system memory, before it is
    // passed here. Then, Unity places it back into GPU memory.
    public void Decode(Parsec.ParsecFrame frame, System.IntPtr image)
    {
        if (frame.format != Parsec.ParsecColorFormat.FORMAT_I420)
            throw new System.Exception($"Decoder does not currently support {frame.format}. Only {Parsec.ParsecColorFormat.FORMAT_I420} is permitted.");

        // The texture will be built if it does not currently exist.
        // Otherwise, only rebuild the texture if the dimensions have changed.
        if (Y == null || (Y.width != frame.fullWidth || Y.height != frame.fullHeight))
        {
            if (Y != null)
            {
                Object.Destroy(Y);
                Object.Destroy(U);
                Object.Destroy(V);
            }       
            
            Y = new Texture2D((int)frame.fullWidth, (int)frame.fullHeight, TextureFormat.R8, false, false);
            U = new Texture2D((int)frame.fullWidth / 2, (int)frame.fullHeight / 2, TextureFormat.R8, false, false);
            V = new Texture2D((int)frame.fullWidth / 2, (int)frame.fullHeight / 2, TextureFormat.R8, false, false);
        }

        int yBytes = (int)frame.fullWidth * (int)frame.fullHeight;
        int uvBytes = (int)frame.fullWidth / 2 * (int)frame.fullHeight / 2;

        Y.LoadRawTextureData(image, yBytes);
        Y.Apply();

        System.IntPtr uPtr = System.IntPtr.Add(image, yBytes);        
        U.LoadRawTextureData(uPtr, uvBytes);
        U.Apply();

        System.IntPtr vPtr = System.IntPtr.Add(uPtr, uvBytes);        
        V.LoadRawTextureData(vPtr, ((int)frame.fullWidth * (int)frame.fullHeight) / 2);
        V.Apply();

        Padding = new Vector2((float)frame.width / frame.fullWidth, (float)frame.height / frame.fullHeight);
        AspectRatio = (float)frame.width / (float)frame.height;
    }
}
