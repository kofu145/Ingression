using GramEngine.Core;
using Ingression;
internal class Program
{
    static void Main(string[] args)
    {
        WindowSettings windowSettings = new WindowSettings()
        {
            NaiveCollision = true,
            WindowTitle = "Ingression",
            Width = 1280,
            Height = 720
        };
        Window window = new Window(new LevelScene("1"), windowSettings);
        window.Run();

    }
}