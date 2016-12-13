using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace SuperSmashPolls.Characters {

    /// <summary>
    /// All character moves are declared and defined here
    /// </summary>
    /// <remarks>TODO make this into an abstract class and make one of the paramaters to the character class that 
    /// abstract and define each character's moves in a separate file all nice and pretty</remarks>
    public class MoveDefinition {

        private const float StandardSpecialRadius = 1.25F;
        private const float LargeHit = 300;
        private readonly Vector2 StandardJumpHeight = new Vector2(0, -5F);
        private readonly Vector2 StandardWalkSpeed  = new Vector2(3, 0);
        private readonly Vector2 StandardPunchForce = new Vector2(10, 0);

#if COMPLEX_BODIES
        /// <summary>
        /// The function to use for idle moves
        /// </summary>
        /// <returns>True, as the idle animation can be canceled at any time</returns>
        public void Idle(int currentFrame, float direction, List<Body> affectedBodies) { }

        /// <summary>
        /// The walking function for The Donald
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldWalkFunc(int currentFrame, float direction, List<Body> affectedBodies) {

            //affectedBodies[0].Position += StandardWalkSpeed*direction;
            affectedBodies[0].ApplyLinearImpulse(StandardWalkSpeed * direction);

        }

        /// <summary>
        /// Makes The Donald jump
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldJumpFunc(int currentFrame, float direction, List<Body> affectedBodies) {

            affectedBodies[0].ApplyLinearImpulse(StandardJumpHeight,
                affectedBodies[0].Position); // - ConvertUnits.ToSimUnits(new Vector2(0, 00/2F)));

        }

        /// <summary>
        /// TODO Make wall here
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldSpecialFunc(int currentFrame, float direction, List<Body> affectedBodies) {
            


        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldSideSpecialFunc(int currentFrame, float direction, List<Body> affectedBodies) {
            


        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldUpSpecialFunc(int currentFrame, float direction, List<Body> affectedBodies) {
            


        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldDownSpecialFunc(int currentFrame, float direction, List<Body> affectedBodies) {
            


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldBasicAttack(int currentFrame, float direction, List<Body> affectedBodies) {
            
            BasicPunch(currentFrame, direction, affectedBodies);

        }

        /// <summary>
        /// A basic punching function
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        protected internal void BasicPunch(int currentFrame, float direction, List<Body> affectedBodies) {
            
            foreach (var I in affectedBodies) 
                I.ApplyForce(StandardPunchForce);

        }

#else
        /// <summary>
        /// The function for any character in their idle state...nothing happens here
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void Idle(Body characterBody, float direction, bool onCharacter, World world) {

            characterBody.AngularVelocity = 0;

        }

        /***********************************************************************************************************//**
         * Moves for The Donald
         **************************************************************************************************************/

        /// <summary>
        /// The function for having The Donald walk
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void TheDonaldWalk(Body characterBody, float direction, bool onCharacter, World world) {

            Vector2 WalkForce = StandardWalkSpeed*new Vector2(direction >= 0 ? 1 : -1, 0);

            if ((WalkForce.X < 0 ? -1 : 1) != (characterBody.LinearVelocity.X < 0 ? -1 : 1))
                characterBody.LinearVelocity = new Vector2(0, characterBody.LinearVelocity.Y);

            characterBody.AngularVelocity = 0;

            characterBody.ApplyForce(WalkForce);

        }

        /// <summary>
        /// The jump function for The Donald
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void TheDonaldJump(Body characterBody, float direction, bool onCharacter, World world) {

            characterBody.ApplyLinearImpulse(StandardJumpHeight);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void TheDonaldSpecial(Body characterBody, float direction, bool onCharacter, World world) {

            CreateAndActivateExplosion(world, characterBody, 
                OffsetFromCharacter(characterBody, 
                new Vector2(15, 0), direction), LargeHit, StandardSpecialRadius * 1.5F);

        }

        /// <summary>
        /// Creates an explosion in front of The Donald
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void TheDonaldSideSpecial(Body characterBody, float direction, bool onCharacter, World world) {

            CreateAndActivateExplosion(world, characterBody,
                OffsetFromCharacter(characterBody, new Vector2(10, 0), direction));

        }

        /// <summary>
        /// Creates and explosion in front of The Donald and above him
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void TheDonaldUpSpecial(Body characterBody, float direction, bool onCharacter, World world) {

            CreateAndActivateExplosion(world, characterBody, LargeHit/4, StandardSpecialRadius,
                OffsetFromCharacter(characterBody, new Vector2(15, 0), direction),
                OffsetFromCharacter(characterBody, new Vector2(5, -5), direction));

        }

        /// <summary>
        /// A fast fall for The Donald
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void TheDonaldDownSpecial(Body characterBody, float direction, bool onCharacter, World world) {

            characterBody.ApplyLinearImpulse(-StandardJumpHeight);
            CreateAndActivateExplosion(world, characterBody, 
                OffsetFromCharacter(characterBody, new Vector2(0, 13)), LargeHit*3, StandardSpecialRadius*3);

        }

        /// <summary>
        /// A basic punch for The Donald
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void TheDonaldBasic(Body characterBody, float direction, bool onCharacter, World world) {

            CreateAndActivateExplosion(world, characterBody,
                OffsetFromCharacter(characterBody, new Vector2(15, 0), direction), LargeHit/3);

        }

        /***********************************************************************************************************//**
         * Moves for Hillary
         **************************************************************************************************************/

        /// <summary>
        /// The function for having Hillary walk
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void HillaryWalk(Body characterBody, float direction, bool onCharacter, World world) {

            Vector2 WalkForce = StandardWalkSpeed*new Vector2(direction >= 0 ? 1 : -1, 0);

            if ((WalkForce.X < 0 ? -1 : 1) != (characterBody.LinearVelocity.X < 0 ? -1 : 1))
                characterBody.LinearVelocity = new Vector2(0, characterBody.LinearVelocity.Y);

            characterBody.AngularVelocity = 0;

            characterBody.ApplyForce(WalkForce);

        }

        /// <summary>
        /// The jump function for Hillary
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void HillaryJump(Body characterBody, float direction, bool onCharacter, World world) {

            characterBody.ApplyLinearImpulse(StandardJumpHeight);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void HillarySpecial(Body characterBody, float direction, bool onCharacter, World world) {

            

        }

        /// <summary>
        /// Creates an explosion in front of Hillary
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void HillarySideSpecial(Body characterBody, float direction, bool onCharacter, World world) {

            CreateAndActivateExplosion(world, characterBody,
                OffsetFromCharacter(characterBody, new Vector2(10, 0), direction));

        }

        /// <summary>
        /// Creates and explosion in front of Hillary and above her
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void HillaryUpSpecial(Body characterBody, float direction, bool onCharacter, World world) {

            CreateAndActivateExplosion(world, characterBody, LargeHit/4, StandardSpecialRadius,
                OffsetFromCharacter(characterBody, new Vector2(15, 0), direction),
                OffsetFromCharacter(characterBody, new Vector2(5, -5), direction));

        }

        /// <summary>
        /// A fast fall for Hillary
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void HillaryDownSpecial(Body characterBody, float direction, bool onCharacter, World world) {

            characterBody.ApplyLinearImpulse(-StandardJumpHeight);

        }

        /// <summary>
        /// A basic punch for Hillary
        /// </summary>
        /// <param name="characterBody">The body of the character preforming this move</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="onCharacter">Whether or not this should be applied to the character (only matters for moves
        /// where sometimes it should go to the player and sometimes to an enemy)</param>
        /// <param name="world">The world that the move is taking place in</param>
        public void HillaryBasic(Body characterBody, float direction, bool onCharacter, World world) {

            CreateAndActivateExplosion(world, characterBody,
                OffsetFromCharacter(characterBody, new Vector2(15, 0), direction), LargeHit/3);

        }

        /***********************************************************************************************************//**
         * Utility Functions
         **************************************************************************************************************/

        /// <summary>
        /// Creates and activates an explosion with the given parameters
        /// </summary>
        /// <param name="world">The world to activate the exploision in</param>
        /// <param name="characterBody">The body of the character creating the explosion</param>
        /// <param name="position">The position of the explosion in the world</param>
        /// <param name="force">The force of the explosion (by default it is Trump's side special)</param>
        /// <param name="radius">The radius of the explosion in meters</param>
        private void CreateAndActivateExplosion(World world, Body characterBody, Vector2 position, 
            float force = LargeHit / 2, float radius = StandardSpecialRadius) {

            SimpleExplosion Explosion = new SimpleExplosion(world) {
                Power = 1,
                DisabledOnGroup = characterBody.CollisionGroup
            };

            Explosion.Activate(position, radius, force);

        }

        /// <summary>
        /// Creates and activates an explosion with the given parameters in multiple positions in the world
        /// </summary>
        /// <param name="world">The world to activate the exploision in</param>
        /// <param name="characterBody">The body of the character creating the explosion</param>
        /// <param name="position">The position(s) of the explosion in the world</param>
        /// <param name="force">The force of the explosion (by default it is Trump's side special)</param>
        /// <param name="radius">The radius of the explosion in meters</param>
        /// <remarks>If you only need one position in the world, use the other function</remarks>
        private void CreateAndActivateExplosion(World world, Body characterBody, float force, float radius, 
            params Vector2[] position) {

            SimpleExplosion Explosion = new SimpleExplosion(world) {
                Power = 1,
                DisabledOnGroup = characterBody.CollisionGroup
            };

            foreach (var I in position)
                Explosion.Activate(I, radius, force);

        }

        /// <summary>
        /// Offsets a value in pixels from the character position
        /// </summary>
        /// <param name="characterBody">The body of the character to offset from</param>
        /// <param name="pixelAmount">The amount of pixels to offset</param>
        /// <param name="direction">If the offset needs to go to a specific side, set this to the direction float
        /// of the character so that instead of using the raw number in pixelAmount it changed the direction based on
        ///  this. DIRECTION ONLY AFFECTS SIDEWAYS, NOT UP AND DOWN</param>
        /// <returns></returns>
        private Vector2 OffsetFromCharacter(Body characterBody, Vector2 pixelAmount, float direction = -2) {

            if (direction == -2)
                return characterBody.Position + ConvertUnits.ToSimUnits(pixelAmount);

            return characterBody.Position + ConvertUnits.ToSimUnits(pixelAmount) * 
                new Vector2(direction < 0 ? -1 : 1, 0);

        }

#endif

    }

}
