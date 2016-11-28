using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using SuperSmashPolls.Graphics;
using SuperSmashPolls.Levels;

namespace SuperSmashPolls.Characters {

    /// <summary>
    /// Parent class for creating character moves.
    /// </summary>
    /// <remarks>To have audio play during a move you must run the AudioHandler.PlayEffect command</remarks>
    public abstract class Moves {

        /// <summary>The audio handler to play during the side special attack</summary>
        protected internal AudioHandler SideSpecialSound;
        /// <summary>The audio handler to play during the up special attack</summary>
        protected internal AudioHandler UpSpecialSound;
        /// <summary>The audio handler to play during the down special attack</summary>
        protected internal AudioHandler DownSpecialSound;
        /// <summary>The audio handler to play during the special attack</summary>
        protected internal AudioHandler SpecialSound;
        /// <summary>The audio handler to play during a basic attack</summary>
        protected internal AudioHandler BasicAttackSound;

        /// <summary>
        /// The basic attack that all characters have
        /// </summary>
        /// <param name="character"></param>
        /// TODO fix this
        protected internal void BasicPunch(Character character) {
            //If true, moving forwards (right), if negative backwards (left)
            bool Direction = character.CharacterBody.LinearVelocity.X > 0;

            Vector2 AttackPosition = character.CharacterBody.Position; //16 = punch texture width
            AttackPosition.X += (Direction) ? ConvertUnits.ToSimUnits(16) : -ConvertUnits.ToSimUnits(16);

            SimpleExplosion Explosion = new SimpleExplosion(character.GameWorld) {
                Power = 1,
                DisabledOnGroup = character.CharacterBody.CollisionGroup
            };

            Explosion.Activate(AttackPosition, ConvertUnits.ToSimUnits(30), 700);

        }

        /// <summary>
        /// The special move for this character
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public abstract void Special(Character character);

        /// <summary>
        /// The side special move for this character
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public abstract void SideSpecial(Character character);

        /// <summary>
        /// The up special move for this character
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public abstract void UpSpecial(Character character);

        /// <summary>
        /// The down special move for this character
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public abstract void DownSpecial(Character character);

        /// <summary>
        /// The basic attack for a character.
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public abstract void BasicAttack(Character character);

        /// <summary>
        /// Adds moves to a character
        /// </summary>
        /// <param name="addTo">The character to add moves to</param>
        public void AddMovesToCharacter(Character addTo) {
            
            addTo.AddCharacterMoves(SideSpecial, UpSpecial, DownSpecial, Special, BasicAttack);

        }

        /// <summary>
        /// Adds the sounds to this class
        /// </summary>
        /// <param name="sideSpecialSound">Audio manajer to handle playing during the side special</param>
        /// <param name="upSpecialSound">Audio manajer to handle playing during the up special</param>
        /// <param name="downSpecialSound">Audio manajer to handle playing during the down special</param>
        /// <param name="specialSound">Audio manajer to handle playing during the special</param>
        /// <param name="basicAttackSound">Audio manajer to handle playing during a basic attack</param>
        public void AddAudio(AudioHandler sideSpecialSound, AudioHandler upSpecialSound, AudioHandler downSpecialSound,
            AudioHandler specialSound, AudioHandler basicAttackSound) {
            SideSpecialSound = sideSpecialSound;
            UpSpecialSound   = upSpecialSound;
            DownSpecialSound = downSpecialSound;
            SpecialSound     = specialSound;
            BasicAttackSound = basicAttackSound;
        }

    }

}
