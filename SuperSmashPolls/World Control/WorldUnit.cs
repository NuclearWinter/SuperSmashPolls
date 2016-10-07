using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSmashPolls.World_Control {

/*******************************************************************************************************************//**
 * TODO This class
 * This class will account for scaling of the physics system based on the size of the player's screen size.
 **********************************************************************************************************************/
    class WorldUnit {
        /* The size of the screen (in pixels) */
        private Vector2 ScreenSize;

/*******************************************************************************************************************//**
 * This gives a position on the screen based on a percentage of placement
 * @param x Percentage of the screen's width to place an item on
 * @param y Percentage of the screens height to place an item on
 * @note The value of x and y should be 0.0 to 1.0
 **********************************************************************************************************************/
        public Point TranslateLocation(float x, float y) {

            return new Point((int) (ScreenSize.X * x), (int) (ScreenSize.Y * y));

        }

/*******************************************************************************************************************//**
 * This translates how big the texture should be to match the size of the player's screen.
 **********************************************************************************************************************/
        public Point ScaleTexture(Point sourceSize, Point sourceScale) {

            return new Point(
                (int) ((sourceSize.X / sourceScale.Y) * ScreenSize.X),
                (int) ((sourceSize.X / sourceScale.Y) * ScreenSize.Y));

        }

    }

}
