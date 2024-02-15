using GramEngine.ECS;

namespace Ingression.Components;

public class Door : Component
{
    public TileNode Tile;
    public Door? OtherDoor;
    
    public Door(TileNode tile, Door otherDoor)
    {
        Tile = tile;
        OtherDoor = otherDoor;
    }

    public void DestroySelf()
    {
        Tile.Occupant = null;
        ParentScene.DestroyEntity(ParentEntity);
    }
    
}