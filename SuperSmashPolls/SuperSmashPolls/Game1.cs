/*******************************************************************************************************************//**
 * This game is now dependent on the Farseer Physics Engine.
 * For information see http://farseerphysics.codeplex.com/
 * @author (For all textures) Joe Brooksbank
 * @note The DEBUG preprocessor enables various debugging views
 * @note The DEBUG_LEVELS preprocessor enables debugging specific to levels
 * @note The OLD_CHARACTER preprocessor enables use of the old (and very deprecated) Character class
 * @note The COMPLEX_BODIES preprocessor enables an experimental (and very slow) system for hitboxes and bodies where 
 * each texture in a spritesheet has its own body (for it getting hit) and hitbox body (for it hitting stuff). Don't
 * expect more than 2 FPS with this
 * @note The MEMES preprocessor enables the map Final Destination, c4u53 m3m35 m8
 **********************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperSmashPolls.Characters;
using SuperSmashPolls.GameItemControl;
using SuperSmashPolls.Graphics;
using SuperSmashPolls.Levels;
using SuperSmashPolls.MenuControl;
using SuperSmashPolls.World_Control;

namespace SuperSmashPolls {

    /// <summary> 
    /// This is the main type of the game.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game {

        /** Handles the different states that the game can be in */
        enum GameState {

            Menu,           //The menu is open
            GameLevel,      //The first level of the game
            ScoreScreen,
            LoadSave,
            SaveGame,

        };

        /** The location to save the settings for the game */
        private const string SettingsLocation = "C:\\Users\\Public\\SmashPollsSettings.txt";
        /** The location to save the game */
        private const string SaveLocation     = "C:\\Users\\Public\\SmashPollsSave.txt";
        /** The name used for the debate level with system events (live saving and loading) */
        private const string DebateSystemName = "DebateRoom";
        /** The total size of the screen */
        private static Vector2 ScreenSize;
        /** The most basic Functioning WorldUnit */
        private readonly WorldUnit EmptyUnit;
        /** The scale of how many pixels are equal to one meter */
        private readonly float PixelToMeterScale;
        /** The moves for all character */
        private readonly MoveDefinition DefinedMoves;
        /** Holds levels for matching from a save and for selection */
        private readonly Dictionary<string, LevelHandler> LevelDictionary;
        /** Holds characters for matching from a save and for selection TODO change to Dictionary */
        private readonly Dictionary<string, CharacterManager> CharacterStringPairs;
        /** The gamemode for this game */
        private Gamemode CurrentGamemode;
#if OLD_CHARACTER
        /** The one, the only, the Donald */
        private Character TheDonald;
        private TheDonaldsMoves TheDonaldsAttacks;
#else
        /** Character based off of Donald Trump */
        private CharacterManager TheDonald;
        /** Character based off of Hillary Clinton */
        private CharacterManager Hillary;
#endif
        /** This is the level currently being played on */
        private LevelHandler CurrentLevel;
        /** Levels for the player to play on */
        private LevelHandler TempleRock, Temple,
#if MEMES
            FinalDestination,
#endif
            Debate, WhiteHouse;
        /** Manages graphics */
        private GraphicsDeviceManager Graphics;
        /** Used to draw multiple 2D textures at one time */
        private SpriteBatch Batch;
        /** A basic font to use for essentially everything in the game */
        private SpriteFont GameFont;
        /** The font used for the title */
        private SpriteFont TitleFont;
        /** A smaller version of the titlefont */
        private SpriteFont TitleFontSmall;
        /** The center of the screen */
        private Vector2 ScreenCenter;
        /** Menu system for the game to use */
        private MenuItem Menu;
        /** The last button pressed during menu updates */
        private GamePadState LastPressed;
        /** The player's in this game */
        private PlayerClass PlayerOne, PlayerTwo, PlayerThree, PlayerFour;
        /** The number of players in the game */
        private int NumPlayers;
        /** Variable to hold the state of the game */
        private GameState State = GameState.Menu;
        

        /// <summary>
        /// Constructs the game's class
        /// </summary>
        public Game1() {

            /* !!! The size of the screen for the game !!! (this should be saved in options) */
            ScreenSize = new Vector2(640, 360);

            LevelDictionary      = new Dictionary<string, LevelHandler>();
            CharacterStringPairs = new Dictionary<string, CharacterManager>();

            DefinedMoves = new MoveDefinition();

            /* This is the player's screen controller */
            Graphics = new GraphicsDeviceManager(this) {
                IsFullScreen              = false,
                PreferredBackBufferHeight = (int) ScreenSize.Y,
                PreferredBackBufferWidth  = (int) ScreenSize.X
            };

            /* This is to import pictures and sounds and stuff */
            Content.RootDirectory = "Content";
            EmptyUnit             = new WorldUnit(ref ScreenSize, new Vector2(0, 0));
            PixelToMeterScale     = ScreenSize.X/25; //How many pixels are in one meter
            LastPressed           = GamePad.GetState(PlayerIndex.One);

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run. 
        /// This is where it can query for any required services and load any non-graphic related content. Calling 
        /// base. Initialize will enumerate through any components and initialize them as well.
        /// </summary>
        protected override void Initialize() {

            /*********************************** Initialization for Physics things ************************************/

            // This sets the width of the screen equal to 25m in the physics engine
            ConvertUnits.SetDisplayUnitToSimUnitRatio(PixelToMeterScale);

            ScreenCenter = ScreenSize / 2F;

            /************************************ Initialization for Level things *************************************/

            TempleRock = new LevelHandler("Temple Rock", Vector2.Zero, new Vector2(4, 0), new Vector2(6, 0),
                new Vector2(8, 0), new Vector2(13.5F, 0));

            Temple = new LevelHandler("Temple", new Vector2(2.5F, 7.21F), new Vector2(9.2F, 5.3F),
                new Vector2(16.5F, 7.21F), new Vector2(20.8F, 7.6F), new Vector2(21.5F, 8.1F));
#if MEMES
            FinalDestination = new LevelHandler("FinalDestination", Vector2.Zero, new Vector2(4, 0), new Vector2(6, 0),
                new Vector2(8, 0), new Vector2(13.5F, 0));
#endif
            Debate = new LevelHandler(DebateSystemName, Vector2.Zero, new Vector2(4, 0), new Vector2(6, 0),
                new Vector2(8, 0), new Vector2(13.5F, 0));

            WhiteHouse = new LevelHandler("Debate", Vector2.Zero, new Vector2(4, 0), new Vector2(6, 0),
                new Vector2(8, 0), new Vector2(13.5F, 0));

            /************************************* Initialization for Menu things *************************************/
            //Some menus hold items for other things to make the menu system more compact, don't worry about it.

            Menu = new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0, 0)), "", true,
                new WorldUnit(ref ScreenSize, new Vector2(0, 0)), false, false, MenuCommands.Nothing, false, false);

