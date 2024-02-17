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
    private bool cratefall;

    public Crate() {
        lerpFrom = new Vector3();
        lerpTo = new Vector3();
        lerping = false;
        currentTile = null;
        speed = 7f;
        finishLerpDir = Direction.NONE;
        cratefall = false;
    }

    private void PlaySmokeAnim()
    {
        var smokeEntity = new Entity();
        smokeEntity.Transform.Position = Transform.Position;
        smokeEntity.Transform.Scale = new Vector2(4, 4);
        smokeEntity.Transform.Position.Y += 7;
        smokeEntity.Transform.Position.Z = 11;
        smokeEntity.AddComponent(new Sprite("./Content/Sprites/empty.png"));
        smokeEntity.AddComponent(new Animation(true));
        smokeEntity.GetComponent<Animation>().LoadTextureAtlas("./Content/Sprites/smokeground-Sheet.png", "groundsmoke", .08f, (16, 16));
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
                        TileNode? doorDir = null;
                        switch (direction)
                        {
                            case Direction.NORTH:
                                doorDir = door.OtherDoor?.Tile.North;
                                break;
                            case Direction.EAST:
                                doorDir = door.OtherDoor?.Tile.East;
                                break;
                            case Direction.SOUTH:
                                doorDir = door.OtherDoor?.Tile.South;
                                break;
                            case Direction.WEST:
                                doorDir = door.OtherDoor?.Tile.West;
                                break;
                        }

                        TileType[] movables = new[]
                        {
                            TileType.FLOOR, TileType.CRATEHOLE, TileType.SWITCH_DOOR_OPEN, TileType.B_SWITCHDOOR_OPEN, TileType.BUTTON_DOWN,
                            TileType.ONEWAY_EAST, TileType.ONEWAY_WEST, TileType.ONEWAY_NORTH, TileType.ONEWAY_SOUTH
                        };
                        if (door.OtherDoor != null && doorDir != null && (movables.Contains(doorDir.Type)))
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
            case TileType.BLUE_BUTTON_UP:
                if (node.Occupant == null)    
                {
                    LerpSetTileNode(node);
                    TriggerSwitchBDoors();
                    node.ChangeType(TileType.BLUE_BUTTON_DOWN);
                }
                break;
            case TileType.BLUE_BUTTON_DOWN:
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
            case TileType.B_SWITCHDOOR_OPEN:
                if(node.Occupant == null) 
                    LerpSetTileNode(node);
                break;
            case TileType.CRATEHOLE:
                cratefall = true;
                LerpSetTileNode(node);
                break;
        }
        
        if (lerping && prevNode.Type == TileType.BUTTON_DOWN)
        {
            prevNode.ChangeType(TileType.BUTTON_UP);
            TriggerSwitchDoors();
        }
        else if (lerping && prevNode.Type == TileType.BLUE_BUTTON_DOWN)
        {
            prevNode.ChangeType(TileType.BLUE_BUTTON_UP);
            TriggerSwitchBDoors();
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
    
    private void TriggerSwitchBDoors()
    {
        var tm = ParentScene.FindWithTag("TileManager").GetComponent<TileManager>();
        foreach(TileNode tile in tm.AllNodes) {
            if(tile.Type == TileType.B_SWITCHDOOR_OPEN) {
                tile.ChangeType(TileType.B_SWITCHDOOR_CLOSED);
            } else if(tile.Type == TileType.B_SWITCHDOOR_CLOSED) {
                tile.ChangeType(TileType.B_SWITCHDOOR_OPEN);
            }
        }
    }
    
    // setTileNode (copy + paste)
    public void SetTileNode(TileNode node)
    {
        if (currentTile.Occupant == ParentEntity)
            currentTile.Occupant = null;
        if (node.Occupant == null)
            node.Occupant = ParentEntity;
        currentTile = node;
        var tilePos = currentTile.Transform.Position;
        Transform.Position = tilePos;
        Transform.Position.Z = 10;
        // other stuff if matters
    }

    private void LerpSetTileNode(TileNode node)
    {
        if (currentTile.Occupant == ParentEntity)
            currentTile.Occupant = null;
        if (node.Occupant == null)
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
                else if (cratefall)
                {
                    currentTile.ChangeType(TileType.CRATEHOLE_FILLED);
                    ParentScene.DestroyEntity(ParentEntity);
                }
            }
        }
    }
}
