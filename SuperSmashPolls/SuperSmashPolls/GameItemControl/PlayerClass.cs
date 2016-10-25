/*******************************************************************************************************************//**
 * @file PlayerClass.cs
 * @author William Kluge
 **********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperSmashPolls.Characters;
using SuperSmashPolls.World_Control;

namespace SuperSmashPolls.GameItemControl {

    /***************************************************************************************************************//**
     * Class to control the movment and interaction of players.
     * @note This class should inlcude an instance of the character class, and should not repeat any affects of that 
     * class.
     ******************************************************************************************************************/
    class PlayerClass {
        /** The ID of the character */
        private readonly PlayerIndex PlayerID;
        /** The player's character */
        private Character PlayerCharacter;

        /***********************************************************************************************************//**
         * Constructor
         **************************************************************************************************************/
        public PlayerClass(PlayerIndex playerId) {
            PlayerID = playerId;
            PlayerCharacter = new Character();
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
        public void UpdatePlayer() {
            
            PlayerCharacter.UpdateCharacter(GamePad.GetState(PlayerID));

        }

        /***********************************************************************************************************//**
         * Draw the character
         **************************************************************************************************************/
        public void DrawPlayer(ref SpriteBatch batch) {
            
            PlayerCharacter.DrawCharacter(ref batch);
            
        }

    }

}
