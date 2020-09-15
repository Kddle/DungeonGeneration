using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    private List<Node> childrenNodeList;

    public List<Node> ChildrenNodeList { get => childrenNodeList; }

    public bool Visited { get; set; }

    public Vector2Int BottomLeftAreaCorridor { get; set; }
    public Vector2Int BottomRightAreaCorridor { get; set; }
    public Vector2Int TopRightAreaCorridor { get; set; }
    public Vector2Int TopLeftAreaCorridor { get; set; }

    public int treeLayerIndex { get; set; }
    public Node parent { get; set; }


    public Node(Node parent)
    {
        childrenNodeList = new List<Node>();
        this.parent = parent;

        if (parent != null)
        {
            parent.AddChild(this);
        }
    }

    public void AddChild(Node node)
    {
        childrenNodeList.Add(node);
    }

    public void RemoveChild(Node node)
    {
        childrenNodeList.Remove(node);
    }
}