using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{
    private int maxIterations;
    private Vector2Int roomSizeMin;

    public RoomGenerator(int maxIterations, Vector2Int roomSizeMin)
    {
        this.maxIterations = maxIterations;
        this.roomSizeMin = roomSizeMin;
    }

    public List<RoomNode> GenerateRoomsInGivenSpaces(List<Node> roomSpaces, float roomBottomCornerModifer, float roomTopCornerModifer, int roomOffset)
    {
        List<RoomNode> listToReturn = new List<RoomNode>();

        foreach (var space in roomSpaces)
        {
            Vector2Int newBottomLeftPoint = StructureHelper.GenerateBottomLeftCornerBetween(space.BottomLeftAreaCorridor, space.TopRightAreaCorridor, roomBottomCornerModifer, roomOffset);
            Vector2Int newTopRightPoint = StructureHelper.GenerateTopRightCornerBetween(space.BottomLeftAreaCorridor, space.TopRightAreaCorridor, roomTopCornerModifer, roomOffset);

            space.BottomLeftAreaCorridor = newBottomLeftPoint;
            space.TopRightAreaCorridor = newTopRightPoint;
            space.BottomRightAreaCorridor = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TopLeftAreaCorridor = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);

            listToReturn.Add((RoomNode)space);

        }

        return listToReturn;
    }
}