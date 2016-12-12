#define COMPLEX_MOVES
#undef COMPLEX_MOVES

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
        protected readonly MoveAssets[] CharacterMoves;
        public Body ActiveBody;
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
        /** Whether or not the current move affects the character (rather than another character) */
        private bool OnCharacter;

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
            Position          = new Vector2();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Moves Clone() {

            return new Moves(CharacterMoves[0].Clone(), CharacterMoves[1].Clone(), CharacterMoves[2].Clone(), CharacterMoves[3].Clone(),
                CharacterMoves[4].Clone(), CharacterMoves[5].Clone(), CharacterMoves[6].Clone(), CharacterMoves[7].Clone(), CharacterCategory,
                HitboxCategory);

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

            ActiveBody = CharacterMoves[CurrentMove].Animation.FirstBody();
            ActiveBody.Enabled = true;

        }

        /// <summary>
        /// Sets the position of the character
        /// </summary>
        /// <param name="positon"></param>
        public void SetPosition(Vector2 positon) {

            ActiveBody.Position = positon;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPostion() {

            return ActiveBody.Position;

        }

        /// <summary>
        /// Updates the running move by either continuing it, or starting the new one
        /// </summary>
        /// <param name="desiredMove">The move that CharacterManager wants to use, if able</param>
        /// <param name="direction">The direction of the character</param>
        public void UpdateMove(int desiredMove, float direction) {

#if COMPLEX_MOVES

            Vector2 tempPosition = ActiveBody.Position;
            Vector2 tempVelocity = ActiveBody.LinearVelocity;
            float tempAngVelocity = ActiveBody.AngularVelocity;
            ActiveBody.Enabled = false;
            ActiveBody.Awake = false;
            Body Placeholder =
                CharacterMoves[CurrentMove].Animation.UpdateAnimation(ConvertUnits.ToDisplayUnits(ActiveBody.Position));// -
//                                                                      new Vector2(0,
//                                                                          CharacterMoves[CurrentMove].Animation
//                                                                              .BodyOrigin.Y));
            //TODO align body correctly (height differential, origin position)
            Placeholder = CharacterMoves[CurrentMove].Animation.FirstBody();
            //Placeholder.ResetDynamics();
            Placeholder.Position = tempPosition;
            Placeholder.LinearVelocity = tempVelocity;
            Placeholder.AngularVelocity = tempAngVelocity;
            Placeholder.Enabled = true;
            Placeholder.Awake = true;

            ActiveBody = Placeholder;

#endif

            if (!CharacterMoves[CurrentMove].UpdateMove(direction, ActiveBody.Position, OnCharacter) && 
                !((CurrentMove == IdleIndex) || (CurrentMove == WalkIndex) || (CurrentMove == JumpIndex)))
               return;

            if (CurrentMove != desiredMove)
                CharacterMoves[desiredMove].StartMove();

            Direction   = direction; //This keeps the direction from updating before the move is done
//            if (CurrentMove != desiredMove)
//                ActiveBody.Position += ConvertUnits.ToSimUnits(new Vector2(0, -10));

            CurrentMove = desiredMove;
            OnCharacter = (CurrentMove == IdleIndex) || (CurrentMove == WalkIndex) || (CurrentMove == JumpIndex);

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
