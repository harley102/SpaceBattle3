using SwinGameSDK;
using System;

namespace SpaceBattle
{
    public class GameLogic
    {
        public static void Main()
        {
            // Opens a new Graphics Window
            SwinGame.OpenGraphicsWindow("Battle Ships", 800, 600);

            // Load Resources
            GameResources.Instance.LoadResources();

            SwinGame.PlayMusic(GameResources.Instance.GameMusic("Background"));
            // Game Loop
            do
            {
                GameController.Instance.HandleUserInput();
                GameController.Instance.DrawScreen();
            }
            while (!SwinGame.WindowCloseRequested() == true && GameController.Instance.CurrentState != GameState.Quitting);
            SwinGame.StopMusic();
            // Free Resources and Close Audio, to end the program.
            GameResources.Instance.FreeResources();
        }
    }
}