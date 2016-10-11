using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperSmashPolls {

    /***************************************************************************************************************//**
     * Class used to control the position and movment of objects.
     * @note TODO This needs to be able to store the direction and magnitude that the object is going in.
     ******************************************************************************************************************/
    public class ObjectClass {

        /** Which action the charater is currently doin */
        enum Action {

            Idle,
            Walking

        }
        /** Holds the current state of the character */
        Action currentAction = Action.Idle;

        /* The objects position on the screen */
        public Vector2 Position;
        /* The vector of where the object is going and how fast */
        private Vector2 objectSpeed = new Vector2(0, 0);
        /* The texture to use with this object */
        public Texture2D Texture;
        /* The max acceleration to use for the object */
        private Vector2 maxSpeed;
        /* The current rate of acceleration of the object */
        private Vector2 acceleration;
        /* The rate the object will decellerate */
        private Vector2 decelleration = new Vector2(0.90F, 0.90F);
        /* The size of the object's drawn texture */
        protected Vector2 size = new Vector2(64, 64);

        /* The amount of animations in the sheet */
        Point sheetAnimationDimentions = new Point(1, 0);
        /* The size of the one character pose */
        Point frameSize = new Point(32, 32);
        /* The current character pose that is selected (in dimentions not in pixels) */
        Point currentFrame = new Point(1, 0);

        /***********************************************************************************************************//**
         * Constructs the object class.
         * @param startPosition The position on the screen to start the object at
         * @param acceleration The maximum delta V that the object can have
         * @param maxSpeed The maximum speed that the object can travel
         **************************************************************************************************************/
        public ObjectClass(Vector2 startPosition, Vector2 maxSpeed, Vector2 decelleration) {

            this.Position      = startPosition;
            this.maxSpeed      = maxSpeed;
            this.decelleration = decelleration;

        }

        /***********************************************************************************************************//**
         * Allows the object's texture to be set during content loading.
         * @param newTexture The texture to set the object to.
         * @note Finished and tested.
         **************************************************************************************************************/
        public void SetTexture(Texture2D newTexture) {

            this.Texture = newTexture;

        }

        /***********************************************************************************************************//**
         * Animates the character.
         * The sheet im using is 32-bit
         * @note Not near being done
         **************************************************************************************************************/
        public void AnimateCharacter() {

            switch (currentAction) {

                case Action.Idle:
                    break;
                case Action.Walking: {



                    break;

                }

            }

        }

        /***********************************************************************************************************//**
         * This is used to move the object based on the left joystick.
         * @param speedRatio The percentage (in decimal) of how fast the object should go relative to its max speed
         * @note Finished and tested.
         **************************************************************************************************************/
        public void Move(Vector2 speedRatio) {

            if (speedRatio != new Vector2(0, 0)) {

                acceleration = maxSpeed * speedRatio;

                objectSpeed += acceleration;

            } else if (objectSpeed != new Vector2(0, 0))
                objectSpeed *= decelleration;

            Position += objectSpeed;

        }

        /***********************************************************************************************************//**
         * Offers an intersect function equivalent for objects with 2D Vectors.
         * @param position The position of the other object.
         * @param texture The texture of the other object.
         * @return Whether or not the object and the other object intersect.
         **************************************************************************************************************/
        public bool Overlap(Vector2 position, Texture2D texture) {

            return new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Texture.Width, this.Texture.Height)
                .Intersects(new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height));

        }

        /***********************************************************************************************************//**
         * Restrict the object from entering the designated area.
         *  @param minimum The coordinates of the first corner of the rectangle.
         *  @param maximum The coordinates of the second corner of the rectangle.
         *  @note WIP
         **************************************************************************************************************/
        public void Boundobject(Vector2 position, Texture2D texture) {

            //finish me

        }

        /***********************************************************************************************************//**
         * Limits the object to sataying inside the designated area.
         * It is assumed that the other (top) corner is (0, 0).
         * @param bottomCorner The right-most bottom corner
         * @return Whether or not the object is hitting on the X or Y sides
         * @note Tested and working
         **************************************************************************************************************/
        public bool KeepInPLay(Vector2 bottomCorner) {

            bool hittingX = true, hittingY = true;

            if (Position.X < 0)
                Position.X = 0;
            else if (Position.X + size.X > bottomCorner.X)
                Position.X = bottomCorner.X - size.X; //This accounts for texture size
            else
                hittingX = false; //If neither of the top two were true, it's not hitting the X side


            if (Position.Y < 0)
                Position.Y = 0;
            else if (Position.Y + size.Y > bottomCorner.Y) { 
                Position.Y = bottomCorner.Y - size.Y; //This accounts for texture size

                hittingY = false; // gravity will always pull the player down

            } else
                hittingY = false; //If neither of the top two were true, it's not hitting the Y side

            return (hittingX || hittingY);

        }

        /***********************************************************************************************************//**
         * Moves the player down with the force of gravity
         **************************************************************************************************************/
        public void AddGravity(float gravity = 9.80F) {

            Position.Y += gravity;

        }

    } //END ObjectClass

} // END namespace WindowsGameTest