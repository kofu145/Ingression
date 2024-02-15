using System.Numerics;
using GramEngine.Core;
using GramEngine.Core.Input;
using GramEngine.ECS;
using GramEngine.ECS.Components;

namespace Ingression.Components;

public enum Direction
{
    NORTH,
    SOUTH,
    EAST,
    WEST
}

public class Player : Component
{
    private const int PlayerSize = 4;
    private const int DoorSize = 4;
    
    private TileNode currentTile;
    private Keys[] inputs = { Keys.W, Keys.S, Keys.A, Keys.D, Keys.K, Keys.L };
    private bool lerping;
    private Vector3 lerpFrom;
    private Vector3 lerpTo;
    private TileNode? finishLerp;
    private float speed;
    private float lerpT;
    private Entity? doorBlue;
    private Entity? doorRed;
    private bool placingDoor;
    private bool placingBlueDoor;

    public Player(float speed)
    {
        this.speed = speed;
        placingDoor = false;
        lerpT = 0;
        lerping = false;
        currentTile = null;
        finishLerp = null;
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

    private void LerpSetTileNode(TileNode node)
    {
        currentTile = node;
        var tilePos = currentTile.Transform.Position;
        lerping = true;
        lerpFrom = Transform.Position;
        lerpTo = new Vector3(tilePos.X, tilePos.Y - 7, 10);
        // other stuff if matters
    }

    private void CheckMove(TileNode? node)
    {
        if (node == null)
        {
            return;
        }

        switch (node.Type)
        {
            case TileType.FLOOR:
                PlaySmokeAnim();
                if (node.Occupant == null)
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
                        }
                    }
                }
                break;
            case TileType.WALL:
                break;
            case TileType.START:
                break;
        }

    }

    private void PlaceDoor(Direction direction)
    {
        switch (direction)
        {
            // north
            case Direction.NORTH:
                if (currentTile.North != null && currentTile.North.Type == TileType.FLOOR && currentTile.North.Occupant == null)
                {
                    PlacePortal(currentTile.North);
                    placingDoor = false;
                }
                break;
            // south
            case Direction.SOUTH:
                if (currentTile.South != null && currentTile.South.Type == TileType.FLOOR && currentTile.South.Occupant == null)
                {
                    PlacePortal(currentTile.South);
                    placingDoor = false;
                }
                break;
            // east
            case Direction.EAST:
                if (currentTile.East != null && currentTile.East.Type == TileType.FLOOR && currentTile.East.Occupant == null)
                {
                    PlacePortal(currentTile.East);
                    placingDoor = false;
                }
                break;
            // west
            case Direction.WEST:
                if (currentTile.West != null && currentTile.West.Type == TileType.FLOOR && currentTile.West.Occupant == null)
                {
                    PlacePortal(currentTile.West);
                    placingDoor = false;
                }
                break;
        }

        if (!placingDoor)
        {
            foreach (var entity in ParentScene.FindEntitiesWithTag("conceptDoor"))
            {
                ParentScene.DestroyEntity(entity);
            }
        }
    }

    public void PlaySmokeAnim()
    {
        var smokeEntity = new Entity();
        smokeEntity.Transform.Position = Transform.Position;
        smokeEntity.Transform.Scale = new Vector2(DoorSize, DoorSize);
        smokeEntity.Transform.Position.Y += 7;
        smokeEntity.Transform.Position.Z = 11;
        smokeEntity.AddComponent(new Sprite("./Content/empty.png"));
        smokeEntity.AddComponent(new Animation(true));
        smokeEntity.GetComponent<Animation>().LoadTextureAtlas("./Content/smokeground-Sheet.png", "groundsmoke", .08f, (16, 16));
        smokeEntity.GetComponent<Animation>().SetState("groundsmoke", false);
        ParentScene.AddEntity(smokeEntity);
        
    }

    private void EnterDoorPlacing()
    {
        placingDoor = true;
        if (currentTile.North != null)
            PlaceConceptualDoor(currentTile.North);
        
        if (currentTile.South != null)
            PlaceConceptualDoor(currentTile.South);
        
        if (currentTile.East != null)
            PlaceConceptualDoor(currentTile.East);
        
        if (currentTile.West != null)
            PlaceConceptualDoor(currentTile.West);
        
    }

    private void PlaceConceptualDoor(TileNode node)
    {
        var conceptDoor = new Entity();
        conceptDoor.Tag = "conceptDoor";
        conceptDoor.AddComponent(new Sprite("./Content/conceptualdoor.png"));
        conceptDoor.Transform.Position = node.Transform.Position;
        conceptDoor.Transform.Scale = new Vector2(DoorSize, DoorSize);
        conceptDoor.Transform.Position.Z = 9;
        ParentScene.AddEntity(conceptDoor);
    }

    private void PlacePortal(TileNode destination)
    {
        Entity door = new Entity();
        door.AddComponent(new Sprite("./Content/empty.png"));
        door.AddComponent(new Animation());
        if (placingBlueDoor) {
            door.AddComponent(new Door(destination, doorRed?.GetComponent<Door>()));
            door.GetComponent<Animation>().LoadTextureAtlas("./Content/doorBlue-Sheet.png", "doorOpen", .08f, (16, 16));
        }
        else {
            door.AddComponent(new Door(destination, doorBlue?.GetComponent<Door>()));
            door.GetComponent<Animation>().LoadTextureAtlas("./Content/doorRed-Sheet.png", "doorOpen", .08f, (16, 16));
        }
        
        door.GetComponent<Animation>().SetState("doorOpen", false);
        door.GetComponent<Animation>().ResetOnFinish = true;
        door.GetComponent<Animation>().Reset();
        door.Transform.Position = destination.Transform.Position;
        door.Transform.Scale = new Vector2(DoorSize, DoorSize);
        door.Transform.Position.Z = 9;

        if (placingBlueDoor)
        {
            doorBlue?.GetComponent<Door>().DestroySelf();
            doorBlue = door;
            if (doorRed != null) 
                doorRed.GetComponent<Door>().OtherDoor = door.GetComponent<Door>();
            door.Tag = "portalBlue";
        }
        else
        {
            doorRed?.GetComponent<Door>().DestroySelf();
            doorRed = door;
            if (doorBlue != null) 
                doorBlue.GetComponent<Door>().OtherDoor = door.GetComponent<Door>();
            door.Tag = "portalRed";
        }
        destination.Occupant = door;

        ParentScene.AddEntity(door);
    }

    public override void Initialize()
    {
        base.Initialize();
        Transform.Scale = new Vector2(PlayerSize, PlayerSize);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (!lerping)
        {
            // Movement
            if (InputManager.GetKeyDown(inputs[0]))
            {
                if (placingDoor)
                    PlaceDoor(Direction.NORTH);
                
                else
                    CheckMove(currentTile.North);
            }

            else if (InputManager.GetKeyDown(inputs[1]))
            {
                if (placingDoor)
                    PlaceDoor(Direction.SOUTH);
                
                else
                    CheckMove(currentTile.South);
            }

            else if (InputManager.GetKeyDown(inputs[2]))
            {
                if (placingDoor)
                    PlaceDoor(Direction.WEST);
                else {
                    CheckMove(currentTile.West);
                    ParentEntity.Transform.Scale.X = -Math.Abs(ParentEntity.Transform.Scale.X);
                }
            }

            else if (InputManager.GetKeyDown(inputs[3]))
            {
                if (placingDoor)
                    PlaceDoor(Direction.EAST);
                else {
                    CheckMove(currentTile.East);
                    ParentEntity.Transform.Scale.X = Math.Abs(ParentEntity.Transform.Scale.X);
                }
            }
            
            // Door placement
            else if (InputManager.GetKeyDown(inputs[4]))
            {
                if (!placingDoor)
                {
                    placingBlueDoor = true;
                    EnterDoorPlacing();
                }
                else
                {
                    placingDoor = false;
                    foreach (var entity in ParentScene.FindEntitiesWithTag("conceptDoor"))
                    {
                        ParentScene.DestroyEntity(entity);
                    }
                }
            }
            
            else if (InputManager.GetKeyDown(inputs[5]))
            {
                if (!placingDoor)
                {
                    placingBlueDoor = false;
                    EnterDoorPlacing();
                }
                else
                {
                    placingDoor = false;
                    foreach (var entity in ParentScene.FindEntitiesWithTag("conceptDoor"))
                    {
                        ParentScene.DestroyEntity(entity);
                    }
                }
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

                if (finishLerp != null)
                {
                    SetTileNode(finishLerp);
                    finishLerp = null;
                }
            }
        }

    }
}