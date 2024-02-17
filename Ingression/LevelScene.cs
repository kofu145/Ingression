using System.Numerics;
using GramEngine.Core;
using GramEngine.Core.Input;
using GramEngine.ECS;
using GramEngine.ECS.Components;
using Ingression.Components;

namespace Ingression;

public class LevelScene : GameState
{
    private Entity tilerEntity;
    private Entity player;
    private string levelName;
    private bool doDialogue;
    public LevelScene(string levelName, bool doDialogue){
        this.levelName = levelName;
        this.doDialogue = doDialogue;
    }

    public override void Initialize()
    {
        GameStateManager.SetSceneTransition(SceneTransition.FadeIn);
        tilerEntity = new Entity();
        tilerEntity.Tag = "TileManager";
        player = new Entity();
        
        Entity talk = new Entity();
        var initDialogueStr = $@"Content/Dialogue/{levelName}.txt";
        Console.WriteLine(initDialogueStr);

        if (Path.Exists(initDialogueStr) && doDialogue)
            talk.AddComponent(new ConversationManager(initDialogueStr));
        
        AddEntity(talk);
        
        AddEntity(tilerEntity);
        
        base.Initialize();

        tilerEntity.AddComponent(new TileManager());
        var tileManager = tilerEntity.GetComponent<TileManager>();
        tileManager.ConstructFromFile($@"./Content/Levels/{levelName}.txt");

        player.Tag = "player";
        player.AddComponent(new Player(8f, levelName));
        player.AddComponent(new Sprite("./Content/Sprites/bob.png"));
        player.AddComponent(new Animation());
        player.AddComponent(new Sound());
        var sound = player.GetComponent<Sound>();
        sound.AddSound("./Content/Sound/key.wav", "key");
        sound.AddSound("./Content/Sound/dash.wav", "dash");
        sound.AddSound("./Content/Sound/cratemove.wav", "cratemove");
        sound.AddSound("./Content/Sound/switch.wav", "switch");
        sound.AddSound("./Content/Sound/doormake.wav", "doormake");
        sound.AddSound("./Content/Sound/doorenter.wav", "doorenter");
        sound.AddSound("./Content/Sound/reset.wav", "reset");
        sound.AddSound("./Content/Sound/oneway.wav", "oneway");
        
        player.GetComponent<Animation>().LoadTextureAtlas("./Content/Sprites/bobidle-Sheet.png", "idle", .2f, (16, 16));
        player.GetComponent<Animation>().SetState("idle");
        
        player.GetComponent<Player>().SetTileNode(tileManager.Head);
        player.Transform.Position.Z = 10;
        player.Transform.Scale = new Vector2(3, 3);

        var bgEntity = new Entity();
        bgEntity.AddComponent(new Sprite("./Content/Sprites/backgroundtext.png"));
        bgEntity.Transform.Position =
            new Vector3(GameStateManager.Window.Width / 2, GameStateManager.Window.Height / 2, 0);
        bgEntity.Transform.Scale = new Vector2(5, 5);


        if (Path.Exists(initDialogueStr) && doDialogue)
        {
            talk.GetComponent<ConversationManager>().StartDialogue();
            player.GetComponent<Player>().Waiting = true;
            talk.GetComponent<ConversationManager>().dialogueManager.Finished += () =>
            {
                player.GetComponent<Player>().Waiting = false;
            };
        }

        
        AddEntity(player);
        AddEntity(bgEntity);

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}