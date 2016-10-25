﻿/*******************************************************************************************************************//**
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
        /* The position used by this WorldUnit class @note This can be used as a size too */
        public Vector2 Position;

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
         * Gets the size of this object
         * @note This is identical to GetThisPosition(), but it is easier to understand like this.
         **************************************************************************************************************/
        public Vector2 GetSize() {

            return ScreenSize*Position;

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