using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace SuperSmashPolls.Levels {

    /***************************************************************************************************************//**
     * Holds and handles the drawing of and contruction of bodies
     ******************************************************************************************************************/ 
    class LevelHandler {
        /** The world for this level */

        /** The bodies of this level <Body, texture, size (in meters)> */
        private List<Tuple<Body, Texture2D, Vector2>> LevelBody;
        /** The background for this level */
        private Texture2D LevelBackground;
        /** The amount that the background needs to be scaled (adjusted for different screen sizes) */
        private Vector2 LevelBackgroundScale;
        /** The place's for players to spawn */
        public Vector2 PlayerOneSpawn, PlayerTwoSpawn, PlayerThreeSpawn, PlayerFourSpawn, RespawnPoint;

        /***********************************************************************************************************//**
         * Constructor
         **************************************************************************************************************/ 
        public LevelHandler(Vector2 playerOneSpawn, Vector2 playerTwoSpawn, Vector2 playerThreeSpawn,
            Vector2 playerFourSpawn, Vector2 respawnPoint, bool gravity = true) {

            PlayerOneSpawn = playerOneSpawn;
            PlayerTwoSpawn = playerTwoSpawn;
            PlayerThreeSpawn = playerThreeSpawn;
            PlayerFourSpawn = playerFourSpawn;
            RespawnPoint = respawnPoint;

            LevelBody = new List<Tuple<Body, Texture2D, Vector2>>();
        }

        /***********************************************************************************************************//**
         * Adds a background to the level
         **************************************************************************************************************/
        public void SetBackground(Texture2D levelBackground, Vector2 levelBackgroundScale) {

            LevelBackground      = levelBackground;
            LevelBackgroundScale = levelBackgroundScale;

        }

        /***********************************************************************************************************//**
         * Creates a polygon from a texture
         * TODO document this
         **************************************************************************************************************/
        public Body CreatePolygonFromTexture(Texture2D tex, World world, float density, Vector2 position, float scale, 
            TriangulationAlgorithm algorithm = TriangulationAlgorithm.Bayazit) {

            uint[] texData = new uint[tex.Width * tex.Height];
            tex.GetData<uint>(texData);

            Vertices vertices = TextureConverter.DetectVertices(texData, tex.Width);
            List<Vertices> vertexList = Triangulate.ConvexPartition(vertices, algorithm);

            Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits(scale));
            foreach (Vertices vert in vertexList)
                vert.Scale(ref vertScale);

            Vector2 centroid = -vertices.GetCentroid();
            vertices.Translate(ref centroid);
            //basketOrigin = -centroid;

            return BodyFactory.CreateCompoundPolygon(world, vertexList, density, position);

        }

        /***********************************************************************************************************//**
         * Creates the body and puts it in the world
         * @param gameworld The world to put the body into
         * @param items All the items to add to the world <Texture, position, size (in meters)>
         **************************************************************************************************************/
        public void AssignToWorld(ref World gameWorld, params Tuple<Texture2D, Vector2, Vector2>[] items) {

            foreach (var i in items) {

                Body TempBody = CreatePolygonFromTexture(i.Item1, gameWorld, 1F, i.Item2,
                    ConvertUnits.ToSimUnits(i.Item1.Width) / i.Item3.X, TriangulationAlgorithm.Bayazit);

                TempBody.BodyType = BodyType.Static;

                TempBody.IsStatic = true;

                LevelBody.Add(new Tuple<Body, Texture2D, Vector2>(TempBody, i.Item1, i.Item3));

            }

        }

        /***********************************************************************************************************//**
         * This draws the texture associated with the world
         * @param spriteBatch The batch to draw with
         **************************************************************************************************************/
        public void DrawLevel(SpriteBatch spriteBatch) {

            if (LevelBackground != null)
                spriteBatch.Draw(LevelBackground, Vector2.Zero, null, Color.White, 0, Vector2.Zero, LevelBackgroundScale,
                    SpriteEffects.None, 0);
            else
                spriteBatch.GraphicsDevice.Clear(Color.White);

            foreach (var i in LevelBody) {

                spriteBatch.Draw(i.Item2, ConvertUnits.ToDisplayUnits(i.Item1.Position), null, Color.White, 0,
                    Vector2.Zero, ConvertUnits.ToSimUnits(i.Item2.Width)/i.Item3.X, SpriteEffects.None, 0);

            }

        }

    }

}
