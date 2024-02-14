using System.Numerics;
using GramEngine.Core;
using GramEngine.ECS;
using GramEngine.ECS.Components;

namespace Ingression.Components;

public enum TileType{
    FLOOR,
    WALL,
    START
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

    public TileNode(TileType tileType)
    {
        North = null;
        East = null;
        West = null;
        South = null;

        this.Type = tileType;
    }

    public TileNode(int tileType)
    {
        North = null;
        East = null;
        West = null;
        South = null;

        this.Type = (TileType)tileType;
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