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
     * @note To use this class to determine the size of something use the position functions
     ******************************************************************************************************************/
    public class WorldUnit {
        /* The size of the screen (in pixels) */
        public Vector2 ScreenSize;
        /* The maximum force that can possibly be applied in the game world */
        public Vector2 GodForce;
        /* The position used by this WorldUnit class @note This can be used as a size too */
        public Vector2 Position;
        /* The force to be applied by this object (as a percent of GodForce)
         * @note For characters and objects this is the fallspeed */
        public Vector2 Force;
        /* The duration of this force on other objects */
        public int Duration;

        /***********************************************************************************************************//**
         * Construct a WorldUnit class for use as a position.
         * @param screenSize Reference to the size of the players screen (in pixels)
         * @param position Position of the unit on screen as a float
         * @note Doing it like this allows for the screensize to be changed without messing up physics.
         **************************************************************************************************************/
        public WorldUnit(ref Vector2 screenSize, Vector2 position) {
            ScreenSize = screenSize;
            Position   = position;
        }

        /***********************************************************************************************************//**
         * Construct a WorldUnit class for use as a force.
         * @param screenSize The size of the player's screen
         * @param godForce The force needed for an object with no other forces acting on it to be knocked off the screen
         * ten times over.
         * @param force The forces to be acting on the objects
         * @param duration The duration of this force on other objects (1 = only applied once)
         **************************************************************************************************************/
        public WorldUnit(ref Vector2 screenSize, ref Vector2 godForce, Vector2 force, int duration = 1) {
            ScreenSize = screenSize;
            GodForce   = godForce;
            Force      = force;
            Duration   = duration;
        }

        /***********************************************************************************************************//**
         * Construct a WorldUnit class based off the same scales as another WorldUnit for use as a force.
         * @param baseScaleUnit The scale to base this new WorldUnit off of.
         * @param force The force for this WorldUnit.
         * @param duration The duration of this force on other objects (1 = only applied once)
         * @return A new WorldUnit Object.
         **************************************************************************************************************/
        public WorldUnit(WorldUnit baseScaleUnit, Vector2 force, int duration = 1) {
            ScreenSize = baseScaleUnit.ScreenSize;
            GodForce   = baseScaleUnit.GodForce;
            Force      = force;
            Duration   = duration;
        }

        /***********************************************************************************************************//**
         * Construct a WorldUnit class based off the same scales as another WorldUnit for use as a position.
         * @param position The position of this object on the screen (as a ratio)
         * @param baseScaleUnit The scale to base this new WorldUnit off of.
         **************************************************************************************************************/
        public WorldUnit(Vector2 position, WorldUnit baseScaleUnit) {
            Position   = position;
            ScreenSize = baseScaleUnit.ScreenSize;
        }

        /***********************************************************************************************************//**
         * Gets this item's position on the screen
         **************************************************************************************************************/
        public Vector2 GetThisPosition() {

            return Position * ScreenSize;

        }

        /***********************************************************************************************************//**
         * Adds two WorldUnits for a new WorldUnit
         **************************************************************************************************************/
        public WorldUnit Add(WorldUnit j) {
            
            return new WorldUnit(ref ScreenSize, Position + j.Position);

        }

        /***********************************************************************************************************//**
         * Scales a WorldUnit
         * @param scalar The amount to divide Force by
         **************************************************************************************************************/
        public WorldUnit Scale(float scalar) {
            
            return new WorldUnit(this, Force/scalar, Duration);

        }

        /***********************************************************************************************************//**
         * Gets the size of this object's X axis (the amount it is from (0,0))
         **************************************************************************************************************/
        public int GetXSize() {

            return (int) (Position.X*ScreenSize.X);

        }


        /***********************************************************************************************************//**
         * Gets the size of this object's Y axis (the amount it is from (0,0))
         **************************************************************************************************************/
        public int GetYSize() {

            return (int) (Position.Y*ScreenSize.Y);

        }
    }

}
