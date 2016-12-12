using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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

        /// <summary> This characters name</summary>
        public string Name;
        /** The mass of the character (kg) */
        protected readonly float Mass;
        /** The friction of the character */
        protected readonly float Friction;
        /** The restitution of the character (how bouncy they are) */
        protected readonly float Restitution;
        /** The collision category for this character's bodies */
        protected Category CollisionCategory;
        /** The category for hitboxes */
        protected Category HitboxCategory;
        /** The moves for this character */
        protected Moves CharacterMoves;
        /** This is the amount the joystick must be over for it to register as intentional */
        private const float Register = 0.2F;
        /** The direction that the character is moving */
        private float Direction;

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
            Mass              = mass;
            Friction          = friction;
            Restitution       = restitution;
            CollisionCategory = collisionCategory;
            HitboxCategory    = hitboxCategory;
            Name              = name;
        }


        /// <summary>
        /// Creates a blank character class for player class intialization
        /// </summary>
        /// <param name="collidesWith"></param>
        /// <param name="hitboxCollidesWith"></param>
        public CharacterManager(Category collidesWith, Category hitboxCollidesWith) {

            Name = "blank";
            CollisionCategory = collidesWith;
            HitboxCategory = hitboxCollidesWith;

        }

        /// <summary>
        /// Copies data from one CharacterManager to another
        /// </summary>
        /// <param name="obj">The other character to copy from</param>
        /// <typeparam name="CharacterManager">The class to manage character data</typeparam>
        /// <returns>A copy of obj</returns>
//        public static CharacterManager DeepClone<CharacterManager>(CharacterManager obj) {
//
//            using (var memStream = new MemoryStream()) {
//                
//                var formatter = new BinaryFormatter();
//                formatter.Serialize(memStream, obj);
//                memStream.Position = 0;
//
//                return (CharacterManager) formatter.Deserialize(memStream);
//
//            }
//
//        }

        public CharacterManager Clone() {

            CharacterManager Clone = new CharacterManager(Mass, Friction, Restitution, CollisionCategory, HitboxCategory,
                Name);

            Clone.CharacterMoves = CharacterMoves.Clone();

            return Clone;

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
        /// Get the position of the character
        /// </summary>
        public Vector2 GetPosition() {

            return CharacterMoves.GetPostion();

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
            CharacterMoves.ActiveBody.ResetDynamics();

        }

        /// <summary>
        /// The function to update the character
        /// </summary>
        public void UpdateCharacter(GamePadState currentState) {

            bool SideMovement  = Math.Abs(currentState.ThumbSticks.Left.X) >= Register;
            bool DownMovement  = currentState.ThumbSticks.Left.Y <= Register;
            bool UpMovement    = currentState.ThumbSticks.Left.Y >= Register;
            bool SpecialAttack = Math.Abs(currentState.Triggers.Left)      >= Register;
            bool Jump          = currentState.IsButtonDown(Buttons.A);
            bool BasicAttack   = currentState.IsButtonDown(Buttons.B); 

            int DesiredMove;
            
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
            else 
                DesiredMove = Moves.IdleIndex;

            Direction = (DesiredMove == Moves.IdleIndex || DesiredMove == Moves.JumpIndex)
                ? Direction
                : currentState.ThumbSticks.Left.X;

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
