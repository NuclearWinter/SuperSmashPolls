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
        /** The category for hitboxes */
        private readonly Category HitboxCategory;
        /** The direction that the character is moving */
        private float Direction;
        /** The moves for this character */
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
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="friction"></param>
        /// <param name="restitution"></param>
        /// <param name="collisionCategory"></param>
        /// <param name="hitboxCategory"></param>
        /// <param name="name"></param>
        public CharacterManager(float mass, float friction, float restitution, Category collisionCategory, 
            Category hitboxCategory, string name) {
            Mass = mass;
            Friction = friction;
            Restitution = restitution;
            CollisionCategory = collisionCategory;
            HitboxCategory = hitboxCategory;
            Name = name;
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
        /// 
        /// </summary>
        /// <param name="position"></param>
        public void Respawn(Vector2 position) {
            
            CharacterMoves.SetPosition(position);

        }

        /// <summary>
        /// The function to update the character
        /// </summary>
        public void UpdateCharacter(PlayerIndex player) {

            GamePadState CurrentState = GamePad.GetState(player);

            bool SideMovement  = Math.Abs(CurrentState.ThumbSticks.Left.X) >= Register;
            bool DownMovement  = CurrentState.ThumbSticks.Left.Y <= Register;
            bool UpMovement    = CurrentState.ThumbSticks.Left.Y >= Register;
            bool SpecialAttack = Math.Abs(CurrentState.Triggers.Left)      >= Register;
            bool Jump          = CurrentState.IsButtonDown(Buttons.A);
            bool BasicAttack   = CurrentState.IsButtonDown(Buttons.B);

            Direction = CurrentState.ThumbSticks.Left.X;

            int DesiredMove = Moves.IdleIndex;
            
            if (SpecialAttack)
                if (SideMovement)
                    DesiredMove = Moves.SideSpecialIndex;
                else if (DownMovement)
                    DesiredMove = Moves.DownSpecialIndex;
                else if (UpMovement)
                    DesiredMove = Moves.UpSpecialIndex;
                else
                    DesiredMove = Moves.SpecialIndex;
            else if (SideMovement)
                DesiredMove = Moves.WalkIndex;
            else if (Jump || UpMovement)
                DesiredMove = Moves.JumpIndex;
            else if (BasicAttack)
                DesiredMove = Moves.BasicIndex;

            CharacterMoves.UpdateMove(DesiredMove, Direction);

        }

        /// <summary>
        /// 
        /// </summary>
        public void DrawCharacter(SpriteBatch spriteBatch) {
            
            CharacterMoves.DrawMove(spriteBatch);

        }

    }

}
