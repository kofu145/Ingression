using GramEngine.Core;
using Ingression;
internal class Program
{
    static void Main(string[] args)
    {
        WindowSettings windowSettings = new WindowSettings()
        {
            NaiveCollision = true,
            WindowTitle = "Pong demo",
            Width = 1280,
            Height = 720
        };
        Window window = new Window(new MainScene(), windowSettings);
        window.Run();

    }
}