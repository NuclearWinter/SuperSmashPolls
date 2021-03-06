﻿using System;
using System.Collections.Generic;
using FarseerPhysics;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Content;

namespace SuperSmashPolls.Levels {

    /// <summary>
    /// Holds and handles the drawing of and contruction of bodies.
    /// This class is responsible for handling the storage and creation of static bodies to use in the game world.
    /// </summary>
    public class LevelHandler {
        /** The bodies of this level (Body, texture, size (in meters)) */
        private readonly List<Tuple<Body, Texture2D, Vector2>> LevelBody;
        /** The background for this level */
        private Texture2D LevelBackground; 
        /** The amount that the background needs to be scaled (adjusted for different screen sizes) */
        private Vector2 LevelBackgroundScale;
        /// <summary>The name of this level</summary>
        public readonly string Name;
        /// <summary>he place for a player to spawn on this map</summary>
        public Vector2 PlayerOneSpawn, PlayerTwoSpawn, PlayerThreeSpawn, PlayerFourSpawn, RespawnPoint;
        /// <summary>The Farseer world for this level</summary>
        public World LevelWorld;

#if DEBUG
        private DebugViewXNA DebugView;
#endif

        //TODO music

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of this level</param>
        /// <param name="playerOneSpawn">The spawn point for player one</param>
        /// <param name="playerTwoSpawn">The spawn point for player two</param>
        /// <param name="playerThreeSpawn">The spawn point for player three</param>
        /// <param name="playerFourSpawn">The spawn point for player four</param>
        /// <param name="respawnPoint">The respawn point for all players when/if they die</param>
        /// <param name="horizontalGravity">The gravity of the world on a horizontal scale</param>
        /// <param name="verticalGravity">The gravity on a vertical scale</param>
        public LevelHandler(string name, Vector2 playerOneSpawn, Vector2 playerTwoSpawn, Vector2 playerThreeSpawn,
            Vector2 playerFourSpawn, Vector2 respawnPoint, float horizontalGravity = 0, float verticalGravity = 9.80F) {

            Name             = name;
            PlayerOneSpawn   = playerOneSpawn;
            PlayerTwoSpawn   = playerTwoSpawn;
            PlayerThreeSpawn = playerThreeSpawn;
            PlayerFourSpawn  = playerFourSpawn;
            RespawnPoint     = respawnPoint;

            LevelWorld = new World(new Vector2(horizontalGravity, verticalGravity));
            LevelBody  = new List<Tuple<Body, Texture2D, Vector2>>();

        }

#if DEBUG
        /// <summary>
        /// Call this in LoadContent after all the level is set up
        /// </summary>
        public void ExtablishDebugView(GraphicsDevice graphicsDevice, ContentManager content) {

            //Give your world to debugView
            DebugView = new DebugViewXNA(LevelWorld);
            //DebugView have many Property you can add
            DebugView.RemoveFlags(DebugViewFlags.Shape);
            DebugView.RemoveFlags(DebugViewFlags.Joint);
            DebugView.DefaultShapeColor = Color.LightPink;
            DebugView.SleepingShapeColor = Color.MediumPurple;
            //add your GraphicsDevice & your ContentManager

            DebugView.AppendFlags(DebugViewFlags.CenterOfMass);
            DebugView.AppendFlags(DebugViewFlags.PerformanceGraph);
            DebugView.AppendFlags(DebugViewFlags.Shape);
            DebugView.AppendFlags(DebugViewFlags.ContactPoints);
            DebugView.AppendFlags(DebugViewFlags.PolygonPoints);
            DebugView.AppendFlags(DebugViewFlags.ContactNormals);

            DebugView.DefaultShapeColor = Color.White;
            DebugView.SleepingShapeColor = Color.LightGray;

            DebugView.DebugPanelPosition = Vector2.Zero;
            DebugView.PerformancePanelBounds = graphicsDevice.ScissorRectangle;

            DebugView.LoadContent(graphicsDevice, content);

        }
#endif

