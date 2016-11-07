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

    class LevelHandler {
        /** The texture to use in this world */
        private readonly Texture2D WorldTexture;
        /** The scale used to say how many pixels are how many meters */
        private readonly float PixelToMeterScale;
        /* Texture sclae */
        private readonly Vector2 TextureScale;
        /*  */
        private Body LevelBody;

        /***********************************************************************************************************//**
         * Gets the information needed to later create the world if it is selected.
         **************************************************************************************************************/ 
        public LevelHandler(Texture2D worldTexture, float pixelToMeterScale, ref Vector2 screenSize) {
            WorldTexture = worldTexture;
            PixelToMeterScale = pixelToMeterScale;
            TextureScale = new Vector2(screenSize.X/WorldTexture.Width, screenSize.Y/WorldTexture.Height);
            //ScreenSize = screenSize;
        }

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
         * @ref gameworld The world to put the body into
         **************************************************************************************************************/
        public void AssignToWorld(ref World gameWorld) {

//            uint[] TextureData = new uint[WorldTexture.Width * WorldTexture.Height];
//
//            WorldTexture.GetData(TextureData);
//
//            Vertices TextureVertices = PolygonTools.CreatePolygon(TextureData, WorldTexture.Width, true);
//
//            TextureVertices.Scale(new Vector2(ConvertUnits.ToSimUnits(WorldTexture.Width),
//                ConvertUnits.ToSimUnits(WorldTexture.Height)));
//
//            List<Vertices> PolygonList = BayazitDecomposer.ConvexPartition(TextureVertices);

            //LevelBody = BodyFactory.CreateCompoundPolygon(gameWorld, PolygonList, 1F);

            Vector2 scale = new Vector2(ConvertUnits.ToSimUnits(WorldTexture.Width),
                ConvertUnits.ToSimUnits(WorldTexture.Height));

            Vector2 position = new Vector2(ConvertUnits.ToSimUnits(WorldTexture.Width / 2),
                ConvertUnits.ToSimUnits(WorldTexture.Height / 2));

            LevelBody = CreatePolygonFromTexture(WorldTexture, gameWorld, 1F, position,
                ConvertUnits.ToSimUnits(WorldTexture.Width)/WorldTexture.Width, TriangulationAlgorithm.Seidel);

            LevelBody.BodyType = BodyType.Static;

        }

        public void MakeTemple(ref World gameWorld) {
            


        }

        /***********************************************************************************************************//**
         * This draws the texture associated with the world
         **************************************************************************************************************/
        public void DrawLevel(SpriteBatch spriteBatch) {

            Vector2 TextureOrigin = new Vector2(WorldTexture.Width/2, WorldTexture.Height/2);

//            spriteBatch.Draw(WorldTexture, LevelBody.Position, null, Color.White, 0, Vector2.Zero, TextureScale,
//                SpriteEffects.None, 0);
            //TODO test where this puts it
            spriteBatch.Draw(WorldTexture, ConvertUnits.ToDisplayUnits(LevelBody.Position) - TextureOrigin, Color.White);

        }

    }

}
