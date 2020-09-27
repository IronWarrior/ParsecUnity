using UnityEngine;
using UnityEngine.UI;

public class RGBPicker : MonoBehaviour
{
    [SerializeField]
    Slider r, g, b;

    [SerializeField]
    Image preview;

    public Color Color => new Color(r.value, g.value, b.value, 1);

    private void Awake()
    {
        r.value = Random.value;
        g.value = Random.value;
        b.value = Random.value;

        r.onValueChanged.AddListener((v) => Set());
        g.onValueChanged.AddListener((v) => Set());
        b.onValueChanged.AddListener((v) => Set());

        Set();
    }

    private void Set()
    {
        preview.color = Color;
    }
}
