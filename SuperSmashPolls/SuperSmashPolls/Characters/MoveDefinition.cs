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
    public class MoveDefinition {

        private World GameWorld;

//        /// <summary>
//        /// Sets the world of the game. This needs to be called whenever a new game is started
//        /// </summary>
//        /// <param name="gameWorld">The world that players are going to be in</param>
//        public void SetWorld(World gameWorld) {
//
//            GameWorld = gameWorld;
//
//        }

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

            affectedBodies[0].ApplyForce(new Vector2(10, 0),
                affectedBodies[0].Position - ConvertUnits.ToSimUnits(new Vector2(19/2F*(direction < 0 ? -1 : 1), 0)));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldJumpFunc(int currentFrame, float direction, List<Body> affectedBodies) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldSpecialFunc(int currentFrame, float direction, List<Body> affectedBodies) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldSideSpecialFunc(int currentFrame, float direction, List<Body> affectedBodies) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldUpSpecialFunc(int currentFrame, float direction, List<Body> affectedBodies) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldDownSpecialFunc(int currentFrame, float direction, List<Body> affectedBodies) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldBasicAttack(int currentFrame, float direction, List<Body> affectedBodies) { }

        /// <summary>
        /// The basic attack that all characters have
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        /// TODO fix this
        protected internal void BasicPunch(int currentFrame, float direction, List<Body> affectedBodies) {
            


        }

    }

}
