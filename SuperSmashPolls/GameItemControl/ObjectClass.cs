/*******************************************************************************************************************//**
 * @file ObjectClass.cs
 **********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperSmashPolls.World_Control;

namespace SuperSmashPolls {

    /***************************************************************************************************************//**
     * Class used to control the position and movment of objects.
     * @note TODO This needs to be able to store the direction and magnitude that the object is going in.
     ******************************************************************************************************************/
    public class ObjectClass {
        /** The objects position */
        private WorldUnit DrawPosition;
        /** The position of this object after forces are applied (without this objects would jump around)
         * @note The character's fallspeed should be in here */
        public WorldUnit PhysicsPosition;
        /** The object's size. @note This must be based off of coming down from (0,0) */
        private WorldUnit Size;
        /** The forces to apply to this object */
        private List<WorldUnit> ApplyForces = new List<WorldUnit>();
        /** The forces that this object is applying */
        private List<WorldUnit> ApplyingForces = new List<WorldUnit>();
        /** The texture for this object */
        private SpritesheetHandler Texture;
        /** How much this object weighs (If it is a static object just don't update it) */
        public int Weight;
        /** The last time this object was updated */
        private DateTime LastTimeUpdated = DateTime.Now;
        /** Whether or not this object's position should be updated */
        private readonly bool Solid;

        /***********************************************************************************************************//**
         * Constructor
         **************************************************************************************************************/
        public ObjectClass(WorldUnit drawPosition, int weight, WorldUnit size, bool solid = false) {
            DrawPosition    = drawPosition;
            PhysicsPosition = drawPosition;
            Weight          = weight;
            Size            = size;
            Solid = solid;
        }

        /***********************************************************************************************************//**
         * Give this object a SpriteSheetHandler
         **************************************************************************************************************/
        public void AssignSpriteSheet(SpritesheetHandler spritesheet) {

            Texture = spritesheet;

        }

        /***********************************************************************************************************//**
         * Update this object
         **************************************************************************************************************/
        public void Update() {

            if (LastTimeUpdated.Subtract(DateTime.Now).TotalSeconds < 1 || Solid) return;

            LastTimeUpdated = DateTime.Now;

            foreach (var i in ApplyForces) {

                PhysicsPosition = PhysicsPosition.Add(i.Scale(Weight));

                if (i.Duration == 0)
                    ApplyForces.Remove(i);
                else
                    i.Duration -= 1;

            }

            DrawPosition.Position = Vector2.Lerp(DrawPosition.Position, PhysicsPosition.Position, 0.25F);

        }

        /***********************************************************************************************************//**
         * Draw this object
         **************************************************************************************************************/
        public void Draw(ref SpriteBatch batch) {
            
            Texture.DrawWithUpdate(ref batch, DrawPosition.GetThisPosition(), ref Size);

        }

        /***********************************************************************************************************//**
         * Apply force to object
         * @param force The force to apply to this object
         **************************************************************************************************************/
        public void AddForce(WorldUnit force) {
            
            ApplyForces.Add(force);

        }

        /***********************************************************************************************************//**
         * Get's the rectangle of this object
         **************************************************************************************************************/
        public Rectangle GetRectangle() {
            
            return new Rectangle((int) PhysicsPosition.Position.X, (int) PhysicsPosition.Position.Y, Size.GetXSize(), 
                Size.GetYSize());

        }

    } //END ObjectClass

} // END namespace WindowsGameTest