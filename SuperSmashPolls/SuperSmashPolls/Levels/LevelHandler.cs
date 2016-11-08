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
     * Gets the information needed to later create the world if it is selected.
     ******************************************************************************************************************/ 
    class LevelHandler {
        /* The bodies of this level */
        private List<Tuple<Body, Texture2D>> LevelBody;

        /***********************************************************************************************************//**
         * Constructor
         **************************************************************************************************************/ 
        public LevelHandler() {

            LevelBody = new List<Tuple<Body, Texture2D>>();

        }

        /***********************************************************************************************************//**
         * Creates a polygon from a texture
         * TODO document this
         **************************************************************************************************************/
        public Body CreatePolygonFromTexture(Texture2D tex, World world, float density, Vector2 position, float scale, TriangulationAlgorithm algorithm = TriangulationAlgorithm.Bayazit) {
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
         * @param items All the items to add to the world
         **************************************************************************************************************/
        public void AssignToWorld(ref World gameWorld, params Tuple<Texture2D, Vector2>[] items) {

            foreach (var i in items) {

                Body TempBody = CreatePolygonFromTexture(i.Item1, gameWorld, 1F, i.Item2,
                    ConvertUnits.ToSimUnits(i.Item1.Width)/i.Item1.Width, TriangulationAlgorithm.Bayazit);

                TempBody.BodyType = BodyType.Static;

                TempBody.IsStatic = true;

                LevelBody.Add(new Tuple<Body, Texture2D>(TempBody, i.Item1));

            }

        }

        /***********************************************************************************************************//**
         * This draws the texture associated with the world
         **************************************************************************************************************/
        public void DrawLevel(SpriteBatch spriteBatch) {

            foreach (var i in LevelBody) {
                
                spriteBatch.Draw(i.Item2, ConvertUnits.ToDisplayUnits(i.Item1.Position), Color.White);

            }

        }

    }

}
