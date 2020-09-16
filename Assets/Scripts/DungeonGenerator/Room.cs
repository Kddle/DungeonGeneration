using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZD.Dungeon
{
    public class Room : MonoBehaviour
    {
        public static Vector2 Size = new Vector2(100f, 50f);
        public RoomConfiguration configuration;
        public bool completed = false;
        public List<Door> doors;

        [SerializeField]
        public Vector2Int coordinates { get; private set; }

        public void Initialize(Vector2Int coordinates, Dungeon dungeon)
        {
            this.coordinates = coordinates;

            transform.SetParent(dungeon.transform);
            transform.localPosition = new Vector3(coordinates.x * Size.x, 0f, coordinates.y * Size.y);
        }
    }
}

