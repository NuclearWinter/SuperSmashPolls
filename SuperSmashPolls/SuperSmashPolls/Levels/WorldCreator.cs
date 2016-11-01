using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace SuperSmashPolls.Levels {

    class WorldCreator {
        /** The texture to use in this world */
        private readonly Texture2D WorldTexture;
        /** The scale used to say how many pixels are how many meters */
        private readonly Vector2 PixelToMeterScale;

        /***********************************************************************************************************//**
         * Gets the information needed to later create the world if it is selected.
         **************************************************************************************************************/ 
        public WorldCreator(Texture2D worldTexture, float pixelToMeterScale) {
            WorldTexture = worldTexture;
            PixelToMeterScale = new Vector2(1/pixelToMeterScale, 1/pixelToMeterScale);

        }

        /***********************************************************************************************************//**
         * Creates the body and puts it in the world
         * @ref gameworld The world to put the body into
         **************************************************************************************************************/ 
        public void AssignToWorld(ref World gameWorld) {

            uint[] TextureData = new uint[WorldTexture.Width * WorldTexture.Height];

            WorldTexture.GetData(TextureData);

            Vertices TextureVertices = PolygonTools.CreatePolygon(TextureData, WorldTexture.Width, true);

            TextureVertices.Scale(PixelToMeterScale);

            List<Vertices> PolygonList = BayazitDecomposer.ConvexPartition(TextureVertices);

            Body StageBody = new Body(gameWorld, Vector2.Zero);

            List<Fixture> Compound = FixtureFactory.AttachCompoundPolygon(PolygonList, 1, StageBody);

            Compound[0].Body.BodyType = BodyType.Static;

        }

    }

}
