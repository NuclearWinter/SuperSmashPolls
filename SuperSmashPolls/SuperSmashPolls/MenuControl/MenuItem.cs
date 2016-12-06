/*******************************************************************************************************************//**
 * /doc:SuperSmashPolls.XML
 **********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperSmashPolls.Graphics;
using SuperSmashPolls.World_Control;

namespace SuperSmashPolls.MenuControl {

    /***************************************************************************************************************//**
     * <summary>
     * Holds the different commands that the menu might need to send into the main game.
     * There should be one of these for everthing the menu is supposed to do.
     * </summary>
     * <remarks>This is a somewhat kluge solution, but it works well</remarks>
     ******************************************************************************************************************/
    enum MenuCommands {
        
        Nothing,          //The game should do nothing
        StartGame,        //The game should start
        MultiplayerMenu,  //Opens the multiplayer menu
        SingleplayerMenu, //Opens the singleplayer menu
        LoadSave,         //Loads the game
        SaveGame,         //Saves the game
        BackToMainMenu,   //Go back to the previous menu
        ExitGame,         //Exits the game
        OnePlayer, TwoPlayer, ThreePlayer, FourPlayer,
        ResumeGame,
        PlayTemple, PlayTempleRock, PlayFinalDestination, PlayDebate,
        SelectTrump,
        CharacterSelection,
        HelpMenu

    }

    /// <summary>
    /// This class handles all properties of an item on a menu.
    /// One very important object is the void function reference to be ran when the item is selected. This function needs
    /// to be declared and defined elsewhere, and when the item is constructed it must be passed as an argument.
    /// </summary>
    /// <remarks>To create multi-leveled menus you must construct items from bottom to top.</remarks>
    class MenuItem {

        /** Position of this item on the screen */
        private readonly WorldUnit Position;
        /** Buffer to move text over from Position */
        private readonly WorldUnit TextBuffer;
        /** If the item has it's own menu that it can display if selected */
        private readonly bool HasSubmenu;
        /** Whether or not to play the audio of the item above it */
        private readonly bool DeferAudio;
        /** The position of characters for American Text mode */
        private readonly List<Vector2> AmericanPositions;
        /** Font to use for this item */
        private SpriteFont Font;
        /** Texture to use as the thumbnail for this item */
        private Texture2D Texture;
        /** The size of this item (as a ratio of the picture's size) */
        private Vector2 TextureSize;
        /** Background for this menu item (if the item is selected). This will cover the entire screen
         * @warning This must be set during load content */
        private Texture2D Background { get; set; } = null;
        /** Overlay this item's sub-menu on top of the current menu TODO impliment */
        private bool SubOverlay { get; set; } = false;
        /* The item on screen that is currently highlighted */
        private int CurrentHighlightedItem;
        /* Counts how many cycles the text has been a color for American text */
        private int AmericanCounter;
        /** The music for this menu */
        private AudioHandler MenuAudio;
        /** Handles looping of audio */
        private SoundEffectInstance MusicInstance;
        /// <summary>Determines if the item is highlihtable or not. Highlightable items can be selected</summary>
        public readonly bool Highlightable;
        /// <summary>Whether or not to center the item horizontally around the position.</summary>
        public readonly bool CenterItem;
        /// <summary>The command to use if clicked on</summary>
        public readonly MenuCommands Command;
        /// <summary>Makes the text more american, not american't</summary>
        public bool AmericaText;
        /// <summary>The item within ContainedItems to draw instead of this one (-1 means draw this one)</summary>
        public int DrawDown;
        /// <summary>Text for this item (drawn on top of the screen if displaying the item's menu</summary>
        public string Text;
        /// <summary>The item's to display on this page</summary>
        public List<MenuItem> ContainedItems;
        /// <summary>Color for this item's text</summary>
        public Color TextColor { get; set; } = Color.Black;
        /// <summary>The options for this menu.</summary>
        public Dictionary<string, string[]> Options;
        /// <summary>The index of the options currently selected in Options</summary>
        public short[] SelectedOption;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">The position of the item on the screen.</param>
        /// <param name="text">The text of this item.</param>
        /// <param name="hasSubmenu">Whether or not this item has a menu below it to navigate to.</param>
        /// <param name="textBuffer">An amount to displace the text past the start of the item (to center with backgrounds)</param>
        /// <param name="centerItem">Whether or not to center the item around the position.</param>
        /// <param name="highlightable">Whether or not this item is highlightable</param>
        /// <param name="command">The command to have this item return</param>
        /// <param name="americaText">Makes the text great again</param>
        /// <param name="deferAudio">Whther or not to play the audio of the MenuItem above this one</param>
        public MenuItem(WorldUnit position, string text, bool hasSubmenu, WorldUnit textBuffer, bool highlightable, 
            bool centerItem = false, MenuCommands command = MenuCommands.Nothing, bool americaText = false, 
            bool deferAudio = true) {
            Position      = position;
            Text          = text;
            HasSubmenu    = hasSubmenu;
            TextBuffer    = textBuffer;
            Highlightable = highlightable;
            CenterItem    = centerItem;
            Command       = command;
            Texture       = null;
            AmericanPositions = new List<Vector2>();
            ContainedItems    = new List<MenuItem>();
            DrawDown          = -1;
            AmericaText       = americaText;
            AmericanCounter   = 0;
            DeferAudio        = deferAudio;
        }

        /// <summary>
        /// Adds options to a submenu. Options can only be added once, otherwise the previously added options will be 
        /// overriden.
        /// </summary>
        /// <param name="options">The first string is the key to display on the menu as the title for the option, the
        ///  second are the values to associate to that option.</param>
        /// TODO allow ranges of numbers
        public void AddOptions(params Tuple<string, string[]>[] options) {

            

        }

        /// <summary>
        /// 
        /// </summary>
        public void ReadOptions() {
            


        }

        /// <summary>
        /// Add an item to display when this screen is being drawn.
        /// </summary>
		/// <param name="addItem">The item to add</param>
        public void AddItem(MenuItem addItem) {

            ContainedItems.Add(addItem);

        }


        /// <summary>
        /// Sets the font for all the items in this menu
        /// </summary>
        /// <param name="font">The font to set for all items in this menu</param>
        public void SetFontForAll(SpriteFont font) {

            Font = font;

            foreach (MenuItem i in ContainedItems) {
                i.SetFontForAll(font);
                i.Font = font;
            }

            if (!AmericaText) return;

            Vector2 pos = Position.Add(TextBuffer).GetThisPosition() - Font.MeasureString(Text) / 2;

            foreach (char i in Text) {

                AmericanPositions.Add(pos);

                pos.X += Font.MeasureString(i.ToString()).X;

            }

        }

        /// <summary>
        /// Sets the font for this item
        /// </summary>
        /// <param name="font">The font to set it to</param>
        public void SetFont(SpriteFont font) {

            Font = font;

        }

        /// <summary>
        /// Add's a texture to the item
        /// </summary>
        /// <param name="texture">The texture of this item</param>
        /// <param name="size">The size of this item (in pixels)</param>
        public void SetTexture(Texture2D texture , Vector2 size) {

            Texture     = texture;
            TextureSize = new Vector2(size.X/Texture.Width, size.Y/Texture.Height);

        }

        /// <summary>
        /// Adds audio to this object to be played
        /// </summary>
        /// <param name="audioHandler">The audio to add to this object</param>
        public void AddAudio(AudioHandler audioHandler) {

            MenuAudio = audioHandler;
            MusicInstance = MenuAudio.GetRandomAudio().CreateInstance();
            MusicInstance.IsLooped = true;

        }

        /// <summary>
        /// Update control of the menu.
        /// </summary>
        /// <param name="currentState">The current state to update from</param>
        /// <param name="lastPressed">The state to check if something was updated the previous time</param>
        /// <remarks>This is done to allow for enumerator values to be changed</remarks>
        public MenuCommands UpdateMenu(GamePadState currentState, GamePadState lastPressed) {

            if (DrawDown == -1) {
                /* Updates the current menu */

                if (!DeferAudio)
                    MusicInstance?.Play();

                foreach (var i in ContainedItems)
                    i.AmericaTextUpdate();

                ContainedItems[CurrentHighlightedItem].TextColor = Color.Black;

                //Goes down to the next menu
                if (currentState.IsButtonDown(Buttons.A) && lastPressed.IsButtonUp(Buttons.A)) {

                    if (ContainedItems[CurrentHighlightedItem].HasSubmenu)
                        DrawDown = CurrentHighlightedItem;
                    else
                        return ContainedItems[CurrentHighlightedItem].Command;

                }

                //Highlightes one item up in the array
                if (currentState.IsButtonDown(Buttons.DPadDown) && lastPressed.IsButtonUp(Buttons.DPadDown))
                    for (int i = CurrentHighlightedItem + 1; i < ContainedItems.Count(); ++i) {

                        if (!ContainedItems[i].Highlightable) continue;

                        CurrentHighlightedItem = i;

                        break;

                    }

                //Highlights one item down in the array
                if (currentState.IsButtonDown(Buttons.DPadUp) && lastPressed.IsButtonUp(Buttons.DPadUp))
                    for (int i = CurrentHighlightedItem - 1; i >= 0; --i) {

                        if (!ContainedItems[i].Highlightable) continue;

                        CurrentHighlightedItem = i;

                        break;

                    }

                ContainedItems[CurrentHighlightedItem].TextColor = Color.Red;

            } else {

                //Moves the menu back a screen
                if (ContainedItems[DrawDown].DrawDown == -1
                    && currentState.IsButtonDown(Buttons.B) && lastPressed.IsButtonUp(Buttons.B)) {
                    DrawDown = -1;

                    return UpdateMenu(currentState, lastPressed);

                }

                if (ContainedItems[DrawDown].DeferAudio && !DeferAudio)
                    MusicInstance?.Play();

                return ContainedItems[DrawDown].UpdateMenu(currentState, lastPressed);

            }

            return MenuCommands.Nothing;

        }

        /// <summary>
        /// Stops playing audio
        /// </summary>
        /// <param name="recursive">Stop audio for all child members as well</param>
        public void StopAudio(bool recursive = false) {
            
            if (recursive)
                foreach (var i in ContainedItems)
                    i.MusicInstance.Stop();
            
            MusicInstance.Stop();

        }

        /// <summary>
        /// Display the items in ContainedItems and other menu items.
        /// This is for if this item has a menu that it can display.
        /// </summary>
        public void DisplayMenu(ref SpriteBatch batch) {

            //Will draw this menu alone or with another menu on top of it
            if (DrawDown == -1 || ContainedItems[DrawDown].SubOverlay) {

                if (Background != null)
                    batch.Draw(Background, new Vector2(0, 0), Color.White);

                foreach (MenuItem i in ContainedItems)
                    i.DisplayItem(ref batch);

            }

            if (DrawDown != -1) {
                
                ContainedItems[DrawDown].DisplayMenu(ref batch);

            }

        }

        /// <summary>
        /// Display this item.
        /// This is for when this item is displayed (on a different menu).
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw with</param>
        public void DisplayItem(ref SpriteBatch batch) {

            if (Texture != null) {

                if (CenterItem)
                    batch.Draw(Texture, Position.GetThisPosition(), null, Color.White, 0, Vector2.Zero, TextureSize,
                        SpriteEffects.None, 0);
                else
                    batch.Draw(Texture, Position.GetThisPosition() - new Vector2(Texture.Width/2F, Texture.Height/2F),
                        null, Color.White, 0, Vector2.Zero, TextureSize, SpriteEffects.None, 0);

            }

            if (AmericaText)
                DrawAmerican(batch);
            else if (CenterItem)
                batch.DrawString(Font, Text, 
                    Position.Add(TextBuffer).GetThisPosition() - Font.MeasureString(Text)/2, TextColor);
            else 
                batch.DrawString(Font, Text, Position.Add(TextBuffer).GetThisPosition(), TextColor);

        }

        /// <summary>
        /// Access function for members of DisplayItems.
        /// </summary>
        /// <param name="index">The indices for the path to the desired item</param>
        public MenuItem AccessItem(params int[] index) {

            if (index.Length == 1) {

                try {

                    return ContainedItems[index[0]];

                } catch (ArgumentOutOfRangeException) {

                    Console.WriteLine("The menu you are trying to acess does not exist.");

                }

            }

            return ContainedItems[index[0]].AccessItem(index.Skip(1).ToArray());

        }

        /// <summary>
        /// Sets the menu to view a specific item.
        /// </summary>
        /// <param name="drawTo">The path to the item to view</param>
        public void SetDrawDown(params int[] drawTo) {

            DrawDown = drawTo[0];

            if (drawTo.Length > 1)
                ContainedItems[drawTo[0]].SetDrawDown(drawTo.Skip(1).ToArray());
            else
                ContainedItems[DrawDown].DrawDown = -1;

        }

        /// <summary>
        /// Continues to make text great
        /// </summary>
        private void AmericaTextUpdate() {

            if (!AmericaText) return;

            if (AmericanCounter >= 50) {

                AmericanCounter = 0;
                TextColor       = (TextColor == Color.Blue) ? Color.Red : Color.Blue;

            } else
                ++AmericanCounter;

        }

        /// <summary>
        /// Draws the text every other color
        /// </summary>
        private void DrawAmerican(SpriteBatch batch) {

            for (int i = 0; i < Text.Length; ++i) {
                
                batch.DrawString(Font, Text[i].ToString(), AmericanPositions[i], TextColor);

                TextColor = (TextColor == Color.Blue) ? Color.Red : Color.Blue;

            }

        }



    }

}
