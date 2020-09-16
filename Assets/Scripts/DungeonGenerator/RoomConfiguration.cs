using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZD.Dungeon
{
    [CreateAssetMenu(fileName = "New Room Config", menuName = "Dungeon Generator/Room Configuration")]
    public class RoomConfiguration : ScriptableObject
    {
        public List<Direction> Openings;
    }
}

