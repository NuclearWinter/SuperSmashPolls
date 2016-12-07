using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
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
    public abstract class Moves {

        /// <summary>Audio handlers to play during attacks</summary>
        protected internal AudioHandler SpecialSound,
            SideSpecialSound,
            UpSpecialSound,
            DownSpecialSound,
            BasicAttackSound;
        /// <summary>The bodies of moves. These are used to detect collisions with characters</summary>
        protected internal List<Vertices> SpecialCollider,
            SideSpecialCollider,
            UpSpecialCollider,
            DownSpecialCollider,
            BasicAttackCollider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="special"></param>
        /// <param name="sideSpecial"></param>
        /// <param name="upSpecial"></param>
        /// <param name="downSpecial"></param>
        /// <param name="basicAttack"></param>
        protected internal void AssignColliderBodies(Texture2D special, Texture2D sideSpecial, Texture2D upSpecial, 
            Texture2D downSpecial, Texture2D basicAttack) {
            
            

        }

        /// <summary>
        /// Trys to play the sound effect for the special attack
        /// </summary>
        protected internal void PlaySpecialSound() {

            try {

                SpecialSound.PlayEffect();

            } catch (NullReferenceException) {

                Console.WriteLine("Special attack sound not available");

            }

        }

        /// <summary>
        /// The basic attack that all characters have
        /// </summary>
        /// <param name="character"></param>
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
        /// Creates a polygon from a texture. This is the important function here.
        /// </summary>
        /// <param name="texture">The texture to make a body from</param>
        /// <param name="density">The density of the object (Will almost always be one</param>
        /// <param name="scale">The scale of the object (how much to change its size)</param>
        /// <param name="algorithm">The decomposition algorithm to use</param>
        /// <remarks> Available algorithms to use are Bayazit, Dealuny, Earclip, Flipcode, Seidel, SeidelTrapazoid</remarks>
        /// @warning In order for this to work the input must have a transparent background. I highly reccomend that you
        /// only use this with PNGs as that is what I have tested and I know they work. This will only produce a bosy as
        /// clean as the texture you give it, so avoid partically transparent areas and little edges.
        public List<Vertices> CreatePolygonFromTexture(Texture2D texture, float scale, float density = 1,
            TriangulationAlgorithm algorithm = TriangulationAlgorithm.Earclip) {

            uint[] TextureData = new uint[texture.Width * texture.Height]; //Array to copy texture info into

            texture.GetData<uint>(TextureData); //Gets which pixels of the texture are actually filled

            Vertices vertices = TextureConverter.DetectVertices(TextureData, texture.Width);
            List<Vertices> VertexList = Triangulate.ConvexPartition(vertices, algorithm);

            Vector2 VertScale = new Vector2(ConvertUnits.ToSimUnits(scale));
            foreach (Vertices Vert in VertexList)
                Vert.Scale(ref VertScale); //Scales the vertices to match the size we specified

            Vector2 Centroid = -vertices.GetCentroid();
            vertices.Translate(ref Centroid);
            //basketOrigin = -centroid;

            return VertexList;

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
        /// Adds moves to a character
        /// </summary>
        /// <param name="addTo">The character to add moves to</param>
        public void AddMovesToCharacter(Character addTo) {
            
            addTo.AddCharacterMoves(SideSpecial, UpSpecial, DownSpecial, Special, BasicAttack);

        }

        /// <summary>
        /// Adds the sounds to this class
        /// </summary>
        /// <param name="sideSpecialSound">Audio manajer to handle playing during the side special</param>
        /// <param name="upSpecialSound">Audio manajer to handle playing during the up special</param>
        /// <param name="downSpecialSound">Audio manajer to handle playing during the down special</param>
        /// <param name="specialSound">Audio manajer to handle playing during the special</param>
        /// <param name="basicAttackSound">Audio manajer to handle playing during a basic attack</param>
        public void AddAudio(AudioHandler sideSpecialSound, AudioHandler upSpecialSound, AudioHandler downSpecialSound,
            AudioHandler specialSound, AudioHandler basicAttackSound) {
            SideSpecialSound = sideSpecialSound;
            UpSpecialSound   = upSpecialSound;
            DownSpecialSound = downSpecialSound;
            SpecialSound     = specialSound;
            BasicAttackSound = basicAttackSound;
        }

    }

}
