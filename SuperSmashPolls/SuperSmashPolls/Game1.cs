/*******************************************************************************************************************//**
 * @file Game1.cs
 * @note This game is now dependent on the Farseer Physics Engine.
 * For information see http://farseerphysics.codeplex.com/
 * @author (For all textures) Joe Brooksbank
 **********************************************************************************************************************/

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SuperSmashPolls.Characters;
using SuperSmashPolls.GameItemControl;
using SuperSmashPolls.Graphics;
using SuperSmashPolls.Levels;
using SuperSmashPolls.MenuControl;
using SuperSmashPolls.World_Control;

namespace SuperSmashPolls {

    /***************************************************************************************************************//** 
     * This is the main type of the game.
     ******************************************************************************************************************/ 
    public class Game1 : Microsoft.Xna.Framework.Game {

        /* The total size of the screen */
        private static Vector2 ScreenSize;
        /* The most basic Functioning WorldUnit */
        private readonly WorldUnit EmptyUnit;
        /* The display size for the floor */
        private readonly Vector2 FloorDisplaySize;
        /* The scale of how many pixels are equal to one meter */
        private readonly float PixelToMeterScale;

        /*   Characters   */

        /** The one, the only, the Donald */
        private Character TheDonald;

        /*  Levels  */

        private LevelHandler Temple;

        /* Manages graphics. */
        private GraphicsDeviceManager Graphics;
        /* Yarr, dis here be da world */
        private World GameWorld;
        /* The center of the screen */
        private Vector2 ScreenCenter;
        /* The body of the floor */
        private Body Floor;
        /* The handling of the texture for the floor @deprecated Using old class */
        private SpritesheetHandler FloorTexture;
        /* The origin of the floor */
        private Vector2 FloorOrigin;
        /* Used to draw multiple 2D textures at one time */
        private SpriteBatch Batch;
        /* A basic font to use for essentially everything in the game */
        private SpriteFont GameFont;
        /* Menu system for the game to use */
        private MenuItem Menu;

        /** The player's in this game */
        private PlayerClass PlayerOne, PlayerTwo, PlayerThree, PlayerFour;
        /* The number of players in the game */
        private int NumPlayers;

        /** Handles the different states that the game can be in */
        enum GameState {

            Menu,           //The menu is open
            GameLevel,      //The first level of the game
            ScoreScreen,
            LoadSave

        };
        /** Variable to hold the state of the game */
        private GameState State = GameState.Menu;

        /***********************************************************************************************************//** 
         * Constructs the game's class
         **************************************************************************************************************/
        public Game1() {
            /* !!! The size of the screen for the game !!! (this should be saved in options) */
            ScreenSize = new Vector2(640, 360);

            /* This is the player's screen controller */
            Graphics = new GraphicsDeviceManager(this) {
                IsFullScreen = false,
                PreferredBackBufferHeight = (int) ScreenSize.Y,
                PreferredBackBufferWidth  = (int) ScreenSize.X
            };

            /* This is to import pictures and sounds and stuff */
            Content.RootDirectory = "Content";

            EmptyUnit = new WorldUnit(ref ScreenSize, new Vector2(0, 0));

            //FloorDisplaySize = new Vector2(ScreenSize.Y * 0.05F, ScreenSize.X);

            //This is equal to how many pixels are in one meter
            PixelToMeterScale = ScreenSize.X/25;

        }

