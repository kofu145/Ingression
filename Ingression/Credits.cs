using System.Numerics;
using GramEngine.Core;
using GramEngine.ECS;
using GramEngine.ECS.Components;
using SFML.Graphics;

namespace Ingression;

public class Credits : GameState
{
    private Entity mainText;
    public override void Initialize()
    {
        base.Initialize();
        mainText = new Entity();
        string text = "CREDITS:" + Environment.NewLine
            + "PROGRAMMING" + Environment.NewLine
            + "Kofu, Xyammerz" + Environment.NewLine
            + "LEVEL DESIGN" + Environment.NewLine
            + "Kofu, Xyammerz, Slime" + Environment.NewLine
            + "DIALOGUE" + Environment.NewLine
            + "Xyammerz" + Environment.NewLine
            + "THANK YOU SO MUCH FOR PLAYING!" + Environment.NewLine;
        mainText.AddComponent(new TextComponent(text, "./SourceFiles/square.ttf",50));
        // mainText.GetComponent<TextComponent>().TextOffset = new Vector2(-500, 0);
        mainText.Transform.Position = new Vector3(270,
            80, 10);
        
        AddEntity(mainText);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}