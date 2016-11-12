using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperSmashPolls.Graphics {

    /// <summary>
    /// Class responsible for handing animations of characters.
    /// </summary>
    public class CharacterAction {
        /// <summary>The spritesheet to take a value from (can be just one image)</summary>
        public readonly Texture2D SpriteSheet;
        /// <summary>The amount of time (in seconds) that it takes to cycle through the sheet</summary>
        public readonly int PlayTime;
        /// <summary>The size of each item in the sheet (ie. 32-bit spritesheet is (32, 32)</summary>
        public readonly Point ImageSize;
        /// <summary>The color to draw the image with, defaults to clear</summary>
        public Color DrawColor;
        /** The amount of images on the X and Y axis, calculated in the constructor */
        private readonly Point SheetSize;
        /** The piece of the sheet to draw based on PlayTime @see Update */
        private Point AnimatedPoint;
        /** The last time that AnimatedPoint was updated */
        private DateTime LastUpdateTime;
        /* The source on the spritesheet to draw */
        private Rectangle Source;
        /* The destination for drawing the source rectangle */
        private Rectangle Destination;

       /// <summary>
       /// Constructor
       /// </summary>
       /// <param name="playTime">The amount of time (in seconds) that it takes to loop through the entire sheet</param>
       /// <param name="imageSize">The size of one image on the sheet (i.e. 32 bit sheet is 32 x 32)</param>
       /// <param name="spriteSheet">The texture of the sheet</param>
        public CharacterAction(int playTime, Point imageSize, Texture2D spriteSheet) {

            PlayTime       = playTime;
            ImageSize      = imageSize;
            SheetSize      = new Point(spriteSheet.Width / imageSize.X, spriteSheet.Height / imageSize.Y);
            SpriteSheet    = spriteSheet;
            AnimatedPoint  = new Point(0, 0);
            DrawColor      = Color.White;
            LastUpdateTime = DateTime.Now;

        }

        /// <summary>
        /// Updates the animation
        /// </summary>
	    /// <param name="position">The position on screen to draw the image</param>
		/// <param name="drawSize">The size to draw the image</param>
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

            Source      = new Rectangle(ImageSize.X * AnimatedPoint.X, ImageSize.Y * AnimatedPoint.Y, ImageSize.X,
                ImageSize.Y);
            Destination = new Rectangle((int)position.X, (int)position.Y, (int)drawSize.Y, (int)drawSize.X);

        }

        /// <summary>
        /// Draws the current image from the spritesheet based on the calculation of <see cref="UpdateAnimation"/>
        /// </summary>
	    /// <param name="batch"> A reference to the current SpriteBatch</param>
	    /// <param name="sidewaysVelocity">The velocity of the character sideways. Used to flip the sprite if the motion
	    /// is in the opposite direction of how it is drawn. Leave blank for no effect.</param>
        public void DrawAnimation(ref SpriteBatch batch, float sidewaysVelocity = 0) {

            SpriteEffects Effect = (sidewaysVelocity < -0.1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            batch.Draw(SpriteSheet, Destination, Source, DrawColor);
            batch.Draw(SpriteSheet, Destination, Source, DrawColor, 0, Vector2.Zero, Effect, 0F);

        }

    }

}
