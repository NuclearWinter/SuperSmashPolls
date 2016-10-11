using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSmashPolls.World_Control {

    /***************************************************************************************************************//**
     * TODO This class
     * This class will account for scaling of the physics system based on the size of the player's screen size.
     ******************************************************************************************************************/
    class WorldUnit {
        /* The size of the screen (in pixels) */
        private Vector2 ScreenSize;

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
