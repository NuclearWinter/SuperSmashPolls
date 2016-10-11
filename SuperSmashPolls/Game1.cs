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

namespace SuperSmashPolls {

    /***************************************************************************************************************//** 
     * This is the main type of the game.
     ******************************************************************************************************************/ 
    public class Game1 : Microsoft.Xna.Framework.Game {
        /* Donald Trump Character */
        private Character TheDonald = new Character();

        /* Manages graphics. */
        GraphicsDeviceManager Graphics;
        /* Used to draw multiple 2D textures at one time */
        private SpriteBatch Batch;
        /* A basic font to use */
        private SpriteFont FirstFont;

        /** Handles the menu interface for the game */
        private MenuController MenuControl = new MenuController(20, 20, 20, Color.AntiqueWhite, Color.Aquamarine, 
            Color.BlanchedAlmond);

        /** Handles the different states that the game can be in */
        enum GameState {

            Menu,           //The menu is open
            LoadingScreen,  //The screen to use while loading the game
            GameLevel,      //The first level of the game
            PurchaseScreen, //Screen for players to use when purchasing items
            ScoreScreen

        };
        /** Variable to hold the state of the game */
        private GameState state = GameState.Menu;

        /** Allows for handling of the menus to be dealbt with differently than gamestates */
        enum MenuState {

            Closed, Main, Score

        }
        /** Variable to hold the state of the menu */
        private MenuState Menu = MenuState.Main;

        enum Difficulty {

            VeryEasy, Easy, Normal, Hard, InHuman

        }
        /** Variable to hold the current difficulty */
        private Difficulty SetDifficulty = Difficulty.Normal;

        /* The total size of the screen */
        private Vector2 ScreenSize = new Vector2(1280, 720);
        /* The first player in the game */
        private PlayerClass PlayerOne = new PlayerClass(new Vector2(200, 200), 
                                                        new Vector2(1, 1), 
                                                        new Vector2(0.9F, 0.9F));

        /***********************************************************************************************************//** 
         * Constructs the game's class
         **************************************************************************************************************/ 
        public Game1() {

            /* This is the player's screen */
            Graphics = new GraphicsDeviceManager(this) {
                IsFullScreen = false,
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
        
            base.Initialize();

        }

        /***********************************************************************************************************//** 
         * LoadContent will be called once per game and is the place to load all of your content.
         **************************************************************************************************************/
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            Batch = new SpriteBatch(GraphicsDevice);

            FirstFont = Content.Load<SpriteFont>("SpriteFont1"); //Load the font in the game

            MenuControl.SetMenuFont(Content.Load<SpriteFont>("SpriteFont1"));

            TheDonald.AddAnimation(new SpritesheetHandler(1, 
                                                          new Point(16, 32), 
                                                          Content.Load<Texture2D>("TheDonaldWalking"), 
                                                          "walking"));

            PlayerOne.SetCharacter(ref TheDonald);

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

            switch (state) {

                case GameState.Menu: { /* The player has the menu open */

                    MenuControl.UpdateOpenMenu(PlayerIndex.One);

                    switch (Menu) { /* Used to deal with the correct menu */

                        case MenuState.Main: { /* The player is at the main menu */

                            state = (MenuControl.StartSinglePlayer(PlayerIndex.One)) ? GameState.GameLevel : state;

                            Menu = (MenuControl.OpenHighScore(PlayerIndex.One)) ? MenuState.Score : Menu;

                            if (MenuControl.QuitGame(PlayerIndex.One))
                                this.Exit();

                            break;

                        } case MenuState.Score: { /* The player is on the score screen */

                                if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed)
                                    
                                    Menu = MenuState.Main;

                            break;

                        }

                    }

                    break;

                } case GameState.LoadingScreen: { /* The character is loading into the game */

                        //TODO create loading screen

                    break;

                } case GameState.GameLevel: { /* The player is currently playing the game */

                        //Moves the player @see ObjectClass.move
                        PlayerOne.MoveController(PlayerIndex.One);

                        //Moves the player down
                        PlayerOne.AddGravity();

                        //Keeps the player on the screen and vibrates the controller if they are hitting the edge
                        PlayerOne.KeepPlayerInPlay(ScreenSize, new Vector2(.5F, .5F), PlayerIndex.One);

                        state = (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) ? 
                                GameState.Menu : state;

                    break;

                } case GameState.PurchaseScreen: {

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

                switch (state) {

                    case GameState.Menu: {

                        MenuControl.DrawOpenMenu(Batch);

                        switch (Menu) {

                            case MenuState.Main: {

                                    //Right now the main menu needs nothing different

                                    break;

                            } case MenuState.Score: {

                                    GraphicsDevice.Clear(Color.Gray);

                                    break;

                            }

                        }

                        break;

                    } case GameState.LoadingScreen: {

                        break;

                    } case GameState.GameLevel: {

                        PlayerOne.DrawPlayer(ref Batch);

                        break;

                    } case GameState.PurchaseScreen: {

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
