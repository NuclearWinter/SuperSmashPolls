﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace SuperSmashPolls.Characters {

    class TheDonaldsMoves : Moves {
        /// <summary>
        /// Builds a wall. This handles the creation of the body and the forces of the wall.
        /// </summary>
        /// <param name="character">The character casting this</param>
        public override void Special(Character character) {
            SideSpecial(character);
        }
        /// <summary>
        /// Creates an explosion outward from TheDonald. This is a linear explosion from the center of TheDonald's body,
        /// with a radius of 4m and a force of 300, maxforce of the max value of a float. This explosion does not affect
        /// TheDonald.
        /// </summary>
        /// <param name="character">The character casting this</param>
        public override void SideSpecial(Character character) {

            SimpleExplosion Explosion = new SimpleExplosion(character.GameWorld) {
                Power = 1,
                DisabledOnGroup = character.CharacterBody.CollisionGroup
            };

            Explosion.Activate(character.GetPosition(), 4, 300);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="character">The character casting this.</param>
        public override void UpSpecial(Character character) {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="character">The character casting this.</param>
        public override void DownSpecial(Character character) {
            throw new NotImplementedException();
        }
    }

}