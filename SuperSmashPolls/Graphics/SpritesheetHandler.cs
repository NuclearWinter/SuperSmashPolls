using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSmashPolls { //joe used 16x32

/*******************************************************************************************************************//**
 * TODO Test this class
 * This class handles spritesheets.
 **********************************************************************************************************************/
    class SpritesheetHandler {
        /* The amount of time (in seconds) that it takes to cycle through the sheet */
        private int PlayTime;
        /* The piece of the sheet to draw based on PlayTime @see DrawWithUpdate */
        private Point AnimatedPoint;
        /* The last time that the character's animation was changed */
        private DateTime LastUpdateTime;
        /* The size of each item in the sheet (ie. 32-bit spritesheet is (32, 32) */
        private Point ImageSize;
        /* The amount of images on the X and Y axis, calculated in the constructor */
        private Point SheetSize;
        /* The spritesheet to take a value from (can be just one image) */
        private Texture2D SpriteSheet;
        /* The color to draw the image with, defaults to clear */
        private Color DrawColor { get; set; } = Color.White;
        /* Used to identify if this animation is the one that should be called */
        public string Key = "null";

/*******************************************************************************************************************//**
 * Constructor
 * @param playTime The amount of time (in seconds) that it takes to loop through the entire sheet
 * @param imageSize The size of one image on the sheet (i.e. 32 bit sheet is 32 x 32)
 * @param spriteSheet The texture of the sheet
 * @param key The key to identify what this animation is (i.e. walking, jumpinng, etc.)
 * @return A filled SpritesheetHandler class
 **********************************************************************************************************************/
        public SpritesheetHandler(int playTime, Point imageSize, Texture2D spriteSheet, string key) {

            PlayTime = playTime;
            ImageSize = imageSize;
            SheetSize = new Point(spriteSheet.Width / imageSize.X, spriteSheet.Height / imageSize.Y);
            SpriteSheet = spriteSheet;
            Key = key;

        }

/*******************************************************************************************************************//**
 * TODO Test this class
 * Gives the user the desired image from the sheet.
 * @param batch A reference to the SpriteBatch to draw the textures with.
 * @param image The X and Y coordinate of the image to get.
 * @param position A reference to the position of the character on the screen.
 * @param drawSize How big to draw the base image.
 **********************************************************************************************************************/
        public void DrawImage(ref SpriteBatch batch, Point image, ref Vector2 position, Point drawSize) {

            Rectangle source = new Rectangle(ImageSize.X * image.X, ImageSize.Y * image.Y, ImageSize.X, ImageSize.Y);
            Rectangle destin = new Rectangle((int) position.X, (int) position.Y, drawSize.X, drawSize.Y);

            batch.Draw(SpriteSheet, destin, source, DrawColor);

        }

/*******************************************************************************************************************//**
 * TODO Test this method
 * This draws the sprite based on the PlayTime variable
 * @param batch The SpriteBatch to draw with.
 * @param position The position of the object on the screen.
 * @param drawSize The size to draw the object.
 **********************************************************************************************************************/
        public void DrawWithUpdate(ref SpriteBatch batch, ref Vector2 position, int drawMultiplier = 1) {

            DateTime now = DateTime.Now;

            if (Math.Abs(now.Millisecond - LastUpdateTime.Millisecond) >= (PlayTime*1000)/(SheetSize.X * SheetSize.Y)) {

                LastUpdateTime = now;

                if (AnimatedPoint.Y == SheetSize.Y -1) {

                    AnimatedPoint.Y = 0;

                    AnimatedPoint.X += 1;

                } else
                    AnimatedPoint.Y += 1;

                if (AnimatedPoint.X >= SheetSize.X)
                    AnimatedPoint = new Point(0, 0);

            }

            Rectangle source = new Rectangle(ImageSize.X * AnimatedPoint.X, ImageSize.Y * AnimatedPoint.Y, ImageSize.X, 
                ImageSize.Y);
            Rectangle destin = new Rectangle((int) position.X, (int) position.Y, 
                                             ImageSize.X * drawMultiplier, ImageSize.Y * drawMultiplier);

            batch.Draw(SpriteSheet, destin, source, DrawColor);

        }

    }

}
