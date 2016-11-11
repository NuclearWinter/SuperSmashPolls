/*******************************************************************************************************************//**
 * @file SpritesheetHandler.cs
 * @author Joe Brooooooooksbnake
 **********************************************************************************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSmashPolls.World_Control;

namespace SuperSmashPolls { //I used 16x32

    /***************************************************************************************************************//**
     * This class handles spritesheets.
     * @deprecated This class has been replaced by CharacterActions
     ******************************************************************************************************************/
    public class SpritesheetHandler {
        /* The amount of time (in seconds) that it takes to cycle through the sheet */
        private readonly int PlayTime;
        /* The piece of the sheet to draw based on PlayTime @see DrawWithUpdate */
        private Point AnimatedPoint;
        /* The last time that AnimatedPoint was updated */
        private DateTime LastUpdateTime;
        /* The size of each item in the sheet (ie. 32-bit spritesheet is (32, 32) */
        private Point ImageSize;
        /* The amount of images on the X and Y axis, calculated in the constructor */
        private Point SheetSize;
        /* The spritesheet to take a value from (can be just one image) */
        public readonly Texture2D SpriteSheet;
        /* The color to draw the image with, defaults to clear */
        private Color DrawColor { get; set; } = Color.White;
        /* Used to identify if this animation is the one that should be called. Public to check keys easily. */
        public string Key = "null";

        /***********************************************************************************************************//**
         * <summary>
         * Constructor
         * <param name="playTime"> The amount of time (in seconds) that it takes to loop through the entire sheet</param>
         * <param name="imageSize"> The size of one image on the sheet (i.e. 32 bit sheet is 32 x 32)</param>
         * <param name="spriteSheet"> The texture of the sheet</param>
         * <param name="key"> The key to identify what this animation is (i.e. walking, jumpinng, etc.)</param>
         * <returns> A filled SpritesheetHandler class
         * </summary>
         **************************************************************************************************************/
        public SpritesheetHandler(int playTime, Point imageSize, Texture2D spriteSheet, string key) {

            PlayTime = playTime;
            ImageSize = imageSize;
            SheetSize = new Point(spriteSheet.Width / imageSize.X, spriteSheet.Height / imageSize.Y);
            SpriteSheet = spriteSheet;
            Key = key;

        }

        /***********************************************************************************************************//**
         * <summary>
         * Gives the user the desired image from the sheet.
         * <param name=""> batch A reference to the SpriteBatch to draw the textures with.
         * <param name=""> image The X and Y coordinate of the image to get.
         * <param name=""> position A reference to the position of the character on the screen.
         * <param name=""> drawSize How big to draw the base image.
         * </summary>
         **************************************************************************************************************/
        public void DrawImage(ref SpriteBatch batch, Point image, ref Vector2 position, Point drawSize) {

            Rectangle source = new Rectangle(ImageSize.X * image.X, ImageSize.Y * image.Y, ImageSize.X, ImageSize.Y);
            Rectangle destin = new Rectangle((int) position.X, (int) position.Y, drawSize.X, drawSize.Y);

            batch.Draw(SpriteSheet, destin, source, DrawColor);

        }

        /***********************************************************************************************************//**
         * <summary>
         * This draws the sprite based on the PlayTime variable
         * <param name=""> batch The SpriteBatch to draw with.
         * <param name=""> position The position of the object on the screen.
         * <param name=""> drawSize The size to draw the object.
         * </summary>
         **************************************************************************************************************/
        public void DrawWithUpdate(ref SpriteBatch batch, Vector2 position, Vector2 drawSize) {

            DateTime Now = DateTime.Now;

            if (Math.Abs(Now.Millisecond - LastUpdateTime.Millisecond) >= (PlayTime*1000)/(SheetSize.X * SheetSize.Y)) {

                LastUpdateTime = Now;

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
            Rectangle destin = new Rectangle((int) position.X, (int) position.Y, (int) drawSize.Y, (int) drawSize.X);

            batch.Draw(SpriteSheet, destin, source, DrawColor);

        }

    }

}
