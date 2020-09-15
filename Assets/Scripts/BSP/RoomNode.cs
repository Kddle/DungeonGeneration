using UnityEngine;

public class RoomNode : Node
{
    public RoomNode(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, Node parent, int index) : base(parent)
    {
        this.BottomLeftAreaCorridor = bottomLeftAreaCorner;
        this.TopRightAreaCorridor = topRightAreaCorner;
        this.BottomRightAreaCorridor = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);
        this.TopLeftAreaCorridor = new Vector2Int(bottomLeftAreaCorner.x, topRightAreaCorner.y);

        this.treeLayerIndex = index;
    }

    public int width => (int)(TopRightAreaCorridor.x - BottomLeftAreaCorridor.x);
    public int length => (int)(TopRightAreaCorridor.y - BottomLeftAreaCorridor.y);

}