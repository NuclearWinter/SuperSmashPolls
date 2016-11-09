/*******************************************************************************************************************//**
 * @file PlayerClass.cs
 * @author William Kluge
 **********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperSmashPolls.Characters;

namespace SuperSmashPolls.GameItemControl {

    /***************************************************************************************************************//**
     * Class to control the movment and interaction of players.
     * @note This class should inlcude an instance of the character class, and should not repeat any affects of that 
     * class.
     ******************************************************************************************************************/
    class PlayerClass {
        /** The ID of the character */
        private readonly PlayerIndex PlayerID;
        /** The health that the character has */
        private float PlayerHealth;
        /** Time number of times that the player has dies */
        private int Deaths;
        /** The player's character */
        public Character PlayerCharacter;

        /***********************************************************************************************************//**
         * Constructor
         **************************************************************************************************************/
        public PlayerClass(PlayerIndex playerId) {
            PlayerID        = playerId;
            PlayerCharacter = new Character();
            PlayerHealth    = 0;
            Deaths          = 0;
        }

        /***********************************************************************************************************//**
         * Sets the character
         **************************************************************************************************************/
        public void SetCharacter(Character playerCharacter) {

            PlayerCharacter = playerCharacter;

        }

        /***********************************************************************************************************//**
         * Update the player
         **************************************************************************************************************/
        public void UpdatePlayer(Vector2 respawnPoint) {

            if (Math.Abs(PlayerCharacter.GetPosition().X) > 40 || Math.Abs(PlayerCharacter.GetPosition().Y) > 30) {
            //Player is past 40 meters
                ++Deaths;
                PlayerCharacter.Respawn(respawnPoint);

            }

            PlayerCharacter.UpdateCharacter(GamePad.GetState(PlayerID));

        }

        /***********************************************************************************************************//**
         * Draw the character
         **************************************************************************************************************/
        public void DrawPlayer(ref SpriteBatch batch) {
            
            PlayerCharacter.DrawCharacter(ref batch);
            
        }

        /***********************************************************************************************************//**
         * Writes the character information
         * @param streamWriter What to write the file with (must already be opened)
         **************************************************************************************************************/
        public void WriteInfo(ref StreamWriter streamWriter) {
            
            streamWriter.WriteLine(PlayerCharacter.Name);
            streamWriter.WriteLine(PlayerCharacter.GetPosition().X);
            streamWriter.WriteLine(PlayerCharacter.GetPosition().Y);

        }

        /***********************************************************************************************************//**
         * Sets up the character from saved data
         * @param streamReader The stream to read data from (must already be opened)
         * @param characterList 
         * @param gameWorld 
         * TODO make the new world system work with this loading methods
         **************************************************************************************************************/
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