        /// <summary>
        /// Creates a polygon from a texture. This is the important function here.
        /// </summary>
        /// <param name="texture">The texture to make a body from</param>
        /// <param name="density">The density of the object (Will almost always be one</param>
        /// <param name="position">The position (in meters) of the object in the world</param>
        /// <param name="scale">The scale of the object (how much to change its size)</param>
        /// <param name="algorithm">The decomposition algorithm to use</param>
        /// <remarks> Available algorithms to use are Bayazit, Dealuny, Earclip, Flipcode, Seidel, SeidelTrapazoid</remarks>
        /// @warning In order for this to work the input must have a transparent background. I highly reccomend that you
        /// only use this with PNGs as that is what I have tested and I know they work. This will only produce a bosy as
        /// clean as the texture you give it, so avoid partically transparent areas and little edges.
        private Body CreatePolygonFromTexture(Texture2D texture, float density, Vector2 position, float scale,
            TriangulationAlgorithm algorithm = TriangulationAlgorithm.Bayazit) {

            uint[] TextureData = new uint[texture.Width * texture.Height]; //Array to copy texture info into
            texture.GetData<uint>(TextureData); //Gets which pixels of the texture are actually filled

            Vertices vertices = TextureConverter.DetectVertices(TextureData, texture.Width);
            List<Vertices> vertexList = Triangulate.ConvexPartition(vertices, algorithm);

            Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits(scale));
            foreach (Vertices vert in vertexList)
                vert.Scale(ref vertScale); //Scales the vertices to match the size we specified

            Vector2 centroid = -vertices.GetCentroid();
            vertices.Translate(ref centroid);
            //basketOrigin = -centroid;

            //This actually creates the body
            return BodyFactory.CreateCompoundPolygon(LevelWorld, vertexList, density, position);

        }

        /// <summary>
        /// Sets a background to the level
        /// </summary>
        /// <param name="levelBackground">The texture to use as a background</param>
        /// <param name="levelBackgroundScale">The scale to use for the background texture (i.e. background is 640x360,
        /// screen is 1280x720 we would need to scale by 2)</param>
        public void SetBackground(Texture2D levelBackground, Vector2 levelBackgroundScale) {

            LevelBackgroundScale = levelBackgroundScale;
            LevelBackground      = levelBackground;

        }

        /// <summary>
        /// Creates the body and puts it in the world
        /// </summary>
        /// <param name="items">All the items to add to the world (Texture, position (meters), size (meters))</param>
        public void AssignToWorld(params Tuple<Texture2D, Vector2, Vector2>[] items) {

            foreach (var i in items) {

                Body TempBody     = CreatePolygonFromTexture(i.Item1, 1F, i.Item2,
                    i.Item3.X/ConvertUnits.ToSimUnits(i.Item1.Width), TriangulationAlgorithm.Earclip);
                TempBody.BodyType = BodyType.Static;
                TempBody.IsStatic = true;
                //TempBody.Restitution = 0;
                //TempBody.CollisionCategories = Category.All;

                LevelBody.Add(new Tuple<Body, Texture2D, Vector2>(TempBody, i.Item1, i.Item3));

            }

        }

        /// <summary>
        /// This draws world
        /// </summary>
        /// <param name="spriteBatch">The batch to draw with</param>
        /// <param name="font">The font to use for debugging (optional)</param>
        public void DrawLevel(SpriteBatch spriteBatch, SpriteFont font = null) {

            if (LevelBackground != null)
                spriteBatch.Draw(LevelBackground, Vector2.Zero, null, Color.White, 0, Vector2.Zero, LevelBackgroundScale,
                    SpriteEffects.None, 0);
            else
                spriteBatch.GraphicsDevice.Clear(Color.White);

            foreach (var i in LevelBody) {

                spriteBatch.Draw(i.Item2, ConvertUnits.ToDisplayUnits(i.Item1.Position), null, Color.White, 0,
                    Vector2.Zero, i.Item3.X/ConvertUnits.ToSimUnits(i.Item2.Width), SpriteEffects.None, 0);

            }

        }

    }

}
