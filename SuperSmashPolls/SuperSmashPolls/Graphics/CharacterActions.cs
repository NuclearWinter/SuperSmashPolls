using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperSmashPolls.Graphics {

    /// <summary>
    /// Class responsible for handing animations of characters.
    /// </summary>
    public class CharacterAction {

        /** The amount of images on the X and Y axis, calculated in the constructor */
        private readonly Point SheetSize;
        /** The piece of the sheet to draw based on PlayTime @see Update */
        private Point AnimatedPoint;
        /** The last time that AnimatedPoint was updated */
        private DateTime LastUpdateTime;
        /** The source on the spritesheet to draw */
        private Rectangle Source;
        /** The destination for drawing the source rectangle */
        private Rectangle Destination;
        /// <summary>Tells if the bodies have been generated for this character</summary>
        public bool BodiesGenerated;
        /// <summary>Holds the generated bodies for this action</summary>
        public readonly Body[] Bodies;
        /// <summary>The spritesheet to take a value from (can be just one image)</summary>
        public readonly Texture2D SpriteSheet;
        /// <summary>The amount of time (in seconds) that it takes to cycle through the sheet</summary>
        public readonly int PlayTime;
        /// <summary>The size of each item in the sheet (ie. 32-bit spritesheet is (32, 32)</summary>
        public readonly Point ImageSize;
        /// <summary>The color to draw the image with, defaults to clear</summary>
        public Color DrawColor;
        /// <summary>The scale to use</summary>
        public int Scale;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="playTime">The amount of time (in seconds) that it takes to loop through the entire sheet</param>
        /// <param name="imageSize">The size of one image on the sheet (i.e. 32 bit sheet is 32 x 32)</param>
        /// <param name="spriteSheet">The texture of the sheet</param>
        public CharacterAction(int playTime, Point imageSize, Texture2D spriteSheet, Body[] bodies) {

            PlayTime       = playTime;
            ImageSize      = imageSize;
            SheetSize      = new Point(spriteSheet.Width / imageSize.X, spriteSheet.Height / imageSize.Y);
            SpriteSheet    = spriteSheet;
            AnimatedPoint  = new Point(0, 0);
            DrawColor      = Color.White;
            LastUpdateTime = DateTime.Now;
            Bodies         = new Body[SheetSize.X * SheetSize.Y];
            Array.Copy(bodies, Bodies, bodies.Length);
            Scale          = 1; //TODO get scale
            BodiesGenerated = true;

        }

        /// </summary>
        /// <param name="playTime">The amount of time (in seconds) that it takes to loop through the entire sheet</param>
        /// <param name="imageSize">The size of one image on the sheet (i.e. 32 bit sheet is 32 x 32)</param>
        /// <param name="spriteSheet">The texture of the sheet</param>
        public CharacterAction(int playTime, Point imageSize, Texture2D spriteSheet) {

            PlayTime = playTime;
            ImageSize = imageSize;
            SheetSize = new Point(spriteSheet.Width / imageSize.X, spriteSheet.Height / imageSize.Y);
            SpriteSheet = spriteSheet;
            AnimatedPoint = new Point(0, 0);
            DrawColor = Color.White;
            LastUpdateTime = DateTime.Now;
            Bodies = new Body[SheetSize.X * SheetSize.Y];
            Scale = 1; //TODO get scale
            BodiesGenerated = false;

        }

        /// <summary>
        /// Generates the bodies for each of the pieces of the sheet.
        /// </summary>
        /// <param name="LevelWorld">The world to create bodies in</param>
        public void GenerateBodies(World LevelWorld) { //TODO get scale

            BodiesGenerated = true;

            int SpriteSheetSize = SpriteSheet.Width * SpriteSheet.Height;
            int IndividualSize  = ImageSize.X * ImageSize.Y;

            uint[] TextureData = new uint[SpriteSheetSize]; //Array to copy texture info into
            SpriteSheet.GetData<uint>(TextureData); //Gets which pixels of the texture are actually filled

            List<uint[]> IndividualData = new List<uint[]>();

            for (int Processed = 0; Processed < SpriteSheetSize; Processed += IndividualSize) {

                uint[] TempArray = new uint[IndividualSize];

                Array.Copy(TextureData, Processed, TempArray, 0, IndividualSize);

                IndividualData.Add(TempArray);

            }

            int BodyIndex = 0;

            for (int count = 0; count < IndividualData.Count; ++count) {

                uint[] I = IndividualData[count];

                Vertices vertices         = TextureConverter.DetectVertices(I, SpriteSheet.Width);
                List<Vertices> VertexList = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Earclip); 
                //error with bayazit and deluny
                //Earclip & flipcode results in glitches

                Vector2 VertScale = new Vector2(ConvertUnits.ToSimUnits(Scale)); 

                foreach (Vertices vert in VertexList)
                    vert.Scale(ref VertScale); //Scales the vertices to match the size we specified

                Vector2 Centroid = -vertices.GetCentroid();
                vertices.Translate(ref Centroid);
                //basketOrigin = -centroid;

                //This actually creates the body
                Bodies[BodyIndex] = BodyFactory.CreateCompoundPolygon(LevelWorld, VertexList, 1, Vector2.Zero);
                Bodies[BodyIndex].BodyType = BodyType.Dynamic;
                Bodies[BodyIndex].Enabled  = false;
                ++BodyIndex;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Body FirstBody() {

            return Bodies[0];

        }

        /// <summary>
        /// Updates the animation
        /// </summary>
	    /// <param name="position">The position on screen to draw the image</param>
		/// <returns>The body related to the current action</returns>
		/// <remarks>The body returned from here must be enabled if it is to collide with anything</remarks>
        public Body UpdateAnimation(Vector2 position) {

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
            Destination = new Rectangle((int)position.X, (int)position.Y, ImageSize.X * Scale, ImageSize.Y * Scale);

            return Bodies[AnimatedPoint.X*AnimatedPoint.Y];

        }

        /// <summary>
        /// Draws the current image from the spritesheet based on the calculation of <see cref="UpdateAnimation"/>
        /// </summary>
	    /// <param name="batch"> A reference to the current SpriteBatch</param>
	    /// <param name="sidewaysVelocity">The velocity of the character sideways. Used to flip the sprite if the motion
	    /// is in the opposite direction of how it is drawn. Leave blank for no effect.</param>
        public void DrawAnimation(ref SpriteBatch batch, float sidewaysVelocity = 0) {

            SpriteEffects Effect = (sidewaysVelocity < -0.1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            batch.Draw(SpriteSheet, Destination, Source, DrawColor, 0, Vector2.Zero, Effect, 0F);

        }

    }

}
