using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZD.Dungeon
{
    public class DungeonGenerator : MonoBehaviour
    {
        public Dungeon dungeonPrefab;
        private Dungeon dungeonInstance;

        public GameObject DebugGameObject;

        public GameObject playerPrefab;
        private GameObject playerInstance;

        // Start is called before the first frame update
        void Start()
        {
            CreateDungeon();
        }

        public void CreateDungeon()
        {
            if (dungeonInstance != null)
                Destroy(dungeonInstance.gameObject);

            dungeonInstance = Instantiate(dungeonPrefab) as Dungeon;
            dungeonInstance.Generate();

            playerInstance = dungeonInstance.SpawnPlayer(playerPrefab);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Destroy(playerInstance.gameObject);
                CreateDungeon();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                var room = dungeonInstance.GetRoomFromWorldPosition(playerInstance.transform.position);

                if (room != null)
                {
                    foreach (var door in room.doors)
                    {
                        door.Toggle();
                    }
                }
            }
        }
    }
}

