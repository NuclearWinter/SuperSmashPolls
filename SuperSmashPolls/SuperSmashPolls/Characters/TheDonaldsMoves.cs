using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace SuperSmashPolls.Characters {

    class TheDonaldsMoves : Moves {
        public override void Special(Character character) {
            SideSpecial(character);
        }
        public override void SideSpecial(Character character) {

            character.CharacterBody.CollisionGroup = Int16.MaxValue - 1;

            SimpleExplosion Explosion = new SimpleExplosion(character.GameWorld) {
                Power = 1,
                DisabledOnGroup = Int16.MaxValue - 1
            };
            Explosion.Activate(character.GetPosition(), 4, 300);

            character.CharacterBody.CollisionCategories = Category.All;

        }
        public override void UpSpecial(Character character) {
            throw new NotImplementedException();
        }
        public override void DownSpecial(Character character) {
            throw new NotImplementedException();
        }
    }

}
