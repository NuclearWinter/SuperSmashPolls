/*******************************************************************************************************************//**
 * @author William Kluge
 **********************************************************************************************************************/

#define DEBUG
 #undef DEBUG

#define DEBUG_INFO

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SuperSmashPolls.Graphics;

namespace SuperSmashPolls.Characters {

    /// <summary>
    /// This class will hold textures for characters, their moves, their effects, other characters they encounter, etc.
    /// </summary>
    public class Character {

        /* These are the indicies that animations can be called with inside Actions */
        private const int IdleIndex = 0,
            JumpIndex               = 1, //A (Players can always jump)
            RunIndex                = 2, //Right Joystick (Can always run in any direction)
            AttackIndex             = 3, //B
            SpecialAttackIndex      = 4, //X
            SpecialUpAttackIndex    = 5, //X + Left joystick up
            SpecialSideAttackIndex  = 6, //X + Left joystick to the left or right
            SpecialDownAttackIndex  = 7; //X + Left joystick down
        /* This is the amount the joystick must be over for it to register as intentional */
        private const float Register = 0.2F;
        /** The size of the character (in display units) */
        private readonly Vector2 CharacterSize;
        /** The mass of the character (kg) */
        private readonly float Mass;
        /** The friction of the character */
        private readonly float Friction;
        /** The restitution of the character */
        private readonly float Restitution;
        /** The multiplier for the character's jump force */
        private readonly float JumpForceMultiplier;
        /** The multiplier for the character's movement */
        private readonly float MovementMultiplier;
        /** The minimum time between character's special attacks */
        private readonly float SpecialAttackInterval;
        /** The move animations for this character */
        private readonly List<CharacterAction> Actions;
        /** The moves for this character. Links with Actions */
        private readonly List<CharacterMove> Moves;
        /** The DateTime to allow the character to take another action */
        private DateTime NextAction;
        /* The last time that the character jumped */
        private DateTime LastJump;
        /* The last time that the character used their special attack */
        private DateTime LastSpecialAttack;
        /* The index of a value in Actions that represents the current action of the player */
        private int CurrentActionIndex;
        /* The group that this character's moves don't affect */
        private Int16 CollisionGroup;
        /** The direction that the character is moving */
        private float Direction;
        /**  */
        private bool InAction, ImportantAction = false;
        /// <summary>The world that this character is in</summary>
        public World GameWorld;
        /// <summary>The body of the character (must be created after level selection)</summary>
        public Body CharacterBody;
        /// <summary>The amount of time between jumps for this character (in seconds)</summary>
        public float JumpInterval;
        /// <summary> This characters name</summary>
        public string Name;
        /// <summary>The type for character's moves</summary>
        public delegate void CharacterMove(Character character);

        /// <summary>
        /// Default constructor for making a blank character. The name is initialized to check and to check after a game 
        /// has already been played (and they already selected a character so it wouldn't be null)
        /// </summary>
        public Character() {
            Name = "blank";
        }

        /// <summary>
        /// Constructor for the Character class
        /// </summary>
        /// <param name="screenSize">A reference to the size of the screen</param>
        /// <param name="characterSize">Size of the in display units</param>
        /// <param name="mass">The mass of the character in kilograms</param>
        /// <param name="friction">The friction of the character</param>
        /// <param name="restitution">The restitution of the character</param>
        /// <param name="movementMultiplier">The multiplier for how much the character should move</param>
        /// <param name="jumpForceMultiplier">The multiplier for how much the character should jump</param>
        /// <param name="jumpInterval">The interval between allowable jumps for the player</param>
        /// <param name="specialAttackInterval">The minimum time between special attacks</param>
        /// <param name="name">The name of this character as refered to in the game system</param>
        public Character(ref Vector2 screenSize, Vector2 characterSize, float mass, float friction, float restitution,
            float movementMultiplier, float jumpForceMultiplier, float jumpInterval, float specialAttackInterval,
            string name) {

            CharacterSize         = characterSize;
            Mass                  = mass;
            Friction              = friction;
            Restitution           = restitution;
            MovementMultiplier    = movementMultiplier;
            JumpForceMultiplier   = jumpForceMultiplier;
            JumpInterval          = jumpInterval;
            SpecialAttackInterval = specialAttackInterval;
            CurrentActionIndex    = IdleIndex;
            LastJump              = DateTime.Now;
            LastSpecialAttack     = DateTime.Now;
            NextAction            = DateTime.Now;
            Actions               = new List<CharacterAction>();
            Moves                 = new List<CharacterMove>();
            Name                  = name;
            Direction = 0;
            InAction = false;

        }

