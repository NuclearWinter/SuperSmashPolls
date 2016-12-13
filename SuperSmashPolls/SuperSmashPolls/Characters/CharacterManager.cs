using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperSmashPolls.Graphics;

namespace SuperSmashPolls.Characters {

    /// <summary>
    /// This class handles everything relating to characters.
    /// </summary>
    public class CharacterManager {

        /// <summary>The index of various moves to be used in their arrays</summary>
        public const int IdleIndex = 0,
            WalkIndex              = 1,
            JumpIndex              = 2,
            SpecialIndex           = 3,
            SideSpecialIndex       = 4,
            UpSpecialIndex         = 5,
            DownSpecialIndex       = 6,
            BasicIndex             = 7;
        /// <summary> This characters name</summary>
        public string Name;
        /** The mass of the character (kg) */
        protected readonly float Mass;
        /** The friction of the character */
        protected readonly float Friction;
        /** The restitution of the character (how bouncy they are) */
        protected readonly float Restitution;
#if COMPLEX_BODIES
        /** The collision category for this character's bodies */
        protected Category CollisionCategory;
        /** The category for hitboxes */
        protected Category HitboxCategory;
        /** The moves for this character */
        protected Moves CharacterMoves;
#else
        /// <summary>The function signiture for moves</summary>
        public delegate void SimpleMove(Body characterBody, float direction, bool onCharacter, World world);
        /// <summary></summary>
        protected Body CharacterBody;
        /** The vertices created from the hitbox texture that are used to construct the body of the character */
        private List<Vertices> CharacterVertices;
        /** The CharacterAction (texture handler) for each of the character moves */
        private CharacterAction[] MoveTextures;
        /** The audio for each of the characters moves */
        private AudioHandler[] MoveAudio;
        /** The functions to run when a move is done */
        private SimpleMove[] MoveFunctions;
        /** The number of moves that have been implimented for this character */
        private int ImplimentedMoves;
        /** The scale of textures */
        private int Scale;
        /** The data so that a new class can be constructed from this one (easily) */
        private Tuple<float, Point, Texture2D, SoundEffect, SimpleMove>[] MoveData;
        /** The texture of the body */
        private Texture2D BodyTexture;
        /** Whether or not the character is allow to do another move */
        private bool CanMove;
        /**  */
        private int CurrentMove;
        /** The move that the character is working on doing */
        private int WorkingMove;
        /**  */
        private GamePadState PreviousState;
        /**  */
        private Vector2 CharacterOrigin;
        /**  */
        private World GameWorld;

#endif
        /** This is the amount the joystick must be over for it to register as intentional */
        private const float Register = 0.4F;
        /** The direction that the character is moving */
        private float Direction;

        /// <summary>
        /// Default constructor for making a blank character. The name is initialized to check and to check after a game 
        /// has already been played (and they already selected a character so it wouldn't be null)
        /// </summary>
        public CharacterManager() {
            Name = "blank";
        }

#if COMPLEX_BODIES

        /// <summary>
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="friction"></param>
        /// <param name="restitution"></param>
        /// <param name="collisionCategory"></param>
        /// <param name="hitboxCategory"></param>
        /// <param name="name"></param>
        public CharacterManager(float mass, float friction, float restitution, Category collisionCategory, 
            Category hitboxCategory, string name) {
            Mass              = mass;
            Friction          = friction;
            Restitution       = restitution;
            CollisionCategory = collisionCategory;
            HitboxCategory    = hitboxCategory;
            Name              = name;
        }


        /// <summary>
        /// Creates a blank character class for player class intialization
        /// </summary>
        /// <param name="collidesWith"></param>
        /// <param name="hitboxCollidesWith"></param>
        public CharacterManager(Category collidesWith, Category hitboxCollidesWith) {

            Name = "blank";
            CollisionCategory = collidesWith;
            HitboxCategory = hitboxCollidesWith;

        }

#else
        /// <summary>
        /// This is a constructor
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="friction"></param>
        /// <param name="restitution"></param>
        /// <param name="name"></param>
        public CharacterManager(float mass, float friction, float restitution, string name) {
            Mass = mass;
            Friction = friction;
            Restitution = restitution;
            Name = name;
            CanMove = false;
            PreviousState = new GamePadState();
        }
#endif

#if COMPLEX_BODIES

