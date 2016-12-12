using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace SuperSmashPolls.Graphics {

    /// <summary>
    /// Handles the playing of audio.
    /// </summary>
    public class AudioHandler {

        /** The effects that this object can play */
        private readonly List<SoundEffect> Effects;
        /** The (pseudo-)Random Number Generator for deciding which file to play */
        private readonly Random RNG;
        /** Instances for the sound effects */
        private List<SoundEffectInstance> EffectInstances;

        /// <summary>
        /// Makes instances for the sound effects
        /// </summary>
        private void MakeEffectInstances() {
            
            EffectInstances = new List<SoundEffectInstance>();

            foreach (var i in Effects) 
                EffectInstances.Add(i.CreateInstance());

        }

        /// <summary>
        /// The default constructor for this class.
        /// </summary>
        public AudioHandler() {
            Effects = new List<SoundEffect>();
            RNG     = new Random();
            MakeEffectInstances();
        }

        /// <summary>
        /// The default constructor for this class.
        /// </summary>
        public AudioHandler(params SoundEffect[] effect) {
            Effects = new List<SoundEffect>(effect);
            RNG     = new Random();
            MakeEffectInstances();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SoundEffect[] GetEffects() {

            return Effects.ToArray();

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

            MakeEffectInstances();

        }

        /// <summary>
        /// Gets a random sound effect from Effects
        /// </summary>
        /// <returns></returns>
        public SoundEffect GetRandomAudio() {

            return Effects[RNG.Next(Effects.Count)];

        }

        /// <summary>
        /// Plays a (pseudo-)random effect from effects, or a selected one.
        /// </summary>
        /// <param name="specifiedEffect">If a specific effect is desired, put its index here to play it</param>
        public void PlayEffect(int specifiedEffect = -1) {

            if (specifiedEffect != -1) {

                try {

                    EffectInstances[specifiedEffect].Play();

                } catch (NullReferenceException) {
                    
                    Console.WriteLine("The item " + specifiedEffect + " cannot be accessed in Effects");

                }

            } else {

                try {

                    EffectInstances[RNG.Next(Effects.Count)].Play();

                } catch (NullReferenceException) {

                    Console.WriteLine("The desired item cannot be accessed in Effects");

                }

            }

        }

    }

}