/* 00 */    Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.2F)), "Local Game", false,
                EmptyUnit, true, true, MenuCommands.SingleplayerMenu));

            const int LocalGameMenu = 0;

                Menu.ContainedItems[LocalGameMenu].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.2F)), 
                    "New Game", true, EmptyUnit, true, true, MenuCommands.StartGame));

                    //This holds the in game pause screen for any amount of players
                    Menu.ContainedItems[LocalGameMenu].ContainedItems[0].AddItem(
                        new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.20F)), "One Player", false, 
                            EmptyUnit, true, true, MenuCommands.OnePlayer));

                    //This holds the level selection screen for any amount of players
                    Menu.ContainedItems[LocalGameMenu].ContainedItems[0].AddItem(
                        new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.30F)), "Two Player", false,
                            EmptyUnit, true, true, MenuCommands.TwoPlayer));

                        Menu.ContainedItems[LocalGameMenu].ContainedItems[0].ContainedItems[1].AddItem(
                            new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.20F)), "Temple", false,
                                EmptyUnit, true, true, MenuCommands.PlayTemple));

                        Menu.ContainedItems[LocalGameMenu].ContainedItems[0].ContainedItems[1].AddItem(
                            new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.30F)), "Temple Rock", false,
                                EmptyUnit, true, true, MenuCommands.PlayTempleRock));
#if MEMES
            Menu.ContainedItems[LocalGameMenu].ContainedItems[0].ContainedItems[1].AddItem(
                            new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.40F)), "Final Destination", 
                                false, EmptyUnit, true, true, MenuCommands.PlayFinalDestination));
#endif

                        Menu.ContainedItems[LocalGameMenu].ContainedItems[0].ContainedItems[1].AddItem(
                            new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.40F)), "Debate Room",
                                false, EmptyUnit, true, true, MenuCommands.PlayDebate));

                        Menu.ContainedItems[LocalGameMenu].ContainedItems[0].ContainedItems[1].AddItem(
                           new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.50F)), "White House",
                               false, EmptyUnit, true, true, MenuCommands.PlayWhiteHouse));

            //This holds character selection for any amount of players
            Menu.ContainedItems[LocalGameMenu].ContainedItems[0].AddItem(   
                        new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.4F)), "Three Player", false, 
                            EmptyUnit, true, true, MenuCommands.ThreePlayer));

                        Menu.AccessItem(LocalGameMenu, 0, 2).AddItem(new MenuItem(new WorldUnit(ref ScreenSize,
                            new Vector2(0.5F, 0.2F)), "The Donald", false, EmptyUnit, true, true,
                            MenuCommands.SelectTrump));

                        Menu.AccessItem(LocalGameMenu, 0, 2).AddItem(new MenuItem(new WorldUnit(ref ScreenSize, 
                            new Vector2(0.5F, 0.1F)), "Player One Character", false, EmptyUnit, false, true));

                        Menu.AccessItem(LocalGameMenu, 0, 2).AddItem(new MenuItem(new WorldUnit(ref ScreenSize, 
                            new Vector2(0.5F, 0.3F)), "Hillary", false, EmptyUnit, true, true, 
                            MenuCommands.SelectHillary));

                    Menu.ContainedItems[LocalGameMenu].ContainedItems[0].AddItem(
                        new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.5F)), "Four Player", false, 
                            EmptyUnit, true, true, MenuCommands.FourPlayer));

                Menu.ContainedItems[LocalGameMenu].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.30F)),
                    "Load Game", false, EmptyUnit, true, true, MenuCommands.LoadSave));

                Menu.ContainedItems[LocalGameMenu].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.4F)),
                    "Back", false, EmptyUnit, true, true, MenuCommands.BackToMainMenu));

/* 01 */    Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.50F, 0.30F)), "Multi Player", false,
                EmptyUnit, true, true, MenuCommands.MultiplayerMenu));

                Menu.ContainedItems[1].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.2F)), 
                    "There is no multiplayer in America!\nYou play by yourself!", false, EmptyUnit, true, true,
                    MenuCommands.BackToMainMenu));

