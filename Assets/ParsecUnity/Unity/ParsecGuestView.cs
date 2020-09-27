using UnityEngine;

public class ParsecGuestView : MonoBehaviour
{
    [SerializeField]
    Renderer frame;

    private Material material;

    private void Awake()
    {
        material = new Material(Shader.Find("Parsec/YUV to RGB"));
        frame.material = material;
    }

    public void Populate(Texture Y, Texture U, Texture V, Vector2 padding, float aspectRatio)
    {
        material.SetTexture("_Y", Y);
        material.SetTexture("_U", U);
        material.SetTexture("_V", V);
        material.SetVector("_Padding", padding);

        frame.transform.localScale = new Vector3(aspectRatio, 1, 1);
    }
}
