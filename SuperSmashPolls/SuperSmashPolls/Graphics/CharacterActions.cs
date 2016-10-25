using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperSmashPolls.Graphics {

    /***************************************************************************************************************//**
     * Class responsible for handing animations of characters.
     ******************************************************************************************************************/
    public class CharacterAction {
        /** The color to draw the image with, defaults to clear */
        public Color DrawColor;
        /** The amount of time (in seconds) that it takes to cycle through the sheet */
        private readonly int PlayTime;
        /** The size of each item in the sheet (ie. 32-bit spritesheet is (32, 32) */
        private readonly Point ImageSize;
        /** The amount of images on the X and Y axis, calculated in the constructor */
        private readonly Point SheetSize;
        /** The spritesheet to take a value from (can be just one image) */
        private readonly Texture2D SpriteSheet;
        /** The piece of the sheet to draw based on PlayTime @see Update */
        private Point AnimatedPoint;
        /** The last time that AnimatedPoint was updated */
        private DateTime LastUpdateTime;
        /* The source on the spritesheet to draw */
        private Rectangle Source;
        /* The destination for drawing the source rectangle */
        private Rectangle Destination;

        /***********************************************************************************************************//**
         * Constructor
         * @param playTime The amount of time (in seconds) that it takes to loop through the entire sheet
         * @param imageSize The size of one image on the sheet (i.e. 32 bit sheet is 32 x 32)
         * @param spriteSheet The texture of the sheet
         * @param move The move that this animation if for
         **************************************************************************************************************/
        public CharacterAction(int playTime, Point imageSize, Texture2D spriteSheet) {

            PlayTime       = playTime;
            ImageSize      = imageSize;
            SheetSize      = new Point(spriteSheet.Width / imageSize.X, spriteSheet.Height / imageSize.Y);
            SpriteSheet    = spriteSheet;
            AnimatedPoint  = new Point(0, 0);
            DrawColor      = Color.White;
            LastUpdateTime = DateTime.Now;
        }

        /***********************************************************************************************************//**
         * Updates the animation
         * @param position The position on screen to draw the image
         * @param drawSize the size to draw the image
         **************************************************************************************************************/
        public void UpdateAnimation(Vector2 position, Vector2 drawSize) {

            DateTime Now = DateTime.Now;

            if ((Now - LastUpdateTime).Milliseconds >= (PlayTime*1000) / (SheetSize.X*SheetSize.Y)) {

                LastUpdateTime = Now;

                if (AnimatedPoint.Y == SheetSize.Y - 1) {

                    AnimatedPoint.Y = 0;

                    AnimatedPoint.X += 1;

                } else
                    AnimatedPoint.Y += 1;

                if (AnimatedPoint.X >= SheetSize.X)
                    AnimatedPoint = new Point(0, 0);

            }

            Source = new Rectangle(ImageSize.X * AnimatedPoint.X, ImageSize.Y * AnimatedPoint.Y, ImageSize.X,
                ImageSize.Y);
            Destination = new Rectangle((int)position.X, (int)position.Y, (int)drawSize.Y, (int)drawSize.X);

        }

        /***********************************************************************************************************//**
         * Draws the current image from the spritesheet based on the calculation of @see UpdateAnimation
         * @param batch A reference to the current SpriteBatch
         **************************************************************************************************************/
        public void DrawAnimation(ref SpriteBatch batch) {

            batch.Draw(SpriteSheet, Destination, Source, DrawColor);

        }

    }

}
