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

        private readonly Vector2 StandardJumpHeight = new Vector2(0, 10);
        private readonly Vector2 StandardWalkSpeed  = new Vector2(10, 0);
        private readonly Vector2 StandardPunchForce = new Vector2(10, 0);

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

            affectedBodies[0].ApplyForce(StandardWalkSpeed,
                affectedBodies[0].Position - ConvertUnits.ToSimUnits(new Vector2(19/2F*(direction < 0 ? -1 : 1), 0)));

        }

        /// <summary>
        /// Makes The Donald jump
        /// </summary>
        /// <param name="currentFrame">The current frame of the animation</param>
        /// <param name="direction">The direction that the character is facing</param>
        /// <param name="affectedBodies">The bodies in the world to be affected</param>
        public void TheDonaldJumpFunc(int currentFrame, float direction, List<Body> affectedBodies) {

            affectedBodies[0].ApplyLinearImpulse(StandardJumpHeight,
                affectedBodies[0].Position - ConvertUnits.ToSimUnits(new Vector2(0, 00/2F)));

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

    }

}
