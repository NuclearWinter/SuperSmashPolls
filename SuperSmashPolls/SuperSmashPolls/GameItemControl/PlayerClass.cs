/*******************************************************************************************************************//**
 * /doc:SuperSmashPolls.XML
 * @author William Kluge
 **********************************************************************************************************************/

#define DEBUG
#undef DEBUG

#define KILL_BUTTON

using System;
using System.Collections.Generic;
using System.Globalization;
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

#if COMPLEX_BODIES
        /// <summary>The collision category for this player</summary>
        public Category CollidesWith;
        /// <summary>The collision category for this player's hitboxes</summary>
        public Category HitboxCollidesWith;
#endif
        /// <summary>The number of times that the player has died</summary>
        public int Deaths;
        /// <summary>The player's character</summary>
        public CharacterManager PlayerCharacter;
        /** The ID of the character */
        private readonly PlayerIndex PlayerID;
        /** The health that the character has */
        private float PlayerHealth;
        /** Whether or not the player died in the last update cycle */
        private bool JustDied;

#if COMPLEX_BODIES
        /// <summary>
        /// Constructor
        /// </summary>
        public PlayerClass(PlayerIndex playerId, Category collidesWith, Category hitboxCollidesWith) {
            PlayerID          = playerId;
            PlayerHealth      = 0;
            Deaths            = 0;
            CollidesWith = collidesWith;
            HitboxCollidesWith = hitboxCollidesWith;
            PlayerCharacter = new CharacterManager(CollidesWith, HitboxCollidesWith);
            PlayerCharacter = new CharacterManager();
            JustDied          = false;
        }
#endif

        /// <summary>
        /// Constructor
        /// </summary>
        public PlayerClass(PlayerIndex playerId) {
            PlayerID = playerId;
            PlayerHealth = 0;
            Deaths = 0;
            PlayerCharacter = new CharacterManager();
            JustDied = false;
        }

        /// <summary>
        /// Sets the character
        /// </summary>
        public void SetCharacter(CharacterManager playerCharacter) {

            PlayerCharacter = playerCharacter;

        }

        public void SetPosition(Vector2 position) {
            
            PlayerCharacter.Respawn(position);

        }

        /// <summary>
        /// Update the player
        /// </summary>
        public void UpdatePlayer(Vector2 respawnPoint, bool eliminated) {

            if (eliminated)
                return;

            if (JustDied) {

                PlayerCharacter.Respawn(respawnPoint);
#if !DEBUG
                ++Deaths;
#endif
                JustDied = false;

            } else if (Math.Abs(PlayerCharacter.GetPosition().X) > 40 || Math.Abs(PlayerCharacter.GetPosition().Y) > 30)
                JustDied = true;
            else 
                PlayerCharacter.UpdateCharacter(GamePad.GetState(PlayerID));

#if KILL_BUTTON

            if (GamePad.GetState(PlayerID).IsButtonDown(Buttons.Y)) {
                
                PlayerCharacter.Respawn(respawnPoint);

            }
#endif

        }

        /// <summary>
        /// Draw the character
        /// </summary>
        public void DrawPlayer(ref SpriteBatch batch, SpriteFont font = null) {

            PlayerCharacter.DrawCharacter(batch);

#if DEBUG
            if (font != null)
                batch.DrawString(font, "X: " + PlayerCharacter.GetPosition().X + "Y: " + PlayerCharacter.GetPosition().Y,
                    new Vector2(ConvertUnits.ToDisplayUnits(PlayerCharacter.GetPosition().X) - 8,
                        ConvertUnits.ToDisplayUnits(PlayerCharacter.GetPosition().Y) - 35), Color.White);
            
#endif

        }

        /// <summary>
        /// Writes the character information
        /// </summary>
		/// <param name="streamWriter"> What to write the file with (must already be opened)</param>
        public void WriteInfo(ref StreamWriter streamWriter) {
            
            streamWriter.WriteLine(PlayerCharacter.Name);

            if (PlayerCharacter.Name != "blank") {

                streamWriter.WriteLine(ConvertUnits.ToDisplayUnits(PlayerCharacter.GetPosition().X));
                streamWriter.WriteLine(ConvertUnits.ToDisplayUnits(PlayerCharacter.GetPosition().Y));
                streamWriter.WriteLine(PlayerCharacter.GetCollisionGroup());

            }

        }

        /// <summary>
        /// Sets up the character from saved data
        /// </summary>
		/// <param name="streamReader"> The stream to read data from (must already be opened)</param>
		/// <param name="characterList"> The characters available in the game </param>
		/// <param name="gameWorld"> The world to use for this </param>
        public void ReadInfo(ref StreamReader streamReader, Dictionary<string, CharacterManager> characterList, World gameWorld) {

            string CharacterName = streamReader.ReadLine();

            if (CharacterName == "blank")
                return;

            if (CharacterName != null && characterList.ContainsKey(CharacterName))
                PlayerCharacter = characterList[CharacterName];
            //else
                //throw new Exception("There was no character name loaded");

            Vector2 Position = new Vector2(ConvertUnits.ToSimUnits(float.Parse(streamReader.ReadLine())),
                    ConvertUnits.ToSimUnits(float.Parse(streamReader.ReadLine())));

            PlayerCharacter.SetupCharacter(gameWorld, Position, short.Parse(streamReader.ReadLine()));

        }

    }

}
