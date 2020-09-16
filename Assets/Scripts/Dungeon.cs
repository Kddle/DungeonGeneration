using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction
{
    N, S, E, W
}

public static class Directions
{
    public static Vector2Int[] vectors = {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0)
    };

    public static Direction[] opposites = {
        Direction.S,
        Direction.N,
        Direction.W,
        Direction.E
    };

    public static Quaternion[] rotations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 180f, 0f),
        Quaternion.Euler(0, 90f, 0f),
        Quaternion.Euler(0, -90f, 0f)
    };

    public static Vector2Int ToVector2Int(this Direction direction)
    {
        return vectors[(int)direction];
    }

    public static Direction GetOpposite(this Direction direction)
    {
        return opposites[(int)direction];
    }

    public static Quaternion ToRotation(this Direction direction)
    {
        return rotations[(int)direction];
    }
}

namespace ZD.FirstMethod
{
    public class Dungeon : MonoBehaviour
    {
        public static Random Random = new Random();
        public Vector2 RoomDimension;

        public DungeonRoom SingleDoorRoomPrefab;
        public DungeonRoom SpawnRoomPrefab;

        public List<DungeonRoom> bossRoomPrefabs = new List<DungeonRoom>();


        public Vector2Int dimension = new Vector2Int(20, 20);
        private List<DungeonRoom> spawnedRooms;

        public bool ContainsCoordinates(Vector2Int coordinates)
        {
            return (coordinates.x >= 0 && coordinates.x < dimension.x && coordinates.y >= 0 && coordinates.y < dimension.y);
        }

        public List<DungeonRoom> TopRooms = new List<DungeonRoom>();
        public List<DungeonRoom> BottomRooms = new List<DungeonRoom>();

        public List<DungeonRoom> LeftRooms = new List<DungeonRoom>();

        public List<DungeonRoom> RightRooms = new List<DungeonRoom>();


        public void Regenerate()
        {
            for (int i = spawnedRooms.Count - 1; i >= 0; i--)
            {
                Destroy(spawnedRooms[i].gameObject);
            }

            Generate();
        }

        public DungeonRoom CreateRoom(DungeonRoom roomPrefab, Vector2Int coordinates)
        {
            var room = Instantiate(roomPrefab) as DungeonRoom;
            room.Initialize(coordinates);
            room.transform.SetParent(transform);
            room.transform.position = new Vector3(coordinates.x * RoomDimension.x, 0f, coordinates.y * RoomDimension.y);

            spawnedRooms.Add(room);
            return room;
        }

        public DungeonRoom CreateRoom(Vector2Int coordinates, Direction direction)
        {
            var realDirection = direction.GetOpposite();

            DungeonRoom roomPrefab = null;

            switch (realDirection)
            {
                case Direction.N:
                    roomPrefab = TopRooms[Random.Range(0, TopRooms.Count)];
                    break;
                case Direction.S:
                    roomPrefab = BottomRooms[Random.Range(0, BottomRooms.Count)];
                    break;
                case Direction.E:
                    roomPrefab = RightRooms[Random.Range(0, RightRooms.Count)];
                    break;
                case Direction.W:
                    roomPrefab = LeftRooms[Random.Range(0, LeftRooms.Count)];
                    break;
            }

            var createdRoom = CreateRoom(roomPrefab, coordinates);
            return createdRoom;
        }

        public void ProcessRoom(DungeonRoom room)
        {
            foreach (var spawnPoint in room.SpawnPoints)
            {
                var spGridPosition = room.coordinates + spawnPoint.ToVector2Int();

                if (ContainsCoordinates(spGridPosition))
                {
                    var spWorldPosition = new Vector3(spGridPosition.x * RoomDimension.x, 0f, spGridPosition.y * RoomDimension.y);

                    if (spawnedRooms.FirstOrDefault(x => x.transform.position == spWorldPosition) == null)
                    {
                        var newRoom = CreateRoom(spGridPosition, spawnPoint);
                        ProcessRoom(newRoom);
                    }
                }
            }

        }

