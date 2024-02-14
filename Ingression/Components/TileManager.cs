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
            
            while ((line = Textfile.ReadLine()) != null)
            {
                var row = line.ToCharArray();
                foreach (char tile in row)
                {
                    Console.Write(tile);
                }
                Console.WriteLine();

            } 
            Textfile.Close(); 
        } 
    }
}