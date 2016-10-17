/*******************************************************************************************************************//**
 * @file WorldUnit.cs
 **********************************************************************************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSmashPolls.World_Control {

    /***************************************************************************************************************//**
     * This class will account for scaling of the physics system based on the size of the player's screen size.
     * @note WorldUnit should replace position and force/weight references throughout the rest of the game.
     * @note 0.01F unit of position is equal to 1% of the screen.
     * @note 1.0F of GodForce = ScreenSize * 10 | 0.1 of GodForce = ScreenSize
     ******************************************************************************************************************/
    class WorldUnit {
        /* The size of the screen (in pixels) */
        public Vector2 ScreenSize;
        /* The maximum force that can possibly be applied in the game world */
        public Vector2 GodForce;
        /* The position used by this WorldUnit class */
        public Vector2 Position;
        /* The force to be applied by this object (as a percent of GodForce) */
        public Vector2 Force;

        /***********************************************************************************************************//**
         * Construct a WorldUnit class for use with position.
         * @param screenSize Reference to the size of the players screen (in pixels)
         * @param position Position of the unit on screen as a float
         * @note Doing it like this allows for the screensize to be changed without messing up physics.
         **************************************************************************************************************/
        public WorldUnit(ref Vector2 screenSize, Vector2 position) {
            ScreenSize = screenSize;
            Position   = position;
        }

        /***********************************************************************************************************//**
        * Construct a WorldUnit class for use with position based off of a known screen size.
        * @param screenSize The size of the player's screen.
        * @param baseScreenSize The size of the screen to use for the ratio.
        * @param baseScreenPosition The position of the object on screen if it were to be the size of baseScreenSize.
        * @note This is mostly for ease of use for programmers.
        ***************************************************************************************************************/
        public WorldUnit(ref Vector2 screenSize, Vector2 baseScreenSize, Vector2 baseScreenPosition) {
            ScreenSize = screenSize;
            Position   = baseScreenPosition/baseScreenSize;
        }

        /***********************************************************************************************************//**
         * Construct a WorldUnit class for use as a force.
         * @param screenSize The size of the player's screen
         * @param godForce The force needed for an object with no other forces acting on it to be knocked off the screen
         * ten times over.
         * @param force The forces to be acting on the objects
         **************************************************************************************************************/
        public WorldUnit(ref Vector2 screenSize, ref Vector2 godForce, Vector2 force) {
            ScreenSize = screenSize;
            GodForce   = godForce;
            Force      = force;
        }

        /***********************************************************************************************************//**
         * Construct a WorldUnit class based off the same scales as another WorldUnit for use as a force.
         * @param baseScaleUnit The scale to base this new WorldUnit off of.
         * @param force The force for this WorldUnit.
         * @return A new WorldUnit Object.
         **************************************************************************************************************/
        public WorldUnit(WorldUnit baseScaleUnit, Vector2 force) {
            ScreenSize = baseScaleUnit.ScreenSize;
            GodForce   = baseScaleUnit.GodForce;
            Force      = force;
        }

        /***********************************************************************************************************//**
         * Construct a WorldUnit class based off the same scales as another WorldUnit for use as a force.
         * @param position The position of this object on the screen (as a ratio)
         * @param baseScaleUnit The scale to base this new WorldUnit off of.
         **************************************************************************************************************/
        public WorldUnit(Vector2 position, WorldUnit baseScaleUnit) {
            Position = position;
            ScreenSize = baseScaleUnit.ScreenSize;
        }

        /***********************************************************************************************************//**
         * Helps to find placement of objects by giving positions on the screen.
         * @param xRatio A number 0.0 - 1.0 representing percent of the screen to move the object over on the x-axis.
         * @param yRatio A number 0.0 - 1.0 representing percent of the screen to move the object over on the y-axis.
         * @return The amount of pixels to displace an object to achieve the desired ratio.
         **************************************************************************************************************/
        public Vector2 GetScreenPosition (float xRatio, float yRatio) {
            
            return new Vector2(xRatio * ScreenSize.X, yRatio * ScreenSize.Y);

        }

        /***********************************************************************************************************//**
         * Gets this item's position on the screen
         **************************************************************************************************************/
        public Vector2 GetThisPosition() {

            return Position * ScreenSize;

        }

        /***********************************************************************************************************//**
         * 
         **************************************************************************************************************/
        public WorldUnit Add(ref WorldUnit i, WorldUnit j) {
            
            return new WorldUnit(ref i.ScreenSize, i.Position + j.Position);

        }

        /***********************************************************************************************************//**
         * Applies a force to this object.
         **************************************************************************************************************/
        public void ApplyForce(ref WorldUnit modifyForce) {
            
            

        }

    }

}
