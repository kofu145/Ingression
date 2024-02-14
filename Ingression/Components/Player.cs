using GramEngine.ECS;

namespace Ingression.Components;

public class Player : Component
{
    private TileNode currentTile;
    
    public Player()
    {
        
    }

    public void SetTileNode(TileNode node)
    {
        currentTile = node;
        // other stuff if matters
        
    }
}