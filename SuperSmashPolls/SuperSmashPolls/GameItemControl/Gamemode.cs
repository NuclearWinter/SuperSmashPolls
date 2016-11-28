using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SuperSmashPolls.GameItemControl {

    /// <summary>
    /// This is to hold information on what game mode people are playing, how much time is left, the max number of lives,
    /// etc.
    /// </summary>
    public class Gamemode {

        /// <summary>
        /// The possible modes the game can be
        /// </summary>
        public enum Mode {
            /// <summary>Sets a time limit</summary>
            Time           = 0,
            /// <summary>Sets a life limit (or death limit)</summary>
            Stock          = 1,
            /// <summary>A combination of time limit and life limit</summary>
            TimeStockCombo = 2
        }

        /** Whether or not the game has ended */
        private bool GameOver;
        /** Holds the references of deaths */
        private int PlayerOneDeaths, PlayerTwoDeaths, PlayerThreeDeaths, PlayerFourDeaths;
        /** The font to use for drawing stuff */
        private SpriteFont GameFont;
        /** The icons for players */
        private Texture2D[] PlayerIcons;
        /// <summary>The lives available to players</summary>
        public int Stock;
        /// <summary>Holds whether or not a player has been eliminated</summary>
        public bool[] EliminationStatus;
        /// <summary>The mode the game currently is</summary>
        public Mode CurrentMode;

        /// <summary>
        /// Initializes this class to control the game
        /// </summary>
        /// <param name="playerOneDeaths">Reference to Deaths within PlayerClass for player one</param>
        /// <param name="playerTwoDeaths">Reference to Deaths within PlayerClass for player two</param>
        /// <param name="playerThreeDeaths">Reference to Deaths within PlayerClass for player three</param>
        /// <param name="playerFourDeaths">Reference to Deaths within PlayerClass for player four</param>
        /// <param name="gameFont">The font to use for displaying information about timeleft/stock</param>
        public Gamemode(ref int playerOneDeaths, ref int playerTwoDeaths, ref int playerThreeDeaths, 
            ref int playerFourDeaths, SpriteFont gameFont) {

            PlayerOneDeaths   = playerOneDeaths;
            PlayerTwoDeaths   = playerTwoDeaths;
            PlayerThreeDeaths = playerThreeDeaths;
            PlayerFourDeaths  = playerFourDeaths;
            GameFont          = gameFont;
            EliminationStatus = new[] {false, false, false, false};
            GameOver          = false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool TimeLimit() {

            return true; //Placeholder

        }

        /// <summary>
        /// Used in LifeLimit to check if a player has used all their lives
        /// </summary>
        /// <param name="set">Reference to the bool to set (whether or not the player is dead)</param>
        /// <param name="deaths">The amount of times that the player has died</param>
        private void CheckPlayerStatus(ref bool set, ref int deaths) {

            set = (deaths >= Stock);

        }

        /// <summary>
        /// Determines if players have reached their life limit.
        /// </summary>
        /// <returns>Whether or not all but one player have reached their life limit</returns>
        private void LifeLimit() {

            CheckPlayerStatus(ref EliminationStatus[0], ref PlayerOneDeaths);
            CheckPlayerStatus(ref EliminationStatus[1], ref PlayerTwoDeaths);
            CheckPlayerStatus(ref EliminationStatus[2], ref PlayerThreeDeaths);
            CheckPlayerStatus(ref EliminationStatus[3], ref PlayerFourDeaths);

        }

        private void DrawStock() {
            


        }

        public void AssignIcons(params Texture2D ) {
            


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamWriter"></param>
        public void WriteGamemode(StreamWriter streamWriter) {
            


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamReader"></param>
        public void ReadGamemode(StreamReader streamReader) {
            


        }

        /// <summary>
        /// Allows for the disableing of extra characters so if there were only two players, only two are counted
        /// </summary>
        /// <param name="numPlayers">The number of players in this game</param>
        /// This just sets their deaths to an illegal character
        public void DisableExtraCharacters(int numPlayers) {

            switch (numPlayers) {
                case 3:
                    PlayerFourDeaths = -1;
                    break;
                case 2:
                    PlayerThreeDeaths = -1;
                    goto case 3;
                case 1:
                    PlayerTwoDeaths = -1;
                    goto case 2;
                default:
                    Console.WriteLine("The number of players used in Gamemode.DisableExtraCharacters wasn't handled");
                    break;
            }

        }

        /// <summary>
        /// Determines if the game is over
        /// </summary>
        /// <returns>Returns the number of a player eliminated, if none were eliminated it returns 0</returns>
        public void GamemodeState() {

            switch (CurrentMode) {
                case Mode.Time:
                    break;
                case Mode.Stock:
                    LifeLimit();
                    break;
                case Mode.TimeStockCombo:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            int EliminatedPlayers = EliminationStatus.Aggregate(0, (Current, i) => (i) ? ++Current : Current);

            if (EliminatedPlayers >= 3)
                GameOver = true;

        }

        /// <summary>
        /// Responsible for displaying two things. The stock of each player during the game, and the win/loss screen
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to draw with</param>
        public void DrawGamemodeOverlay(SpriteBatch spriteBatch) {

            switch (GameOver) {

                case false:
                    break;
                case true:
                    break;

            }

        }

    }

}
