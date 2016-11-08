/*******************************************************************************************************************//**
 * @file Character.cs
 * @author William Kluge
 **********************************************************************************************************************/

 #define DEBUG
 #undef DEBUG

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
using SuperSmashPolls.World_Control;

namespace SuperSmashPolls.Characters {

    /***************************************************************************************************************//**
     * This class will hold textures for characters, their moves, their effects, other characters they encounter, etc.
     ******************************************************************************************************************/ 
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
        /** The character's origin (in display units) */
        private readonly Vector2 CharacterOrigin;
        /** The mass of the character (kg) */
        private readonly float Mass;
        /** The friction of the character */
        private readonly float Friction;
        /** The restitution of the character */
        private readonly float Restitution;
        /** The multiplier for the character's jump force */
        private readonly float JumpForceMultiplier;
        /** The amount of time between jumps for this character (in seconds) */
        private readonly float JumpInterval;
        /** The multiplier for the character's movement */
        private readonly float MovementMultiplier;
        /** The minimum time between character's special attacks */
        private readonly float SpecialAttackInterval;
        /* The last time that the character jumped */
        private DateTime LastJump;
        /* The last time that the character used their special attack */
        private DateTime LastSpecialAttack;
        /** The body of the character (must be created after level selection) */
        private Body CharacterBody;
        /** The move animations for this character */
        private List<CharacterAction> Actions;
        /* The index of a value in Actions that represents the current action of the player */
        private int CurrentActionIndex;
        /* This characters name */
        public string Name;

        /***********************************************************************************************************//**
         * Default constructor
         **************************************************************************************************************/
         public Character() { }

        /***********************************************************************************************************//**
         * Constructor for the Character class
         * @param screenSize A reference to the size of the screen
         * @param characterSize Size of the in display units
         * @param mass The mass of the character in kilograms
         * @param friction The friction of the character
         * @param restitution The restitution of the character
         * @param movementMultiplier The multiplier for how much the character should move
         * @param jumpForceMultiplier The multiplier for how much the character should jump
         * @param jumpInterval The interval between allowable jumps for the player
         * @param specialAttackInterval The minimum time between special attacks
         **************************************************************************************************************/
        public Character(ref Vector2 screenSize, Vector2 characterSize, float mass, float friction, float restitution, 
            float movementMultiplier, float jumpForceMultiplier, float jumpInterval, float specialAttackInterval, 
            String name) {
            CharacterSize         = characterSize;
            CharacterOrigin       = new Vector2((characterSize.Y)/2F, (characterSize.X)/2F);
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
            Actions               = new List<CharacterAction>();
            Name                  = name;
        }

        /***********************************************************************************************************//**
         * Constructor for the Character class from another character
         **************************************************************************************************************/
        public Character(Character otherCharacter, World gameWorld, Vector2 position) {
            CharacterSize         = otherCharacter.CharacterSize;
            CharacterOrigin       = otherCharacter.CharacterOrigin;
            Mass                  = otherCharacter.Mass;
            Friction              = otherCharacter.Friction;
            Restitution           = otherCharacter.Restitution;
            MovementMultiplier    = otherCharacter.MovementMultiplier;
            JumpForceMultiplier   = otherCharacter.JumpForceMultiplier;
            JumpInterval          = otherCharacter.JumpInterval;
            SpecialAttackInterval = otherCharacter.SpecialAttackInterval;
            CurrentActionIndex    = IdleIndex;
            LastJump              = DateTime.Now;
            LastSpecialAttack     = DateTime.Now;
            Actions               = new List<CharacterAction> {
                otherCharacter.Actions[0],
                otherCharacter.Actions[1], //attempted to fix the multiplayer action bug
                otherCharacter.Actions[2],
                otherCharacter.Actions[3],
                otherCharacter.Actions[4],
                otherCharacter.Actions[5],
                otherCharacter.Actions[6],
                otherCharacter.Actions[7]
            };
            Name = otherCharacter.Name;

            CharacterBody = BodyFactory.CreateRectangle(gameWorld, ConvertUnits.ToSimUnits(CharacterSize.Y),
                ConvertUnits.ToSimUnits(CharacterSize.X), 1F, position);
            CharacterBody.BodyType    = BodyType.Dynamic;
            CharacterBody.Friction    = Friction;
            CharacterBody.Mass        = Mass;
            CharacterBody.Restitution = Restitution;
        }

        /***********************************************************************************************************//**
         * Allows for multiple items to be easily added to the list
         * @param actions The actions to add to Actions
         **************************************************************************************************************/
        private void AddToActions(params CharacterAction[] actions) {

            foreach (var i in actions)
                Actions.Add(i);

        }

        /***********************************************************************************************************//**
         * Adds adds all of the animations for moves to the character
         * @note Each character <i>should</i> have one of every move
         * @note It is done like this so that we can avoid having to compute this all the time based off a key
         **************************************************************************************************************/
        public void AddCharacterActions(CharacterAction idle, CharacterAction jump, CharacterAction run, 
            CharacterAction attack, CharacterAction specialAttack, CharacterAction specialUpAttack, 
            CharacterAction sideSpecialAttack, CharacterAction downSpecialAttack) {
            
            AddToActions(idle, jump, run, attack, specialAttack, specialUpAttack, sideSpecialAttack, downSpecialAttack);

        }

