using System.Drawing;
using System.Numerics;
using Ingression.Prefabs;
using GramEngine.Core;
using GramEngine.ECS;
using GramEngine.ECS.Components;
using GramEngine.ECS.Components.UI;
using Ingression.Components;

namespace Ingression.Components;

public class ConversationManager : Component
{
    private string convName;
    private bool conversing;
    private Dialogue textGenerator;
    private Entity boxEntity;
    private DialogueManager dialogueManager;
    private string[] options;
    private List<string> mainFile;
    public bool talkedFlag;
    private float timer;
    private bool useFinish;
    private Entity player;
    private string name;

    public ConversationManager(string conversationPath, string name)
    {
        convName = conversationPath.Split(".")[0];
        conversing = false;
        talkedFlag = false;
        useFinish = false;
        options = new string[2];
        mainFile = new List<string>();
        timer = 0;
        this.name = name;

    }

    public override void Initialize()
    {
        player = ParentScene.FindWithTag("player");
    }

    public void StartDialogue()
    {
        if (!useFinish)
        {
            InvokeText(convName + ".txt");
        }
    }

    private void InvokeText(string name)
    {
        mainFile.Clear();
        for (int i = 0; i < options.Length; i++)
        {
            options[i] = "none";
        }
        
        var readFile = File.ReadAllLines(name);
        for (int i=0; i<readFile.Length; i++)
        {
            if (readFile[i].StartsWith("option:"))
            {
                options[i] = readFile[i].Split(":")[1];
            }
            else
            {
                mainFile.Add(readFile[i]);
            }
        }
        
        textGenerator = new Dialogue(mainFile.ToArray(), this.name);
        boxEntity = textGenerator.Instantiate();
        dialogueManager = boxEntity.GetComponent<DialogueManager>();
        ParentScene.AddEntity(boxEntity);
        conversing = true;

    }

    
}
        