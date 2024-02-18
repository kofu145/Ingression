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
    WEST,
    NONE
}

public class Player : Component
{
    public bool Waiting;
    
    private const int PlayerSize = 4;
    private const int DoorSize = 4;
    
    private TileNode currentTile;
    private Keys[] inputs = { Keys.W, Keys.S, Keys.A, Keys.D, Keys.K, Keys.L, Keys.R };
    private bool lerping;
    private Vector3 lerpFrom;
    private Vector3 lerpTo;
    private TileNode? finishLerp;
    private TileNode? lerpToFinishLerp;
    private Direction finishLerpDir;
    private float speed;
    private float lerpT;
    private Entity? doorBlue;
    private Entity? doorRed;
    private bool placingDoor;
    private bool placingBlueDoor;
    private string levelName;
    private Sound sound;

    public Player(float speed, string levelName)
    {
        this.speed = speed;
        placingDoor = false;
        lerpT = 0;
        lerping = false;
        Waiting = false;
        currentTile = null;
        finishLerp = null;
        lerpToFinishLerp = null;
        lerpFrom = new Vector3();
        lerpTo = new Vector3();
        finishLerpDir = Direction.NONE;
        this.levelName = levelName;
    }

    public void SetTileNode(TileNode node)
    {
        currentTile = node;
        var tilePos = currentTile.Transform.Position;
        Transform.Position = tilePos;
        //Transform.Position.Y -= 9;
        Transform.Position.Z = 10;
        // other stuff if matters
    }

    private void LerpSetTileNode(TileNode node)
    {
        currentTile = node;
        var tilePos = currentTile.Transform.Position;
        lerping = true;
        lerpFrom = Transform.Position;
        lerpTo = new Vector3(tilePos.X, tilePos.Y, 10);
        // other stuff if matters
    }

