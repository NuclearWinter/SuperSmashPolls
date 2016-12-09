using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SuperSmashPolls.Graphics;

namespace SuperSmashPolls.Characters {

    /// <summary>
    /// Class for handling character moves. The moves themself must be defined within the MoveDefinition class, than
    /// that class passed to here, than the filled class has to be put within its corresponding character.
    /// </summary>
    /// <remarks>To have audio play during a move you must run the AudioHandler.PlayEffect command</remarks>
    public class Moves { //Setup contact

        /// <summary>The index of various moves to be used in their arrays</summary>
        public const int IdleIndex = 0,
            WalkIndex              = 1,
            JumpIndex              = 2,
            SpecialIndex           = 3,
            SideSpecialIndex       = 4,
            UpSpecialIndex         = 5,
            DownSpecialIndex       = 6,
            BasicIndex             = 7;
        /** The moves for this character */
        private readonly MoveAssets[] CharacterMoves;
        /** The index of the current move */
        private int CurrentMove;
        /** The collision category of the player */
        private readonly Category CharacterCategory;
        /** The category for hitboxes to collide with */
        private readonly Category HitboxCategory;
        /** The direction that the character is facing */
        private float Direction;
        /** The position of this character */
        private Vector2 Position;

        /// <summary>
        /// Constructs the class to handle moves
        /// </summary>
        /// <param name="idle"></param>
        /// <param name="walk"></param>
        /// <param name="jump"></param>
        /// <param name="special"></param>
        /// <param name="sideSpecial"></param>
        /// <param name="upSpecial"></param>
        /// <param name="downSpecial"></param>
        /// <param name="basic"></param>
        /// <param name="characterCategory"></param>
        /// <param name="hitboxCategory"></param>
        public Moves(MoveAssets idle, MoveAssets walk, MoveAssets jump, MoveAssets special, MoveAssets sideSpecial,
            MoveAssets upSpecial, MoveAssets downSpecial, MoveAssets basic, Category characterCategory, 
            Category hitboxCategory) { 

            CharacterCategory = characterCategory;
            HitboxCategory    = hitboxCategory;
            CurrentMove       = 0;
            CharacterMoves    = new[] {idle, walk, jump, special, sideSpecial, upSpecial, downSpecial, basic};
            Position = new Vector2();

        }

        /// <summary>
        /// Makes the hitbox and movement bodies in the world
        /// </summary>
        /// <param name="world"></param>
        /// <param name="mass"></param>
        /// <param name="friction"></param>
        /// <param name="restitution"></param>
        public void MakeBodies(World world, float mass, float friction, float restitution) {

            foreach (var I in CharacterMoves) {

                I.ConstructBodies(world, CharacterCategory, HitboxCategory);
                I.Animation.SetCharactaristics(mass, friction, restitution, CharacterCategory);
                    
            }

        }

        /// <summary>
        /// Sets the position of the character
        /// </summary>
        /// <param name="positon"></param>
        public void SetPosition(Vector2 positon) {

            CharacterMoves[CurrentMove].Animation.Bodies[CharacterMoves[CurrentMove].Animation.GetCurrentIndex()]
                .Position = positon;

        }

        /// <summary>
        /// Updates the running move by either continuing it, or starting the new one
        /// </summary>
        /// <param name="desiredMove">The move that CharacterManager wants to use, if able</param>
        /// <param name="direction">The direction of the character</param>
        public void UpdateMove(int desiredMove, float direction) {

            Position = CharacterMoves[CurrentMove].GetPosition();

            if (!CharacterMoves[CurrentMove].UpdateMove(direction, Position))
                return;

            Direction   = direction; //This keeps the direction from updating before the move is done
            CurrentMove = desiredMove;

        }

        /// <summary>
        /// Draws the current move
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawMove(SpriteBatch spriteBatch) {
            
            CharacterMoves[CurrentMove].Animation.DrawAnimation(ref spriteBatch, Direction);

        }

    }

}
