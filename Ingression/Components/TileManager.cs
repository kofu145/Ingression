using System.Numerics;
using GramEngine.ECS;

namespace Ingression.Components;

public class TileManager : Component
{
    public TileNode? Head { get; private set; }

    public TileManager()
    {
        Head = null;
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
                        tiles[lineNumber, position] = new TileNode(tile - '0');
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
                        if (Head == null)
                        {
                            Head = tiles[i, j];
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
                        
                        // create new entity
                        Entity newTile = new Entity();
                        // new entity.addcomponent tiles[i,j] (add our tile component to entity)
                        newTile.AddComponent(tiles[i, j]);
                        // add entity to ParentScene through ParentScene.AddEntity();
                        ParentScene.AddEntity(newTile);
                        // set the entity's position, relative to graph (offset it using i and j)

                        newTile.Transform.Scale = new Vector2(4f, 4f);
                        newTile.Transform.Position = new Vector3(j * 15*4 + 100, i * 15*4 + 100, 1f);
                        if (tiles[i, j].Type == TileType.WALL)
                        {
                            newTile.Transform.Position.Z = 100f;
                        }
                    }

                    
                }
            }
        } 
    }
}