using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using SuperSmashPolls.Levels;

namespace SuperSmashPolls.Characters {

    /// <summary>
    /// Parent class for creating character moves.
    /// </summary>
    public abstract class Moves {

        /// <summary>
        /// The special move for this character
        /// </summary>
        public abstract void Special(Character character);

        /// <summary>
        /// The side special move for this character
        /// </summary>
        public abstract void SideSpecial(Character character);

        /// <summary>
        /// The up special move for this character
        /// </summary>
        public abstract void UpSpecial(Character character);

        /// <summary>
        /// The down special move for this character
        /// </summary>
        public abstract void DownSpecial(Character character);

        /// <summary>
        /// Adds moves to a character
        /// </summary>
        /// <param name="addTo">The character to add moves to</param>
        public void AddMovesToCharacter(Character addTo) {
            
            addTo.AddCharacterMoves(SideSpecial, UpSpecial, DownSpecial, Special);

        }

    }

}