        public CharacterManager Clone() {

            CharacterManager Clone = new CharacterManager(Mass, Friction, Restitution, CollisionCategory, HitboxCategory,
                Name);

            Clone.CharacterMoves = CharacterMoves.Clone();

            return Clone;

        }

        /// <summary>
        /// Add moves to this character
        /// </summary>
        public void AddMoves(MoveAssets idle, MoveAssets walk, MoveAssets jump, MoveAssets special, 
            MoveAssets sideSpecial, MoveAssets upSpecial, MoveAssets downSpecial, MoveAssets basic) {

            CharacterMoves = new Moves(idle, walk, jump, special, sideSpecial, upSpecial, downSpecial, basic,
                CollisionCategory, HitboxCategory);

        }

        /// <summary>
        /// Get the position of the character
        /// </summary>
        public Vector2 GetPosition() {

            return CharacterMoves.GetPostion();

        }

        /// <summary>
        /// Creates the Farseer bodies for the character in the specified world
        /// </summary>
        public void ConstructInWorld(World world) {
            
            CharacterMoves.MakeBodies(world, Mass, Friction, Restitution);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public void Respawn(Vector2 position) {
            
            CharacterMoves.SetPosition(position);
            CharacterMoves.ActiveBody.ResetDynamics();

        }

        /// <summary>
        /// The function to update the character
        /// </summary>
        public void UpdateCharacter(GamePadState currentState) {

            //KeyboardState CurrentKeyboardState = Keyboard.GetState();

            bool SideMovement  = Math.Abs(currentState.ThumbSticks.Left.X) >= Register;
            bool DownMovement  = currentState.ThumbSticks.Left.Y <= Register;
            bool UpMovement    = currentState.ThumbSticks.Left.Y >= Register;
            bool SpecialAttack = Math.Abs(currentState.Triggers.Left)      >= Register;
            bool Jump          = currentState.IsButtonDown(Buttons.A);
            bool BasicAttack   = currentState.IsButtonDown(Buttons.B); 

            int DesiredMove;
            
            if (SpecialAttack)
                if (SideMovement)
                    DesiredMove = Moves.SideSpecialIndex;
                else if (DownMovement)
                    DesiredMove = Moves.DownSpecialIndex;
                else if (UpMovement)
                    DesiredMove = Moves.UpSpecialIndex;
                else
                    DesiredMove = Moves.SpecialIndex;
            else if (SideMovement)
                DesiredMove = Moves.WalkIndex;
            else if (Jump || UpMovement)
                DesiredMove = Moves.JumpIndex;
            else if (BasicAttack)
                DesiredMove = Moves.BasicIndex;
            else 
                DesiredMove = Moves.IdleIndex;

            Direction = (DesiredMove == Moves.IdleIndex || DesiredMove == Moves.JumpIndex)
                ? Direction
                : currentState.ThumbSticks.Left.X;

            CharacterMoves.UpdateMove(DesiredMove, Direction);

        }

        /// <summary>
        /// 
        /// </summary>
        public void DrawCharacter(SpriteBatch spriteBatch) {
            
            CharacterMoves.DrawMove(spriteBatch);

        }
#else
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodyTexture"></param>
        /// <param name="scale"></param>
        /// <param name="moveData"></param>
        /// <remarks>Load moves in this order: IdleIndex, WalkIndex, JumpIndex, SpecialIndex, SideSpecialIndex, 
        /// UpSpecialIndex, DownSpecialIndex, BasicIndex</remarks>
        public void LoadCharacterContent(Texture2D bodyTexture, int scale, 
            params Tuple<float, Point, Texture2D, SoundEffect, SimpleMove>[] moveData) {

            CharacterVertices = CreateVerticesFromTexture(bodyTexture, scale);
            ImplimentedMoves  = moveData.Length;
            BodyTexture       = bodyTexture;
            MoveData          = moveData;
            MoveFunctions     = new SimpleMove[ImplimentedMoves];
            MoveTextures      = new CharacterAction[ImplimentedMoves];
            MoveAudio         = new AudioHandler[ImplimentedMoves];
            Scale             = scale;
            CharacterOrigin   = new Vector2(0, bodyTexture.Height);

            for (int i = 0; i < ImplimentedMoves; ++i) {
                MoveFunctions[i] = moveData[i].Item5;
                MoveTextures[i]  = new CharacterAction(moveData[i].Item1, moveData[i].Item2, moveData[i].Item3, Scale);
                if (moveData[i].Item4 != null)
                    MoveAudio[i] = new AudioHandler(moveData[i].Item4);
            }

        }

        /// <summary>
        /// Makes a new CharacterManager from this one so that we can store information in one, and play in another
        /// </summary>
        /// <returns>A CharacterManager with the same attributes as this one</returns>
        public CharacterManager Clone() {
            
            CharacterManager Clone = new CharacterManager(Mass, Friction, Restitution, Name);
            Clone.LoadCharacterContent(BodyTexture, Scale, MoveData);

            return Clone;

        }

        /// <summary>
        /// Creates the body of the character in the game's world for use during a match. This must be called before the
        /// character is updates.
        /// </summary>
        /// <param name="gameWorld">The world to put the character in</param>
        /// <param name="position">The position to put the character in</param>
        /// <param name="playerGroup">The collision group for this player to be placed in</param>
        public void SetupCharacter(World gameWorld, Vector2 position, short playerGroup) {

            CharacterBody                = BodyFactory.CreateCompoundPolygon(gameWorld, CharacterVertices, 1F, position);
            CharacterBody.CollisionGroup = playerGroup;
            CharacterBody.Mass           = Mass;
            CharacterBody.Friction       = Friction;
            CharacterBody.Restitution    = Restitution;
            CharacterBody.BodyType       = BodyType.Dynamic;
            CharacterBody.Enabled        = true;
            CharacterBody.Awake          = true;
            GameWorld                    = gameWorld;
            CurrentMove                  = 0;

        }

        /// <summary>
        /// Gets the position of the character in meters
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPosition() {

            return CharacterBody.Position;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public void Respawn(Vector2 position) {

            CharacterBody.Position = position;
            CharacterBody.ResetDynamics();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentState"></param>
        public void UpdateCharacter(GamePadState currentState) {

            MoveTextures[CurrentMove].UpdateAnimation(ConvertUnits.ToDisplayUnits(CharacterBody.Position) -
                                          CharacterOrigin);

            if (CurrentMove != IdleIndex && CurrentMove != WalkIndex && CurrentMove != JumpIndex)
                if (!MoveTextures[CurrentMove].AnimationAtEnd())
                    return;

            bool SideMovement  = Math.Abs(currentState.ThumbSticks.Left.X) >= Register;
            bool DownMovement  = currentState.ThumbSticks.Left.Y <= -Register;
            bool UpMovement    = currentState.ThumbSticks.Left.Y >= Register;
            bool UpTilt = currentState.ThumbSticks.Left.Y > 0 && !UpMovement;
            bool SpecialAttack = Math.Abs(currentState.Triggers.Left) >= Register 
                || currentState.IsButtonDown(Buttons.X) && PreviousState.IsButtonUp(Buttons.X);
            bool Jump          = currentState.IsButtonDown(Buttons.A) && PreviousState.IsButtonUp(Buttons.A) 
                || UpMovement && PreviousState.ThumbSticks.Left.Y < Register;
            bool BasicAttack   = currentState.IsButtonDown(Buttons.B) && PreviousState.IsButtonUp(Buttons.B);

            int DesiredMove = -1;

            if (SpecialAttack) {
                if (SideMovement && IsImplimented(SpecialIndex)) {
                    DesiredMove = Moves.SideSpecialIndex;
                } else if (DownMovement && IsImplimented(DownSpecialIndex)) {
                    DesiredMove = Moves.DownSpecialIndex;
                } else if (UpTilt && IsImplimented(UpSpecialIndex)) {
                    DesiredMove = Moves.UpSpecialIndex;
                } else if (IsImplimented(SpecialIndex)) {
                    DesiredMove = Moves.SpecialIndex;
                }
            } else if (SideMovement && IsImplimented(WalkIndex)) {
                DesiredMove = Moves.WalkIndex;
            } else if (Jump && IsImplimented(JumpIndex)) {
                DesiredMove = Moves.JumpIndex;
            } else if (BasicAttack && IsImplimented(BasicIndex)) {
                DesiredMove = Moves.BasicIndex;
            } else {
                DesiredMove = IdleIndex;
            }

            Direction = (DesiredMove == Moves.IdleIndex || DesiredMove == Moves.JumpIndex)
                ? Direction : currentState.ThumbSticks.Left.X;

            switch (DesiredMove) {
                case IdleIndex:
                    CurrentMove = IdleIndex;
                    break;
                case WalkIndex:
                    CurrentMove = WalkIndex;
                    MoveFunctions[WalkIndex](CharacterBody, Direction, true, GameWorld);
                    break;
                case JumpIndex:
                    CurrentMove = JumpIndex;
                    MoveFunctions[JumpIndex](CharacterBody, Direction, true, GameWorld);
                    break;
                case BasicIndex:
                    CurrentMove = BasicIndex;
                    MoveTextures[BasicIndex].StartAnimation();
                    MoveFunctions[BasicIndex](CharacterBody, Direction, false, GameWorld);
                    MoveAudio[BasicIndex]?.PlayEffect();
                    break;
                case SpecialIndex:
                    CurrentMove = SpecialIndex;
                    MoveTextures[SpecialIndex].StartAnimation();
                    MoveFunctions[SpecialIndex](CharacterBody, Direction, false, GameWorld);
                    MoveAudio[SpecialIndex]?.PlayEffect();
                    break;
                case SideSpecialIndex:
                    CurrentMove = SideSpecialIndex;
                    MoveTextures[SideSpecialIndex].StartAnimation();
                    MoveFunctions[SideSpecialIndex](CharacterBody, Direction, false, GameWorld);
                    MoveAudio[SideSpecialIndex]?.PlayEffect();
                    break;
                case UpSpecialIndex:
                    CurrentMove = UpSpecialIndex;
                    MoveTextures[UpSpecialIndex].StartAnimation();
                    MoveFunctions[UpSpecialIndex](CharacterBody, Direction, false, GameWorld);
                    MoveAudio[UpSpecialIndex]?.PlayEffect();
                    break;
                case DownSpecialIndex:
                    CurrentMove = DownSpecialIndex;
                    MoveTextures[DownSpecialIndex].StartAnimation();
                    MoveFunctions[DownSpecialIndex](CharacterBody, Direction, false, GameWorld);
                    MoveAudio[DownSpecialIndex]?.PlayEffect();
                    break;
            }

            PreviousState = currentState;

            MoveTextures[CurrentMove].UpdateAnimation(ConvertUnits.ToDisplayUnits(CharacterBody.Position) -
                CharacterOrigin);

        }

        /// <summary>
        /// Draws the character on the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawCharacter(SpriteBatch spriteBatch) {
            
            MoveTextures[CurrentMove].DrawAnimation(ref spriteBatch, Direction);

        }

        /// <summary>
        /// Creates a list of vertices from a texture.
        /// </summary>
        /// <param name="texture">The texture to make a body from</param>
        /// <param name="scale">The scale of the texture</param>
        /// <param name="algorithm">The decomposition algorithm to use</param>
        /// <remarks> Available algorithms to use are Bayazit, Dealuny, Earclip, Flipcode, Seidel, SeidelTrapazoid</remarks>
        /// @warning In order for this to work the input must have a transparent background. I highly reccomend that you
        /// only use this with PNGs as that is what I have tested and I know they work. This will only produce a bosy as
        /// clean as the texture you give it, so avoid partically transparent areas and little edges.
        private List<Vertices> CreateVerticesFromTexture(Texture2D texture, float scale, 
            TriangulationAlgorithm algorithm = TriangulationAlgorithm.Earclip) {

            int SpriteSheetSize = texture.Width * texture.Height;
            uint[] TextureData  = new uint[SpriteSheetSize]; //Array to copy texture info into
            texture.GetData(TextureData); //Gets which pixels of the texture are actually filled

            Vertices vertices         = TextureConverter.DetectVertices(TextureData, texture.Width);
            List<Vertices> VertexList = Triangulate.ConvexPartition(vertices, algorithm);

            Vector2 VertScale = new Vector2(ConvertUnits.ToSimUnits(scale));

            foreach (Vertices vert in VertexList)
                vert.Scale(ref VertScale); //Scales the vertices to match the size we specified

            Vector2 Centroid = -vertices.GetCentroid();
            vertices.Translate(ref Centroid);
            //basketOrigin = -centroid;

            return VertexList;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        private bool IsImplimented(int move) {

            return ImplimentedMoves > move;

        }

#endif
    }

}
