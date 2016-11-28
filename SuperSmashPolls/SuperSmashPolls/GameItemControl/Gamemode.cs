using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            Time           = 0,
            Stock          = 1,
            TimeStockCombo = 2
        }

        private int PlayerOneDeaths, PlayerTwoDeaths, PlayerThreeDeaths, PlayerFourDeaths;

        /// <summary>The mode the game currently is</summary>
        public Mode CurrentMode;

        /// <summary>
        /// Initializes this class to control the game
        /// </summary>
        /// <param name="playerOneDeaths">Reference to the death </param>
        /// <param name="playerTwoDeaths"></param>
        /// <param name="playerThreeDeaths"></param>
        /// <param name="playerFourDeaths"></param>
        public Gamemode(ref int playerOneDeaths, ref int playerTwoDeaths, ref int playerThreeDeaths, 
            ref int playerFourDeaths) {

            PlayerOneDeaths   = playerOneDeaths;
            PlayerTwoDeaths   = playerTwoDeaths;
            PlayerThreeDeaths = playerThreeDeaths;
            PlayerFourDeaths  = playerFourDeaths;

        }

        /// <summary>
        /// Allows for the disableing of extra characters so if there were only two players, only two are counted
        /// </summary>
        /// <param name="numPlayers">The number of players in this game</param>
        public void DisableExtraCharacters(int numPlayers) {
            


        }

        //TODO timer

        private bool Lives() {

            

        }

        //TODO write to file

        //TODO read from file

        public bool GameOver() {
            


        }

    }

}
