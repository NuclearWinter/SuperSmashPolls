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
        private readonly float SmallStockScale;
        /** The space between stock icons */
        private readonly float IconBuffer;
        /** The amount needed to scale the icons to the correct size */
        private Vector2 PlayerOneIconScale, PlayerTwoIconScale, PlayerThreeIconScale, PlayerFourIconScale;
        /** The font to use for drawing stuff */
        private SpriteFont GameFont;
        /** The icons for players */
        private Texture2D[] PlayerIcons;
        /** The places of players for winning and loosing TODO */
        private string[] Places;
        /** The winner of the game */
        private string Winner;
        /// <summary>Holds the references of deaths</summary>
        public int PlayerOneDeaths, PlayerTwoDeaths, PlayerThreeDeaths, PlayerFourDeaths;
        /// <summary>Whether or not the game has ended</summary>
        public bool GameOver;
        /// <summary>The number of players in the game</summary>
        public int NumberOfPlayers;
        /// <summary>The lives available to players</summary>
        public int Stock;
        /// <summary>Holds which players have been eliminated</summary>
        public bool[] ElimStatus;
        /// <summary>The mode the game currently is</summary>
        public Mode CurrentMode;

        /// <summary>
        /// Initializes this class to control the game
        /// </summary>
        public Gamemode() {

            FullIconSize        = ConvertUnits.ToDisplayUnits(new Vector2(1.5F, 1.5F));
            SmallStockScale     = 0.24F;
            IconBuffer          = ConvertUnits.ToDisplayUnits(0.11F);
            ElimStatus          = new[] {false, false, false, false};
            GameOver            = false;
            PlayerOneStockPos      = new Vector2[6];
            PlayerOneStockPos[0]   = ConvertUnits.ToDisplayUnits(new Vector2(1, 12));
            PlayerTwoStockPos      = new Vector2[6];
            PlayerTwoStockPos[0]   = ConvertUnits.ToDisplayUnits(new Vector2(7.25F, 12));
            PlayerThreeStockPos    = new Vector2[6];
            PlayerThreeStockPos[0] = ConvertUnits.ToDisplayUnits(new Vector2(13.5F, 12));
            PlayerFourStockPos     = new Vector2[6];
            PlayerFourStockPos[0]  = ConvertUnits.ToDisplayUnits(new Vector2(19.85F, 12));

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

            Vector2 SmallIconSize = FullIconSize * SmallStockScale;

            iconPosition[1] = iconPosition[0] - new Vector2(0, IconBuffer + SmallIconSize.Y);

            for (int i = 2; i <= 5; ++i)
                iconPosition[i] = iconPosition[i-1] + new Vector2(IconBuffer + SmallIconSize.X, 0);

        }

        /// <summary>
        /// Assigns icons to player to use for drawing gamemode screens
        /// </summary>
        /// <param name="playerOneIcon">The icon for player one</param>
        /// <param name="playerTwoIcon">The icon for player two</param>
        /// <param name="playerThreeIcon">The icon for player three</param>
        /// <param name="playerFourIcon">The icon for player four</param>
        /// <param name="gameFont">The font to use for game mode displays</param>
        public void AssignIcons(Texture2D playerOneIcon, Texture2D playerTwoIcon, Texture2D playerThreeIcon,
            Texture2D playerFourIcon, SpriteFont gameFont) {

            PlayerIcons = new[] {playerOneIcon, playerTwoIcon, playerThreeIcon, playerFourIcon };

            if (PlayerIcons[0] != null) {

                PlayerOneIconScale = new Vector2();
                PlayerOneIconScale = FullIconSize / new Vector2(PlayerIcons[0].Width, PlayerIcons[0].Height);

            }

            if (PlayerIcons[1] != null) {

                PlayerTwoIconScale = new Vector2();
                PlayerTwoIconScale = FullIconSize / new Vector2(PlayerIcons[1].Width, PlayerIcons[1].Height);

            }

            if (PlayerIcons[2] != null) {

                PlayerThreeIconScale = new Vector2();
                PlayerThreeIconScale = FullIconSize / new Vector2(PlayerIcons[2].Width, PlayerIcons[2].Height);

            }

            if (PlayerIcons[3] != null) {

                PlayerFourIconScale = new Vector2();
                PlayerFourIconScale = FullIconSize / new Vector2(PlayerIcons[3].Width, PlayerIcons[3].Height);

            }

            GameFont = gameFont;

        }

        /// <summary>
        /// TODO impliment a time limit mode
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
        private void CheckPlayerStatus(ref bool set, int deaths) {

            set = (deaths >= Stock);

        }

        /// <summary>
        /// Determines if players have reached their life limit.
        /// </summary>
        /// <returns>Whether or not all but one player have reached their life limit</returns>
        private void LifeLimit() {

            CheckPlayerStatus(ref ElimStatus[0], PlayerOneDeaths);
            CheckPlayerStatus(ref ElimStatus[1], PlayerTwoDeaths);
            CheckPlayerStatus(ref ElimStatus[2], PlayerThreeDeaths);
            CheckPlayerStatus(ref ElimStatus[3], PlayerFourDeaths);

        }

        /// <summary>
        /// Draws the indicator for a player's stock
        /// </summary>
        /// <param name="icon">The icon for the player</param>
        /// <param name="position">The position to draw the stock indicator</param>
        /// <param name="largeScale">The scale of the player's large icon</param>
        /// <param name="stock">The stock of the player (if time based 0)</param>
        /// <param name="eliminated">Whether or not they have been eliminated</param>
        /// <param name="spriteBatch">The batch to draw with</param>
        /// TODO get small stock icon to scale correctly (using the scale of each player's icon)
        private void DrawStock(Texture2D icon, Vector2[] position, Vector2 largeScale, int stock, bool eliminated, 
            SpriteBatch spriteBatch) {

            spriteBatch.Draw(icon, position[0], null, !eliminated ? Color.White : Color.Red, 0, Vector2.Zero,
                PlayerOneIconScale, SpriteEffects.None, 0);

            if (stock < 5)
                for (int i = stock; i > 0; --i)
                    spriteBatch.Draw(icon, position[i], null, Color.White, 0, Vector2.Zero, largeScale * SmallStockScale,
                        SpriteEffects.None, 0);
            else
                spriteBatch.DrawString(GameFont, stock.ToString(), position[1], Color.Black);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamWriter"></param>
        public void WriteGamemode(StreamWriter streamWriter) {
            
            streamWriter.WriteLine(CurrentMode);
            streamWriter.WriteLine(PlayerOneDeaths);
            streamWriter.WriteLine(PlayerTwoDeaths);
            streamWriter.WriteLine(PlayerThreeDeaths);
            streamWriter.WriteLine(PlayerFourDeaths);

        }

        /// <summary>
        /// Reads the information about a previosuly started game from a file
        /// </summary>
        /// <param name="streamReader"></param>
        public void ReadGamemode(StreamReader streamReader) {

            CurrentMode       = (Mode)Enum.Parse(typeof(Mode), streamReader.ReadLine());
            PlayerOneDeaths   = int.Parse(streamReader.ReadLine());
            PlayerTwoDeaths   = int.Parse(streamReader.ReadLine());
            PlayerThreeDeaths = int.Parse(streamReader.ReadLine());
            PlayerFourDeaths  = int.Parse(streamReader.ReadLine());

        }

        /// <summary>
        /// Determines if the game is over. Should be called during update.
        /// </summary>
        /// <returns>Returns the number of a player eliminated, if none were eliminated it returns 0</returns>
        public void UpdateGamemodeState(int playerOneDeaths, int playerTwoDeaths, int playerThreeDeaths,
            int playerFourDeaths) {

            if (!GameOver) {

                PlayerOneDeaths   = playerOneDeaths;
                PlayerTwoDeaths   = playerTwoDeaths;
                PlayerThreeDeaths = playerThreeDeaths;
                PlayerFourDeaths  = playerFourDeaths; //TODO replace this shitty system with pointers

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

                int EliminatedPlayers = ElimStatus.Aggregate(0, (Current, i) => (i) ? ++Current : Current);

                if (EliminatedPlayers >= NumberOfPlayers - 1)
                    GameOver = true;

                if (NumberOfPlayers == 1)
                    GameOver = ElimStatus[0];

            } else {

                if (PlayerOneDeaths < Stock) {
                    Winner = "Player one";
                } else if (PlayerTwoDeaths < Stock)
                    Winner = "Player two";
                else if (PlayerThreeDeaths < Stock)
                    Winner = "Player three";
                else if (PlayerFourDeaths < Stock)
                    Winner = "Player four";

                if (NumberOfPlayers == 1)
                    Winner = "none";

            }

        }

        /// <summary>
        /// Responsible for displaying two things. The stock of each player during the game, and the win/loss screen.
        /// Should be called during draw.
        /// </summary>
        /// <param name="batch">The spritebatch to draw with</param>
        public void DrawGamemodeOverlay(SpriteBatch batch) {

            if (!GameOver) {

                switch (NumberOfPlayers) {
                    case 4:
                        DrawStock(PlayerIcons[3], PlayerFourStockPos, PlayerFourIconScale,
                            Stock - PlayerFourDeaths, ElimStatus[3], batch);
                        goto case 3;
                    case 3:
                        DrawStock(PlayerIcons[2], PlayerThreeStockPos, PlayerThreeIconScale,
                            Stock - PlayerThreeDeaths, ElimStatus[2], batch);
                        goto case 2;
                    case 2:
                        DrawStock(PlayerIcons[1], PlayerTwoStockPos, PlayerTwoIconScale,
                            Stock - PlayerTwoDeaths, ElimStatus[1], batch);
                        goto case 1;
                    case 1:
                        DrawStock(PlayerIcons[0], PlayerOneStockPos, PlayerOneIconScale,
                            Stock - PlayerOneDeaths, ElimStatus[0], batch);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            } else {

                batch.GraphicsDevice.Clear(Color.DarkGray);

                if (Winner != "none")
                    batch.DrawString(GameFont, Winner + " is the next president!",
                        ConvertUnits.ToDisplayUnits(new Vector2(25/2 - 6, 6)), Color.Maroon);
                else
                    batch.DrawString(GameFont, "Everyone was terrible...no next president",
                        ConvertUnits.ToDisplayUnits(new Vector2(25/2 - 7, 6)), Color.Maroon);

                //TODO just make this screen into a menuitem so it is navigatable

            }

        }

    }

}
