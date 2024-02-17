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
    public DialogueManager dialogueManager;
    private string[] options;
    private List<string> mainFile;
    public bool talkedFlag;
    private float timer;
    private bool useFinish;
    private Entity player;
    private string name;

    public ConversationManager(string conversationPath)
    {
        convName = conversationPath.Split(".")[0];
        conversing = false;
        talkedFlag = false;
        useFinish = false;
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
        
        var readFile = File.ReadAllLines(name);
        for (int i=0; i<readFile.Length; i++)
        {
            mainFile.Add(readFile[i]);
        }
        
        textGenerator = new Dialogue(mainFile.ToArray());
        boxEntity = textGenerator.Instantiate();
        dialogueManager = boxEntity.GetComponent<DialogueManager>();
        ParentScene.AddEntity(boxEntity);
        conversing = true;

    }

    
}
        