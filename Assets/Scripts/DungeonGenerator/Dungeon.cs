using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZD.Dungeon
{
    public class Dungeon : MonoBehaviour
    {
        public static Vector2Int Size = new Vector2Int(40, 40);

        public List<Room> roomPrefabs = new List<Room>();
        public Door doorPrefab;

        private List<Room> spawnedRooms;
        private List<Door> spawnedDoors;

        public void Generate()
        {
            spawnedDoors = new List<Door>();
            spawnedRooms = new List<Room>();

            var roomSpawnPoints = new Queue<RoomSpawnPoint>();

            CreateSpawnRoom(spawnedRooms, roomSpawnPoints);

            int iterations = 0;

            while (roomSpawnPoints.Count > 0 && iterations < 100)
            {
                ProcessSpawnPoints(roomSpawnPoints, spawnedRooms);
                iterations++;
            }

            PlaceBossRoom();

            PlaceDoors();
        }

        public GameObject SpawnPlayer(GameObject playerPrefab)
        {
            var spawnRoom = spawnedRooms.FirstOrDefault(x => x.tag == "SpawnRoom");

            if (spawnRoom != null)
            {
                var playerSpawnPoint = spawnRoom.transform.Find("PlayerSpawnPoint");

                if (playerSpawnPoint != null)
                {
                    var player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
                    return player;
                }
                else
                    throw new System.InvalidOperationException("PlayerSpawnPoint not found in SpawnRoom prefab");
            }
            else
                throw new System.InvalidOperationException("SpawnRoom not found in spawned rooms");
        }

        private void PlaceDoors()
        {
            foreach (var room in spawnedRooms)
            {
                foreach (var opening in room.configuration.Openings)
                {
                    var doorCoordinates = Door.GetCoordinatesFromRoomOpening(room.coordinates, opening);

                    if (spawnedDoors.FirstOrDefault(x => x.coordinates == doorCoordinates) == null)
                    {
                        var door = Instantiate(doorPrefab) as Door;
                        door.coordinates = doorCoordinates;
                        door.transform.SetParent(transform);
                        door.transform.localPosition = new Vector3(doorCoordinates.x * Room.Size.x, 0f, doorCoordinates.y * Room.Size.y);

                        if (opening == Direction.E || opening == Direction.W)
                            door.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);

                        room.doors.Add(door);
                        door.room1 = room;

                        var roomNeighbor = GetRoom(room.coordinates + opening.ToVector2Int());

                        if (roomNeighbor != null)
                        {
                            roomNeighbor.doors.Add(door);
                            door.room2 = roomNeighbor;
                        }

                        spawnedDoors.Add(door);
                    }
                }
            }
        }

        private void ProcessSpawnPoints(Queue<RoomSpawnPoint> roomSpawnPoints, List<Room> rooms)
        {
            var currentSpawnPoint = roomSpawnPoints.Dequeue();

            if (rooms.FirstOrDefault(x => x.coordinates == currentSpawnPoint.coordinates) == null)
            {
                var newRoom = CreateRoom(currentSpawnPoint, rooms);
                AddRoomSpawnPoints(roomSpawnPoints, newRoom);
                rooms.Add(newRoom);
            }
        }

        private Room CreateRoom(RoomSpawnPoint currentSpawnPoint, List<Room> rooms, Room prefab = null)
        {
            Room roomPrefab;

            if (prefab == null)
                roomPrefab = FindValidRoomPrefab(currentSpawnPoint, rooms);
            else
                roomPrefab = prefab;

            var room = Instantiate(roomPrefab) as Room;
            room.Initialize(currentSpawnPoint.coordinates, this);

            return room;
        }

        public Room GetRoomFromWorldPosition(Vector3 position)
        {
            var coordinates = new Vector2Int(Mathf.RoundToInt(position.x / Room.Size.x), Mathf.RoundToInt(position.z / Room.Size.y));

            return GetRoom(coordinates);
        }

        private Room FindValidRoomPrefab(RoomSpawnPoint currentSpawnPoint, List<Room> rooms)
        {
            var firstTurnCandidates = roomPrefabs.Where(x => x.configuration.Openings.Contains(currentSpawnPoint.direction) && x.tag != "SpawnRoom" && x.tag != "BossRoom");

            var secondTurnCandidates = new List<Room>();

            foreach (var validPrefab in firstTurnCandidates)
            {
                var isValid = true;

                foreach (var opening in validPrefab.configuration.Openings)
                {
                    var neighbor = GetRoom(currentSpawnPoint.coordinates + opening.ToVector2Int());

                    if (neighbor != null && !neighbor.configuration.Openings.Contains(opening.GetOpposite()))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                    secondTurnCandidates.Add(validPrefab);
            }

            var finalCandidate = secondTurnCandidates[UnityEngine.Random.Range(0, secondTurnCandidates.Count)];

            return finalCandidate;
        }

        public Room GetRoom(Vector2Int coordinates)
        {
            return spawnedRooms.FirstOrDefault(x => x.coordinates == coordinates);
        }

        public void PlaceBossRoom()
        {
            var bossRoomPrefab = roomPrefabs.FirstOrDefault(x => x.tag == "BossRoom");
            var bossRoomSpot = spawnedRooms.Last(x => x.configuration.Openings.Count == 1);

            var roomDirection = bossRoomSpot.configuration.Openings.First();
            var bossRoom = CreateRoom(new RoomSpawnPoint() { coordinates = bossRoomSpot.coordinates, direction = roomDirection }, spawnedRooms, bossRoomPrefab);
            bossRoom.transform.Find("Model").localRotation = roomDirection.ToRotation();
            bossRoom.configuration = new RoomConfiguration() { Openings = new List<Direction>() { roomDirection } };

            spawnedRooms.Remove(bossRoomSpot);
            Destroy(bossRoomSpot.gameObject);

            spawnedRooms.Add(bossRoom);
        }

        private void CreateSpawnRoom(List<Room> rooms, Queue<RoomSpawnPoint> spawnPoints)
        {
            var spawnRoomPrefab = roomPrefabs.FirstOrDefault(x => x.tag == "SpawnRoom");

            if (spawnRoomPrefab == null)
                throw new System.InvalidOperationException("Couldn't find SpawnRoom in RoomPrefabs");

            var spawnRoomCoordinates = new Vector2Int(Size.x / 2, Size.y / 2);

            var spawnRoom = Instantiate(spawnRoomPrefab) as Room;
            spawnRoom.Initialize(spawnRoomCoordinates, this);

            AddRoomSpawnPoints(spawnPoints, spawnRoom);
            rooms.Add(spawnRoom);
        }

        private void AddRoomSpawnPoints(Queue<RoomSpawnPoint> spawnPoints, Room room)
        {
            foreach (var direction in room.configuration.Openings)
            {
                var candidateRoomCoordinates = room.coordinates + direction.ToVector2Int();

                if (spawnPoints.FirstOrDefault(x => x.coordinates == candidateRoomCoordinates) == null)
                {
                    var newSpawnPoint = new RoomSpawnPoint() { coordinates = candidateRoomCoordinates, direction = direction.GetOpposite() };
                    spawnPoints.Enqueue(newSpawnPoint);
                }
            }
        }
    }
}

