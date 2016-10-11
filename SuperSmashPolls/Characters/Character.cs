/*******************************************************************************************************************//**
 * @file Character.cs
 **********************************************************************************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSmashPolls.Characters {

    /***************************************************************************************************************//**
     * TODO Test this class
     * This class will hold textures for characters, their moves, their effects, other characters they encounter, etc.
     * @note Character's moves need to be established by an object of the PhysicsAffect class.
     * @note Character position is not determined by this class.
     * @note Actual characters need to be declared as constants in an instance of this class.
     ******************************************************************************************************************/ 
    class Character {
        /** The thumbnail of the character to show on the character selection screen */
        private Texture2D Thumbnail;
        /** The character's face spritesheet @warning Must have items added to it with @see AddAnimation */
        private List<SpritesheetHandler> CharacterSprite = new List<SpritesheetHandler>();

        /***********************************************************************************************************//**
         * Constructor
         **************************************************************************************************************/
         public Character() { }

        /***********************************************************************************************************//**
         * Adds a new action to the CharacterSprite list.
         * @param emplaceAction This should be a list of items added in the content loading phase.
         **************************************************************************************************************/
        public void AddAnimation(SpritesheetHandler characterSprite) {
            
            CharacterSprite.Add(characterSprite);

        }

        /***********************************************************************************************************//**
         * Gives the desired action of the character.
         * @param action The action of the character to preform. If the key does not match anything in the List, than 
         * the 0th item will be returned.
         * @return A SpritesheetHandler that matches the desired action (or the default).
         **************************************************************************************************************/
        public SpritesheetHandler DrawCharacter(string action) {

            foreach (SpritesheetHandler sheet in CharacterSprite) {

                if (sheet.Key == action)
                    return sheet;

            }

            return CharacterSprite[2]; //Implied else if the function gets here

        }

    }

}
