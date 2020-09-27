using UnityEngine;

public class ParsecGuestView : MonoBehaviour
{
    private Material material;

    private void Awake()
    {
        material = new Material(Shader.Find("Parsec/YUV to RGB"));

        GetComponentInChildren<Renderer>().material = material;
    }

    public void Populate(Texture Y, Texture U, Texture V, Vector2 padding)
    {
        material.SetTexture("_Y", Y);
        material.SetTexture("_U", U);
        material.SetTexture("_V", V);
        material.SetVector("_Padding", padding);
    }
}
