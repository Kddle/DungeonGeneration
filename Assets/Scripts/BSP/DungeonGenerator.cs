using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator
{

    List<RoomNode> allSpaceNodes = new List<RoomNode>();
    private Vector2Int dungeonSize;

    public DungeonGenerator(UnityEngine.Vector2Int size)
    {
        dungeonSize = size;
    }

    public List<Node> CalculateDungeon(int maxIterations, Vector2Int roomSizeMin, float roomBottomCornerModifer, float roomTopCornerModifier, int roomOffset, int corridorWidth)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonSize);
        allSpaceNodes = bsp.PrepareNodesCollection(maxIterations, roomSizeMin);

        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(bsp.RootNode);
        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomSizeMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomsInGivenSpaces(roomSpaces, roomBottomCornerModifer, roomTopCornerModifier, roomOffset);

        CorridorsGenerator corridorsGenerator = new CorridorsGenerator();
        var corridorList = corridorsGenerator.CreateCorridor(allSpaceNodes, corridorWidth);

        return new List<Node>(roomList).Concat(corridorList).ToList();
    }
}