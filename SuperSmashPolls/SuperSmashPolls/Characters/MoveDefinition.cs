using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Common.PhysicsLogic;
using Microsoft.Xna.Framework;

namespace SuperSmashPolls.Characters {

    /// <summary>
    /// All character moves are declared and defined here
    /// </summary>
    public class MoveDefinition {

        /// <summary>
        /// Default constructor for MoveDefinition
        /// </summary>
        protected MoveDefinition() { }

        /// <summary>
        /// The function to use for idle moves
        /// </summary>
        /// <returns>True, as the idle animation can be canceled at any time</returns>
        public void Idle() { }

        /// <summary>
        /// The basic attack that all characters have
        /// </summary>
        /// <param name="character">The character preforming this move</param>
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

    }

}
