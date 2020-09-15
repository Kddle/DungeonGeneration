using System.Collections.Generic;
using UnityEngine;

public class BinarySpacePartitioner
{
    RoomNode rootNode;
    public static Random Random = new Random();

    public BinarySpacePartitioner(Vector2Int dungeonSize)
    {
        this.rootNode = new RoomNode(new Vector2Int(0, 0), dungeonSize, null, 0);
    }

    public RoomNode RootNode { get => rootNode; }

    public List<RoomNode> PrepareNodesCollection(int maxIterations, Vector2Int roomSizeMin)
    {
        Queue<RoomNode> graph = new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();

        graph.Enqueue(this.rootNode);
        listToReturn.Add(this.rootNode);
        int iterations = 0;

        while (iterations < maxIterations && graph.Count > 0)
        {
            iterations++;
            RoomNode currentNode = graph.Dequeue();

            if (currentNode.width >= roomSizeMin.x * 2 || currentNode.length >= roomSizeMin.y * 2)
            {
                SplitSpace(currentNode, listToReturn, roomSizeMin, graph);
            }
        }

        return listToReturn;
    }

    private void SplitSpace(RoomNode currentNode, List<RoomNode> listToReturn, Vector2Int roomSizeMin, Queue<RoomNode> graph)
    {
        Line line = GetLineDividingSpace(currentNode.BottomLeftAreaCorridor, currentNode.TopRightAreaCorridor, roomSizeMin);

        RoomNode node1, node2;

        if (line.Orientation == Orientation.Horizontal)
        {
            node1 = new RoomNode(currentNode.BottomLeftAreaCorridor, new Vector2Int(currentNode.TopRightAreaCorridor.x, line.Coordinates.y), currentNode, currentNode.treeLayerIndex + 1);
            node2 = new RoomNode(new Vector2Int(currentNode.BottomLeftAreaCorridor.x, line.Coordinates.y), currentNode.TopRightAreaCorridor, currentNode, currentNode.treeLayerIndex + 1);
        }
        else
        {
            node1 = new RoomNode(currentNode.BottomLeftAreaCorridor, new Vector2Int(line.Coordinates.x, currentNode.TopRightAreaCorridor.y), currentNode, currentNode.treeLayerIndex + 1);
            node2 = new RoomNode(new Vector2Int(line.Coordinates.x, currentNode.BottomLeftAreaCorridor.y), currentNode.TopRightAreaCorridor, currentNode, currentNode.treeLayerIndex + 1);
        }

        AddNewNodeToCollections(listToReturn, graph, node1);
        AddNewNodeToCollections(listToReturn, graph, node2);

    }

    private void AddNewNodeToCollections(List<RoomNode> listToReturn, Queue<RoomNode> graph, RoomNode node)
    {
        listToReturn.Add(node);
        graph.Enqueue(node);
    }

    private Line GetLineDividingSpace(Vector2Int bottomLeftAreaCorridor, Vector2Int topRightAreaCorridor, Vector2Int roomSizeMin)
    {
        Orientation orientation;

        bool lengthStatus = (topRightAreaCorridor.y - bottomLeftAreaCorridor.y) >= 2 * roomSizeMin.y;
        bool widthStatus = (topRightAreaCorridor.x - bottomLeftAreaCorridor.x) >= 2 * roomSizeMin.x;

        if (lengthStatus && widthStatus)
        {
            orientation = (Orientation)(Random.Range(0, 2));
        }
        else if (widthStatus)
        {
            orientation = Orientation.Vertical;
        }
        else
        {
            orientation = Orientation.Horizontal;
        }

        return new Line(orientation, GetCoordinatesForOrientation(orientation, bottomLeftAreaCorridor, topRightAreaCorridor, roomSizeMin));
    }

    private Vector2Int GetCoordinatesForOrientation(Orientation orientation, Vector2Int bottomLeftAreaCorridor, Vector2Int topRightAreaCorridor, Vector2Int roomSizeMin)
    {
        Vector2Int coordinates = Vector2Int.zero;

        if (orientation == Orientation.Horizontal)
        {
            coordinates = new Vector2Int(0, Random.Range((bottomLeftAreaCorridor.y + roomSizeMin.y), (topRightAreaCorridor.y - roomSizeMin.y)));
        }
        else
        {
            coordinates = new Vector2Int(Random.Range((bottomLeftAreaCorridor.x + roomSizeMin.x), (topRightAreaCorridor.x - roomSizeMin.x)), 0);
        }

        return coordinates;
    }
}