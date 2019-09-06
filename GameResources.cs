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

namespace SpaceBattle
{
    public class GameResources
    {
        private static GameResources instance = null;

        GameResources() { }

        public static GameResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameResources();
                }
                return instance;
            }
        }

        private void LoadFonts()
        {
            NewFont("ArialLarge", "arial.ttf", 80);
            NewFont("Courier", "cour.ttf", 14);
            NewFont("CourierSmall", "cour.ttf", 8);
            NewFont("Menu", "ffaccess.ttf", 8);
        }

        private void LoadImages()
        {
            // Backgrounds
            NewImage("Menu", "main_page.jpg");
            NewImage("Discovery", "discover.jpg");
            NewImage("Deploy", "deploy.jpg");

            // Deployment
            NewImage("LeftRightButton", "deploy_dir_button_horiz.png");
            NewImage("UpDownButton", "deploy_dir_button_vert.png");
            NewImage("SelectedShip", "deploy_button_hl.png");
            NewImage("PlayButton", "deploy_play_button.png");
            NewImage("RandomButton", "deploy_randomize_button.png");

            // Ships
            int i;
            for (i = 1; i <= 5; i++)
            {
                NewImage("ShipLR" + i, "ship_deploy_horiz_" + i + ".png");
                NewImage("ShipUD" + i, "ship_deploy_vert_" + i + ".png");
            }

            // Explosions
            NewImage("Explosion", "explosion.png");
            NewImage("Splash", "splash.png");
        }

        private void LoadSounds()
        {
            NewSound("Error", "error.wav");
            NewSound("Hit", "hit.wav");
            NewSound("Sink", "sink.wav");
            NewSound("Siren", "siren.wav");
            NewSound("Miss", "watershot.wav");
            NewSound("Winner", "winner.wav");
            NewSound("Lose", "lose.wav");
        }

        private void LoadMusic()
        {
            NewMusic("Background", "horrordrone.mp3");
        }

        /// <summary>
        ///     ''' Gets a Font Loaded in the Resources
        ///     ''' </summary>
        ///     ''' <param name="font">Name of Font</param>
        ///     ''' <returns>The Font Loaded with this Name</returns>

        public Font GameFont(string font)
        {
            return _Fonts[font];
        }

        /// <summary>
        ///     ''' Gets an Image loaded in the Resources
        ///     ''' </summary>
        ///     ''' <param name="image">Name of image</param>
        ///     ''' <returns>The image loaded with this name</returns>

        public Bitmap GameImage(string image)
        {
            return _Images[image];
        }

        /// <summary>
        ///     ''' Gets an sound loaded in the Resources
        ///     ''' </summary>
        ///     ''' <param name="sound">Name of sound</param>
        ///     ''' <returns>The sound with this name</returns>

        public SoundEffect GameSound(string sound)
        {
            return _Sounds[sound];
        }

        /// <summary>
        ///     ''' Gets the music loaded in the Resources
        ///     ''' </summary>
        ///     ''' <param name="music">Name of music</param>
        ///     ''' <returns>The music with this name</returns>

        public Music GameMusic(string music)
        {
            return _Music[music];
        }

        private Dictionary<string, Bitmap> _Images = new Dictionary<string, Bitmap>();
        private Dictionary<string, Font> _Fonts = new Dictionary<string, Font>();
        private Dictionary<string, SoundEffect> _Sounds = new Dictionary<string, SoundEffect>();
        private Dictionary<string, Music> _Music = new Dictionary<string, Music>();

        private Bitmap _Background;
        private Bitmap _Animation;
        private Bitmap _LoaderFull;
        private Bitmap _LoaderEmpty;
        private Font _LoadingFont;
        private SoundEffect _StartSound;

        /// <summary>
        ///     ''' The Resources Class stores all of the Games Media Resources, such as Images, Fonts
        ///     ''' Sounds, Music.
        ///     ''' </summary>

        public void LoadResources()
        {
            int width, height;

            width = SwinGame.ScreenWidth();
            height = SwinGame.ScreenHeight();

            SwinGame.ChangeScreenSize(800, 600);

            ShowLoadingScreen();

            ShowMessage("Loading fonts...", 0);
            LoadFonts();
            SwinGame.Delay(100);

            ShowMessage("Loading images...", 1);
            LoadImages();
            SwinGame.Delay(100);

            ShowMessage("Loading sounds...", 2);
            LoadSounds();
            SwinGame.Delay(100);

            ShowMessage("Loading music...", 3);
            LoadMusic();
            SwinGame.Delay(100);

            SwinGame.Delay(100);
            ShowMessage("Game loaded...", 5);
            SwinGame.Delay(100);
            EndLoadingScreen(width, height);
        }

        private void ShowLoadingScreen()
        {
            _Background = SwinGame.LoadBitmap(SwinGame.PathToResource("SplashBack.png", ResourceKind.BitmapResource));
            SwinGame.DrawBitmap(_Background, 0, 0);
            SwinGame.RefreshScreen();
            SwinGame.ProcessEvents();

            _Animation = SwinGame.LoadBitmap(SwinGame.PathToResource("SwinGameAni.jpg", ResourceKind.BitmapResource));
            _LoadingFont = SwinGame.LoadFont(SwinGame.PathToResource("arial.ttf", ResourceKind.FontResource), 12);
            _StartSound = Audio.LoadSoundEffect(SwinGame.PathToResource("SwinGameStart.ogg", ResourceKind.SoundResource));

            _LoaderFull = SwinGame.LoadBitmap(SwinGame.PathToResource("loader_full.png", ResourceKind.BitmapResource));
            _LoaderEmpty = SwinGame.LoadBitmap(SwinGame.PathToResource("loader_empty.png", ResourceKind.BitmapResource));

            PlaySwinGameIntro();
        }

        private void PlaySwinGameIntro()
        {
            const int ANI_CELL_COUNT = 11;

            Audio.PlaySoundEffect(_StartSound);
            SwinGame.Delay(200);

            int i;
            for (i = 0; i <= ANI_CELL_COUNT - 1; i++)
            {
                SwinGame.DrawBitmap(_Background, 0, 0);
                SwinGame.Delay(20);
                SwinGame.RefreshScreen();
                SwinGame.ProcessEvents();
            }

            SwinGame.Delay(1500);
        }

        private void ShowMessage(string message, int number)
        {
            const int TX = 310;
            const int TY = 493;
            const int TW = 200;
            const int TH = 25;
            const int STEPS = 5;
            const int BG_X = 279;
            const int BG_Y = 453;

            int fullW;
            Rectangle toDraw = new Rectangle();

            fullW = 260 * number / STEPS;
            SwinGame.DrawBitmap(_LoaderEmpty, BG_X, BG_Y);
            SwinGame.DrawCell(_LoaderFull, 0, BG_X, BG_Y);
            // SwinGame.DrawBitmapPart(_LoaderFull, 0, 0, fullW, 66, BG_X, BG_Y)

            toDraw.X = TX;
            toDraw.Y = TY;
            toDraw.Width = TW;
            toDraw.Height = TH;
            SwinGame.DrawText(message, Color.White, Color.Transparent, _LoadingFont, FontAlignment.AlignCenter, toDraw);
            // SwinGame.DrawTextLines(message, Color.White, Color.Transparent, _LoadingFont, FontAlignment.AlignCenter, TX, TY, TW, TH)

            SwinGame.RefreshScreen();
            SwinGame.ProcessEvents();
        }

        private void EndLoadingScreen(int width, int height)
        {
            SwinGame.ProcessEvents();
            SwinGame.Delay(500);
            SwinGame.ClearScreen();
            SwinGame.RefreshScreen();
            SwinGame.FreeFont(_LoadingFont);
            SwinGame.FreeBitmap(_Background);
            SwinGame.FreeBitmap(_Animation);
            SwinGame.FreeBitmap(_LoaderEmpty);
            SwinGame.FreeBitmap(_LoaderFull);
            //SwinGame.FreeSoundEffect(_StartSound);
            //Audio.FreeSoundEffect
            SwinGame.ChangeScreenSize(width, height);
        }

        private void NewFont(string fontName, string filename, int size)
        {
            _Fonts.Add(fontName, SwinGame.LoadFont(SwinGame.PathToResource(filename, ResourceKind.FontResource), size));
        }

        private void NewImage(string imageName, string filename)
        {
            _Images.Add(imageName, SwinGame.LoadBitmap(SwinGame.PathToResource(filename, ResourceKind.BitmapResource)));
        }

        private void NewTransparentColorImage(string imageName, string fileName, Color transColor)
        {
            _Images.Add(imageName, SwinGame.LoadBitmap(SwinGame.PathToResource(fileName, ResourceKind.BitmapResource)));
        }

        private void NewTransparentColourImage(string imageName, string fileName, Color transColor)
        {
            NewTransparentColorImage(imageName, fileName, transColor);
        }

        private void NewSound(string soundName, string filename)
        {
            _Sounds.Add(soundName, Audio.LoadSoundEffect(SwinGame.PathToResource(filename, ResourceKind.SoundResource)));
        }

        private void NewMusic(string musicName, string filename)
        {
            _Music.Add(musicName, Audio.LoadMusic(SwinGame.PathToResource(filename, ResourceKind.SoundResource)));
        }

        private void FreeFonts()
        {
            foreach (Font obj in _Fonts.Values)
                SwinGame.FreeFont(obj);
        }

        private void FreeImages()
        {
            foreach (Bitmap obj in _Images.Values)
                SwinGame.FreeBitmap(obj);
        }

        private void FreeSounds()
        {
            foreach (SoundEffect obj in _Sounds.Values)
                Audio.FreeSoundEffect(obj);
        }

        private void FreeMusic()
        {
            foreach (Music obj in _Music.Values)
                Audio.FreeMusic(obj);
        }

        public void FreeResources()
        {
            FreeFonts();
            FreeImages();
            FreeMusic();
            FreeSounds();
            SwinGame.ProcessEvents();
        }
    }
}