    private void CheckMove(Direction direction)
    {
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
                            sound.Play("doorenter");
                        }
                    }
                    else if (node.Occupant.HasComponent<Crate>())
                    {
                        CrateMove(direction, node);
                    }
                }
                break;
            case TileType.WALL:
                break;
            case TileType.START:
                break;
            case TileType.ONEWAY_NORTH:
                PlaySmokeAnim();
                if (node.North.Occupant != null && node.North.Occupant.HasComponent<Crate>())    
                    CrateMove(direction, node.North);
                LerpSetTileNode(node);
                //lerpToFinishLerp = node.North;
                finishLerpDir = Direction.NORTH;
                sound.Play("oneway");
                break;
            case TileType.ONEWAY_SOUTH:
                PlaySmokeAnim();
                if (node.South.Occupant != null && node.South.Occupant.HasComponent<Crate>())    
                    CrateMove(direction, node.South);
                LerpSetTileNode(node);
                //lerpToFinishLerp = node.South;
                finishLerpDir = Direction.SOUTH;
                sound.Play("oneway");
                break;
            case TileType.ONEWAY_WEST:
                PlaySmokeAnim();
                if (node.West.Occupant != null && node.West.Occupant.HasComponent<Crate>())    
                    CrateMove(direction, node.West);
                LerpSetTileNode(node);
                //lerpToFinishLerp = node.West;
                finishLerpDir = Direction.WEST;
                sound.Play("oneway");
                break;
            case TileType.ONEWAY_EAST:
                PlaySmokeAnim();
                if (node.East.Occupant != null && node.East.Occupant.HasComponent<Crate>())    
                    CrateMove(direction, node.East);
                LerpSetTileNode(node);
                //lerpToFinishLerp = node.East;
                finishLerpDir = Direction.EAST;
                sound.Play("oneway");
                break;
            case TileType.BUTTON_UP:
                PlaySmokeAnim();
                if (node.Occupant == null)    
                {
                    LerpSetTileNode(node);
                    TriggerSwitchDoors();
                    node.ChangeType(TileType.BUTTON_DOWN);
                    sound.Play("switch");
                }
                break;
            case TileType.BUTTON_DOWN:
                PlaySmokeAnim();

                if (node.Occupant != null && node.Occupant.HasComponent<Crate>())    
                    CrateMove(direction, node);
                else
                    LerpSetTileNode(node);
                break;
            
            case TileType.BLUE_BUTTON_UP:
                PlaySmokeAnim();
                if (node.Occupant == null)    
                {
                    LerpSetTileNode(node);
                    TriggerSwitchBDoors();
                    node.ChangeType(TileType.BLUE_BUTTON_DOWN);
                    sound.Play("switch");
                }
                break;
            case TileType.BLUE_BUTTON_DOWN:
                PlaySmokeAnim();

                if (node.Occupant != null && node.Occupant.HasComponent<Crate>())    
                    CrateMove(direction, node);
                else
                    LerpSetTileNode(node);
                break;
            case TileType.REGULAR_DOOR:
                LerpSetTileNode(node);
                sound.Play("leveldoor");
                levelName = (Int32.Parse(levelName)+1).ToString();
                switch (levelName)
                {
                    case "3":
                        Program.GlobalMusic.Stop();
                        GameStateManager.AddScreen(new CutsceneOne());
                        break;
                    case "5":
                        Program.GlobalMusic.Stop();
                        GameStateManager.AddScreen(new CutsceneTwo());
                        break;
                    case "7":
                        Program.GlobalMusic.Stop();
                        GameStateManager.AddScreen(new CutsceneThree());
                        break;
                    case "9":
                        Program.GlobalMusic.Stop();
                        GameStateManager.AddScreen(new CutsceneFour());
                        break;
                    case "11":
                        Program.GlobalMusic.Stop();
                        GameStateManager.AddScreen(new CutsceneFive());
                        break;
                    default:
                        GameStateManager.AddScreen(new LevelScene(levelName, true));
                        break;
                }
                break;
            case TileType.LEVER_LEFT:
                //PlaySmokeAnim();
                node.ChangeType(TileType.LEVER_RIGHT);
                TriggerSwitchDoors();
                sound.Play("switch");
                break;
            case TileType.LEVER_RIGHT:
                //PlaySmokeAnim();
                node.ChangeType(TileType.LEVER_LEFT);
                TriggerSwitchDoors();
                sound.Play("switch");
                break;
            case TileType.SWITCH_DOOR_OPEN:
                PlaySmokeAnim();
                if (node.Occupant != null && node.Occupant.HasComponent<Crate>())    
                    CrateMove(direction, node);
                else
                    LerpSetTileNode(node);
                break;
            case TileType.B_SWITCHDOOR_OPEN:
                PlaySmokeAnim();
                if (node.Occupant != null && node.Occupant.HasComponent<Crate>())    
                    CrateMove(direction, node);
                else
                    LerpSetTileNode(node);
                break;
            case TileType.KEY:
                PlaySmokeAnim();
                LerpSetTileNode(node);
                node.ChangeType(TileType.FLOOR);
                var temp = ParentScene.FindWithTag("TileManager").GetComponent<TileManager>();
                foreach(TileNode tile in temp.AllNodes){
                    if(tile.Type == TileType.LOCKED_DOOR) {
                        tile.ChangeType(TileType.REGULAR_DOOR);
                    }
                }
                sound.Play("key");
                break;
            case TileType.BLUE_LEVER_LEFT:
                //PlaySmokeAnim();
                node.ChangeType(TileType.BLUE_LEVER_RIGHT);
                TriggerSwitchBDoors();
                sound.Play("switch");
                break;
            case TileType.BLUE_LEVER_RIGHT:
                //PlaySmokeAnim();
                node.ChangeType(TileType.BLUE_LEVER_LEFT);
                TriggerSwitchBDoors();
                sound.Play("switch");
                break;
            case TileType.CRATEHOLE_FILLED:
                PlaySmokeAnim();
                if (node.Occupant != null && node.Occupant.HasComponent<Crate>())    
                    CrateMove(direction, node);
                else
                    LerpSetTileNode(node);
                break;
            case TileType.SIDEWALK: 
                PlaySmokeAnim();
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

    private void CrateMove(Direction direction, TileNode node){
        var crate = node.Occupant.GetComponent<Crate>();
        TileNode? crateTile = null;
        switch (direction)
        {
            case Direction.NORTH:
                crateTile = node.North;
                break;
            case Direction.EAST:
                crateTile = node.East;
                break;
            case Direction.SOUTH:
                crateTile = node.South;
                break;
            case Direction.WEST:
                crateTile = node.West;
                break;
        } 
        crate.CheckMove(direction);
        sound.Play("cratemove");
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

    private void PlaySmokeAnim()
    {
        var smokeEntity = new Entity();
        smokeEntity.Transform.Position = Transform.Position;
        smokeEntity.Transform.Scale = new Vector2(DoorSize, DoorSize);
        smokeEntity.Transform.Position.Y += 7;
        smokeEntity.Transform.Position.Z = 11;
        smokeEntity.AddComponent(new Sprite("./Content/Sprites/empty.png"));
        smokeEntity.AddComponent(new Animation(true));
        smokeEntity.GetComponent<Animation>().LoadTextureAtlas("./Content/Sprites/smokeground-Sheet.png", "groundsmoke", .08f, (16, 16));
        smokeEntity.GetComponent<Animation>().SetState("groundsmoke", false);
        ParentScene.AddEntity(smokeEntity);
        sound.Play("dash");
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
        if (placingBlueDoor)
            conceptDoor.AddComponent(new Sprite("./Content/Sprites/conceptualdoor.png"));
        else
            conceptDoor.AddComponent(new Sprite("./Content/Sprites/conceptualdoor_red.png"));
        conceptDoor.Transform.Position = node.Transform.Position;
        conceptDoor.Transform.Scale = new Vector2(DoorSize, DoorSize);
        conceptDoor.Transform.Position.Z = 9;
        ParentScene.AddEntity(conceptDoor);
    }

    private void PlacePortal(TileNode destination)
    {
        Entity door = new Entity();
        door.AddComponent(new Sprite("./Content/Sprites/empty.png"));
        door.AddComponent(new Animation());
        if (placingBlueDoor) {
            door.AddComponent(new Door(destination, doorRed?.GetComponent<Door>()));
            door.GetComponent<Animation>().LoadTextureAtlas("./Content/Sprites/doorBlue-Sheet.png", "doorOpen", .08f, (16, 16));
        }
        else {
            door.AddComponent(new Door(destination, doorBlue?.GetComponent<Door>()));
            door.GetComponent<Animation>().LoadTextureAtlas("./Content/Sprites/doorRed-Sheet.png", "doorOpen", .08f, (16, 16));
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
        sound.Play("doormake");
    }

    public override void Initialize()
    {
        base.Initialize();
        sound = ParentEntity.GetComponent<Sound>();
        Transform.Scale = new Vector2(PlayerSize, PlayerSize);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (!lerping && !Waiting)
        {
            // Movement
            if (InputManager.GetKeyDown(inputs[0]))
            {
                if (placingDoor)
                    PlaceDoor(Direction.NORTH);
                
                else
                    CheckMove(Direction.NORTH);
            }

            else if (InputManager.GetKeyDown(inputs[1]))
            {
                if (placingDoor)
                    PlaceDoor(Direction.SOUTH);
                
                else
                    CheckMove(Direction.SOUTH);
            }

            else if (InputManager.GetKeyDown(inputs[2]))
            {
                if (placingDoor)
                    PlaceDoor(Direction.WEST);
                else {
                    CheckMove(Direction.WEST);
                    ParentEntity.Transform.Scale.X = -Math.Abs(ParentEntity.Transform.Scale.X);
                }
            }

            else if (InputManager.GetKeyDown(inputs[3]))
            {
                if (placingDoor)
                    PlaceDoor(Direction.EAST);
                else {
                    CheckMove(Direction.EAST);
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
            else if (InputManager.GetKeyDown(inputs[6])) 
            {
                sound.Play("reset");
                if (levelName != "0")
                    GameStateManager.SwapScreen(new LevelScene(levelName, false));
                else
                    GameStateManager.SwapScreen(new IntroScene(false));
            }
        }
        else if (lerping)
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
                if (lerpToFinishLerp != null)
                {
                    LerpSetTileNode(lerpToFinishLerp);
                    lerpToFinishLerp = null;
                }

                if (finishLerpDir != Direction.NONE)
                {
                    var tmp = (Direction)(int)finishLerpDir;
                    finishLerpDir = Direction.NONE;
                    CheckMove(tmp);
                }
                
            }
        }

    }
}