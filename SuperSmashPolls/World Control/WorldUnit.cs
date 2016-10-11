/*******************************************************************************************************************//**
 * @file WorldUnit.cs
 **********************************************************************************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSmashPolls.World_Control {

    /***************************************************************************************************************//**
     * TODO This class
     * This class will account for scaling of the physics system based on the size of the player's screen size.
     * @note WorldUnit should replace position and force references throughout the rest of the game.
     ******************************************************************************************************************/
    class WorldUnit {
        /* The size of the screen (in pixels) */
        private Vector2 ScreenSize;
        /* The position used by this WorldUnit class */
        private Vector2 Position;
        /* The maximum force that can possibly be applied in the game world */
        private Vector2 GodForce;

        /***********************************************************************************************************//**
         * Construct a WorldUnit class.
         * @param screenSize Reference to the size of the players screen (in pixels)
         * @note Doing it like this allows for the screensize to be changed without messing up physics.
         **************************************************************************************************************/
        public WorldUnit(ref Vector2 screenSize, Vector2 position, ref Vector2 godForce) {
            ScreenSize = screenSize;
            Position = position;
            GodForce = godForce;
        }

        /***********************************************************************************************************//**
         * TODO This method
         * Helps to find placement of objects by giving positions on the screen.
         * @param xRatio A number 0.0 - 1.0 representing percent of the screen to move the object over on the x-axis.
         * @param yRatio A number 0.0 - 1.0 representing percent of the screen to move the object over on the y-axis.
         * @return The amount of pixels to displace an object to achieve the desired ratio.
         **************************************************************************************************************/
        public Vector2 GetScreenPosition (float xRatio, float yRatio) {
            
            return new Vector2(xRatio * ScreenSize.X, yRatio * ScreenSize.Y);

        }

    }

}
