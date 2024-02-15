using System.Numerics;
using GramEngine.Core;
using GramEngine.ECS;
using GramEngine.ECS.Components;

namespace Ingression.Components;

public enum TileType{
    FLOOR = 0,
    WALL = 1,
    START = 2,
    ONEWAY_NORTH = 3,
    ONEWAY_SOUTH = 4,
    ONEWAY_EAST = 5, 
    ONEWAY_WEST = 6,
    BUTTON_DOWN = 7,
    BUTTON_UP = 8,
    LEVER_RIGHT = 9,
    LEVER_LEFT = 10,
    REGULAR_DOOR = 11,
    LOCKED_DOOR = 12
}

public class TileNode : Component
{
    public TileType Type
    {
        get;
        private set;
    }
    
    public TileNode? North;
    public TileNode? East;
    public TileNode? West;
    public TileNode? South;

    // For objects that might be on the tile
    public Entity? Occupant;

    public TileNode(TileType tileType)
    {
        North = null;
        East = null;
        West = null;
        South = null;
        
        Occupant = null;

        this.Type = tileType;
    }

    public TileNode(int tileType)
    {
        North = null;
        East = null;
        West = null;
        South = null;
        
        Occupant = null;

        this.Type = (TileType)tileType;
    }

    public void ChangeType(TileType type)
    {
        Type = type;
        ParentEntity.RemoveComponent<Sprite>();
        ParentEntity.AddComponent(new Sprite("./Content/" + Type.ToString() + ".png"));
    }

    // Use this only in the rarest of circumstances!!! (basically never unless your name is TileManager)
    public void ChangeValueType(TileType type)
    {
        Type = type;
    }

    public override void Initialize()
    {
        base.Initialize();
        string tileTexture = "./Content/" + Type.ToString() + ".png";
        ParentEntity.AddComponent(new Sprite(tileTexture));

        switch (Type)
        {
            case TileType.FLOOR:
                break;
            case TileType.WALL:
                var sprite = ParentEntity.GetComponent<Sprite>();
                sprite.Origin = new Vector2(sprite.Width / 2, sprite.Height / 2 + 2);
                break;
            case TileType.START:
                break;
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
    
    
}