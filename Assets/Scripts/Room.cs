using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] Painting[] paintings;

    int[] paintingSeeds;

    public void SetPaintingSeeds(int[] seeds)
    {
        if (seeds.Length != paintings.Length)
        {
            Debug.LogError("The number of seeds provided does not match the number of paintings.");
            return;
        }
        paintingSeeds = seeds;
        AssignPaintingSeeds();
    }

    void AssignPaintingSeeds()
    {
        for (int i = 0; i < paintings.Length; i++)
        {
            paintings[i].SetSeed(paintingSeeds[i]);
        }
    }

    public void InitializePaintingUI(PaintingUI paintingUI)
    {
        for(int i = 0; i < paintings.Length; i++)
        {
            paintings[i].InitializePaintingUI(paintingUI);
        }
    }
}