/* 02 */    Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.4F)), "Help", true, EmptyUnit,
                true, true));

                Menu.AccessItem(2).AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.2F)),
                    "Jump - A", false, EmptyUnit, false, true));

                Menu.AccessItem(2).AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.3F)),
                    "Move - Left stick", false, EmptyUnit, false, true));

                Menu.AccessItem(2).AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.4F)),
                    "Special Attack - RT (Right Trigger)\n                 OR X", false, EmptyUnit, false, true));

/* 03 */    Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.5F)), "BIO", true,
                EmptyUnit, true, true, MenuCommands.ExitGame));

                Menu.AccessItem(3).AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.1F)),
                     "BIOs", false, EmptyUnit, false, true));

                Menu.AccessItem(3).AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.3F)),
                     "Donald Trump", true, EmptyUnit, true, true));
                     Menu.AccessItem(3, 1).AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.05F, 0.1F)),
                       "Coming from humble beginnings in \nNew York City, Donald Trump \nrealized that only he has the " +
                       "\nability to truly make America \nGreat Again. With a small loan of \none million dollars and the " +
                       "\nfriendship of neighboring \ncountries, Donald Trump has begun \nhis journey to take the throne of " +
                       "\nthe United States.", false, EmptyUnit, false, false));
                

                Menu.AccessItem(3).AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.4F)),
                     "Hillary Clinton", true, EmptyUnit, true, true));
                     Menu.AccessItem(3,2).AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.05F, 0.1F)),
                         "Hillary \"Roddy\" Clinton is back for \nher second attempt at ruling the \nUnited States of " +
                         "America. With a \nstrong background in technology \nand internet security, Clinton \nvows to " +
                         "improve personal \nfreedoms and bring America back \ntogether again. Will she become \nthe first "+
                         " woman president, or \nwill she need to wait for a third \nattempt?", false, EmptyUnit, false, false));

                Menu.AccessItem(3).AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.7F)),
                    "Back", false, EmptyUnit, true, true, MenuCommands.BackToMainMenu));

            /* 04 */
            Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.6F)), "Exit", false,
                EmptyUnit, true, true, MenuCommands.ExitGame));

/* 05 */    Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.1F)), "Super Smash Polls ", false,
                EmptyUnit, false, true, MenuCommands.Nothing, true));

                //Menu.AccessItem(4).TextColor = Color.Red;

/* 06 */    Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0F, 0.9F)),
                "Use DPad up and down to navigate the menu", false, EmptyUnit, false));

                Menu.ContainedItems[0].ContainedItems[0].ContainedItems[0].AddItem(
                    new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.20F)), "Save game", false,
                        EmptyUnit, true, true, MenuCommands.SaveGame));

                Menu.ContainedItems[0].ContainedItems[0].ContainedItems[0].AddItem(
                    new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.30F)), "Continue", false,
                        EmptyUnit, true, true, MenuCommands.ResumeGame));

                Menu.ContainedItems[0].ContainedItems[0].ContainedItems[0].AddItem(
                    new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.40F)), "Main Menu", false,
                        EmptyUnit, true, true, MenuCommands.BackToMainMenu));

/* 07 */    Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.8F)),
                "William Kluge & Joe Brooksbank", false, EmptyUnit, false, false, MenuCommands.Nothing, true));

#if COMPLEX_BODIES
            /******************************************** Category Setup **********************************************/

            Category PlayerOneCat = Category.Cat1,
                PlayerTwoCat      = Category.Cat2,
                PlayerThreeCat    = Category.Cat3,
                PlayerFourCat     = Category.Cat4,
                PlayerOneHitbox   = Xor(PlayerTwoCat, PlayerThreeCat, PlayerFourCat),
                PlayerTwoHitbox   = Xor(PlayerOneCat, PlayerThreeCat, PlayerFourCat),
                PlayerThreeHitbox = Xor(PlayerOneCat, PlayerTwoCat, PlayerFourCat),
                PlayerFourHitbox  = Xor(PlayerOneCat, PlayerTwoCat, PlayerThreeCat);
#endif
            /************************************** Initialization for Players ****************************************/
#if COMPLEX_BODIES
            PlayerOne   = new PlayerClass(PlayerIndex.One, Xor(Category.All, PlayerOneCat), PlayerOneHitbox);
            PlayerTwo   = new PlayerClass(PlayerIndex.Two, Xor(Category.All, PlayerTwoCat), PlayerTwoHitbox);
            PlayerThree = new PlayerClass(PlayerIndex.Three, Xor(Category.All, PlayerThreeCat), PlayerThreeHitbox);
            PlayerFour  = new PlayerClass(PlayerIndex.Four, Xor(Category.All, PlayerFourCat), PlayerFourHitbox);
#else
            PlayerOne = new PlayerClass(PlayerIndex.One);
            PlayerTwo   = new PlayerClass(PlayerIndex.Two);
            PlayerThree = new PlayerClass(PlayerIndex.Three);
            PlayerFour  = new PlayerClass(PlayerIndex.Four);
#endif

            /************************************* Initialization for Gamemode ****************************************/

            CurrentGamemode = new Gamemode {
                CurrentMode = Gamemode.Mode.Stock,
                Stock = 5
            };

            /************************************* Initialization for Characters **************************************/

#if OLD_CHARACTER
            TheDonald = new Character(ref ScreenSize, ConvertUnits.ToDisplayUnits(new Vector2(1.88F, 0.6F)), 40, 0.5F,
                0F, 500F, 10F, 1F, 1F, "TheDonald");
