using System.Numerics;
using GramEngine.ECS;
using GramEngine.Core;
using GramEngine.ECS.Components;
using Ingression.Components;

namespace Ingression;

public class CutsceneFive : GameState
{
    private Entity bob;
    public override void Initialize()
    {
        Program.GlobalMusic.Play("final");
        bob = new Entity();
        
        Entity talk = new Entity();
        talk.AddComponent(new ConversationManager("Content/Dialogue/Scene5.txt"));
        AddEntity(talk);
        
        base.Initialize();
        talk.GetComponent<ConversationManager>().StartDialogue();
        talk.GetComponent<ConversationManager>().dialogueManager.Finished += () =>
        {
            Program.GlobalMusic.Stop();
            GameStateManager.SetSceneTransition(SceneTransition.FadeIn);
            GameStateManager.SwapScreen(new Credits());
        };                

        
        AddCharacter("bob", new Vector3(GameStateManager.Window.Width / 2 + 40, 
            GameStateManager.Window.Height / 2 + 60, 10), true);
        
        AddCharacter("angela", new Vector3(GameStateManager.Window.Width / 2 + 110, 
            GameStateManager.Window.Height / 2 + 60, 10), false);

        var bgEntity = new Entity();
        bgEntity.AddComponent(new Sprite("./Content/Sprites/environment_house.png"));
        bgEntity.Transform.Position =
            new Vector3(GameStateManager.Window.Width / 2, GameStateManager.Window.Height / 2, 0);
        bgEntity.Transform.Scale = new Vector2(5, 5);
        
        AddEntity(bgEntity);
        

    }

    public void AddCharacter(string name, Vector3 pos, bool facingRight)
    {
        var character = new Entity();
        character.AddComponent(new Sprite($"./Content/Sprites/Characters/{name}.png"));
        character.AddComponent(new Animation());
        character.GetComponent<Animation>().LoadTextureAtlas($"./Content/Sprites/Characters/{name}idle-Sheet.png", "idle", .2f, (16, 16));
        character.GetComponent<Animation>().SetState("idle");
        character.Transform.Position = pos;
        int x = -4;
        if (facingRight)
            x = 4;
        character.Transform.Scale = new Vector2(x, 4);
        AddEntity(character);
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}