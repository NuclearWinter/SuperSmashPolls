using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperSmashPolls.GameItemControl {

/*******************************************************************************************************************//**
 * Class to control the movment and interaction of players
 **********************************************************************************************************************/
    class PlayerClass : ObjectClass {
        
        /* The position on the spritesheet to use as the character */
        public Point character = new Point(0, 7);
        /* The color to draw the character with (to show damage state etc.) */
        private Color playerColor = Color.White;

        /* TODO this needs to hold which character the player has selected */

/*******************************************************************************************************************//** 
 * Constructs the player class.
 * @param startPosition The position to start the character at
 * @param speed The maximum speed that the player can have
 **********************************************************************************************************************/
        public PlayerClass(Vector2 startPosition, Vector2 maxSpeed, Vector2 decelleration) 
            : base(startPosition, maxSpeed, decelleration) { }

/*******************************************************************************************************************//** 
 * Moves the player with the controller and accounts for inversed controls.
 * @param player The index of the player's controller to get data from.
 **********************************************************************************************************************/ 
        public void MoveController(PlayerIndex player) {

            Move(GamePad.GetState(player).ThumbSticks.Left * new Vector2(1, -1));

        }

/*******************************************************************************************************************//**
 * Keeps the player on the screen.
 **********************************************************************************************************************/
        public void KeepPlayerInPlay(Vector2 bottomCorner, Vector2 vibration, PlayerIndex player) {

            if (KeepInPLay(bottomCorner))
                GamePad.SetVibration(player, vibration.X, vibration.Y);
            else
                GamePad.SetVibration(player, 0, 0);

        }

/*******************************************************************************************************************//**
 *  TODO Override the bound object with an option for controller vibration (call that function inside new)
 **********************************************************************************************************************/

/*******************************************************************************************************************//**
 * Draws the object without any animations.
 * @param drawWithBatch The sprite batch to draw with
 * @note This function must be called from within a games Draw function
 **********************************************************************************************************************/
        public void DrawNoAnim(SpriteBatch drawWithBatch) {
            
            Rectangle source = new Rectangle(32 * character.X, 32 * character.Y, 32, 32);
            Rectangle destin = new Rectangle((int) Position.X, (int) Position.Y, (int) size.X, (int) size.Y);

            drawWithBatch.Draw(Texture, destin, source, playerColor);

        }

/*******************************************************************************************************************//**
 * Get the rectangle version of the player.
 * @return The player's position and texture size turned into a rectangle
 **********************************************************************************************************************/
        public Rectangle GetRectangle() {

            return new Rectangle((int) Position.X, (int) Position.Y, (int) size.X, (int) size.Y);

        }

/*******************************************************************************************************************//**
 * Change the color to draw the player with if they intersect with another object.
 **********************************************************************************************************************/
        public void ChangeOnIntersect(Rectangle otherObject, Color newColor) {

            if (GetRectangle().Intersects(otherObject))
                playerColor = newColor;
            else
                playerColor = Color.White;

        }

    }

}
