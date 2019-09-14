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
/// The menu controller handles the drawing and user interactions
/// from the menus in the game. These include the main menu, game
/// menu and the settings m,enu.
/// </summary>

namespace SpaceBattle
{
    class MenuController
    {
        private static MenuController instance = null;

        MenuController() { }

        public static MenuController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MenuController();
                }
                return instance;
            }
        }

        /// <summary>
        /// The menu structure for the game.
        /// </summary>
        /// <remarks>
        /// These are the text captions for the menu items.
        /// </remarks>
        private readonly string[][] _menuStructure = new[] { new string[] { "PLAY", "SETUP", "SCORES", "QUIT" }, new string[] { "RETURN", "SURRENDER", "QUIT" }, new string[] { "EASY", "MEDIUM", "HARD" } };

        private const int MENU_TOP = 575;
        private const int MENU_LEFT = 30;
        private const int MENU_GAP = 0;
        private const int BUTTON_WIDTH = 75;
        private const int BUTTON_HEIGHT = 15;
        private const int BUTTON_SEP = BUTTON_WIDTH + MENU_GAP;
        private const int TEXT_OFFSET = 0;

        private const int MAIN_MENU = 0;
        private const int GAME_MENU = 1;
        private const int SETUP_MENU = 2;

        private const int MAIN_MENU_PLAY_BUTTON = 0;
        private const int MAIN_MENU_SETUP_BUTTON = 1;
        private const int MAIN_MENU_TOP_SCORES_BUTTON = 2;
        private const int MAIN_MENU_QUIT_BUTTON = 3;

        private const int SETUP_MENU_EASY_BUTTON = 0;
        private const int SETUP_MENU_MEDIUM_BUTTON = 1;
        private const int SETUP_MENU_HARD_BUTTON = 2;
        private const int SETUP_MENU_EXIT_BUTTON = 3;

        private const int GAME_MENU_RETURN_BUTTON = 0;
        private const int GAME_MENU_SURRENDER_BUTTON = 1;
        private const int GAME_MENU_QUIT_BUTTON = 2;

        private readonly Color MENU_COLOR = SwinGame.RGBAColor(2, 167, 252, 255);
        private readonly Color HIGHLIGHT_COLOR = SwinGame.RGBAColor(1, 57, 86, 255);

        /// <summary>
        /// Handles the processing of user input when the main menu is showing
        /// </summary>
        public void HandleMainMenuInput()
        {
            HandleMenuInput(MAIN_MENU, 0, 0);
        }

        /// <summary>
        /// Handles the processing of user input when the main menu is showing
        /// </summary>
        public void HandleSetupMenuInput()
        {
            bool handled;
            handled = HandleMenuInput(SETUP_MENU, 1, 1);

            if (!handled)
                HandleMenuInput(MAIN_MENU, 0, 0);
        }

        /// <summary>
        /// Handle input in the game menu.
        /// </summary>
        /// <remarks>
        /// Player can return to the game, surrender, or quit entirely
        /// </remarks>
        public void HandleGameMenuInput()
        {
            HandleMenuInput(GAME_MENU, 0, 0);
        }

        /// <summary>
        /// Handles input for the specified menu.
        /// </summary>
        /// <param name="menu">the identifier of the menu being processed</param>
        /// <param name="level">the vertical level of the menu</param>
        /// <param name="xOffset">the xoffset of the menu</param>
        /// <returns>false if a clicked missed the buttons. This can be used to check prior menus.</returns>
        private bool HandleMenuInput(int menu, int level, int xOffset)
        {
            if (SwinGame.KeyTyped(KeyCode.EscapeKey))
            {
                GameController.Instance.EndCurrentState();
                return true;
            }

            if (SwinGame.MouseClicked(MouseButton.LeftButton))
            {
                int i;
                for (i = 0; i <= _menuStructure[menu].Length - 1; i++)
                {
                    // IsMouseOver the i'th button of the menu
                    if (IsMouseOverMenu(i, level, xOffset))
                    {
                        PerformMenuAction(menu, i);
                        return true;
                    }
                }

                if (level > 0)
                    // none clicked - so end this sub menu
                    GameController.Instance.EndCurrentState();
            }

            return false;
        }

        /// <summary>
        /// Draws the main menu to the screen.
        /// </summary>
        public void DrawMainMenu()
        {
            // Clears the Screen to Black
            // SwinGame.DrawText("Main Menu", Color.White, GameFont("ArialLarge"), 50, 50)

            DrawButtons(MAIN_MENU);
        }

        /// <summary>
        /// Draws the Game menu to the screen
        /// </summary>
        public void DrawGameMenu()
        {
            // Clears the Screen to Black
            // SwinGame.DrawText("Paused", Color.White, GameFont("ArialLarge"), 50, 50)

            DrawButtons(GAME_MENU);
        }

        /// <summary>
        /// Draws the settings menu to the screen.
        /// </summary>
        /// <remarks>
        /// Also shows the main menu
        /// </remarks>
        public void DrawSettings()
        {
            // Clears the Screen to Black
            // SwinGame.DrawText("Settings", Color.White, GameFont("ArialLarge"), 50, 50)

            DrawButtons(MAIN_MENU);
            DrawButtons(SETUP_MENU, 1, 1);
        }

        /// <summary>
        /// Draw the buttons associated with a top level menu.
        /// </summary>
        /// <param name="menu">the index of the menu to draw</param>
        private void DrawButtons(int menu)
        {
            DrawButtons(menu, 0, 0);
        }

        /// <summary>
        /// Draws the menu at the indicated level.
        /// </summary>
        /// <param name="menu">the menu to draw</param>
        /// <param name="level">the level (height) of the menu</param>
        /// <param name="xOffset">the offset of the menu</param>
        /// <remarks>
        /// The menu text comes from the _menuStructure field. The level indicates the height
        /// of the menu, to enable sub menus. The xOffset repositions the menu horizontally
        /// to allow the submenus to be positioned correctly.
        /// </remarks>
        private void DrawButtons(int menu, int level, int xOffset)
        {
            int btnTop;
            Rectangle toDraw = new Rectangle();

            btnTop = MENU_TOP - (MENU_GAP + BUTTON_HEIGHT) * level;
            int i;
            for (i = 0; i <= _menuStructure[menu].Length - 1; i++)
            {
                int btnLeft;

                btnLeft = MENU_LEFT + BUTTON_SEP * (i + xOffset);
                // SwinGame.FillRectangle(Color.White, btnLeft, btnTop, BUTTON_WIDTH, BUTTON_HEIGHT)
                toDraw.X = btnLeft + TEXT_OFFSET;
                toDraw.Y = btnTop + TEXT_OFFSET;
                toDraw.Width = BUTTON_WIDTH;
                toDraw.Height = BUTTON_HEIGHT;
                // -changed? - SwinGame.DrawText(_menuStructure[menu](i), MENU_COLOR, Color.Black, GameResources.Instance.GameFont("Menu"), FontAlignment.AlignCenter, toDraw);
                SwinGame.DrawText(_menuStructure[menu][i], MENU_COLOR, Color.Black, GameResources.Instance.GameFont("Menu"), FontAlignment.AlignCenter, toDraw);

                if (SwinGame.MouseDown(MouseButton.LeftButton) & IsMouseOverMenu(i, level, xOffset))
                    SwinGame.DrawRectangle(HIGHLIGHT_COLOR, btnLeft, btnTop, BUTTON_WIDTH, BUTTON_HEIGHT);
            }
        }

        /// <summary>
        /// Determined if the mouse is over one of the button in the main menu.
        /// </summary>
        /// <param name="button">the index of the button to check</param>
        /// <returns>true if the mouse is over that button</returns>
        private bool IsMouseOverButton(int button)
        {
            return IsMouseOverMenu(button, 0, 0);
        }

        /// <summary>
        /// Checks if the mouse is over one of the buttons in a menu.
        /// </summary>
        /// <param name="button">the index of the button to check</param>
        /// <param name="level">the level of the menu</param>
        /// <param name="xOffset">the xOffset of the menu</param>
        /// <returns>true if the mouse is over the button</returns>
        private bool IsMouseOverMenu(int button, int level, int xOffset)
        {
            int btnTop = MENU_TOP - (MENU_GAP + BUTTON_HEIGHT) * level;
            int btnLeft = MENU_LEFT + BUTTON_SEP * (button + xOffset);

            return UtilityFunctions.IsMouseInRectangle(btnLeft, btnTop, BUTTON_WIDTH, BUTTON_HEIGHT);
        }

        /// <summary>
        /// A button has been clicked, perform the associated action.
        /// </summary>
        /// <param name="menu">the menu that has been clicked</param>
        /// <param name="button">the index of the button that was clicked</param>
        private void PerformMenuAction(int menu, int button)
        {
            switch (menu)
            {
                case MAIN_MENU:
                    {
                        PerformMainMenuAction(button);
                        break;
                    }

                case SETUP_MENU:
                    {
                        PerformSetupMenuAction(button);
                        break;
                    }

                case GAME_MENU:
                    {
                        PerformGameMenuAction(button);
                        break;
                    }
            }
        }

        /// <summary>
        /// The main menu was clicked, perform the button's action.
        /// </summary>
        /// <param name="button">the button pressed</param>
        private void PerformMainMenuAction(int button)
        {
            switch (button)
            {
                case MAIN_MENU_PLAY_BUTTON:
                    {
                        GameController.Instance.StartGame();
                        break;
                    }

                case MAIN_MENU_SETUP_BUTTON:
                    {
                        GameController.Instance.AddNewState(GameState.AlteringSettings);
                        break;
                    }

                case MAIN_MENU_TOP_SCORES_BUTTON:
                    {
                        GameController.Instance.AddNewState(GameState.ViewingHighScores);
                        break;
                    }

                case MAIN_MENU_QUIT_BUTTON:
                    {
                        GameController.Instance.EndCurrentState();
                        break;
                    }
            }
        }

        /// <summary>
        /// The setup menu was clicked, perform the button's action.
        /// </summary>
        /// <param name="button">the button pressed</param>
        private void PerformSetupMenuAction(int button)
        {
            switch (button)
            {
                case SETUP_MENU_EASY_BUTTON:
                    {
                        GameController.Instance.SetDifficulty(AIOption.Hard);
                        break;
                    }

                case SETUP_MENU_MEDIUM_BUTTON:
                    {
                        GameController.Instance.SetDifficulty(AIOption.Hard);
                        break;
                    }

                case SETUP_MENU_HARD_BUTTON:
                    {
                        GameController.Instance.SetDifficulty(AIOption.Hard);
                        break;
                    }
            }
            // Always end state - handles exit button as well
            GameController.Instance.EndCurrentState();
        }

        /// <summary>
        /// The game menu was clicked, perform the button's action.
        /// </summary>
        /// <param name="button">the button pressed</param>
        private void PerformGameMenuAction(int button)
        {
            switch (button)
            {
                case GAME_MENU_RETURN_BUTTON:
                    {
                        GameController.Instance.EndCurrentState();
                        break;
                    }

                case GAME_MENU_SURRENDER_BUTTON:
                    {
                        GameController.Instance.EndCurrentState(); // end game menu
                        GameController.Instance.EndCurrentState(); // end game
                        break;
                    }

                case GAME_MENU_QUIT_BUTTON:
                    {
                        GameController.Instance.AddNewState(GameState.Quitting);
                        break;
                    }
            }
        }
    }
}