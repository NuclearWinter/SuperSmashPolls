﻿/*******************************************************************************************************************//**
 * /doc:SuperSmashPolls.XML
 **********************************************************************************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSmashPolls.World_Control {

    ///<summary>
    ///This class will account for scaling of the physics system based on the size of the player's screen size.
    ///</summary>
    ///<remarks>WorldUnit should replace position and force/weight references throughout the rest of the game.</remarks>
    ///<remarks>0.01F unit of position is equal to 1% of the screen.</remarks>
    ///<remarks>1.0F of GodForce = ScreenSize * 10 | 0.1 of GodForce = ScreenSize</remarks>
    ///<remarks>To use this class to determine the size of something use the position functions</remarks>
    public class WorldUnit {

        /// <summary>The size of the currently used screen</summary>
        public Vector2 ScreenSize;
        /// <summary>The position of this object on the screen</summary>
        public Vector2 Position;

        ///<summary>
        ///Construct a WorldUnit class for use as a position.
        ///</summary>
        ///<param name="screenSize">Reference to the size of the players screen (in pixels)</param>
        ///<param name="position">Position of the unit on screen as a float</param>
        ///<remarks> Doing it like this allows for the screensize to be changed without messing up physics.</remarks>
        public WorldUnit(ref Vector2 screenSize, Vector2 position) {
            ScreenSize = screenSize;
            Position   = position;
        }

        ///<summary>
        ///Construct a WorldUnit class based off the same scales as another WorldUnit for use as a position.
        ///</summary>
        ///<param name="position">The position of this object on the screen (as a ratio)</param>
        ///<param name="baseScaleUnit">The scale to base this new WorldUnit off of.</param>
        public WorldUnit(Vector2 position, WorldUnit baseScaleUnit) {
            Position   = position;
            ScreenSize = baseScaleUnit.ScreenSize;
        }

        ///<summary>
        ///Gets this item's position on the screen
        ///</summary>
        public Vector2 GetThisPosition() {

            return Position * ScreenSize;

        }

        ///<summary>
        ///Adds two WorldUnits for a new WorldUnit
        ///</summary>
        public WorldUnit Add(WorldUnit j) {
            
            return new WorldUnit(ref ScreenSize, Position + j.Position);

        }

        ///<summary>
        ///Gets the size of this object
        ///</summary>
        ///<remarks> This is identical to GetThisPosition(), but it is easier to understand like this.</remarks>
        public Vector2 GetSize() {

            return ScreenSize*Position;

        }

        ///<summary>
        ///Gets the size of this object's X axis (the amount it is from (0,0))
        ///</summary>
        public int GetXSize() {

            return (int) (Position.X*ScreenSize.X);

        }


        ///<summary>
        ///Gets the size of this object's Y axis (the amount it is from (0,0))
        ///</summary>
        public int GetYSize() {

            return (int) (Position.Y*ScreenSize.Y);

        }

    }

}
