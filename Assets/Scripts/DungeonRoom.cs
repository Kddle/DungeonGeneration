using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DungeonRoom : MonoBehaviour
{
    // public Vector2Int Dimension;
    // public List<Vector2Int> DoorCoordinates;
    public Vector2Int coordinates;
    public List<Direction> SpawnPoints;

    public static List<Vector2Int> processedSpawnPoints = new List<Vector2Int>();

    public void Initialize(Vector2Int coordinates)
    {
        this.coordinates = coordinates;
    }

    // public void Generate(Vector2Int coordinates)
    // {
    //     this.coordinates = coordinates;

    //     foreach (var spawnPoint in SpawnPoints)
    //     {
    //         var spawnPointGridPosition = this.coordinates + spawnPoint.ToVector2Int();

    //         if (Dungeon.ContainsCoordinates(spawnPointGridPosition) && !processedSpawnPoints.Contains(spawnPointGridPosition))
    //         {
    //             processedSpawnPoints.Add(spawnPointGridPosition);


    //         }
    //     }
    // }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