        /// <summary>
        /// Constructor for the Character class from another character.
        /// </summary>
        /// <param name="otherCharacter">The character to copy from</param>
        /// <param name="gameWorld">The world to put the character into</param>
        /// <param name="position">The position to place the character</param>
        public Character(Character otherCharacter, World gameWorld, Vector2 position) {

            CharacterSize             = otherCharacter.CharacterSize;
            Mass                      = otherCharacter.Mass;
            Friction                  = otherCharacter.Friction;
            Restitution               = otherCharacter.Restitution;
            MovementMultiplier        = otherCharacter.MovementMultiplier;
            JumpForceMultiplier       = otherCharacter.JumpForceMultiplier;
            JumpInterval              = otherCharacter.JumpInterval;
            SpecialAttackInterval     = otherCharacter.SpecialAttackInterval;
            CurrentActionIndex        = IdleIndex;
            LastJump                  = DateTime.Now;
            LastSpecialAttack         = DateTime.Now;
            NextAction                = DateTime.Now;
            Actions                   = new List<CharacterAction>();
            Moves                     = new List<CharacterMove>();
            GameWorld                 = gameWorld;
            Name                      = otherCharacter.Name;
            Direction = 0;
            InAction = false;

            foreach (CharacterAction i in otherCharacter.Actions)
                Actions.Add(new CharacterAction(i.PlayTime, i.ImageSize, i.SpriteSheet, i.Bodies));

            CharacterBody = Actions[0].FirstBody();

            foreach (CharacterMove i in otherCharacter.Moves)
                Moves.Add(i);

        }

        /// <summary>
        /// Allows for multiple items to be easily added to the list
        /// </summary>
        /// <param name="actions"> The actions to add to Actions</param>
        private void AddToActions(params CharacterAction[] actions) {

            foreach (var i in actions)
                Actions.Add(i);

        }

        /// <summary>
        /// Adds adds all of the animations for moves to the character
        /// </summary>
        /// <remarks> Each character should have one of every move</remarks>
        /// <remarks> It is done like this so that we can avoid having to compute this all the time based off a key</remarks>
        public void AddCharacterActions(CharacterAction idle, CharacterAction jump, CharacterAction run,
            CharacterAction attack, CharacterAction specialAttack, CharacterAction specialUpAttack,
            CharacterAction sideSpecialAttack, CharacterAction downSpecialAttack) {

            AddToActions(idle, jump, run, attack, specialAttack, specialUpAttack, sideSpecialAttack, downSpecialAttack);

            CharacterBody = Actions[0].FirstBody();

        }

        /// <summary>
        /// Tells if the bodies hae been generated for this character
        /// </summary>
        /// <returns>Whether or not bodies have been generated</returns>
        public bool BodiesGenerated() {

            return Actions[0].BodiesGenerated;

        }

        /// <summary>
        /// Sets the position of the character in the world
        /// </summary>
        /// <param name="position">The position to put the character at (in meters)</param>
        public void SetPosition(Vector2 position) {

            Actions[0].Bodies[0].Position = position;

        }

        /// <summary>
        /// Add moves to the character.
        /// </summary>
        /// <param name="specialSide">The special side move to add</param>
        /// <param name="specialUp">The special up move to add</param>
        /// <param name="specialDown">The special down move to add</param>
        /// <param name="special">The special move (basic) to add</param>
        /// <param name="basicAttack">The "punch" for this character</param>
        public void AddCharacterMoves(CharacterMove specialSide, CharacterMove specialUp, CharacterMove specialDown,
            CharacterMove special, CharacterMove basicAttack) {

            Moves.Add(specialSide);
            Moves.Add(specialUp);
            Moves.Add(specialDown);
            Moves.Add(special);
            Moves.Add(basicAttack);

        }

