using System.Drawing;
using System.Numerics;
using Ingression.Components;
using GramEngine.Core;
using GramEngine.ECS;
using GramEngine.ECS.Components;

namespace Ingression.Prefabs;

public class Dialogue : Prefab
{
    private Entity dialogueEntity;
    private string[] dialogue;
    private string text;
    private string name;

    public Dialogue(string[] dialogue, string name)
    {
        this.dialogue = dialogue;
        this.name = name;
    }
    
    public override Entity Instantiate()
    {
        var fontSize = (int)(GameStateManager.Window.settings.Width * .025f);
        
        dialogueEntity = new Entity();
        dialogueEntity.Tag = "textbox";
        dialogueEntity.AddComponent(new TextComponent("", "SourceFiles/square.ttf", 
            fontSize));
        dialogueEntity.AddComponent(new RenderRect(
        new Vector2(
                (float)GameStateManager.Window.settings.Width * (4f / 5f),
                (float)GameStateManager.Window.settings.Height * (1.2f / 4f)
            )));
        dialogueEntity.AddComponent(new Sound("Content/Sound/blip.wav", "scroll"));
        dialogueEntity.GetComponent<Sound>().Volume = 30;
        dialogueEntity.GetComponent<Sound>().AddSound("Content/Sound/sound.wav", "advance");
        
        var rect = dialogueEntity.GetComponent<RenderRect>();
        var textComponent = dialogueEntity.GetComponent<TextComponent>();
        
        rect.FillColor = Color.FromArgb(75, 128, 202);
        rect.BorderThickness = 4;
        rect.OutlineColor = Color.FromArgb(33, 33, 35);
        dialogueEntity.Transform.Position =
            new Vector3(
                (float)GameStateManager.Window.settings.Width / 2 - rect.Size.X / 2,
                (float)GameStateManager.Window.settings.Height / 2 + rect.Size.Y / 2,
                100f); 
        
        textComponent.TextOffset = new Vector2(rect.Size.X*.03f, rect.Size.Y*.1f);
        dialogueEntity.AddComponent(new DialogueManager(dialogue, .02, 
            50, name));

        return dialogueEntity;
    }
}