        /***********************************************************************************************************//** 
         * Allows the game to perform any initialization it needs to before starting to run. 
         * This is where it can query for any required services and load any non-graphic related content. Calling 
         * base.Initialize will enumerate through any components and initialize them as well.
         **************************************************************************************************************/
        protected override void Initialize() {

            /*********************************** Initialization for Physics things ************************************/

            // This sets the width of the screen equal to 25m in the physics engine
            ConvertUnits.SetDisplayUnitToSimUnitRatio(PixelToMeterScale);

            GameWorld = new World(new Vector2(0F, 0F)); //Creates the GameWorld with 9.82m/s^2 as a downward acceleration

            ScreenCenter = ScreenSize / 2F;

//            Vector2 FloorPosition = ConvertUnits.ToSimUnits(ScreenCenter) + new Vector2(0, 0);
//
//            //This creates the body in Farseer's world
//            Floor = BodyFactory.CreateRectangle(GameWorld, //The GameWorld to put the floor in
//                                                ConvertUnits.ToSimUnits(FloorDisplaySize.Y), //The floor size is
//                                                ConvertUnits.ToSimUnits(FloorDisplaySize.X), //converted to meters
//                                                1F,             //Density
//                                                FloorPosition); //This is the position of the floor
//            Floor.IsStatic    = true;  //The floor does not move
//            Floor.Restitution = 0.3F;  //This is how bouncy the object is
//            Floor.Friction    = 0.09F; //This is how frictiony the floor is (
//
//            //To draw the floor properly with XNA we need to center is position about the origin
//            FloorOrigin = new Vector2(FloorDisplaySize.Y / 2F, FloorDisplaySize.X / 2F);

            /************************************ Initialization for Level things *************************************/

            Temple = new LevelHandler();

            Temple.AssignToWorld(ref GameWorld);

            /************************************* Initialization for Menu things *************************************/

            Menu = new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0, 0)), "", true,
                new WorldUnit(ref ScreenSize, new Vector2(0, 0)), false);

            Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.2F)), "Local Game", false,
                EmptyUnit, true, true, MenuCommands.SingleplayerMenu));

                Menu.ContainedItems[0].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.2F)), 
                    "New Game", true, EmptyUnit, true, true, MenuCommands.StartGame));

                    Menu.ContainedItems[0].ContainedItems[0].AddItem(
                        new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.20F)), "One Player", false, 
                                     EmptyUnit, true, true, MenuCommands.OnePlayer));

                    Menu.ContainedItems[0].ContainedItems[0].AddItem(
                        new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.30F)), "Two Player", false,
                                     EmptyUnit, true, true, MenuCommands.TwoPlayer));

                    Menu.ContainedItems[0].ContainedItems[0].AddItem(
                        new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.4F)), "Three Player", false, 
                                     EmptyUnit, true, true, MenuCommands.ThreePlayer));

                    Menu.ContainedItems[0].ContainedItems[0].AddItem(
                        new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.5F)), "Four Player", false, 
                                     EmptyUnit, true, true, MenuCommands.FourPlayer));

                        

                            //Level select

                Menu.ContainedItems[0].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.30F)),
                    "Load Game", false, EmptyUnit, true, true, MenuCommands.StartGame));

                Menu.ContainedItems[0].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.4F)),
                    "Back", false, EmptyUnit, true, true, MenuCommands.BackToMainMenu));

            Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.50F, 0.30F)), "Multi Player", false,
                EmptyUnit, true, true, MenuCommands.MultiplayerMenu));

                Menu.ContainedItems[1].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.2F)), 
                    "Back", false, EmptyUnit, true, true, MenuCommands.BackToMainMenu));

            Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.4F)), "Exit", false,
                EmptyUnit, true, true, MenuCommands.ExitGame));

            /************************************** Initialization for Players ****************************************/

            PlayerOne   = new PlayerClass(PlayerIndex.One);
            PlayerTwo   = new PlayerClass(PlayerIndex.Two);
            PlayerThree = new PlayerClass(PlayerIndex.Three);
            PlayerFour  = new PlayerClass(PlayerIndex.Four);

            /************************************* Initialization for Characters **************************************/

            //!@note These values are based off of the real Donald
            TheDonald = new Character(ref ScreenSize, ConvertUnits.ToDisplayUnits(new Vector2(1.88F, 0.6F)), 89F, 0.5F,
                0.01F, 500F, 25F, 0.1F, 1F);

            TheDonald.CreateBody(ref GameWorld, new Vector2(8, 8));

            base.Initialize();

        }

        /***********************************************************************************************************//** 
         * LoadContent will be called once per game and is the place to load all of your content.
         * @note The menu is created here
         **************************************************************************************************************/
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            Batch = new SpriteBatch(GraphicsDevice);

            GameFont = Content.Load<SpriteFont>("SpriteFont1"); //Load the font in the game

            TheDonald.AddCharacterActions(
                new CharacterAction(2, new Point(16, 30), Content.Load<Texture2D>("Donald\\donald64-stand")),
                new CharacterAction(1, new Point(16, 30), Content.Load<Texture2D>("Donald\\donald64-jump")),
                new CharacterAction(1, new Point(16, 30), Content.Load<Texture2D>("Donald\\donald64-walk")),
                new CharacterAction(2, new Point(16, 32), Content.Load<Texture2D>("TheDonaldWalking")),
                new CharacterAction(2, new Point(16, 32), Content.Load<Texture2D>("TheDonaldWalking")),
                new CharacterAction(2, new Point(16, 32), Content.Load<Texture2D>("TheDonaldWalking")),
                new CharacterAction(2, new Point(16, 32), Content.Load<Texture2D>("TheDonaldWalking")),
                new CharacterAction(2, new Point(16, 32), Content.Load<Texture2D>("TheDonaldWalking")));
            //TODO finish animations for TheDonald

            FloorTexture = new SpritesheetHandler(1, new Point(10, 10), Content.Load<Texture2D>("Black Floor"), "f");

            Menu.SetFontForAll(GameFont);

        }

        /***********************************************************************************************************//** 
         * UnloadContent will be called once per game and is the place to unload all content.
         **************************************************************************************************************/
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /***********************************************************************************************************//** 
         * Allows the game to run logic such as updating the world, checking for collisions, gathering input, and 
         * playing audio.
         **************************************************************************************************************/
        protected override void Update(GameTime gameTime) {
                
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            switch (State) {

                case GameState.Menu: { /* The player has the menu open */

                    MenuCommands CurrentCommand = Menu.UpdateMenu(PlayerIndex.One);

                    switch (CurrentCommand) {
                        case MenuCommands.StartGame:
                            State = GameState.GameLevel;
                            Menu.ContainedItems[0].ContainedItems[0].Text = "Continue";  //Changes New Game
                            Menu.ContainedItems[0].ContainedItems[2].Text = "Main Menu"; //Changes Back

                            PlayerOne.SetCharacter(TheDonald); //debugging
                            PlayerTwo.SetCharacter(new Character(TheDonald, GameWorld, new Vector2(8, 0)));
                            PlayerThree.SetCharacter(TheDonald);
                            PlayerFour.SetCharacter(TheDonald);
                            break;
                        case MenuCommands.ExitGame:
                            this.Exit();
                            break;
                        case MenuCommands.BackToMainMenu:
                            Menu.DrawDown = -1;
                            break;
                        case MenuCommands.MultiplayerMenu:
                            Menu.DrawDown = 1;
                            break;
                        case MenuCommands.SingleplayerMenu:
                            Menu.DrawDown = 0;
                            break;
                        case MenuCommands.Nothing:
                            break;
                        case MenuCommands.OnePlayer:
                            NumPlayers = 1;
                            goto case MenuCommands.StartGame;
                        case MenuCommands.TwoPlayer:
                            NumPlayers = 2;
                            goto case MenuCommands.StartGame;
                        case MenuCommands.ThreePlayer:
                            NumPlayers = 3;
                            goto case MenuCommands.StartGame;
                        case MenuCommands.FourPlayer:
                            NumPlayers = 4;
                            goto case MenuCommands.StartGame;
                        default:
                            break;
                    } 

                    break;

                } case GameState.GameLevel: { /* The player is currently playing the game */

                    State = (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) ? 
                            GameState.Menu : State;

                    switch (NumPlayers) {

                        case 4:
                            PlayerFour.UpdatePlayer();
                            goto case 3;
                        case 3:
                            PlayerThree.UpdatePlayer();
                            goto case 2;
                        case 2:
                            PlayerTwo.UpdatePlayer();
                            goto default;
                        default:
                            PlayerOne.UpdatePlayer();
                            break;

                    }

                    GameWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

                    break;

                } case GameState.ScoreScreen:
                    break;
                case GameState.LoadSave:
                    break;
                default: {

                    break;

                }

            }

            base.Update(gameTime);

        }

        /***********************************************************************************************************//** 
         * This is where the game draw's the screen.
         **************************************************************************************************************/
        protected override void Draw(GameTime gameTime) {

            /* Gives the screen a background color */
            GraphicsDevice.Clear(Color.LightSlateGray);

            Batch.Begin();

                switch (State) {

                    case GameState.Menu: {

                        Menu.DisplayMenu(ref Batch);

                        break;

                    } case GameState.GameLevel: {

                        Temple.DrawLevel(Batch);

                        switch (NumPlayers) {

                            case 4:
                                PlayerFour.DrawPlayer(ref Batch);
                                goto case 3;
                            case 3:
                                PlayerThree.DrawPlayer(ref Batch);
                                goto case 2;
                            case 2:
                                PlayerTwo.DrawPlayer(ref Batch);
                                goto case 1;
                            case 1:
                                PlayerOne.DrawPlayer(ref Batch);
                                break;

                        }

                        GraphicsDevice.Clear(Color.Wheat);

                        break;

                    } default: {

                        break;

                    }

                }

            Batch.End();

            base.Draw(gameTime);

        }

    }

}
