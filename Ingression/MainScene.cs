using System.Numerics;
using GramEngine.Core;
using GramEngine.Core.Input;
using GramEngine.ECS;
using GramEngine.ECS.Components;
using Ingression.Components;

namespace Ingression;

public class MainScene : GameState
{
    private Entity tilerEntity;
    private Entity player;
    private float test;
    public override void Initialize()
    {
        test = 0;
        tilerEntity = new Entity();
        player = new Entity();
        
        AddEntity(tilerEntity);
        
        base.Initialize();

        
        
        tilerEntity.AddComponent(new TileManager());
        var tileManager = tilerEntity.GetComponent<TileManager>();
        tileManager.ConstructFromFile(@"./Content/test.txt");

        player.AddComponent(new Player(10f));
        player.AddComponent(new Sprite("./Content/bob.png"));
        player.AddComponent(new Animation());
        player.GetComponent<Animation>().LoadTextureAtlas("./Content/bobidle-Sheet.png", "idle", .2f, (16, 16));
        player.GetComponent<Animation>().SetState("idle");
        
        player.GetComponent<Player>().SetTileNode(tileManager.Head);
        player.Transform.Position.Z = 10;
        player.Transform.Scale = new Vector2(3, 3);

        var bgEntity = new Entity();
        bgEntity.AddComponent(new Sprite("./Content/backgroundtext.png"));
        bgEntity.Transform.Position =
            new Vector3(GameStateManager.Window.Width / 2, GameStateManager.Window.Height / 2, 0);
        bgEntity.Transform.Scale = new Vector2(5, 5);
        
        AddEntity(player);
        AddEntity(bgEntity);

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}