using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] float roomSize = 25f;
    [SerializeField] int paintingsPerRoom = 8;  
    
    [SerializeField] int seed = 1337;

    [SerializeField] int viewDistance = 1;

    [SerializeField] PaintingUI paintingUI;

    [SerializeField] Transform playerTransform;


    Queue<GameObject> roomPool = new();
    Dictionary<Vector2Int, GameObject> activeRooms = new();
    Vector2Int lastPlayerCell;
    private void Start()
    {
        int poolSize = (viewDistance * 2 + 1) * (viewDistance * 2 + 1) + (viewDistance * 2 + 1);
        for (int i = 0; i < poolSize; i++)
        {
            GameObject roomObj = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
            roomObj.SetActive(false);
            roomPool.Enqueue(roomObj);
        }

        lastPlayerCell = GetCell(playerTransform.position);
        UpdateRooms();
    }

    Vector2Int GetCell(Vector3 position)
    {
        float half = roomSize * 0.5f;
        int cx = Mathf.FloorToInt((position.x + half) / roomSize);
        int cy = Mathf.FloorToInt((position.z + half) / roomSize);
        return new Vector2Int(cx, cy);
    }

    private void Update()
    {
        Vector2Int currentCell = GetCell(playerTransform.position);
        if (currentCell != lastPlayerCell)
        {
            lastPlayerCell = currentCell;
            UpdateRooms();
        }
    }
    void UpdateRooms()
    {
        Vector2Int center = lastPlayerCell;
        HashSet<Vector2Int> neededRooms = new HashSet<Vector2Int>();

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int coord = new Vector2Int(center.x + x, center.y + y);
                neededRooms.Add(coord);

                if (activeRooms.ContainsKey(coord))
                    continue;

                if (roomPool.Count == 0) Debug.LogWarning("Room pool exhausted!");

                GameObject roomObj = roomPool.Dequeue();

                Vector3 roomPosition = new Vector3(coord.x * roomSize, 0, coord.y * roomSize);
                roomObj.transform.position = roomPosition;
                roomObj.SetActive(true);

                Room room = roomObj.GetComponent<Room>();
                room.InitializePaintingUI(paintingUI);
                room.SetPaintingSeeds(GeneratePaintingSeeds(coord));

                activeRooms.Add(coord, roomObj);
            }
        }

        List<Vector2Int> roomsToRemove = new List<Vector2Int>();

        foreach(var kvp in activeRooms)
        {
            if(!neededRooms.Contains(kvp.Key))
            {
                roomsToRemove.Add(kvp.Key);
                kvp.Value.SetActive(false);
                roomPool.Enqueue(kvp.Value);
            }
        }
        
        foreach(var c in roomsToRemove)
        {
            activeRooms.Remove(c);
        }
    }

    
    int[] GeneratePaintingSeeds(Vector2Int coord)
    {
        System.Random rng = new System.Random(HashCoords(coord.x, coord.y, seed));
        int[] seeds = new int[paintingsPerRoom];

        for (int i = 0; i < paintingsPerRoom; i++)
        {
            seeds[i] = rng.Next(0, int.MaxValue);
        }

        return seeds;
    }

    int HashCoords(int x, int y, int seed)
    {
        unchecked
        {
            int h = 17;
            h = h * 31 + x;
            h = h * 31 + y;
            h = h * 31 + seed;
            return h;
        }
    }
}