        /***********************************************************************************************************//**
         * Creates the body for the character and adds it to the world
         * @note For now, character's bodies are all rectangles
         * @note This must be called from within the PlayerClass after the world has been selected
         **************************************************************************************************************/
        public void CreateBody(ref World gameWorld, Vector2 position) {
            
            CharacterBody = BodyFactory.CreateRectangle(gameWorld, ConvertUnits.ToSimUnits(CharacterSize.Y),
                ConvertUnits.ToSimUnits(CharacterSize.X), 1F, position);
            CharacterBody.BodyType    = BodyType.Dynamic;
            CharacterBody.Friction    = Friction;
            CharacterBody.Mass        = Mass;
            CharacterBody.Restitution = Restitution;

        }

        /***********************************************************************************************************//**
         * Updates the character.
         * This method controls movement, actions, and updates and character models accordingly
         * @notes For debugging, Black = idle | Aqua = moving | YellowGreen = jumping | Beige = side special
         * Magenta = up special | Maroon = down special | OliveDrab = regular special
         **************************************************************************************************************/
        public void UpdateCharacter(GamePadState gamePadState) {

            DateTime Now = DateTime.Now;

            CurrentActionIndex = IdleIndex;

#if DEBUG
            Actions[CurrentActionIndex].DrawColor = Color.Black;
#endif

            if (Math.Abs(CharacterBody.LinearVelocity.Y) > 0.01F)
                CurrentActionIndex = JumpIndex;

            if (gamePadState.IsButtonDown(Buttons.A) && (Now - LastJump).TotalMilliseconds > JumpInterval * 1000) {
            //The character is jumping
                LastJump = Now;

                CurrentActionIndex = JumpIndex;

                CharacterBody.ApplyLinearImpulse(new Vector2(0, -10 * JumpForceMultiplier));

#if DEBUG
                Actions[CurrentActionIndex].DrawColor = Color.YellowGreen;
#endif

            }

            if (gamePadState.IsButtonDown(Buttons.B)) {

                CurrentActionIndex = AttackIndex;

#if DEBUG
                Actions[CurrentActionIndex].DrawColor = Color.Violet;
#endif

            }

            if (Math.Abs(gamePadState.ThumbSticks.Left.X) > Register) {
            //The character is moving
                CharacterBody.ApplyForce(new Vector2(gamePadState.ThumbSticks.Left.X, 0)*MovementMultiplier);

                CurrentActionIndex = RunIndex;

#if DEBUG
                Actions[CurrentActionIndex].DrawColor = Color.Aqua;
#endif

            }

            if (gamePadState.Triggers.Right > Register &&
                (Now - LastSpecialAttack).TotalMilliseconds > SpecialAttackInterval * 1000) {

                LastSpecialAttack = Now;

                Vector2 RightStickValue = gamePadState.ThumbSticks.Right;

                if (Math.Abs(RightStickValue.X) > Register && Math.Abs(RightStickValue.X) > Math.Abs(RightStickValue.Y)) {
                //It is a special attack to the side
                    CurrentActionIndex = SpecialSideAttackIndex;

#if DEBUG
                    Actions[CurrentActionIndex].DrawColor = Color.Beige;
#endif

                    //TODO special side attack code

                } else if (RightStickValue.Y > Register) {
                //This is a special attack up
                    CurrentActionIndex = SpecialUpAttackIndex;

#if DEBUG
                    Actions[CurrentActionIndex].DrawColor = Color.Magenta;
#endif

                    //TODO special up attack code

                } else if (RightStickValue.Y < -Register) {
                //This is a special down attack
                    CurrentActionIndex = SpecialDownAttackIndex;

#if DEBUG
                    Actions[CurrentActionIndex].DrawColor = Color.Maroon;
#endif
                    //TODO special down attack code

                } else {
                //This is a regular special attack
                    CurrentActionIndex = SpecialAttackIndex;

#if DEBUG
                    Actions[CurrentActionIndex].DrawColor = Color.OliveDrab;
#endif

                    //TODO special attack code

                }

            }

            //Updates the character model
            Actions[CurrentActionIndex].UpdateAnimation(
                ConvertUnits.ToDisplayUnits(CharacterBody.Position) - CharacterOrigin, //I don't like this line :(
                CharacterSize);

        }

        /***********************************************************************************************************//**
         * Draws the character
         **************************************************************************************************************/
        public void DrawCharacter(ref SpriteBatch spriteBatch) {
            
            Actions[CurrentActionIndex].DrawAnimation(ref spriteBatch);

        }

        /***********************************************************************************************************//**
         * Gets the position of the charatcer
         * @return The position of the character
         **************************************************************************************************************/
        public Vector2 GetPosition() {

            return CharacterBody.Position;

        }

        /***********************************************************************************************************//**
         * Respawns the character
         **************************************************************************************************************/
        public void Respawn(Vector2 position) {

            CharacterBody.Position = position;
            CharacterBody.ResetDynamics();

        }


    }

}
