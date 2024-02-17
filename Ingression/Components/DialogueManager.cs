using System.Drawing;
using System.Numerics;
using GramEngine.Core;
using GramEngine.Core.Input;
using GramEngine.ECS;
using GramEngine.ECS.Components;

namespace Ingression.Components;

public class DialogueManager : Component
{
    public bool Rendering;
    public event OnFinish Finished;

    public delegate void OnFinish();
    
    private string[] dialogue;
    private int currText;
    private TextComponent textComponent;
    private string toRender;
    private double advanceTime;
    private double toAdvanceTime;
    private int currChar;
    private bool advanceState;
    private int cutoff;
    private Keys advanceTextKey = Keys.Enter;
    private Sound sound;
    private string name;
    private Entity nameBox;
    private TextComponent nameText;
    private Entity popupSprite;
    
    
    /// <summary>
    /// A dialogue managing class that renders RPG-like textboxes. Requires a TextComponent.
    /// </summary>
    /// <param name="dialogue">The dialogue to be rendered</param>
    /// <param name="toAdvanceTime">The time spent in between rendering each individual character</param>
    public DialogueManager(string[] dialogue, double toAdvanceTime, int cutoff)
    {
        this.dialogue = dialogue;
        currText = 0;
        currChar = 0;
        this.toAdvanceTime = toAdvanceTime;
        Rendering = true;
        this.cutoff = cutoff;
        this.name = "";
    }
    public override void Initialize()
    {
        base.Initialize();
        textComponent = ParentEntity.GetComponent<TextComponent>();
        sound = ParentEntity.GetComponent<Sound>();
        name = dialogue[currText].Split(":")[0];
        toRender = TextWrap(dialogue[currText].Split(":")[1], cutoff);
        advanceTime = 0;
        
        nameBox = new Entity();
        
        var fontSize = (int)(GameStateManager.Window.settings.Width * .025f);
        
        nameBox.AddComponent(new TextComponent("", "SourceFiles/square.ttf", 
            fontSize));
        nameBox.AddComponent(new RenderRect(
            new Vector2(
                (float)GameStateManager.Window.settings.Width * (1f / 5f),
                (float)GameStateManager.Window.settings.Height * (1f / 14f)
            )));
        
        var rect = nameBox.GetComponent<RenderRect>();
        nameText = nameBox.GetComponent<TextComponent>();
        
        rect.FillColor = Color.FromArgb(75, 128, 202);
        rect.BorderThickness = 5;
        rect.OutlineColor = Color.FromArgb(33, 33, 35);
        nameBox.Transform.Position =
            new Vector3(rect.Size.X / 2 - 10,
                (float)GameStateManager.Window.settings.Height / 2 + rect.Size.Y * 1.9f,
                100f); 
        
        nameText.TextOffset = new Vector2(rect.Size.X*.05f, rect.Size.Y*.1f);

        popupSprite = new Entity();
        popupSprite.AddComponent(new Sprite($@"./Content/Sprites/{name.ToLower()}.png"));
        popupSprite.Transform.Position =
            new Vector3(rect.Size.X, (float)GameStateManager.Window.settings.Height / 2, 99f);
        popupSprite.Transform.Scale = new Vector2(20f, 20f);
        
        ParentScene.AddEntity(popupSprite);
        ParentScene.AddEntity(nameBox);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (gameTime.TotalTime.TotalSeconds >= advanceTime && Rendering)
        {
            advanceTime = gameTime.TotalTime.TotalSeconds + toAdvanceTime;
            currChar++;
            if (currChar >= toRender.Length)
                Rendering = false;
            if (currChar % 3 == 0)
                sound.Play("scroll");
            popupSprite.RemoveComponent<Sprite>();
            popupSprite.AddComponent(new Sprite($@"./Content/Sprites/{name.ToLower()}.png"));
            nameText.Text = name;
            textComponent.Text = Environment.NewLine + toRender.Substring(0, currChar);
        }
        else if (!Rendering)
        {
            if (InputManager.GetKeyPressed(advanceTextKey))
            {
                Rendering = true;
                if (currText >= dialogue.Length - 1)
                {
                    Finished?.Invoke();
                    Rendering = false;
                    ParentScene.DestroyEntity(nameBox);
                    ParentScene.DestroyEntity(popupSprite);
                    ParentScene.DestroyEntity(ParentEntity);
                }
                else
                {
                    currText++;
                    name = dialogue[currText].Split(":")[0];
                    toRender = TextWrap(dialogue[currText].Split(":")[1], cutoff);
                    currChar = 0;
                    sound.Play("advance");
                }
            }
        }
    }

    public static string TextWrap(string text, int cutoff)
    {
        var toPrintLine = "";
        var dialogueWords = text.Split(" ");
        var wordCount = 0;
        for (int i = 0; i < dialogueWords.Length; i++)
        {
            wordCount += dialogueWords[i].Length + 1; // + 1 is to account for the space
            if (wordCount < cutoff)
                toPrintLine += dialogueWords[i] + " ";
            else
            {
                toPrintLine += Environment.NewLine;
                toPrintLine += dialogueWords[i] + " ";
                wordCount = 0;
            }
        }

        // get rid of the last " "
        toPrintLine = toPrintLine.Substring(0, toPrintLine.Length - 1);
        return toPrintLine;
    }
}