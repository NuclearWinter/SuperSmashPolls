using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace SuperSmashPolls.Graphics {

    /// <summary>
    /// Handles the playing of audio.
    /// </summary>
    class AudioHandler {
        /** The effects that this object can play */
        private readonly List<SoundEffect> Effects;
        /** The (pseudo-)Random Number Generator for deciding which file to play */
        private readonly Random RNG;

        /// <summary>
        /// The default constructor for this class.
        /// </summary>
        public AudioHandler() {
            Effects = new List<SoundEffect>();
            RNG     = new Random();
        }

        /// <summary>
        /// Add effects to this item.
        /// </summary>
        /// <param name="effects">The effects to add</param>
        public void AddAudio(params SoundEffect[] effects) {

            if (effects.Length == 0)
                Console.WriteLine("The AddAudio method was called for an object, but no sounds were added");

            foreach (SoundEffect i in effects)
                Effects.Add(i);

        }

        /// <summary>
        /// Plays a (pseudo-)random effect from effects, or a selected one.
        /// </summary>
        /// <param name="specifiedEffect">If a specific effect is desired, put its index here to play it</param>
        public void PlayEffect(int specifiedEffect = -1) {

            if (specifiedEffect != -1)
                Effects[specifiedEffect].Play();
            else 
                Effects[RNG.Next(Effects.Count)].Play();

        }

    }

}
