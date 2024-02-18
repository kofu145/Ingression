using System.Numerics;
using GramEngine.ECS;
using GramEngine.Core;
using GramEngine.ECS.Components;
using Ingression.Components;

namespace Ingression;

public class CutsceneFour : GameState
{
    private Entity bob;
    public override void Initialize()
    {
        bob = new Entity();
        
        Entity talk = new Entity();
        talk.AddComponent(new ConversationManager("Content/Dialogue/Scene4.txt"));
        AddEntity(talk);
        
        base.Initialize();
        talk.GetComponent<ConversationManager>().StartDialogue();
        talk.GetComponent<ConversationManager>().dialogueManager.Finished += () =>
        {
            GameStateManager.SetSceneTransition(SceneTransition.FadeIn);
            GameStateManager.SwapScreen(new LevelScene("9", true));
        };        
        
        
        AddCharacter("bob", new Vector3(GameStateManager.Window.Width / 2 + 190, 
            GameStateManager.Window.Height / 2, 10), true);
        
        var bgEntity = new Entity();
        bgEntity.AddComponent(new Sprite("./Content/Sprites/backgroundtext.png"));
        bgEntity.Transform.Position =
            new Vector3(GameStateManager.Window.Width / 2, GameStateManager.Window.Height / 2, 0);
        bgEntity.Transform.Scale = new Vector2(5, 5);
        
        var livingroom = new Entity();
        livingroom.AddComponent(new Sprite("./Content/Sprites/livingroom.png"));
        livingroom.Transform.Position =
            new Vector3(GameStateManager.Window.Width / 2, GameStateManager.Window.Height / 2, 1);
        livingroom.Transform.Scale = new Vector2(5, 5);
        
        AddEntity(bgEntity);
        AddEntity(livingroom);
        

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