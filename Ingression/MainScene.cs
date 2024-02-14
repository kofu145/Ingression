using GramEngine.Core;
using GramEngine.ECS;
using Ingression.Components;

namespace Ingression;

public class MainScene : GameState
{
    private Entity test;
    public override void Initialize()
    {
        base.Initialize();
        test = new Entity();
        test.AddComponent(new TileManager());
        test.GetComponent<TileManager>().ConstructFromFile(@"./Content/test.txt");
        
        AddEntity(test);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}