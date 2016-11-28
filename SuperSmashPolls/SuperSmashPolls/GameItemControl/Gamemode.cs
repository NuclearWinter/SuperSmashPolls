using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FarseerPhysics;
using Microsoft.Xna.Framework;
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

        /** The size of the full icon for players */
        private readonly Vector2 FullIconSize;
        /** The position to put the player's stock indicators, indices 1+ are for the small stock icons/text */
        private readonly Vector2[] PlayerOneStockPos, PlayerTwoStockPos, PlayerThreeStockPos, PlayerFourStockPos;
        /** The scale of how big the small stock icons are compared to the full icon */
        private readonly float SmallStockIconScale;
        /** The space between stock icons */
        private readonly float IconBuffer;
        /** The amount needed to scale the icons to the correct size */
        private Vector2 PlayerOneIconScale, PlayerTwoIconScale, PlayerThreeIconScale, PlayerFourIconScale;
        /** Whether or not the game has ended */
        private bool GameOver;
        /** Holds the references of deaths */
        private int PlayerOneDeaths, PlayerTwoDeaths, PlayerThreeDeaths, PlayerFourDeaths;
        /** The font to use for drawing stuff */
        private SpriteFont GameFont;
        /** The icons for players */
        private Texture2D[] PlayerIcons;
        /** The number of players in the game */
        private int NumberOfPlayers;
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

            PlayerOneDeaths     = playerOneDeaths;
            PlayerTwoDeaths     = playerTwoDeaths;
            PlayerThreeDeaths   = playerThreeDeaths;
            PlayerFourDeaths    = playerFourDeaths;
            GameFont            = gameFont;
            FullIconSize        = ConvertUnits.ToDisplayUnits(new Vector2(0.1F, 0.1F));
            SmallStockIconScale = 0.15F;
            IconBuffer          = ConvertUnits.ToDisplayUnits(0.11F);
            EliminationStatus   = new[] {false, false, false, false};
            GameOver            = false;
            PlayerOneStockPos   = new[] {ConvertUnits.ToDisplayUnits(new Vector2(1, 9))};
            PlayerTwoStockPos   = new[] {ConvertUnits.ToDisplayUnits(new Vector2(7.25F, 9))};
            PlayerThreeStockPos = new[] {ConvertUnits.ToDisplayUnits(new Vector2(13.5F, 9))};
            PlayerFourStockPos  = new[] {ConvertUnits.ToDisplayUnits(new Vector2(19.85F, 9))};

            FillStockIconPositions(PlayerOneStockPos);
            FillStockIconPositions(PlayerTwoStockPos);
            FillStockIconPositions(PlayerThreeStockPos);
            FillStockIconPositions(PlayerFourStockPos);

        }

        /// <summary>
        /// Calculates the smaller icons used to display individual stocks for players
        /// </summary>
        /// <param name="iconPosition">The Vector2 array being used to hold the positions</param>
        /// Only five stock icons cans can be displayed, than it must go to text
        private void FillStockIconPositions(Vector2[] iconPosition) {

            Vector2 SmallIconSize = FullIconSize * SmallStockIconScale;

            iconPosition[1] = iconPosition[0] + new Vector2(0, IconBuffer);

            for (int i = 2; i <= 5; ++i) {
                
                iconPosition[i] = iconPosition[i-1] + new Vector2(0, IconBuffer + SmallIconSize.X);

            } 

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

        /// <summary>
        /// Draws the indicator for a player's stock
        /// </summary>
        /// <param name="icon">The icon for the player</param>
        /// <param name="position">The position to draw the stock indicator</param>
        /// <param name="stock">The stock of the player (if time based 0)</param>
        /// <param name="eliminated">Whether or not they have been eliminated</param>
        /// <param name="spriteBatch">The batch to draw with</param>
        private void DrawStock(Texture2D icon, Vector2[] position, int stock, bool eliminated, SpriteBatch spriteBatch) {

            spriteBatch.Draw(icon, position[0], null, !eliminated ? Color.White : Color.Red, 0, Vector2.Zero,
                PlayerOneIconScale, SpriteEffects.None, 0);

            if (stock < 5)
                for (int i = stock; i > 0; --i)
                    spriteBatch.Draw(icon, position[i], null, Color.White, 0, Vector2.Zero, SmallStockIconScale,
                        SpriteEffects.None, 0);
            else
                spriteBatch.DrawString(GameFont, stock.ToString(), position[1], Color.White);

        }

        /// <summary>
        /// Assigns icons to player to use for drawing gamemode screens
        /// </summary>
        /// <param name="playerOneIcon">The icon for player one</param>
        /// <param name="playerTwoIcon">The icon for player two</param>
        /// <param name="playerThreeIcon">The icon for player three</param>
        /// <param name="playerFourIcon">The icon for player four</param>
        /// <param name="numberOfPlayers">A reference to the number of players in the game</param>
        public void AssignIcons(Texture2D playerOneIcon, Texture2D playerTwoIcon, Texture2D playerThreeIcon, 
            Texture2D playerFourIcon, ref int numberOfPlayers) {
            
            PlayerIcons     = new[] {playerOneIcon, playerTwoIcon, playerThreeIcon, playerFourIcon};
            NumberOfPlayers = numberOfPlayers;

            if (PlayerIcons[0] != null)
                PlayerOneIconScale   = new Vector2(PlayerIcons[0].Width, PlayerIcons[0].Height) / FullIconSize;

            if (PlayerIcons[1] != null)
                PlayerTwoIconScale   = new Vector2(PlayerIcons[1].Width, PlayerIcons[1].Height) / FullIconSize;

            if (PlayerIcons[2] != null)
                PlayerThreeIconScale = new Vector2(PlayerIcons[2].Width, PlayerIcons[2].Height) / FullIconSize;

            if (PlayerIcons[3] != null)
                PlayerOneIconScale   = new Vector2(PlayerIcons[3].Width, PlayerIcons[3].Height) / FullIconSize;

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

            if (!GameOver) {

                switch (NumberOfPlayers) {
                    case 4:

                        break;
                    case 3:
                        break;
                    case 2:
                        break;
                    case 1:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            } else {

                //win/loss screen

            }

        }

    }

}
