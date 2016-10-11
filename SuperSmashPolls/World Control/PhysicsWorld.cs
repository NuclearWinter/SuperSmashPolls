/*******************************************************************************************************************//**
 * @file PhysicsWorld.cs
 **********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SuperSmashPolls.World_Control {

    /***************************************************************************************************************//**
     * TODO This class
     * This class will hold and manage object interaction in the world. This class will hold and control ALL objects in 
     * the simulated world. This class also needs to scale the world based on the size of the screen.
     * @note World scaling is going to be done through the WorldUnit class.
     * @note Characters's moves will have an affect on the physics world, this affect should be set through an object of
     * the PhysicsAffect class.
     * @note This also needs to be able to batch draw the entire world
     * @note This should also do timescale
     * @note If there is going to be a developer console it should be implimented here
     ******************************************************************************************************************/
    class PhysicsWorld {
        /** The scale of time in the world */
        private int TimeScale { get; set; } = 1;
        /** The natural force occuring in the world */
        private Vector2 NaturalForce { get; set; } = new Vector2(0, -9.8F);
        /** The maximum force that can be applied in the game */
        private Vector2 GodForce { get; set; } = new Vector2();

        /***********************************************************************************************************//**
         * Applies the natural force in the world to an object.
         * @param modify The vector to apply the natural force of the world to.
         **************************************************************************************************************/
        public void ApplyNaturalForce(ref Vector2 modify) {

            modify += NaturalForce;

        }

    }

}
