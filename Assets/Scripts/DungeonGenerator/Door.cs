using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZD.Dungeon
{
    public class Door : MonoBehaviour
    {
        public Room room1;
        public Room room2;

        public Vector2 coordinates;

        public bool isOpen = false;

        public static Vector2 GetCoordinatesFromRoomOpening(Vector2Int roomCoordinates, Direction opening)
        {
            var movementVector = new Vector2(opening.ToVector2Int().x, opening.ToVector2Int().y) / 2f;
            return roomCoordinates + movementVector;
        }


        public void Toggle()
        {
            isOpen = !isOpen;
            UpdateDoorStatus(isOpen);
        }

        public void Open()
        {
            isOpen = true;
            UpdateDoorStatus(isOpen);
        }

        public void Close()
        {
            isOpen = false;
            UpdateDoorStatus(isOpen);
        }

        private void UpdateDoorStatus(bool isOpen)
        {
            if (isOpen)
                transform.localPosition = new Vector3(transform.localPosition.x, 2f, transform.localPosition.z);
            else
                transform.localPosition = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
        }
    }
}

