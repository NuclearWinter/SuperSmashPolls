/*******************************************************************************************************************//**
 * @file PhysicsWorld.cs
 **********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SuperSmashPolls.GameItemControl;

namespace SuperSmashPolls.World_Control {

    /***************************************************************************************************************//**
     * TODO This class
     ******************************************************************************************************************/
    class PhysicsWorld {
        /** The static objects of the world */
        private List<ObjectClass> StaticObjects;
        /** The non-static objects in the world */
        private List<PlayerClass> VariableObjects;

        /***********************************************************************************************************//**
         * Adds a static object to the world
         **************************************************************************************************************/
        public void AddStatic(ObjectClass objectClass) {
            
            StaticObjects.Add(objectClass);

        }

        /***********************************************************************************************************//**
         * Add a variable object to the world
         **************************************************************************************************************/
        public void AddVariable(PlayerClass player) {
            
            VariableObjects.Add(player);

        }

        /***********************************************************************************************************//**
         * Stops VariableObjects from clipping through stuff
         **************************************************************************************************************/
        public void UpdateWorld() {

            foreach (var i in VariableObjects) {

                foreach (var j in StaticObjects) {

                    Rectangle Collision = Rectangle.Intersect(i.GetRectangle(), j.GetRectangle());

                    i.PhysicsPosition.Position -= new Vector2(Collision.Width, Collision.Height);

                }

            }

        }

    }

}
