using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperSmashPolls.Characters {

    /// <summary>
    /// This class handles everything relating to characters.
    /// </summary>
    public class CharacterManager {

        /** These are the indicies that animations can be called with inside Actions */
        private const int IdleIndex = 0,
            JumpIndex               = 1, //A (Players can always jump)
            RunIndex                = 2, //Right Joystick (Can always run in any direction)
            AttackIndex             = 3, //B
            SpecialAttackIndex      = 4, //X
            SpecialUpAttackIndex    = 5, //X + Left joystick up
            SpecialSideAttackIndex  = 6, //X + Left joystick to the left or right
            SpecialDownAttackIndex  = 7; //X + Left joystick down
        /** This is the amount the joystick must be over for it to register as intentional */
        private const float Register = 0.2F;
        /** The mass of the character (kg) */
        private readonly float Mass;
        /** The friction of the character */
        private readonly float Friction;
        /** The restitution of the character (how bouncy they are) */
        private readonly float Restitution;
        /** The collision category for this character's bodies */
        private readonly Category CollisionCategory;
        /**  */
        private readonly Category HitboxCategory;
        /** The direction that the character is moving */
        private float Direction;
        /**  */
        private Moves CharacterMoves;

        /// <summary> This characters name</summary>
        public string Name;

        /// <summary>
        /// Default constructor for making a blank character. The name is initialized to check and to check after a game 
        /// has already been played (and they already selected a character so it wouldn't be null)
        /// </summary>
        public CharacterManager() {
            Name = "blank";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="friction"></param>
        /// <param name="restitution"></param>
        /// <param name="collisionCategory"></param>
        /// <param name="hitboxCategory"></param>
        public CharacterManager(float mass, float friction, float restitution, Category collisionCategory, Category hitboxCategory) {
            Mass = mass;
            Friction = friction;
            Restitution = restitution;
            CollisionCategory = collisionCategory;
            HitboxCategory = hitboxCategory;
        }

        /// <summary>
        /// Add moves to this character
        /// </summary>
        public void AddMoves(MoveAssets idle, MoveAssets walk, MoveAssets jump, MoveAssets special, 
            MoveAssets sideSpecial, MoveAssets upSpecial, MoveAssets downSpecial, MoveAssets basic) {

            CharacterMoves = new Moves(idle, walk, jump, special, sideSpecial, upSpecial, downSpecial, basic,
                CollisionCategory, HitboxCategory);

        }

        /// <summary>
        /// Set the position of the character
        /// </summary>
        public void SetPosition() {
            


        }

        /// <summary>
        /// Creates the Farseer bodies for the character in the specified world
        /// </summary>
        public void ConstructInWorld(World world) {
            
            CharacterMoves.MakeBodies(world, Mass, Friction, Restitution);

        }

        /// <summary>
        /// The function to update the character
        /// </summary>
        public void UpdateCharacter(PlayerIndex player) {

            GamePadState CurrentState = GamePad.GetState(player);

            bool SideMovement  = Math.Abs(CurrentState.ThumbSticks.Left.X) >= Register;
            bool DownMovement  = Math.Abs(CurrentState.ThumbSticks.Left.Y) >= Register;
            bool SpecialAttack = Math.Abs(CurrentState.Triggers.Left)      >= Register;
            //bool Jump = 

        }

        /// <summary>
        /// 
        /// </summary>
        public void DrawCharacter(SpriteBatch spriteBatch) {
            
            CharacterMoves.DrawMove(spriteBatch);

        }

    }

}
