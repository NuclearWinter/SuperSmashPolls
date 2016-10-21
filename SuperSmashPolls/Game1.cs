/*******************************************************************************************************************//**
 * @file Game1.cs
 **********************************************************************************************************************/ 

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SuperSmashPolls.Characters;
using SuperSmashPolls.GameItemControl;
using SuperSmashPolls.MenuControl;
using SuperSmashPolls.World_Control;

namespace SuperSmashPolls {

    /***************************************************************************************************************//** 
     * This is the main type of the game.
     ******************************************************************************************************************/ 
    public class Game1 : Microsoft.Xna.Framework.Game {
        /* Donald Trump Character */
        private Character TheDonald = new Character();

        /* Manages graphics. */
        GraphicsDeviceManager Graphics;
        /* The total size of the screen */
        private static Vector2 ScreenSize = new Vector2(1280, 720);
        /* Used to draw multiple 2D textures at one time */
        private SpriteBatch Batch;
        /* A basic font to use for essentially everything in the game */
        private SpriteFont GameFont;
        /* Menu system for the game to use */
        private MenuItem Menu;
        /* The most basic Functioning WorldUnit */
        private readonly WorldUnit EmptyUnit = new WorldUnit(ref ScreenSize, new Vector2(0, 0));

        private ObjectClass Floor;

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

            /* This is the player's screen */
            Graphics = new GraphicsDeviceManager(this) {
                IsFullScreen = true,
                PreferredBackBufferHeight = (int) ScreenSize.Y,
                PreferredBackBufferWidth  = (int) ScreenSize.X
            };

            /* This is to import pictures and sounds and stuff */
            Content.RootDirectory = "Content";

        }

        /***********************************************************************************************************//** 
         * Allows the game to perform any initialization it needs to before starting to run. 
         * This is where it can query for any required services and load any non-graphic related content. Calling 
         * base.Initialize will enumerate through any components and initialize them as well.
         **************************************************************************************************************/
        protected override void Initialize() {
            /* Initialize varibales here */
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

            Floor = new ObjectClass(new WorldUnit(new Vector2(0.0F, 0.90F), EmptyUnit), 999999, 
                                    new WorldUnit(new Vector2(1.0F, 0.05F), EmptyUnit));

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

            TheDonald.AddAnimation(new SpritesheetHandler(1, 
                                                          new Point(16, 32), 
                                                          Content.Load<Texture2D>("TheDonaldWalking"), 
                                                          "walking"));

            Menu.SetFontForAll(GameFont);

            Floor.AssignSpriteSheet(new SpritesheetHandler(1, new Point(10, 10), 
                                    Content.Load<Texture2D>("Black Floor"), "Floor"));

            //PlayerOne.SetCharacter(ref TheDonald);

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

                    MenuCommands currentCommand = Menu.UpdateMenu(PlayerIndex.One);

                    switch (currentCommand) {
                        case MenuCommands.StartGame:
                            State = GameState.GameLevel;
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
                        default:
                            break;
                    } 

                    break;

                } case GameState.GameLevel: { /* The player is currently playing the game */

                        //Moves the player @see ObjectClass.move
                        //PlayerOne.MoveController(PlayerIndex.One);

                        //Moves the player down
                        //PlayerOne.AddGravity();

                        //Keeps the player on the screen and vibrates the controller if they are hitting the edge
                        //PlayerOne.KeepPlayerInPlay(ScreenSize, new Vector2(.5F, .5F), PlayerIndex.One);

                        State = (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) ? 
                                GameState.Menu : State;

                    break;

                } default: {

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

                        Menu.DisplayMenu(Batch);

                        break;

                    } case GameState.GameLevel: {

                        GraphicsDevice.Clear(Color.Wheat);

                        Floor.Draw(ref Batch);

                        //PlayerOne.DrawPlayer(ref Batch);

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
