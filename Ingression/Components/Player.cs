using System.Numerics;
using GramEngine.Core;
using GramEngine.Core.Input;
using GramEngine.ECS;
using GramEngine.ECS.Components;

namespace Ingression.Components;


public class Player : Component
{
    private TileNode currentTile;
    private Keys[] inputs = { Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space, Keys.LShift };
    private bool lerping;
    private Vector3 lerpFrom;
    private Vector3 lerpTo;
    private float speed;
    private float lerpT;

    public Player(float speed)
    {
        this.speed = speed;
        lerpT = 0;
        lerping = false;
        currentTile = null;
        lerpFrom = new Vector3();
        lerpTo = new Vector3();
    }

    public void SetTileNode(TileNode node)
    {
        currentTile = node;
        var tilePos = currentTile.Transform.Position;
        Transform.Position = tilePos;
        Transform.Position.Y -= 7;
        Transform.Position.Z = 10;
        // other stuff if matters
    }

    public void LerpSetTileNode(TileNode node)
    {
        currentTile = node;
        var tilePos = currentTile.Transform.Position;
        lerping = true;
        lerpFrom = Transform.Position;
        lerpTo = new Vector3(tilePos.X, tilePos.Y - 7, 10);
        // other stuff if matters
    }

    public void CheckMove(TileNode? node)
    {
        if (node == null)
        {
            return;
        }

        switch (node.Type)
        {
            case TileType.FLOOR:
                PlaySmokeAnim();
                LerpSetTileNode(node);
                break;
            case TileType.WALL:
                break;
            case TileType.START:
                break;
        }

    }

    public void PlaySmokeAnim()
    {
        var smokeEntity = new Entity();
        smokeEntity.Transform.Position = Transform.Position;
        smokeEntity.Transform.Scale = new Vector2(3, 3);
        //smokeEntity.Transform.Position.Y;
        smokeEntity.AddComponent(new Sprite("./Content/empty.png"));
        smokeEntity.AddComponent(new Animation());
        smokeEntity.GetComponent<Animation>().LoadTextureAtlas("./Content/smokeground-Sheet.png", "groundsmoke", .1f, (16, 16));
        smokeEntity.GetComponent<Animation>().SetState("groundsmoke", false);
        ParentScene.AddEntity(smokeEntity);
        
    }

public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (!lerping)
        {

            if (InputManager.GetKeyDown(inputs[0]))
                CheckMove(currentTile.North);

            else if (InputManager.GetKeyDown(inputs[1]))
                CheckMove(currentTile.South);

            else if (InputManager.GetKeyDown(inputs[2]))
            {
                CheckMove(currentTile.West);
                ParentEntity.Transform.Scale.X = -Math.Abs(ParentEntity.Transform.Scale.X);
            }

            else if (InputManager.GetKeyDown(inputs[3]))
            {
                CheckMove(currentTile.East);
                ParentEntity.Transform.Scale.X = Math.Abs(ParentEntity.Transform.Scale.X);
            }
        }
        else
        {
            lerpT += speed * gameTime.DeltaTime;
            Transform.Position = MathUtil.Lerp(lerpFrom, lerpTo, lerpT);
            if (lerpT >= 1f)
            {
                lerpT = 0;
                lerping = false;
            }
        }

}
}