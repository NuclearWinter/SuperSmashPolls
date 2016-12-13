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
#if COMPLEX_BODIES
        /// <summary>Holds the generated bodies for this action</summary>
        public readonly Body[] Bodies;
        /// <summary>The origin to each body</summary>
        public readonly Vector2 BodyOrigin;
                /// <summary>Tells if the bodies have been generated for this character</summary>
        public bool BodiesGenerated;
#endif
        /// <summary>The spritesheet to take a value from (can be just one image)</summary>
        public readonly Texture2D SpriteSheet;
        /// <summary>The amount of time (in seconds) that it takes to cycle through the sheet</summary>
        public readonly float PlayTime;
        /// <summary>The size of each item in the sheet (ie. 32-bit spritesheet is (32, 32)</summary>
        public readonly Point ImageSize;
        /// <summary>The color to draw the image with, defaults to clear</summary>
        public Color DrawColor;
        /// <summary>The scale to use</summary>
        public int Scale;
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
        /** The time that the animation was started */
        private DateTime StartedAnimation;

#if COMPLEX_BODIES
        /// <summary>
        /// Constructor from an already constructed CharacterAction
        /// </summary>
        /// <param name="playTime">The amount of time (in seconds) that it takes to loop through the entire sheet</param>
        /// <param name="imageSize">The size of one image on the sheet (i.e. 32 bit sheet is 32 x 32)</param>
        /// <param name="spriteSheet">The texture of the sheet</param>
        /// <param name="bodies"></param>
        /// <param name="scale"></param>
        public CharacterAction(float playTime, Point imageSize, Texture2D spriteSheet, Body[] bodies, int scale) {

            PlayTime       = playTime;
            ImageSize      = imageSize;
            Scale          = scale;
            SheetSize      = new Point(spriteSheet.Width / imageSize.X, spriteSheet.Height / imageSize.Y);
            SpriteSheet    = spriteSheet;
            AnimatedPoint  = new Point(0, 0);
            DrawColor      = Color.White;
            LastUpdateTime = DateTime.Now;

            Bodies         = new Body[SheetSize.X * SheetSize.Y];
            Array.Copy(bodies, Bodies, bodies.Length);
            BodiesGenerated = true;
            BodyOrigin = CalculateOrigin(imageSize.X, imageSize.Y);
            StartedAnimation = DateTime.Now;

        }
#endif

        /// <summary>
        /// Constructor from only imported items and constants
        /// </summary>
        /// <param name="playTime">The amount of time (in milliseconds) that it takes to loop through the entire sheet</param>
        /// <param name="imageSize">The size of one image on the sheet (i.e. 32 bit sheet is 32 x 32)</param>
        /// <param name="spriteSheet">The texture of the sheet</param>
        /// <param name="scale"></param>
        public CharacterAction(float playTime, Point imageSize, Texture2D spriteSheet, int scale) {
#if COMPLEX_BODIES
            BodiesGenerated  = false;
            SpriteSheet      = spriteSheet;
            BodyOrigin       = CalculateOrigin(imageSize.X, imageSize.Y);
            Bodies           = new Body[SheetSize.X * SheetSize.Y];

#endif
            StartedAnimation = DateTime.Now;
            LastUpdateTime   = DateTime.Now;
            AnimatedPoint    = new Point(0, 0);
            DrawColor        = Color.White;
            ImageSize        = imageSize;
            SheetSize        = new Point(spriteSheet.Width / imageSize.X, spriteSheet.Height / imageSize.Y);
            PlayTime         = playTime;
            Scale            = scale;
        }