        public void Generate()
        {
            spawnedRooms = new List<DungeonRoom>();
            var spawnRoomPosition = new Vector2Int(dimension.x / 2, dimension.y / 2);

            var spawnRoom = CreateRoom(SpawnRoomPrefab, spawnRoomPosition);

            foreach (var spawnPoint in spawnRoom.SpawnPoints)
            {
                var spawnPointGridPosition = spawnRoom.coordinates + spawnPoint.ToVector2Int();

                if (ContainsCoordinates(spawnPointGridPosition))
                {
                    var spawnPointWorldPosition = new Vector3(spawnPointGridPosition.x * RoomDimension.x, 0f, spawnPointGridPosition.y * RoomDimension.y);

                    if (spawnedRooms.FirstOrDefault(x => x.transform.position == spawnPointWorldPosition) == null)
                    {
                        // Spawn Point is valid
                        var newRoom = CreateRoom(spawnPointGridPosition, spawnPoint);
                        ProcessRoom(newRoom);
                    }
                }
            }

            var potentialBossRooms = spawnedRooms.Where(x => x.SpawnPoints.Count == 1).ToList();

            if (potentialBossRooms != null && potentialBossRooms.Count > 0)
            {
                var maxDistance = 0f;
                DungeonRoom farestRoom = null;

                foreach (var room in potentialBossRooms)
                {
                    var distance = Vector3.Distance(spawnedRooms[0].transform.position, room.transform.position);

                    if (distance > maxDistance)
                        farestRoom = room;
                }

                var openingDirection = farestRoom.SpawnPoints.First();
                var spawnPosition = farestRoom.transform.localPosition;
                var gridPosition = farestRoom.coordinates;

                spawnedRooms.Remove(farestRoom);
                Destroy(farestRoom.gameObject);

                var bossRoomPrefab = bossRoomPrefabs[Random.Range(0, bossRoomPrefabs.Count)];
                var bossRoom = Instantiate(bossRoomPrefab) as DungeonRoom;

                bossRoom.transform.SetParent(transform);
                bossRoom.transform.localPosition = spawnPosition;
                bossRoom.transform.localRotation = openingDirection.ToRotation();
                bossRoom.Initialize(gridPosition);

                spawnedRooms.Add(bossRoom);
            }
            else
            {
                throw new System.Exception("Should not happen");
            }



            // var spawnPoints = new Dictionary<Vector2Int, Direction>();

            // var spawnRoom = Instantiate(SpawnRoomPrefab) as DungeonRoom;

            // spawnRoom.Initialize(spawnRoomPosition);
            // spawnRoom.transform.SetParent(transform);
            // spawnRoom.transform.position = new Vector3(spawnRoomPosition.x * RoomDimension.x, 0f, spawnRoomPosition.y * RoomDimension.y);

            // spawnedRoom.Add(spawnRoom);

            // FillSpawnPoints(spawnRoom, ref spawnPoints);

            // while (spawnPoints.Count > 0)
            // {
            //     var newRoom = SpawnRoom(ref spawnPoints);

            //     if (newRoom != null)
            //         FillSpawnPoints(newRoom, ref spawnPoints);

            //     spawnPoints.Remove(spawnPoints.ElementAt(spawnPoints.Count - 1).Key);

            //     if (spawnedRoom.Count > 20)
            //         return;
            // }

        }

        public DungeonRoom SpawnRoom(ref Dictionary<Vector2Int, Direction> spawnPoints)
        {
            var current = spawnPoints.ElementAt(spawnPoints.Count - 1);

            var direction = current.Value.GetOpposite();

            DungeonRoom roomPrefab;

            switch (direction)
            {
                case Direction.N:
                    roomPrefab = TopRooms[Random.Range(0, TopRooms.Count)];
                    break;
                case Direction.S:
                    roomPrefab = BottomRooms[Random.Range(0, BottomRooms.Count)];
                    break;
                case Direction.E:
                    roomPrefab = RightRooms[Random.Range(0, RightRooms.Count)];
                    break;
                case Direction.W:
                    roomPrefab = LeftRooms[Random.Range(0, LeftRooms.Count)];
                    break;
                default: throw new System.Exception("Pas normal");

            }

            if (spawnedRooms.FirstOrDefault(x => x.transform.position == new Vector3(current.Key.x * RoomDimension.x, 0f, current.Key.y * RoomDimension.y)) == null)
            {
                var spawnRoom = Instantiate(roomPrefab) as DungeonRoom;

                spawnRoom.Initialize(current.Key);
                spawnRoom.transform.SetParent(transform);
                spawnRoom.transform.position = new Vector3(current.Key.x * RoomDimension.x, 0f, current.Key.y * RoomDimension.y);

                spawnedRooms.Add(spawnRoom);
                return spawnRoom;
            }

            return null;
        }

        public Dictionary<Vector2Int, Direction> FillSpawnPoints(DungeonRoom room, ref Dictionary<Vector2Int, Direction> spawnPointsPool)
        {
            foreach (var spawnPoint in room.SpawnPoints)
            {
                var positionToTest = room.coordinates + spawnPoint.ToVector2Int();

                if (ContainsCoordinates(positionToTest) && !spawnPointsPool.ContainsKey(positionToTest))
                    spawnPointsPool.Add(positionToTest, spawnPoint);
            }

            return spawnPointsPool;
        }

        // Start is called before the first frame update
        void Start()
        {
            Generate();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Regenerate();
        }
    }

}
