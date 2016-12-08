using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperSmashPolls.Graphics;
using SuperSmashPolls.Levels;

namespace SuperSmashPolls.Characters {

    /// <summary>
    /// Parent class for creating character moves.
    /// </summary>
    /// <remarks>To have audio play during a move you must run the AudioHandler.PlayEffect command</remarks>
    public abstract class Moves { //Setup contact

        /// <summary>The index of various moves to be used in their arrays</summary>
        protected internal const int SpecialIndex = 0,
            SideSpecialIndex = 1,
            UpSpecialIndex   = 2,
            DownSpecialIndex = 3,
            BasicIndex       = 4;

        /// <summary>Audio handlers to play during attacks</summary>
        protected internal AudioHandler[] MoveSounds;

        /// <summary>The bodies of moves. These are used to detect collisions with characters</summary>
        protected internal List<Vertices>[] MoveVertices;

        /// <summary>The bodies of moves. These are used to detect collisions with characters</summary>
        protected internal Body[] MoveColliders;

        /// <summary>These are the collision groups for the other players in the game</summary>
        protected internal short OtherIdOne, OtherIdTwo, OtherIdThree;

        /// <summary>The scale of collider textures</summary>
        protected internal float ColliderScale;

        /// <summary>
        /// Trys to play the sound effect for the special attack
        /// </summary>
        /// <param name="move">The move to play the sound for</param>
        protected internal void PlaySound(int move = SpecialIndex) {

            try {

                MoveSounds[move].PlayEffect();

            } catch (NullReferenceException) {

                Console.WriteLine("Attack sound not available");

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A list of points in the world where bodies should be effected by the move</returns>
        /// TODO test this
        protected internal List<FixedArray2<ManifoldPoint>> UseSpecial() {

            var Contacts = MoveColliders[SpecialIndex].ContactList;

            List<FixedArray2<ManifoldPoint>> WorldPoints = new List<FixedArray2<ManifoldPoint>>();

            for (var Contact = Contacts.Contact; Contact != null; Contact = Contacts.Next.Contact) 
                if (Contact.IsTouching && Contact.Enabled)
                    WorldPoints.Add(Contact.Manifold.Points);

            return WorldPoints;

        }

        /// <summary>
        /// The basic attack that all characters have
        /// </summary>
        /// <param name="character">The character preforming this move</param>
        /// TODO fix this
        protected internal void BasicPunch(Character character) {
            //If true, moving forwards (right), if negative backwards (left)
            bool Direction = character.CharacterBody.LinearVelocity.X > 0;

            Vector2 AttackPosition = character.CharacterBody.Position; //16 = punch texture width
            AttackPosition.X += (Direction) ? ConvertUnits.ToSimUnits(16) : -ConvertUnits.ToSimUnits(16);

            SimpleExplosion Explosion = new SimpleExplosion(character.GameWorld) {
                Power = 1,
                DisabledOnGroup = character.CharacterBody.CollisionGroup
            };

            Explosion.Activate(AttackPosition, ConvertUnits.ToSimUnits(30), 700);

        }

        /// <summary>
        /// Generates the bodies for use as hitboxes
        /// </summary>
        /// <param name="world"></param>
        /// <param name="otherIdOne"></param>
        /// <param name="otherIdTwo"></param>
        /// <param name="otherIdThree"></param>
        public void GenerateHitboxBodies(World world, short otherIdOne, short otherIdTwo, short otherIdThree) {

            OtherIdOne    = otherIdOne;
            OtherIdTwo    = otherIdTwo;
            OtherIdThree  = otherIdThree;
            MoveColliders = new Body[5];
            
            for (int i = 0; i < 5; ++i)
                MoveColliders[i] = BodyFactory.CreateCompoundPolygon(world, MoveVertices[i], 1, Vector2.Zero);

        }

        /// <summary>
        /// Adds moves to a character
        /// </summary>
        /// <param name="addTo">The character to add moves to</param>
        public void AddMovesToCharacter(Character addTo) {

            addTo.AddCharacterMoves(SideSpecial, UpSpecial, DownSpecial, Special, BasicAttack);

        }

        /// <summary>
        /// Assigns the vertices to their respective lists to be used for creating hitbox bodies.
        /// </summary>
        /// <param name="scale">The scale of these textures compared to the current screen size</param>
        /// <param name="moveTextures">The five textures to use as move hitboxes. Needs to be in this order: special, 
        /// side special, up special, down special, basic attack</param>
        public void AssignColliderTextures(float scale, params Texture2D[] moveTextures) {

            ColliderScale = scale;
            MoveVertices  = new List<Vertices>[5];

            if (moveTextures.Length != 5)
                throw new InvalidDataException("Five textures were not given to the AssignColliderTextures method");

            for (int i = 0; i < 5; ++i)
                MoveVertices[i] = CreateVerticesFromTexture(moveTextures[i]);

        }

        /// <summary>
        /// Adds the sounds to this class
        /// </summary>
        /// <param name="moveSounds">The five sounds to use for the moves. Needs to be in this order: special, 
        /// side special, up special, down special, basic attack</param>
        public void AddAudio(params AudioHandler[] moveSounds) {

            if (moveSounds.Length != 5)
                throw new InvalidDataException("Five sounds were not given to the AddAudio method");

            MoveSounds = new AudioHandler[5];

            for (int i = 0; i < 5; ++i)
                MoveSounds[i] = moveSounds[i];

        }

        /// <summary>
        /// The special move for this character
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public abstract void Special(Character character);

        /// <summary>
        /// The side special move for this character
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public abstract void SideSpecial(Character character);

        /// <summary>
        /// The up special move for this character
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public abstract void UpSpecial(Character character);

        /// <summary>
        /// The down special move for this character
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public abstract void DownSpecial(Character character);

        /// <summary>
        /// The basic attack for a character.
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public abstract void BasicAttack(Character character);

        /// <summary>
        /// Creates a list of vertices from a texture.
        /// </summary>
        /// <param name="texture">The texture to make a body from</param>
        /// <param name="density">The density of the object (Will almost always be one</param>
        /// <param name="algorithm">The decomposition algorithm to use</param>
        /// <remarks> Available algorithms to use are Bayazit, Dealuny, Earclip, Flipcode, Seidel, SeidelTrapazoid</remarks>
        /// @warning In order for this to work the input must have a transparent background. I highly reccomend that you
        /// only use this with PNGs as that is what I have tested and I know they work. This will only produce a bosy as
        /// clean as the texture you give it, so avoid partically transparent areas and little edges.
        private List<Vertices> CreateVerticesFromTexture(Texture2D texture, float density = 1,
            TriangulationAlgorithm algorithm = TriangulationAlgorithm.Earclip) {

            uint[] TextureData = new uint[texture.Width * texture.Height]; //Array to copy texture info into

            texture.GetData<uint>(TextureData); //Gets which pixels of the texture are actually filled

            Vertices vertices = TextureConverter.DetectVertices(TextureData, texture.Width);
            List<Vertices> VertexList = Triangulate.ConvexPartition(vertices, algorithm);

            Vector2 VertScale = new Vector2(ConvertUnits.ToSimUnits(ColliderScale));
            foreach (Vertices Vert in VertexList)
                Vert.Scale(ref VertScale); //Scales the vertices to match the size we specified

            Vector2 Centroid = -vertices.GetCentroid();
            vertices.Translate(ref Centroid);
            //basketOrigin = -centroid;

            return VertexList;

        }

    }

}
