﻿/*******************************************************************************************************************//**
 * /doc:SuperSmashPolls.XML
 * @author William Kluge
 **********************************************************************************************************************/

 #define DEBUG
#undef DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperSmashPolls.Characters;

namespace SuperSmashPolls.GameItemControl {

    /// <summary>
    /// Class to control the movment and interaction of players.
    /// </summary>
    /// <remarks> This class should inlcude an instance of the character class, and should not repeat any affects of that
    ///class.</remarks>
    class PlayerClass {

        /** The ID of the character */
        private readonly PlayerIndex PlayerID;
        /** The health that the character has */
        private float PlayerHealth;
        /// <summary>The number of times that the player has died</summary>
        public int Deaths;
        /// <summary>The player's character</summary>
        public Character PlayerCharacter;
        /// <summary>The collision category for this player</summary>
        public Int16 CollisionCategory;

        /// <summary>
        /// Constructor
        /// </summary>
        public PlayerClass(PlayerIndex playerId, Int16 collisionCategory) {
            PlayerID          = playerId;
            PlayerCharacter   = new Character();
            PlayerHealth      = 0;
            Deaths            = 0;
            CollisionCategory = collisionCategory;
        }

        /// <summary>
        /// Sets the character
        /// </summary>
        public void SetCharacter(Character playerCharacter) {

            PlayerCharacter = playerCharacter;
            playerCharacter.CharacterBody.CollisionGroup = CollisionCategory;

        }

        /// <summary>
        /// Update the player
        /// </summary>
        public void UpdatePlayer(Vector2 respawnPoint) {

            if (Math.Abs(PlayerCharacter.GetPosition().X) > 40 || Math.Abs(PlayerCharacter.GetPosition().Y) > 30) {
            //Player is past 40 meters
                ++Deaths;
                PlayerCharacter.Respawn(respawnPoint);

            }

            PlayerCharacter.UpdateCharacter(GamePad.GetState(PlayerID));

        }

        /// <summary>
        /// Draw the character
        /// </summary>
        public void DrawPlayer(ref SpriteBatch batch, SpriteFont font = null) {

            PlayerCharacter.DrawCharacter(ref batch);

#if DEBUG
            if (font != null)
                batch.DrawString(font, "X: " + PlayerCharacter.GetPosition().X + "Y: " + PlayerCharacter.GetPosition().Y,
                    new Vector2(ConvertUnits.ToDisplayUnits(PlayerCharacter.GetPosition().X) - 8,
                        ConvertUnits.ToDisplayUnits(PlayerCharacter.GetPosition().Y) - 35), Color.White);
            
#endif

            if (font != null) {

                const int DeathTextHeight = 13;

                Vector2 DeathTextPosition = new Vector2();

                switch (PlayerID) {
                    case PlayerIndex.One:
                        DeathTextPosition = new Vector2(1, DeathTextHeight);
                        break;
                    case PlayerIndex.Two:
                        DeathTextPosition = new Vector2(7.25F, DeathTextHeight);
                        break;
                    case PlayerIndex.Three:
                        DeathTextPosition = new Vector2(13.5F, DeathTextHeight);
                        break;
                    case PlayerIndex.Four:
                        DeathTextPosition = new Vector2(19.75F, DeathTextHeight);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                batch.DrawString(font, "Deaths: " + Deaths, ConvertUnits.ToDisplayUnits(DeathTextPosition), Color.Black);

            }

        }

        /// <summary>
        /// Writes the character information
        /// </summary>
		/// <param name="streamWriter"> What to write the file with (must already be opened)</param>
        public void WriteInfo(ref StreamWriter streamWriter) {
            
            streamWriter.WriteLine(PlayerCharacter.Name);
            streamWriter.WriteLine(PlayerCharacter.GetPosition().X);
            streamWriter.WriteLine(PlayerCharacter.GetPosition().Y);

        }

        /// <summary>
        /// Sets up the character from saved data
        /// </summary>
		/// <param name="streamReader"> The stream to read data from (must already be opened)</param>
		/// <param name="characterList"> The characters available in the game </param>
		/// <param name="gameWorld"> The world to use for this </param>
        /// TODO make the new world system work with this loading methods
        public void ReadInfo(ref StreamReader streamReader, List<Tuple<Character, string>> characterList, World gameWorld) {

            string CharacterName = streamReader.ReadLine();

            if (null == CharacterName)
                Console.WriteLine("Character name is null, expect an exception here");

            for (int i = 0; i < characterList.Count(); ++i) {

                if (CharacterName != characterList[i].Item2) continue;

                PlayerCharacter = new Character(characterList[i].Item1, gameWorld,
                    new Vector2(float.Parse(streamReader.ReadLine()), float.Parse(streamReader.ReadLine())));

                break;

            }

        }

    }

}
