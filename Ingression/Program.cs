using GramEngine.Core;
using GramEngine.ECS;
using GramEngine.ECS.Components;
using Ingression;
internal class Program
{
    public static Sound GlobalMusic;
    static void Main(string[] args)
    {
        
        // super cringe hack but holy fuck I dont have time
        var musicEntity = new Entity();
        musicEntity.AddComponent(new Sound("./Content/Sound/puzzlemusic.wav", "music"));
        GlobalMusic = musicEntity.GetComponent<Sound>();
        GlobalMusic.Loop = true;
        
        WindowSettings windowSettings = new WindowSettings()
        {
            NaiveCollision = true,
            WindowTitle = "Ingression",
            Width = 1280,
            Height = 720
        };
        Window window = new Window(new TitleScreen(), windowSettings);
        window.Run();

    }
}