#elif COMPLEX_BODIES
            TheDonald = new CharacterManager(50F, 0.5F, 0F, Category.None, Category.None, "TheDonald");
#else
            TheDonald = new CharacterManager(50, 0.5F, 0, "TheDonald");
            Hillary   = new CharacterManager(40, 0.5F, 0, "Hillary");
#endif

            base.Initialize();

        }

        /// <summary> 
        /// LoadContent will be called once per game and is the place to load all of your content.
        /// </summary>
        protected override void LoadContent() {

            /***************** System Creations *****************/

            Batch = new SpriteBatch(GraphicsDevice);

            /****************** Font Loading ********************/

            GameFont = Content.Load<SpriteFont>("SpriteFont1"); //Load the font in the game

            try {

                TitleFont = Content.Load<SpriteFont>("TitleFont");
                TitleFontSmall = Content.Load<SpriteFont>("TitleFont-Small");

            } catch (Exception e) {
                
                Console.WriteLine(e.Message);
                Console.WriteLine("Please install the font 8-BIT WONDER for the full Super Smash Polls experience");

                TitleFont      = GameFont; //In case the font 8-Bit wonder isn't installed
                TitleFontSmall = GameFont;

            }

            /************ The Donald Content Loading ************/
#if OLD_CHARACTER
    
#elif COMPLEX_BODIES

            int ItemScale = (int)(ScreenSize/new Vector2(640, 360)).X;

            MoveAssets TheDonaldIdle = new MoveAssets(1000, new Point(21, 27),
                    Content.Load<Texture2D>("Donald\\donald_stand"), ItemScale, Content.Load<Texture2D>("Donald\\donald_stand"), 
                    DefinedMoves.Idle),
                TheDonaldWalk = new MoveAssets(1000, new Point(23, 29), 
                    Content.Load<Texture2D>("Donald\\donald_walk"),
                    ItemScale, Content.Load<Texture2D>("Donald\\donald_walk"), 
                    DefinedMoves.TheDonaldWalkFunc),
                TheDonaldJump = new MoveAssets(1000, new Point(19, 26), 
                    Content.Load<Texture2D>("Donald\\donald_jump"),
                    ItemScale, Content.Load<Texture2D>("Donald\\donald_jump"), 
                    DefinedMoves.TheDonaldJumpFunc),
                TheDonaldSpecial = new MoveAssets(2000, new Point(23, 29), 
                    Content.Load<Texture2D>("Donald\\donald_punch"),
                    ItemScale, Content.Load<Texture2D>("Donald\\donald_punch"), 
                    DefinedMoves.TheDonaldSpecialFunc),
                TheDonaldSideSpecial = new MoveAssets(2000, new Point(21, 27),
                    Content.Load<Texture2D>("Donald\\donald_stand"), 
                    ItemScale, Content.Load<Texture2D>("Donald\\donald_stand"),
                    DefinedMoves.TheDonaldSideSpecialFunc),
                TheDonaldUpSpecial = new MoveAssets(2000, new Point(26, 33),
                    Content.Load<Texture2D>("Donald\\donald_upmash"), ItemScale, Content.Load<Texture2D>("Donald\\donald_upmash"),
                    DefinedMoves.TheDonaldUpSpecialFunc),
                TheDonaldDownSpecial = new MoveAssets(2000, new Point(21, 27),
                    Content.Load<Texture2D>("Donald\\donald_stand"), ItemScale, Content.Load<Texture2D>("Donald\\donald_stand"),
                    DefinedMoves.TheDonaldDownSpecialFunc),
                TheDonaldBasicAttack = new MoveAssets(2000, new Point(23, 29),
                    Content.Load<Texture2D>("Donald\\donald_punch"), ItemScale, Content.Load<Texture2D>("Donald\\donald_punch"), 
                    DefinedMoves.TheDonaldBasicAttack,
                    Content.Load<SoundEffect>("Donald\\donald_basic_sound"));

            TheDonald.AddMoves(TheDonaldIdle, TheDonaldWalk, TheDonaldJump, TheDonaldSpecial,
                TheDonaldSideSpecial, TheDonaldUpSpecial, TheDonaldDownSpecial, TheDonaldBasicAttack);
#else

            TheDonald.LoadCharacterContent(Content.Load<Texture2D>("Donald\\donald_hitbox"), 1, 
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(1000, new Point(21, 27),
                    Content.Load<Texture2D>("Donald\\donald_stand"),
                    null, //Don't need sound for standing
                    DefinedMoves.Idle), 
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(1000, new Point(23, 29),
                    Content.Load<Texture2D>("Donald\\donald_walk"),
                    null, //Don't need sound for walking
                    DefinedMoves.TheDonaldWalk),
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(1000, new Point(19, 26),
                    Content.Load<Texture2D>("Donald\\donald_jump"),
                    null, //Don't need sound for jumping
                    DefinedMoves.TheDonaldJump),
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(2000, new Point(26, 26),
                    Content.Load<Texture2D>("Donald\\donald_punch"),
                    Content.Load<SoundEffect>("Donald\\donald_special_sound"), 
                    DefinedMoves.TheDonaldSpecial),
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(1000, new Point(23, 29),
                    Content.Load<Texture2D>("Donald\\donald_side_smash"),
                    null, 
                    DefinedMoves.TheDonaldSideSpecial),
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(2000, new Point(26, 33),
                    Content.Load<Texture2D>("Donald\\donald_upmash"), 
                    Content.Load<SoundEffect>("Donald\\donald_up_special_sound"), 
                    DefinedMoves.TheDonaldUpSpecial),
<<<<<<< HEAD
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(1500, new Point(22, 30),
                    Content.Load<Texture2D>("Donald\\donald_down_smash"),
                    PlaceholdderAudio, 
=======
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(2000, new Point(21, 27),
                    Content.Load<Texture2D>("Donald\\donald_stand"),
                    null, 
>>>>>>> origin/master
                    DefinedMoves.TheDonaldDownSpecial),
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(500, new Point(26, 26),
                    Content.Load<Texture2D>("Donald\\donald_punch"), 
                    Content.Load<SoundEffect>("Donald\\donald_basic_sound"), 
                    DefinedMoves.TheDonaldBasic));

            /************** Hillary Content Loading *************/

            Hillary.LoadCharacterContent(Content.Load<Texture2D>("Hillary\\hill_hitbox"), 1,
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(1000, new Point(17, 25),
                    Content.Load<Texture2D>("Hillary\\hill_stand"),
                    null, //Don't need sound for standing
                    DefinedMoves.Idle),
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(500, new Point(17, 26),
                    Content.Load<Texture2D>("Hillary\\hill_walk"),
                    null, //Don't need sound for walking
                    DefinedMoves.HillaryWalk),
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(1000, new Point(17, 29),
                    Content.Load<Texture2D>("Hillary\\hill_jump"),
                    null, //Don't need sound for jumping
                    DefinedMoves.HillaryJump),
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(1000, new Point(17, 25),
                    Content.Load<Texture2D>("Hillary\\hill_special"),
                    null,
                    DefinedMoves.HillarySpecial),
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(500, new Point(26, 25),
                    Content.Load<Texture2D>("Hillary\\hill_side_smash"),
                    null,
                    DefinedMoves.HillarySideSpecial),
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(500, new Point(24, 30),
                    Content.Load<Texture2D>("Hillary\\hill_up_smash"),
                    null,
                    DefinedMoves.HillaryUpSpecial),
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(2000, new Point(17, 29),
                    Content.Load<Texture2D>("Hillary\\hill_jump"),
                    null,
                    DefinedMoves.HillaryDownSpecial),
                new Tuple<float, Point, Texture2D, SoundEffect, CharacterManager.SimpleMove>(500, new Point(24, 200/8),
                    Content.Load<Texture2D>("Hillary\\hill_punch"),
                    null,
                    DefinedMoves.HillaryBasic));

#endif

            /***** Add characters to character string pairs *****/

            CharacterStringPairs.Add("TheDonald", TheDonald);
            CharacterStringPairs.Add("Hillary", Hillary);

            /******************* Menu content *******************/

            Menu.SetFontForAll(TitleFont);
            Menu.AccessItem(6).SetFont(TitleFontSmall);
            Menu.AccessItem(7).SetFont(TitleFontSmall);

            Menu.AddAudio(new AudioHandler(Content.Load<SoundEffect>("Music\\MainSong")));

            /******** TODO remove DEBUG Gamemode content ********/

            Texture2D Icon = Content.Load<Texture2D>("Donald\\donald_icon");

            CurrentGamemode.AssignIcons(Icon, Icon, Icon, Icon, TitleFontSmall);

            /*************** Load for Temple Rock ***************/

            Texture2D TempleRockTexture = Content.Load<Texture2D>("TempleRock");

            TempleRock.AssignToWorld(new Tuple<Texture2D, Vector2, Vector2>(TempleRockTexture,
                new Vector2(0, InMeters(360) - InMeters(TempleRockTexture.Height)),
                new Vector2(InMeters(TempleRockTexture.Width), InMeters(TempleRockTexture.Height))));

            Texture2D SpaceBackground = Content.Load<Texture2D>("space");

            TempleRock.SetBackground(SpaceBackground, new Vector2(SpaceBackground.Width,
                SpaceBackground.Height) / ScreenSize);

            /******************* Load Temple ********************/

            Texture2D TempleLeft = Content.Load<Texture2D>("TempleItems\\TempleLeft"),
                TempleMiddle     = Content.Load<Texture2D>("TempleItems\\TempleMiddle"),
                TempleRight      = Content.Load<Texture2D>("TempleItems\\TempleRight"),
                TempleTop        = Content.Load<Texture2D>("TempleItems\\TempleTop"),
                TempleBackground = Content.Load<Texture2D>("TempleItems\\TempleBackground");

            Temple.SetBackground(TempleBackground,
                new Vector2(ScreenSize.X / TempleBackground.Width, ScreenSize.Y / TempleBackground.Height));

            Temple.AssignToWorld(
                new Tuple<Texture2D, Vector2, Vector2>(TempleLeft,   MetersV2(8, 188),   MetersV2(132, 79)),
                new Tuple<Texture2D, Vector2, Vector2>(TempleMiddle, MetersV2(181, 140), MetersV2(146, 74)),
                new Tuple<Texture2D, Vector2, Vector2>(TempleRight,  MetersV2(309, 176), MetersV2(324, 137)),
                new Tuple<Texture2D, Vector2, Vector2>(TempleTop,    MetersV2(185, 37),  MetersV2(132, 45)));
#if MEMES
            /************* Load Final Destination ***************/

            {

                Texture2D FinalPlatform = Content.Load<Texture2D>("FinalDestination\\FinalPlatform"),
                    FinalBackground     = Content.Load<Texture2D>("FinalDestination\\FinalBackground");

                Vector2 ObjectScale = new Vector2(ScreenSize.X/FinalBackground.Width,
                    ScreenSize.Y/FinalBackground.Height);

                FinalDestination.SetBackground(FinalBackground, ObjectScale);

                FinalDestination.AssignToWorld(new Tuple<Texture2D, Vector2, Vector2>(FinalPlatform,
                    MetersV2(218, 336)/ObjectScale, MetersV2(658, 243)/ObjectScale));

            }
#endif
            /**************** Load Debate Room ******************/

            {

                Texture2D DebateBackground = Content.Load<Texture2D>("Debate\\DebateBackground"),
                    DebatePlatform         = Content.Load<Texture2D>("Debate\\DebatePlatform");

                Vector2 ObjectScale = new Vector2(ScreenSize.X/ DebateBackground.Width,
                    ScreenSize.Y/ DebateBackground.Height);

                Debate.SetBackground(DebateBackground, ObjectScale);

                Debate.AssignToWorld(new Tuple<Texture2D, Vector2, Vector2>(DebatePlatform,
                    MetersV2(0, 156), MetersV2(640, 204)));

            }
            /**************** Load White House ******************/
            {
                Texture2D WhiteHouseBackground = Content.Load<Texture2D>("WhiteHouse\\WhiteHouseBackground"),
                          WhiteHousePlatform = Content.Load<Texture2D>("WhiteHouse\\WhiteHouseForeground");

                Vector2 ObjectScale = new Vector2(ScreenSize.X / WhiteHouseBackground.Width,
                    ScreenSize.Y / WhiteHouseBackground.Height);

                WhiteHouse.SetBackground(WhiteHouseBackground, ObjectScale);

                WhiteHouse.AssignToWorld(new Tuple<Texture2D, Vector2, Vector2>(WhiteHousePlatform,
                    MetersV2(0, 156), MetersV2(640, 204)));

            }

            /************* Add levels to dictionary *************/

            LevelDictionary.Add("Temple",     Temple);
            LevelDictionary.Add("TempleRock", TempleRock);
#if MEMES
            LevelDictionary.Add("FinalDestination", FinalDestination);
#endif
            LevelDictionary.Add(DebateSystemName, Debate);
            LevelDictionary.Add("White House", WhiteHouse);

            /************* Load in game music  *************/
            CurrentGamemode.AddAudio(new AudioHandler(Content.Load<SoundEffect>("Music\\Hail to the Chief")));
        }
           

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload all content.
        /// </summary>
        protected override void UnloadContent() {
            //Unload any non ContentManager content here
        }
 
        /// <summary>
        /// Allows the game to run logic such as updating the world, checking for collisions, gathering input, and 
        /// playing audio.
        /// </summary>
        protected override void Update(GameTime gameTime) {
                
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            switch (State) {

                case GameState.Menu: { /* The player has the menu open */

                    GamePadState CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
                    MenuCommands CurrentCommand      = Menu.UpdateMenu(CurrentGamePadState, LastPressed);
                    LastPressed                      = CurrentGamePadState;

                    switch (CurrentCommand) {
                        case MenuCommands.PlayTemple:
                            CurrentLevel = Temple;
                            goto case MenuCommands.CharacterSelection;
                        case MenuCommands.PlayTempleRock:
                            CurrentLevel = TempleRock;
                            goto case MenuCommands.CharacterSelection;
                        case MenuCommands.PlayDebate:
                            CurrentLevel = Debate;
                            goto case MenuCommands.CharacterSelection;
                        case MenuCommands.PlayWhiteHouse:
                             CurrentLevel = WhiteHouse;
                            goto case MenuCommands.CharacterSelection;
                        case MenuCommands.PlayFinalDestination:
#if MEMES
                            CurrentLevel = FinalDestination;
                            goto case MenuCommands.CharacterSelection;
#else
                            goto case MenuCommands.PlayDebate;
#endif
                            case MenuCommands.OnePlayer:
                            NumPlayers = 1;
                            Menu.ContainedItems[0].ContainedItems[0].DrawDown = 1;
                            break;
                        case MenuCommands.TwoPlayer:
                            Menu.ContainedItems[0].ContainedItems[0].DrawDown = 1;
                            NumPlayers = 2;
                            break;
                        case MenuCommands.ThreePlayer:
                            Menu.ContainedItems[0].ContainedItems[0].DrawDown = 1;
                            NumPlayers = 3;
                            break;
                        case MenuCommands.FourPlayer:
                            Menu.ContainedItems[0].ContainedItems[0].DrawDown = 1;
                            NumPlayers = 4;
                            break;
                        case MenuCommands.LoadSave:
                            State = GameState.LoadSave;
                            break;
                        case MenuCommands.StartGame:
                            State = GameState.GameLevel;

                            Menu.ContainedItems[0].ContainedItems[0].ContainedItems[0].SetFontForAll(TitleFont);

                            Menu.ContainedItems[0].ContainedItems[0].DrawDown = 0;

                            Menu.StopAudio();

                            CurrentGamemode.NumberOfPlayers = NumPlayers;
                            CurrentGamemode.GameOver        = false;
                            CurrentGamemode.MusicPlayed     = false;
                            break;
                        case MenuCommands.BackToMainMenu:
                            ResetPlayerStats();
                            Menu.DrawDown = -1;
                            Menu.ContainedItems[0].ContainedItems[0].DrawDown = -1;
                            break;
                        case MenuCommands.MultiplayerMenu:
                            Menu.DrawDown = 1;
                            break;
                        case MenuCommands.SingleplayerMenu:
                            Menu.DrawDown = 0;
                            break;
                        case MenuCommands.HelpMenu:
                            Menu.DrawDown = 2;
                            break;
                        case MenuCommands.Nothing:
                            break;
                        case MenuCommands.SaveGame:
                            State = GameState.SaveGame;
                            break;
                        case MenuCommands.ResumeGame:
                            State = GameState.GameLevel;
                            Menu.StopAudio();
                            break;
                        case MenuCommands.ExitGame:
                            Exit();
                            break;
                        case MenuCommands.SelectTrump:
                            SetCharacter(TheDonald);
                            goto default;
                        case MenuCommands.SelectHillary:
                            SetCharacter(Hillary);
                            goto default;
                        case MenuCommands.CharacterSelection:
                            Menu.SetDrawDown(0, 0, 2);
                            break;
                        default:
                            if (AllCharactersSet())
                                goto case MenuCommands.StartGame;
                            goto case MenuCommands.CharacterSelection;

                    } 

                    break;

                } case GameState.GameLevel: { /* The player is currently playing the game */

                    State = (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) ? 
                            GameState.Menu : State;

                        CurrentGamemode.UpdateGamemodeState(PlayerOne.Deaths, PlayerTwo.Deaths, PlayerThree.Deaths,
                            PlayerFour.Deaths);

                    switch (NumPlayers) {

                        case 4:
                            PlayerFour.UpdatePlayer(CurrentLevel.RespawnPoint, CurrentGamemode.ElimStatus[3]);
                            goto case 3;
                        case 3:
                            PlayerThree.UpdatePlayer(CurrentLevel.RespawnPoint, CurrentGamemode.ElimStatus[2]);
                            goto case 2;
                        case 2:
                            PlayerTwo.UpdatePlayer(CurrentLevel.RespawnPoint, CurrentGamemode.ElimStatus[1]);
                            goto default;
                        default:
                            PlayerOne.UpdatePlayer(CurrentLevel.RespawnPoint, CurrentGamemode.ElimStatus[0]);
                            break;

                    }

                    CurrentLevel.LevelWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

                    break;

                } case GameState.ScoreScreen: {
                    break;
                } case GameState.SaveGame: {

                    try {

                        StreamWriter FileWriter = new StreamWriter(SaveLocation);

                        FileWriter.WriteLine(CurrentLevel.Name);

                        FileWriter.WriteLine(NumPlayers);

                        PlayerOne.WriteInfo(ref FileWriter);
                        PlayerTwo.WriteInfo(ref FileWriter);
                        PlayerThree.WriteInfo(ref FileWriter);
                        PlayerFour.WriteInfo(ref FileWriter);

                        CurrentGamemode.WriteGamemode(FileWriter);

                        FileWriter.Close();

                    } catch (Exception E) { Console.WriteLine("Exception: " + E.Message); }

                    State = GameState.GameLevel;

                    break;

                } case GameState.LoadSave: {

                    try { //TODO fix glitch where is there is no save data it get's stuck

                        StreamReader FileReader = new StreamReader(SaveLocation);

                        string LevelNameRead = FileReader.ReadLine();

                        try {

                            LevelDictionary.TryGetValue(LevelNameRead, out CurrentLevel);

                        } catch (Exception) {

                            State = GameState.Menu;
                            goto case GameState.Menu;

                        }

                        NumPlayers = int.Parse(FileReader.ReadLine());

                        PlayerOne.ReadInfo(ref FileReader, CharacterStringPairs, CurrentLevel.LevelWorld);
                        PlayerTwo.ReadInfo(ref FileReader, CharacterStringPairs, CurrentLevel.LevelWorld);
                        PlayerThree.ReadInfo(ref FileReader, CharacterStringPairs, CurrentLevel.LevelWorld);
                        PlayerFour.ReadInfo(ref FileReader, CharacterStringPairs, CurrentLevel.LevelWorld);

                        CurrentGamemode.ReadGamemode(FileReader);
                        CurrentGamemode.NumberOfPlayers = NumPlayers;
                        PlayerOne.Deaths   = CurrentGamemode.PlayerOneDeaths;
                        PlayerTwo.Deaths   = CurrentGamemode.PlayerTwoDeaths;
                        PlayerThree.Deaths = CurrentGamemode.PlayerThreeDeaths;
                        PlayerFour.Deaths  = CurrentGamemode.PlayerFourDeaths;

                        FileReader.Close();

                        State = GameState.GameLevel;
                        Menu.StopAudio();

                    } catch (Exception E) {

                        Console.WriteLine("Exception: " + E.Message);

                    }


                    break;

                } default: {

                    break;

                }

            }

            base.Update(gameTime);

        }

        /// <summary>
        /// This is where the game draw's the screen.
        /// </summary>
        protected override void Draw(GameTime gameTime) {

            Batch.Begin();

                switch (State) {

                    case GameState.Menu: {

                        Menu.DisplayMenu(ref Batch);

                        break;

                    } case GameState.GameLevel: {

                        CurrentLevel.DrawLevel(Batch, GameFont);

                        switch (NumPlayers) {

                            case 4:
                                PlayerFour.DrawPlayer(ref Batch, GameFont);
                                goto case 3;
                            case 3:
                                PlayerThree.DrawPlayer(ref Batch, GameFont);
                                goto case 2;
                            case 2:
                                PlayerTwo.DrawPlayer(ref Batch, GameFont);
                                goto case 1;
                            case 1:
                                PlayerOne.DrawPlayer(ref Batch, GameFont);
                                break;

                        }

                        CurrentGamemode.DrawGamemodeOverlay(Batch);

                        break;

                    } case GameState.SaveGame: {
                        Batch.DrawString(GameFont, "Saving game data...", ScreenCenter, Color.Black);
                        break;
                    } case GameState.LoadSave: {
                        Batch.DrawString(GameFont, "Loading game data...", ScreenCenter, Color.Black);
                        break;
                    } default: {

                        break;

                    }

                }

            Batch.End();

            base.Draw(gameTime);

        }

        /// <summary>
        /// Exclusive or for Farseer categories
        /// </summary>
        /// <param name="categories">The categories to preform the XOR function to</param>
        /// <returns>The result of XOR from categories</returns>
        private Category Xor(params Category[] categories) {

            Category Temp = Category.None;

            foreach (Category I in categories)
                Temp = Temp ^ I;

            return Temp;

        }

        /// <summary>
        /// Get's the meters of something drawn in a 640x360 scale in a vector 2
        /// </summary>
        /// <param name="x">The x value to convert</param>
        /// <param name="y">The y value to convert</param>
        /// <returns>The size of x and y as a Vector2 converted to meters assuming that x and y were from a 640x360
        /// native resolution</returns>
        /// <remarks>x and y don't need to be 640x360, they just need to be drawn in that scale</remarks>
        private Vector2 MetersV2(float x, float y) {

            return new Vector2(InMeters(x), InMeters(y));

        }

        /// <summary> 
        /// Get's the meters of something drawn in a 640x360 scale
        /// </summary>
        /// <param name="pixels">The amount of pixels to convert</param>
        private static float InMeters(float pixels) {

            return (pixels / 640) * 25;

        }

        /// <summary>
        /// Handles setting characters
        /// </summary>
        /// <returns>Whether or not a character has been set for each player in the game</returns>
        private void SetCharacter(CharacterManager character) {

            if ("blank" == PlayerOne.PlayerCharacter.Name) {
                PlayerOne.PlayerCharacter = character.Clone();
#if COMPLEX_BODIES
                PlayerOne.PlayerCharacter.ConstructInWorld(CurrentLevel.LevelWorld);
#else
                PlayerOne.PlayerCharacter.SetupCharacter(CurrentLevel.LevelWorld, CurrentLevel.PlayerOneSpawn,
                    short.MaxValue - 1);
#endif
                PlayerOne.Deaths = 0;
                Menu.AccessItem(0, 0, 2, 1).Text = "Player Two Character";
            } else if ("blank" == PlayerTwo.PlayerCharacter.Name) {
                PlayerTwo.PlayerCharacter = character.Clone();
#if COMPLEX_BODIES
                PlayerTwo.PlayerCharacter.ConstructInWorld(CurrentLevel.LevelWorld);
#else
                PlayerTwo.PlayerCharacter.SetupCharacter(CurrentLevel.LevelWorld, CurrentLevel.PlayerTwoSpawn,
                    short.MaxValue - 2);
#endif
                PlayerTwo.Deaths = 0;
                Menu.AccessItem(0, 0, 2, 1).Text = "Player Three Character";
            } else if ("blank" == PlayerThree.PlayerCharacter.Name) {
                PlayerThree.PlayerCharacter = character.Clone();
#if COMPLEX_BODIES
                PlayerThree.PlayerCharacter.ConstructInWorld(CurrentLevel.LevelWorld);
#else
                PlayerThree.PlayerCharacter.SetupCharacter(CurrentLevel.LevelWorld, CurrentLevel.PlayerThreeSpawn,
                    short.MaxValue - 3);
#endif
                PlayerThree.Deaths = 0;
                Menu.AccessItem(0, 0, 2, 1).Text = "Player Four Character";
            } else {
                PlayerFour.PlayerCharacter = character.Clone();
#if COMPLEX_BODIES
                PlayerFour.PlayerCharacter.ConstructInWorld(CurrentLevel.LevelWorld);
#else
                PlayerFour.PlayerCharacter.SetupCharacter(CurrentLevel.LevelWorld, CurrentLevel.PlayerFourSpawn,
                    short.MaxValue - 4);
#endif
                PlayerFour.Deaths = 0;
            }

        }

        /// <summary>
        /// This helps to determine if all of the player characters have been set.
        /// </summary>
        /// <returns>Whether or not each player has a character set for them</returns>
        private bool AllCharactersSet() {

            switch (NumPlayers) {

                case 4:
                    return (PlayerOne.PlayerCharacter.Name != "blank" && PlayerTwo.PlayerCharacter.Name != "blank" &&
                            PlayerThree.PlayerCharacter.Name != "blank" && PlayerFour.PlayerCharacter.Name != "blank");
                case 3:
                    return (PlayerOne.PlayerCharacter.Name != "blank" && PlayerTwo.PlayerCharacter.Name != "blank" &&
                            PlayerThree.PlayerCharacter.Name != "blank");
                case 2:
                    return (PlayerOne.PlayerCharacter.Name != "blank" && PlayerTwo.PlayerCharacter.Name != "blank");
                default:
                    return PlayerOne.PlayerCharacter.Name != "blank";

            }

        }

        /// <summary>
        /// Resets players to their basic state
        /// </summary>
        private void ResetPlayerStats() {

            PlayerOne   = new PlayerClass(PlayerIndex.One);
            PlayerTwo   = new PlayerClass(PlayerIndex.Two);
            PlayerThree = new PlayerClass(PlayerIndex.Three);
            PlayerFour  = new PlayerClass(PlayerIndex.Four);

        }

    }

}
