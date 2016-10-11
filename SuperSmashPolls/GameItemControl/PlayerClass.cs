using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperSmashPolls.Characters;

namespace SuperSmashPolls.GameItemControl {

/*******************************************************************************************************************//**
 * Class to control the movment and interaction of players.
 * @note This class should inlcude an instance of the character class, and should not repeat any affects of that class.
 **********************************************************************************************************************/

    class PlayerClass : ObjectClass {

        /* The color to draw the character with (to show damage state etc.) */
        private Color playerColor = Color.White;
        /* Holds the character the player has selected */
        private Character PlayerCharacter;

/*******************************************************************************************************************//** 
 * Constructs the player class.
 * @param startPosition The position to start the character at
 * @param speed The maximum speed that the player can have
 **********************************************************************************************************************/
        public PlayerClass(Vector2 startPosition, Vector2 maxSpeed, Vector2 decelleration)
            : base(startPosition, maxSpeed, decelleration) {
        }

/*******************************************************************************************************************/ /**
 * Set's the player's character.
 * @param playerCharacter The character to set the player to.
 **********************************************************************************************************************/
        public void SetCharacter(ref Character playerCharacter) {

            PlayerCharacter = playerCharacter;

        }

/*******************************************************************************************************************/ /** 
 * Moves the player with the controller and accounts for inversed controls.
 * @param player The index of the player's controller to get data from.
 **********************************************************************************************************************/
        public void MoveController(PlayerIndex player) {

            Move(GamePad.GetState(player).ThumbSticks.Left * new Vector2(1, -1));

        }

/*******************************************************************************************************************/ /**
 * Keeps the player on the screen.
 **********************************************************************************************************************/
        public void KeepPlayerInPlay(Vector2 bottomCorner, Vector2 vibration, PlayerIndex player) {

            if (KeepInPLay(bottomCorner))
                GamePad.SetVibration(player, vibration.X, vibration.Y);
            else
                GamePad.SetVibration(player, 0, 0);

        }

/*******************************************************************************************************************/ /**
 *  TODO Override the bound object with an option for controller vibration (call that function inside new)
 **********************************************************************************************************************/

/*******************************************************************************************************************/ /**
 * Draws the player's character.
 * TODO Complete this function with different states
 **********************************************************************************************************************/
        public void DrawPlayer(ref SpriteBatch batch) {

            SpritesheetHandler test = PlayerCharacter.DrawCharacter("walking");
            test.DrawWithUpdate(ref batch, ref Position);

        }

    }

}
