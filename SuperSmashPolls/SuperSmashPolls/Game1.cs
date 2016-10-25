/*******************************************************************************************************************//**
 * @file Game1.cs
 * @note This game is now using Farseer Physics Engine
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
        /** The one, the only, the Donald */
        private Character TheDonald;
        /* Manages graphics. */
        private GraphicsDeviceManager Graphics;
        /* Yarr, dis here be da world */
        private World world;
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

            ScreenSize = new Vector2(600, 720);

            /* This is the player's screen */
            Graphics = new GraphicsDeviceManager(this) {
                IsFullScreen = false,
                PreferredBackBufferHeight = (int) ScreenSize.Y,
                PreferredBackBufferWidth  = (int) ScreenSize.X
            };

            /* This is to import pictures and sounds and stuff */
            Content.RootDirectory = "Content";

            EmptyUnit = new WorldUnit(ref ScreenSize, new Vector2(0, 0));

            FloorDisplaySize = new Vector2(ScreenSize.Y * 0.05F, ScreenSize.X);

            world = new World(new Vector2(0f, 9.82f));

        }

        /***********************************************************************************************************//** 
         * Allows the game to perform any initialization it needs to before starting to run. 
         * This is where it can query for any required services and load any non-graphic related content. Calling 
         * base.Initialize will enumerate through any components and initialize them as well.
         **************************************************************************************************************/
        protected override void Initialize() {

            /************************************* Initialization for Menu things *************************************/

            Menu = new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0, 0)), "", true,
                new WorldUnit(ref ScreenSize, new Vector2(0, 0)), false);

            Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.25F)), "Single Player", false,
                EmptyUnit, true, true, MenuCommands.SingleplayerMenu));

                Menu.ContainedItems[0].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.25F)), 
                    "New Game", false, EmptyUnit, true, true, MenuCommands.StartGame));

                Menu.ContainedItems[0].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.30F)),
                    "Load Game", false, EmptyUnit, true, true, MenuCommands.StartGame));

                Menu.ContainedItems[0].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.35F)),
                    "Back", false, EmptyUnit, true, true, MenuCommands.BackToMainMenu));

            Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.50F, 0.30F)), "Multi Player", false,
                EmptyUnit, true, true, MenuCommands.MultiplayerMenu));

                Menu.ContainedItems[1].AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.35F)), 
                    "Back", false, EmptyUnit, true, true, MenuCommands.BackToMainMenu));

            Menu.AddItem(new MenuItem(new WorldUnit(ref ScreenSize, new Vector2(0.5F, 0.35F)), "Exit", false,
                EmptyUnit, true, true, MenuCommands.ExitGame));

            /************************************* Initialization for Characters **************************************/

            //!@note Values used for TheDonald are just for debugging           | These are the fun ones |
            TheDonald = new Character(ref ScreenSize, new Vector2(0.10F, 0.05F), 35F, 0.02F, 0.2F, 45F, 25F, 0.5F, 1F);

            TheDonald.CreateBody(ref world, new Vector2(4, 4)); //TODO do this with the real map

            /*********************************** Initialization for Physics things ************************************/

            /* As a standard, all maps should be a total of 100m across */

            // Farseer expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 1% of the player's screen size pixels here
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64);

            ScreenCenter = ScreenSize/2F;

            Vector2 FloorPosition = ConvertUnits.ToSimUnits(ScreenCenter) + new Vector2(0, 0);

            //Creates the Floor within world with a width of the entire screen, a height of 5% of the screen, a density 
            //of 1, and a position at the equivilent of WorldUnit (0.0F, 0.90F)
            Floor = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(FloorDisplaySize.Y), 
                ConvertUnits.ToSimUnits(FloorDisplaySize.X), 1F, FloorPosition);
            Floor.IsStatic    = true; //The floor does not move
            Floor.Restitution = 0.3F;
            Floor.Friction    = 0.05F;

            FloorOrigin = new Vector2(FloorDisplaySize.Y / 2F, FloorDisplaySize.X/2F);

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
                new CharacterAction(2, new Point(16, 32), Content.Load<Texture2D>("TheDonaldWalking")),
                new CharacterAction(2, new Point(16, 32), Content.Load<Texture2D>("TheDonaldWalking")),
                new CharacterAction(2, new Point(16, 32), Content.Load<Texture2D>("TheDonaldWalking")),
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
                        default:
                            break;
                    } 

                    break;

                } case GameState.GameLevel: { /* The player is currently playing the game */

                        State = (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) ? 
                                GameState.Menu : State;

                        TheDonald.UpdateCharacter(GamePad.GetState(PlayerIndex.One));

                        world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

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

                        FloorTexture.DrawWithUpdate(ref Batch, ConvertUnits.ToDisplayUnits(Floor.Position) - FloorOrigin,
                            FloorDisplaySize);

                        TheDonald.DrawCharacter(ref Batch);

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