        /// <summary>
        /// Creates the body for the character and adds it to the world.
        /// </summary>
        /// <remarks>This must be called from within the PlayerClass after the world has been selected</remarks>
        public void CreateBody(ref World gameWorld, Vector2 position, Int16 collisionGroup) {

            foreach (var I in Actions)
                I.GenerateBodies(gameWorld);

            CharacterBody                = Actions[IdleIndex].FirstBody();
            CollisionGroup               = collisionGroup;
            CharacterBody.CollisionGroup = CollisionGroup;
            GameWorld                    = gameWorld;

            foreach (CharacterAction i in Actions) 
                i.SetCharactaristics(Mass, Friction, Restitution, CollisionGroup);

        }

        private bool test = false;

        /// <summary>
        /// Updates the character. This method controls movement, actions, and updates and character model.
        /// </summary>
        /// <remarks>For debugging, Black = idle | Aqua = moving | YellowGreen = jumping | Beige = side special
        /// Magenta = up special | Maroon = down special | OliveDrab = regular special</remarks>
        public void UpdateCharacter(GamePadState gamePadState) {

            CharacterBody.AngularVelocity = 0;

            InAction = !Actions[CurrentActionIndex].AnimationAtEnd();

            if (CurrentActionIndex == RunIndex ||
                CurrentActionIndex == JumpIndex || CurrentActionIndex == IdleIndex)
                InAction = false;

            if (Math.Abs(gamePadState.ThumbSticks.Left.X) > Register) {
                //The character is moving

                //var tempAngle = CharacterBody.AngularVelocity;

                CharacterBody.ApplyForce(new Vector2(gamePadState.ThumbSticks.Left.X, 0) * MovementMultiplier);

                if (!InAction) 
                    CurrentActionIndex = RunIndex;

                Direction = gamePadState.ThumbSticks.Left.X;

#if (DEBUG)
                Actions[CurrentActionIndex].DrawColor = Color.Aqua;
#endif

            } else {
                if (!InAction)
                    CurrentActionIndex = IdleIndex;

            }

            DateTime Now = DateTime.Now;


#if (DEBUG)
            Actions[CurrentActionIndex].DrawColor = Color.Black;
#endif

            test = Now.Ticks > NextAction.Ticks;

            if (Now.Ticks > NextAction.Ticks && !InAction) {

                if (gamePadState.IsButtonDown(Buttons.A) && (Now - LastJump).TotalMilliseconds > JumpInterval*1000) {
                    //The character is jumping
                    LastJump = Now;

                    CurrentActionIndex = JumpIndex;

                    CharacterBody.ApplyLinearImpulse(new Vector2(0, -50*JumpForceMultiplier));

                    NextAction = Now.AddSeconds(SpecialAttackInterval); // Testing
#if (DEBUG)
                    Actions[CurrentActionIndex].DrawColor = Color.YellowGreen;
#endif

                }

                if (gamePadState.IsButtonDown(Buttons.B)) {

                    CurrentActionIndex = AttackIndex;

                    Moves[4](this);

                    NextAction = Now.AddSeconds(SpecialAttackInterval); // Testing
#if (DEBUG)
                    Actions[CurrentActionIndex].DrawColor = Color.Violet;
#endif

                } else if (gamePadState.Triggers.Right > Register &&
                           (Now - LastSpecialAttack).TotalMilliseconds > SpecialAttackInterval*1000) {

                    LastSpecialAttack = Now;

                    Vector2 RightStickValue = gamePadState.ThumbSticks.Right;

                    if (Math.Abs(RightStickValue.X) > Register &&
                        Math.Abs(RightStickValue.X) > Math.Abs(RightStickValue.Y)) {
                        //It is a special attack to the side
                        CurrentActionIndex = SpecialSideAttackIndex;

#if (DEBUG)
                        Actions[CurrentActionIndex].DrawColor = Color.Beige;
#endif

                        try {

                            Moves[0](this);

                        } catch (NotImplementedException) {

                            Actions[CurrentActionIndex].DrawColor = Color.Red;
                        }

                        NextAction = Now.AddSeconds(SpecialAttackInterval); // Testing

                    } else if (RightStickValue.Y > Register) {
                        //This is a special attack up
                        CurrentActionIndex = SpecialUpAttackIndex;

                        NextAction = Now.AddSeconds(SpecialAttackInterval); // Testing

#if (DEBUG)
                        Actions[CurrentActionIndex].DrawColor = Color.Magenta;
#endif

                        try {

                            Moves[1](this);

                        } catch (NotImplementedException) {

                            Actions[CurrentActionIndex].DrawColor = Color.Red;
                        }

                    } else if (RightStickValue.Y < -Register) {
                        //This is a special down attack
                        CurrentActionIndex = SpecialDownAttackIndex;

#if (DEBUG)
                        Actions[CurrentActionIndex].DrawColor = Color.Maroon;
#endif
                        try {

                            Moves[2](this);

                        } catch (NotImplementedException) {

                            Actions[CurrentActionIndex].DrawColor = Color.Red;
                        }

                    } else {
                        //This is a regular special attack
                        CurrentActionIndex = SpecialAttackIndex;

#if (DEBUG)
                        Actions[CurrentActionIndex].DrawColor = Color.OliveDrab;
#endif

                        try {

                            Moves[3](this);

                        } catch (NotImplementedException) {

                            Actions[CurrentActionIndex].DrawColor = Color.Red;
                        }

                    }

                }

            }

            Actions[CurrentActionIndex].PrepareBody(CharacterBody,
                CharacterBody.Position);

            CharacterBody =
                Actions[CurrentActionIndex].UpdateAnimation(ConvertUnits.ToDisplayUnits(CharacterBody.Position) -
                                                            Actions[CurrentActionIndex].BodyOrigin * 2);

        }

