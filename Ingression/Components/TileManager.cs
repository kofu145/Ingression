using GramEngine.ECS;

namespace Ingression.Components;

public class TileManager : Component
{
    private TileNode head;

    public TileManager()
    {
        
    }

    public void ConstructFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            var lines = File.ReadLines(filePath);
            TileNode[ , ] tiles = new TileNode[lines.Count(), lines.First().Count()];

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
                        tiles[lineNumber, position] = new TileNode(tile);
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
                        // check up
                        if ((i - 1) >= 0 && tiles[i - 1, j] != null) {
                            tiles[i, j].North = tiles[i - 1, j];
                        }
                        // check down
                        if ((i + 1) <= tiles.GetLength(0) && tiles[i + 1, j] != null) {
                            tiles[i, j].South = tiles[i + 1, j];
                        }
                        // check left
                        if ((j - 1) >= 0 && tiles[i, j - 1] != null) {
                            tiles[i, j].West = tiles[i, j - 1];
                        } 
                        // check right
                        if ((j + 1) <= tiles.GetLength(1) && tiles[i, j + 1] != null) {
                            tiles[i, j].East = tiles[i, j + 1];
                        }
                    }
                }
            }
        } 
    }
}