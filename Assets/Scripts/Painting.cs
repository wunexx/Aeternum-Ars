using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class Painting : MonoBehaviour, IInteractable
{
    [Header("Painting Settings")]
    [SerializeField] int width = 600;
    [SerializeField] int height = 250;

    [Header("References")]
    [SerializeField] Image paintingImage;

    int paintingSeed = 0;

    PaintingUI paintingUI;

    Sprite currentSprite;
    Texture2D currentTexture;
    public void SetSeed(int seed)
    {
        paintingSeed = seed;
    }
    private void OnDestroy()
    {
        if (currentTexture != null)
            Destroy(currentTexture);
    }

    void Start()
    {
        currentTexture = GenerateTexture(paintingSeed);

        currentSprite = ConvertTextureToSprite(currentTexture);

        ApplySprite(currentSprite);
    }

    void ApplySprite(Sprite sprite)
    {
        paintingImage.sprite = sprite;
    }
    Sprite ConvertTextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    Texture2D GenerateTexture(int seed)
    {
        System.Random rng = new System.Random(seed);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float r = (float)rng.NextDouble();
                float g = (float)rng.NextDouble();
                float b = (float)rng.NextDouble();
                texture.SetPixel(x, y, new Color(r, g, b));
            }
        }

        texture.Apply();
        return texture;
    }


    public void Interact()
    {
        paintingUI.OpenPanel(currentSprite, currentTexture);
    }

    public void InitializePaintingUI(PaintingUI ui)
    {
        paintingUI = ui;
    }
}
