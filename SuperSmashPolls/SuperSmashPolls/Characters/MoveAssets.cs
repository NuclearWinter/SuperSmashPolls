#define DRAW_DEBUG

using System;
using System.Collections.Generic;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SuperSmashPolls.Graphics;

namespace SuperSmashPolls.Characters {

    /// <summary>
    /// Holds and manages the assets needed for every move
    /// </summary>
    public class MoveAssets {

        /// <summary>The action for this move. This is the animated texture that is drawn to the screen</summary>
        public CharacterAction Animation;
        /// <summary>This is the sound played when this move is done</summary>
        public AudioHandler Sound;
        /// <summary>These are the vertices to create hitboxes from for the collision bodies. These need to be here 
        /// so that we don't store the texture and need to re-decompose it every time we need a new body</summary>
        public List<Vertices>[] HitboxVertices;
        /// <summary>These are the bodies for the hitboxes to be generated when a level is being started</summary>
        public Body[] HitboxBodies;
        /// <summary>This is the type of function that needs to be used as a move</summary>
        /// <param name="currentFrame">The current frame of the move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedPoints">The points in the world that will be affected by this move</param>
        public delegate void MoveFunction(int currentFrame, float direction, List<Vector2> affectedPoints);
        /// <summary>The function to run when this move is activated</summary>
        public MoveFunction Function;

        /// <summary>
        /// Adds the assets needed for each move
        /// </summary>
        /// <param name="playTime">The time it takes for the animation to run</param>
        /// <param name="imageSize">The size of each individual image</param>
        /// <param name="spriteSheet">The spritesheet to draw as the animation for this move</param>
        /// <param name="scale">The scale needed to fit the textures on the screen properly</param>
        /// <param name="hitboxes">The texture of the hitboxes for moves. Must be the same image size AND scale as 
        /// spriteSheet</param>
        /// <param name="function">The function to run when this move is used</param>
        /// <param name="effect">Any effects to play when this move is used</param>
        public MoveAssets(float playTime, Point imageSize, Texture2D spriteSheet, int scale, Texture2D hitboxes,
            MoveFunction function, params SoundEffect[] effect) {

            Animation      = new CharacterAction(playTime, imageSize, spriteSheet, scale);
            Sound          = new AudioHandler(effect);
            HitboxVertices = CreateVerticesFromTexture(hitboxes, scale, imageSize);
            Function       = function;

        }

        /// <summary>
        /// Constructs the hitbox bodies for this move
        /// </summary>
        /// <param name="world">The world to put these bodies in</param>
        /// <param name="characterCollides">The collision category for the player</param>
        /// <param name="hitboxCollides">The categories of objects that these moves should collide with</param>
        public void ConstructBodies(World world, Category characterCollides, params Category[] hitboxCollides) {

            Animation.GenerateBodies(world, characterCollides);

            Category Result = Category.None;

            foreach (var I in hitboxCollides)
                Result = Result | I;

            for (int I = 0; I < HitboxVertices.Length; ++I) {
                //This actually creates the body
                HitboxBodies[I] = BodyFactory.CreateCompoundPolygon(world, HitboxVertices[I], 1, Vector2.Zero);
                HitboxBodies[I].BodyType     = BodyType.Dynamic;
                HitboxBodies[I].Enabled      = false;
                HitboxBodies[I].CollidesWith = Result; //TODO test this

            }

        }

        /// <summary>
        /// Updates the move of the character
        /// </summary>
        /// <returns>If the move is done or inturruptable</returns>
        /// <remarks>Drawing for moves is taken care of in the Moves class</remarks>
        public bool UpdateMove(float direction, Vector2 characterLocation) {

            int CurrentIndex = Animation.GetCurrentIndex();

            HitboxBodies[CurrentIndex].Enabled  = true;
            HitboxBodies[CurrentIndex].Position = characterLocation;

            List<Vector2> AffectedPoints = FindTouchingPoints();

            HitboxBodies[CurrentIndex].Enabled = false;

            Animation.UpdateAnimation(characterLocation);
            Function(CurrentIndex, direction, AffectedPoints);
            Sound.PlayEffect();

            return Animation.AnimationAtEnd();

        }

        /// <summary>
        /// Gets the position of the current body
        /// </summary>
        /// <returns>The position of the current action</returns>
        public Vector2 GetPosition() {

            return Animation.GetPosition();

        }

        /// <summary>
        /// Finds the points in the world where the current hitbox is colliding with something
        /// </summary>
        /// <returns>The points in the world where the hitbox is colliding with something</returns>
        private List<Vector2> FindTouchingPoints() {

            List<Vector2> ContactPoints = new List<Vector2>();

            ContactEdge Contacts = HitboxBodies[Animation.GetCurrentIndex()].ContactList;
            Contact HitContact   = Contacts.Contact;

            while (HitContact != null) {
                
                if (HitContact.IsTouching)
                    ContactPoints.Add(HitContact.Manifold.Points[0].LocalPoint); //TODO see if this is the correct way to do this

                HitContact = Contacts.Next.Contact;

            }

            return ContactPoints;

        }

        /// <summary>
        /// Creates a list of vertices from a texture.
        /// </summary>
        /// <param name="texture">The texture to make a body from</param>
        /// <param name="scale">The scale of the texture</param>
        /// <param name="imageSize">The size of each individual image in the hitbox</param>
        /// <param name="density">The density of the object (Will almost always be one</param>
        /// <param name="algorithm">The decomposition algorithm to use</param>
        /// <remarks> Available algorithms to use are Bayazit, Dealuny, Earclip, Flipcode, Seidel, SeidelTrapazoid</remarks>
        /// @warning In order for this to work the input must have a transparent background. I highly reccomend that you
        /// only use this with PNGs as that is what I have tested and I know they work. This will only produce a bosy as
        /// clean as the texture you give it, so avoid partically transparent areas and little edges.
        private List<Vertices>[] CreateVerticesFromTexture(Texture2D texture, float scale, Point imageSize,
            float density = 1, TriangulationAlgorithm algorithm = TriangulationAlgorithm.Earclip) {

            int SpriteSheetSize = texture.Width * texture.Height;
            int IndividualSize = imageSize.X * imageSize.Y;

            uint[] TextureData = new uint[SpriteSheetSize]; //Array to copy texture info into
            texture.GetData(TextureData); //Gets which pixels of the texture are actually filled

            List<uint[]> IndividualData = new List<uint[]>();

            for (int Processed = 0; Processed < SpriteSheetSize; Processed += IndividualSize) {

                uint[] TempArray = new uint[IndividualSize];

                Array.Copy(TextureData, Processed, TempArray, 0, IndividualSize);

                IndividualData.Add(TempArray);

            }

            List<Vertices>[] TextureVertices = new List<Vertices>[IndividualData.Count];

            for (int count = 0; count < IndividualData.Count; ++count) {

                uint[] I = IndividualData[count];

                Vertices vertices = TextureConverter.DetectVertices(I, texture.Width);
                List<Vertices> VertexList = Triangulate.ConvexPartition(vertices, algorithm);

                Vector2 VertScale = new Vector2(ConvertUnits.ToSimUnits(scale));

                foreach (Vertices vert in VertexList)
                    vert.Scale(ref VertScale); //Scales the vertices to match the size we specified

                Vector2 Centroid = -vertices.GetCentroid();
                vertices.Translate(ref Centroid);
                //basketOrigin = -centroid;

                TextureVertices[count] = VertexList;

            }

            return TextureVertices;

        }

    }

}