        /// <summary>
        /// Draws the character
        /// </summary>
        /// TODO fix jump animation cause Joe hates it
        public void DrawCharacter(ref SpriteBatch spriteBatch, SpriteFont font = null) {

#if DEBUG_INFO
            Vector2 testing = new Vector2(3F, 1F);

            if (font != null) {

                spriteBatch.DrawString(font, "Angular Velocity:" + CharacterBody.AngularVelocity,
                    ConvertUnits.ToDisplayUnits(testing), Color.Black);

                spriteBatch.DrawString(font, "Linear Velocity:" + CharacterBody.LinearVelocity,
                    ConvertUnits.ToDisplayUnits(testing + new Vector2(0, 2)), Color.Black);

                spriteBatch.DrawString(font, "Able to use action: " + test,
                    ConvertUnits.ToDisplayUnits(testing + new Vector2(0, 4)), Color.Black);

                spriteBatch.DrawString(font, "In action: " + InAction,
                    ConvertUnits.ToDisplayUnits(testing + new Vector2(0, 6)), Color.Black);

                spriteBatch.DrawString(font, "Action: " + CurrentActionIndex,
                    ConvertUnits.ToDisplayUnits(testing + new Vector2(0, 8)), Color.Black);

            }
#endif

            Actions[CurrentActionIndex].DrawAnimation(ref spriteBatch, Direction);

        }

        /// <summary>
        /// Gets the position of the charatcer
        /// </summary>
        /// <returns>The position of the character</returns>
        public Vector2 GetPosition() {

            return CharacterBody.Position;

        }

        /// <summary>
        /// Respawns the character by placing them at a position and reseting their dynamics.
        /// </summary>
        /// <param name="position">The position to respawn the character</param>
        public void Respawn(Vector2 position) {

            CharacterBody.Position = position;
            CharacterBody.ResetDynamics();

        }

    }

}
