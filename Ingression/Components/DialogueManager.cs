using GramEngine.Core;
using GramEngine.Core.Input;
using GramEngine.ECS;
using GramEngine.ECS.Components;

namespace Ingression.Components;

public class DialogueManager : Component
{
    public bool Rendering;
    public bool Finished;
    
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
    
    /// <summary>
    /// A dialogue managing class that renders RPG-like textboxes. Requires a TextComponent.
    /// </summary>
    /// <param name="dialogue">The dialogue to be rendered</param>
    /// <param name="toAdvanceTime">The time spent in between rendering each individual character</param>
    public DialogueManager(string[] dialogue, double toAdvanceTime, int cutoff, string name)
    {
        this.dialogue = dialogue;
        currText = 0;
        currChar = 0;
        this.toAdvanceTime = toAdvanceTime;
        Rendering = true;
        Finished = false;
        this.cutoff = cutoff;
        this.name = name;
    }
    public override void Initialize()
    {
        base.Initialize();
        textComponent = ParentEntity.GetComponent<TextComponent>();
        sound = ParentEntity.GetComponent<Sound>();
        toRender = TextWrap(dialogue[currText], cutoff);
        advanceTime = 0;
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
            textComponent.Text = $"{name}" + Environment.NewLine + toRender.Substring(0, currChar);
        }
        else if (!Rendering)
        {
            if (InputManager.GetKeyPressed(advanceTextKey))
            {
                Rendering = true;
                if (currText >= dialogue.Length - 1)
                {
                    Finished = true;
                    Rendering = false;
                    ParentScene.DestroyEntity(ParentEntity);
                }
                else
                {
                    currText++;
                    toRender = TextWrap(dialogue[currText], cutoff);
                    currChar = 0;
                    sound.Play("advance");
                }
            }
        }
    }

    public static string TextWrap(string text, int cutoff)
    {
        var toPrintLine = "  ";
        var dialogueWords = text.Split(" ");
        var wordCount = 0;
        for (int i = 0; i < dialogueWords.Length; i++)
        {
            wordCount += dialogueWords[i].Length + 1; // + 1 is to account for the space
            if (wordCount < cutoff)
                toPrintLine += dialogueWords[i] + " ";
            else
            {
                toPrintLine += Environment.NewLine + "  ";
                toPrintLine += dialogueWords[i] + " ";
                wordCount = 0;
            }
        }

        // get rid of the last " "
        toPrintLine = toPrintLine.Substring(0, toPrintLine.Length - 1);
        return toPrintLine;
    }
}