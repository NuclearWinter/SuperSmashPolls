using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSmashPolls.Characters {

    /// <summary>
    /// Parent class for creating character moves.
    /// </summary>
    public abstract class Moves {

        /// <summary>
        /// The special move for this character
        /// </summary>
        public abstract void Special();

        /// <summary>
        /// The side special move for this character
        /// </summary>
        public abstract void SideSpecial();

        /// <summary>
        /// The up special move for this character
        /// </summary>
        public abstract void UpSpecial();

        /// <summary>
        /// The down special move for this character
        /// </summary>
        public abstract void DownSpecial();

        /// <summary>
        /// Adds moves to a character
        /// </summary>
        /// <param name="addTo">The character to add moves to</param>
        public void AddMovesToCharacter(Character addTo) {
            
            addTo.AddCharacterMoves(SideSpecial, UpSpecial, DownSpecial, Special);

        }

    }

}
