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
            Width = 600,
            Height = 400
        };
        Window window = new Window(new MainScene(), windowSettings);
        window.Run();

    }
}