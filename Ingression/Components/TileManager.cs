using System.Numerics;
using GramEngine.Core;
using GramEngine.ECS;
using GramEngine.ECS.Components;
using Ingression;

namespace Ingression.Components;

public class TileManager : Component
{
    private const int TileSize = 16;
    private const int TileScale = 5;
    public TileNode? Head { get; private set; }
    public List<TileNode> AllNodes;

    public TileManager()
    {
        Head = null;
        AllNodes = new List<TileNode>();
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public void ConstructFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            var lines = File.ReadLines(filePath);
            TileNode?[ , ] tiles = new TileNode[lines.Count(), lines.First().Count()];

            // Reads file line by line 
            StreamReader Textfile = new StreamReader(filePath); 
            string line;
            int lineNumber = 0;
            int position = 0;
            
            while ((line = Textfile.ReadLine()) != null)
            {
                var row = line.ToCharArray();
                foreach (char tile in row)
                {
                    if (tile != ' ') {
                        tiles[lineNumber, position] = new TileNode(MathExtend.Decode(tile.ToString(), 36));
                    }
                    Console.Write(tile);
                    position++;
                }
                Console.WriteLine();
                position = 0;
                lineNumber++;
            } 
            Textfile.Close(); 
            
            for (int i = 0; i < tiles.GetLength(0); i++) {
                for(int j = 0; j < tiles.GetLength(1); j++) {
                    
                    if (tiles[i, j] != null) {
                        if (Head == null && tiles[i, j].Type == TileType.START)
                        {
                            Head = tiles[i, j];
                            tiles[i, j].ChangeValueType(TileType.FLOOR);
                        }
                        else if (Head == null && tiles[i, j].Type == TileType.SIDEWALK)
                        {
                            Head = tiles[i, j];
                            tiles[i, j].ChangeValueType(TileType.SIDEWALK);
                        }
                        if (tiles[i, j].Type == TileType.CRATE)
                        {
                            tiles[i, j].ChangeValueType(TileType.FLOOR);
                            var crate = new Entity();
                            crate.Tag = "crate";
                            tiles[i, j].Occupant = crate;
                            crate.AddComponent(new Sprite("./Content/Sprites/CRATE.png"));
                            crate.AddComponent(new Crate());
                            crate.GetComponent<Crate>().currentTile = tiles[i, j];
                            // crate.Transform.Position = tiles[i, j].ParentEntity.Transform.Position;
                            crate.Transform.Scale = new Vector2(4, 4);

                        }
                        // check up
                        if ((i - 1) >= 0 && tiles[i - 1, j] != null) {
                            tiles[i, j].North = tiles[i - 1, j];
                        }
                        // check down
                        if ((i + 1) < tiles.GetLength(0) && tiles[i + 1, j] != null) {
                            tiles[i, j].South = tiles[i + 1, j];
                        }
                        // check left
                        if ((j - 1) >= 0 && tiles[i, j - 1] != null) {
                            tiles[i, j].West = tiles[i, j - 1];
                        } 
                        // check right
                        if ((j + 1) < tiles.GetLength(1) && tiles[i, j + 1] != null) {
                            tiles[i, j].East = tiles[i, j + 1];
                        }
                        AllNodes.Add(tiles[i, j]);
                        // create new entity
                        Entity newTile = new Entity();
                        // new entity.addcomponent tiles[i,j] (add our tile component to entity)
                        newTile.AddComponent(tiles[i, j]);
                        // add entity to ParentScene through ParentScene.AddEntity();
                        ParentScene.AddEntity(newTile);

                        // set the entity's position, relative to graph (offset it using i and j)
                        newTile.Transform.Scale = new Vector2(TileScale, TileScale);
                        
                        // How did I find the numbers below? I fucked around and found out.
                        newTile.Transform.Position = new Vector3(
                            j * TileSize * TileScale + (float)GameStateManager.Window.settings.BaseWindowWidth / 2 
                            - (tiles.GetLength(1) * TileSize * TileScale) / 2 + TileSize * TileScale / 2, 
                            i * TileSize * TileScale + (float)GameStateManager.Window.settings.BaseWindowHeight / 2
                            - (tiles.GetLength(0) * TileSize * TileScale) / 2 + TileSize * TileScale / 2,
                            1f
                            );
                        if(tiles[i, j].Occupant != null)
                        {
                            tiles[i, j].Occupant.Transform.Position = newTile.Transform.Position;
                            tiles[i, j].Occupant.Transform.Position.Z = 11f;
                            ParentScene.AddEntity(tiles[i, j].Occupant);
                        }
                        if (tiles[i, j].Type == TileType.WALL)
                        {
                            newTile.Transform.Position.Z = 20f;

                        }
                    }

                    
                }
            }
        } 
    }
}