using System.Numerics;
using GramEngine.Core;
using GramEngine.Core.Input;
using GramEngine.ECS;
using GramEngine.ECS.Components;

namespace Ingression.Components;

public class Crate : Component
{
    private Vector3 lerpFrom;
    private Vector3 lerpTo;
    private bool lerping;
    public TileNode currentTile;
    private float lerpT;
    private float speed;
    private TileNode? finishLerp;
    private TileNode? lerpToFinishLerp;
    private Direction finishLerpDir;

    public Crate() {
        lerpFrom = new Vector3();
        lerpTo = new Vector3();
        lerping = false;
        currentTile = null;
        speed = 7f;
        finishLerpDir = Direction.NONE;
    }

    private void PlaySmokeAnim()
    {
        var smokeEntity = new Entity();
        smokeEntity.Transform.Position = Transform.Position;
        smokeEntity.Transform.Scale = new Vector2(4, 4);
        smokeEntity.Transform.Position.Y += 7;
        smokeEntity.Transform.Position.Z = 11;
        smokeEntity.AddComponent(new Sprite("./Content/empty.png"));
        smokeEntity.AddComponent(new Animation(true));
        smokeEntity.GetComponent<Animation>().LoadTextureAtlas("./Content/smokeground-Sheet.png", "groundsmoke", .08f, (16, 16));
        smokeEntity.GetComponent<Animation>().SetState("groundsmoke", false);
        ParentScene.AddEntity(smokeEntity);
        
    }

    // checkMove
    public void CheckMove(Direction direction) {
        TileNode? node = null;
        switch (direction)
        {
            case Direction.NORTH:
                node = currentTile.North;
                break;
            case Direction.EAST:
                node = currentTile.East;
                break;
            case Direction.SOUTH:
                node = currentTile.South;
                break;
            case Direction.WEST:
                node = currentTile.West;
                break;
        }
        if (node == null)
        {
            return;
        }
        var prevNode = currentTile;

        switch(node.Type) 
        {
            case TileType.FLOOR:
                if(node.Occupant == null) 
                    LerpSetTileNode(node);
                else
                {
                    if (node.Occupant.HasComponent<Door>())
                    {
                        var door = node.Occupant?.GetComponent<Door>();
                        if (door.OtherDoor != null)
                        {
                            node.Occupant.GetComponent<Animation>().Play();
                            door.OtherDoor.ParentEntity.GetComponent<Animation>().Play();
                            LerpSetTileNode(node);
                            finishLerp = door.OtherDoor?.Tile;
                            finishLerpDir = direction;
                            ParentScene.FindWithTag("player").GetComponent<Sound>().Play("doorenter");
                        }
                    }
                }

                break;
            case TileType.WALL:
                break;
            case TileType.START:
                break;
            case TileType.ONEWAY_NORTH:
                LerpSetTileNode(node);
                finishLerpDir = Direction.NORTH;
                break;
            case TileType.ONEWAY_SOUTH:
                LerpSetTileNode(node);
                finishLerpDir = Direction.SOUTH;
                break;
            case TileType.ONEWAY_WEST:
                LerpSetTileNode(node);
                finishLerpDir = Direction.WEST;
                break;
            case TileType.ONEWAY_EAST:
                LerpSetTileNode(node);
                finishLerpDir = Direction.EAST;
                break;
            case TileType.BUTTON_UP:
                if(node.Occupant == null) 
                {
                    LerpSetTileNode(node);
                    node.ChangeType(TileType.BUTTON_DOWN);
                    TriggerSwitchDoors();
                }
                break;
            case TileType.BUTTON_DOWN:
                if(node.Occupant == null) 
                    LerpSetTileNode(node);
                break;
            case TileType.LEVER_LEFT:
                break;
            case TileType.LEVER_RIGHT:
                break;
            case TileType.SWITCH_DOOR_OPEN:
                if(node.Occupant == null) 
                    LerpSetTileNode(node);
                break;
        }
        
        if (lerping && prevNode.Type == TileType.BUTTON_DOWN)
        {
            prevNode.ChangeType(TileType.BUTTON_UP);
            TriggerSwitchDoors();
        }
    }
    
    private void TriggerSwitchDoors()
    {
        var tm = ParentScene.FindWithTag("TileManager").GetComponent<TileManager>();
        foreach(TileNode tile in tm.AllNodes) {
            if(tile.Type == TileType.SWITCH_DOOR_OPEN) {
                tile.ChangeType(TileType.SWITCH_DOOR_CLOSED);
            } else if(tile.Type == TileType.SWITCH_DOOR_CLOSED) {
                tile.ChangeType(TileType.SWITCH_DOOR_OPEN);
            }
        }
    }
    // setTileNode (copy + paste)
    public void SetTileNode(TileNode node)
    {
        currentTile.Occupant = null;
        node.Occupant = ParentEntity;
        currentTile = node;
        var tilePos = currentTile.Transform.Position;
        Transform.Position = tilePos;
        Transform.Position.Z = 10;
        // other stuff if matters
    }

    private void LerpSetTileNode(TileNode node)
    {
        currentTile.Occupant = null;
        node.Occupant = ParentEntity;
        currentTile = node;
        var tilePos = currentTile.Transform.Position;
        lerping = true;
        lerpFrom = Transform.Position;
        lerpTo = new Vector3(tilePos.X, tilePos.Y, 10);
        // other stuff if matters
    }
    // settilenodeLerp
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (lerping){
            lerpT += speed * gameTime.DeltaTime;
            Transform.Position = MathUtil.Lerp(lerpFrom, lerpTo, lerpT);
            if (lerpT >= 1f)
            {
                lerpT = 0;
                lerping = false;

                if (finishLerp != null)
                {
                    SetTileNode(finishLerp);
                    finishLerp = null;
                }

                if (lerpToFinishLerp != null)
                {
                    LerpSetTileNode(lerpToFinishLerp);
                    lerpToFinishLerp = null;
                }
                
                else if (finishLerpDir != Direction.NONE)
                {
                    var tmp = finishLerpDir;
                    finishLerpDir = Direction.NONE;
                    CheckMove(tmp);
                }
            }
        }
    }
}
