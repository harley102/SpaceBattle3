using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using SwinGameSDK;

/// <summary>
/// The battle phase is handled by the DiscoveryController.
/// </summary>
namespace SpaceBattle
{
    class DiscoveryController
    {

        /// <summary>
        /// Handles input during the discovery phase of the game.
        /// </summary>
        /// <remarks>
        /// Escape opens the game menu. Clicking the mouse will
        /// attack a location.
        /// </remarks>
        public static void HandleDiscoveryInput()
        {
            if (SwinGame.KeyTyped(KeyCode.EscapeKey))
                GameController.Instance.AddNewState(GameState.ViewingGameMenu);

            if (SwinGame.MouseDown(MouseButton.LeftButton))
                DoAttack();
        }

        /// <summary>
        /// Attack the location that the mouse if over.
        /// </summary>
        private static void DoAttack()
        {
            Point2D mouse;

            mouse = SwinGame.MousePosition();

            // Calculate the row/col clicked
            int row, col;
            row = Convert.ToInt32(Math.Floor((mouse.Y - UtilityFunctions.FIELD_TOP) / (double)(UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP)));
            col = Convert.ToInt32(Math.Floor((mouse.X - UtilityFunctions.FIELD_LEFT) / (double)(UtilityFunctions.CELL_WIDTH + UtilityFunctions.CELL_GAP)));

            if (row >= 0 & row < GameController.Instance.HumanPlayer.EnemyGrid.Height)
            {
                if (col >= 0 & col < GameController.Instance.HumanPlayer.EnemyGrid.Width)
                    GameController.Instance.Attack(row, col);
            }
        }

        /// <summary>
        /// Draws the game during the attack phase.
        /// </summary>s
        public static void DrawDiscovery()
        {
            const int SCORES_LEFT = 172;
            const int SHOTS_TOP = 157;
            const int HITS_TOP = 206;
            const int SPLASH_TOP = 256;

            if ((SwinGame.KeyDown(KeyCode.LeftShiftKey) | SwinGame.KeyDown(KeyCode.RightShiftKey)) & SwinGame.KeyDown(KeyCode.CKey))
                UtilityFunctions.DrawField(GameController.Instance.HumanPlayer.EnemyGrid, GameController.Instance.ComputerPlayer, true);
            else
                UtilityFunctions.DrawField(GameController.Instance.HumanPlayer.EnemyGrid, GameController.Instance.ComputerPlayer, false);

            UtilityFunctions.DrawSmallField(GameController.Instance.HumanPlayer.PlayerGrid, GameController.Instance.HumanPlayer);
            UtilityFunctions.DrawDestoyedField(GameController.Instance.ComputerPlayer.PlayerGrid, GameController.Instance.ComputerPlayer);
            UtilityFunctions.Instance.DrawMessage();

            SwinGame.DrawText(GameController.Instance.HumanPlayer.Shots.ToString(), Color.White, GameResources.Instance.GameFont("Menu"), SCORES_LEFT, SHOTS_TOP);
            SwinGame.DrawText(GameController.Instance.HumanPlayer.Hits.ToString(), Color.White, GameResources.Instance.GameFont("Menu"), SCORES_LEFT, HITS_TOP);
            SwinGame.DrawText(GameController.Instance.HumanPlayer.Missed.ToString(), Color.White, GameResources.Instance.GameFont("Menu"), SCORES_LEFT, SPLASH_TOP);
        }
    }
}