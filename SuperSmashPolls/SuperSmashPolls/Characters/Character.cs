/*******************************************************************************************************************//**
 * @file Character.cs
 **********************************************************************************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using SuperSmashPolls.World_Control;

namespace SuperSmashPolls.Characters {

    /***************************************************************************************************************//**
     * This class will hold textures for characters, their moves, their effects, other characters they encounter, etc.
     * @note Character's moves need to be established by an object of the PhysicsAffect class.
     * @note Character position is not determined by this class.
     * @note Actual characters need to be declared as constants in an instance of this class.
     ******************************************************************************************************************/ 
    public class Character {
        /* The character's body */
        private Body CharacterBody;
        /* The character's size */
        private Vector2 CharacterSize;
        /* The character's origin */
        private Vector2 CharacterOrigin;

        /***********************************************************************************************************//**
         * Constructor for the Character class
         * @param screenSize A reference to the size of the screen
         * @param characterSize Size of the character relative to screenSize (i.e. (0.10F, 0.20F) is 10% of the screen 
         * by 20% of the screen.
         * 
         **************************************************************************************************************/
        public Character(ref Vector2 screenSize, Vector2 characterSize) {
            CharacterSize = characterSize;
        }


    }

}