#if COMPLEX_BODIES
        /// <summary>
        /// Generates the bodies for each of the pieces of the sheet.
        /// </summary>
        /// <param name="levelWorld">The world to create bodies in</param>
        /// <param name="collisionCategory">The FULL category of what these bodies should collide with</param>
        public void GenerateBodies(World levelWorld, Category collisionCategory) { //TODO get scale

            BodiesGenerated = true;

            int SpriteSheetSize = SpriteSheet.Width * SpriteSheet.Height;
            int IndividualSize  = ImageSize.X * ImageSize.Y;

            uint[] TextureData = new uint[SpriteSheetSize]; //Array to copy texture info into
            SpriteSheet.GetData<uint>(TextureData); //Gets which pixels of the texture are actually filled

            List<uint[]> IndividualData = new List<uint[]>();

            for (int Processed = 0; Processed < SpriteSheetSize && IndividualData.Count < Bodies.Length; Processed += IndividualSize) {
                //TODO CHECK IF THIS WORKS TESTING TO CUT OFF ARRAY ARGUMENT EXCEPTION
                uint[] TempArray = new uint[IndividualSize];

                try {

                    Array.Copy(TextureData, Processed, TempArray, 0, IndividualSize);

                } catch (ArgumentException) {
                    //At the end of textures the amount of data left might be to small
                    Array.Copy(TextureData, Processed, TempArray, 0, TextureData.Length - Processed);

                }

                IndividualData.Add(TempArray);

            }

            int BodyIndex = 0;

            for (int count = 0; count < IndividualData.Count; ++count) {

                uint[] I = IndividualData[count];

                Vertices vertices         = TextureConverter.DetectVertices(I, ImageSize.X);
                List<Vertices> VertexList = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Earclip); 
                //error with bayazit and deluny
                //Siedle doesnt wortk
                //Earclip & flipcode results in glitches
                //Earclip works very well

                Vector2 VertScale = new Vector2(ConvertUnits.ToSimUnits(Scale)); 

                foreach (Vertices vert in VertexList)
                    vert.Scale(ref VertScale); //Scales the vertices to match the size we specified

                Vector2 Centroid = -vertices.GetCentroid();
                vertices.Translate(ref Centroid);
                //basketOrigin = -centroid;

                //This actually creates the body
                Bodies[BodyIndex] = BodyFactory.CreateCompoundPolygon(levelWorld, VertexList, 1, Vector2.Zero);
                Bodies[BodyIndex].BodyType = BodyType.Dynamic;
                Bodies[BodyIndex].Enabled  = false;
                //Bodies[BodyIndex].CollisionCategories = collisionCategory;
                ++BodyIndex;

            }

        }

                /// <summary>
        /// Gets the first frame's body
        /// </summary>
        /// <returns></returns>
        public Body FirstBody() {

            return Bodies[0];

        }

        /// <summary>
        /// Sets the properties for the bodies in this object
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="friction"></param>
        /// <param name="restitution"></param>
        /// <param name="collision"></param>
        public void SetCharactaristics(float mass, float friction, float restitution, Category collision) {

            foreach (Body i in Bodies) {

                //i.CollisionCategories = collision;
                i.Restitution         = restitution;
                i.Friction            = friction;
                i.Mass                = mass;
                i.Awake = false;
                i.Enabled = false;
                i.AngularDamping = 1;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prepareFrom"></param>
        /// <param name="position"></param>
        public void PrepareBody(Body prepareFrom, Vector2 position) {

            prepareFrom.Enabled = false;
            Bodies[GetCurrentIndex()].LinearVelocity = prepareFrom.LinearVelocity;
            Bodies[GetCurrentIndex()].Position       = position;
            Bodies[GetCurrentIndex()].Enabled        = true;

        }

        /// <summary>
        /// Gets the position of the character body
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPosition() {

            return Bodies[GetCurrentIndex()].Position;

        }

        /// <summary>
        /// Calculates the origin of a body
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static Vector2 CalculateOrigin(float x, float y) {

            return (new Vector2(x, y)) / 2;

        }

#endif

        /// <summary>
        /// Gets the index of the current frame of the animation
        /// </summary>
        /// <returns></returns>
        public int GetCurrentIndex() {

            return AnimatedPoint.X*AnimatedPoint.Y;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool AnimationAtEnd() {

            return (DateTime.Now - StartedAnimation).TotalMilliseconds > PlayTime;

        }

        /// <summary>
        /// Updates the animation
        /// </summary>
	    /// <param name="position">The position on screen to draw the image</param>
		/// <returns>The body related to the current action (if bodies are enabled)</returns>
		/// <remarks>The body returned from here must be enabled if it is to collide with anything</remarks>
        public
#if COMPLEX_BODIES
            Body
#else
            void
#endif
            UpdateAnimation(Vector2 position) {

            if ((DateTime.Now - StartedAnimation).TotalMilliseconds > PlayTime)
                StartedAnimation = DateTime.Now;

            DateTime Now = DateTime.Now;

            if ((Now - LastUpdateTime).Milliseconds >= (PlayTime) / (SheetSize.X*SheetSize.Y)) {

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
#if COMPLEX_BODIES
            return Bodies[AnimatedPoint.X*AnimatedPoint.Y];
#endif

        }

        /// <summary>
        /// Draws the current image from the spritesheet based on the calculation of <see cref="UpdateAnimation"/>
        /// </summary>
	    /// <param name="batch"> A reference to the current SpriteBatch</param>
	    /// <param name="sidewaysVelocity">The velocity of the character sideways. Used to flip the sprite if the motion
	    /// is in the opposite direction of how it is drawn. Leave blank for no effect.</param>
        public void DrawAnimation(ref SpriteBatch batch, float sidewaysVelocity = 0) {

            SpriteEffects Effect = (sidewaysVelocity < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            batch.Draw(SpriteSheet, Destination, Source, DrawColor, 0, Vector2.Zero, Effect, 0F);

        }

    }

}
