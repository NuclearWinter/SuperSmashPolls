/*******************************************************************************************************************//**
 * /doc:SuperSmashPolls.XML
 **********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperSmashPolls.World_Control;

namespace SuperSmashPolls.MenuControl {

    /***************************************************************************************************************//**
     * <summary>
     * Holds the different commands that the menu might need to send into the main game.
     * There should be one of these for everthing the menu is 
     * <remarks> This is a somewhat kluge solution, but it works well</remarks>
     * </summary>
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
        PlayTemple,
        PlayTempleRock,
        SelectTrump

    }

     ///<summary>
     ///This class handles all properties of an item on a menu.
     ///One very important object is the void function reference to be ran when the item is selected. This function needs
     ///to be declared and defined elsewhere, and when the item is constructed it must be passed as an argument.
     ///</summary>
     ///<remarks> To create multi-leveled menus you must construct items from bottom to top.</remarks>
    class MenuItem {
        /** Determines if the item is highlihtable or not */
        public readonly bool Highlightable;
        /** Whether or not to center the text */
        public readonly bool CenterItem;
        /* The command to use if clicked on */
        public readonly MenuCommands Command;
        /** Buffer to move text over from Position */
        private readonly WorldUnit TextBuffer;
        /** Position of this item on the screen */
        private readonly WorldUnit Position;
        /** Text for this item (drawn on top of the screen if displaying the item's menu */
        public string Text;

        /* Anything below here must be loaded after the constructor */

        /** Color for this item's text */
        private Color TextColor { get; set; } = Color.Black;
        /** Font to use for this item */
        private SpriteFont Font;
        /** Texture to use as the background for this item (not drawn if displaying the item's menu) */
        private Texture2D Texture;
        /** The size of this item (as a ratio of the picture's size) */
        private Vector2 TextureSize;
        /** Background for this menu item (if the item is selected). <remarks> This will cover the entire screen
         * @warning This must be set during load content */
        private Texture2D Background { get; set; } = null;

        /* Anything below here is for if this item can be selected to display another menu */

        /** The item's to display on this page */
        public List<MenuItem> ContainedItems;
        /** Holds if the item has it's own menu that it can display */
        private readonly bool HasSubmenu;
        /** Overlay this item's sub-menu on top of the current menu */
        private bool SubOverlay { get; set; } = false;
        /* The item within ContainedItems to draw instead of this one (-1 means draw this one) */
        public int DrawDown;

        /* The item on screen that is currently highlighted */
        private int CurrentHighlightedItem;
        /* Holds the last time the menu was updates */
        private int LastTimeUpdated;

         ///<summary>
         ///Constructor
		 ///<param name="position"> The position of the item on the screen.</param>
	     ///<param name="text"> The text of this item.</param>
		 ///<param name="hasSubmenu"> Whether or not this item has a menu below it to navigate to.</param>
		 ///<param name="textBuffer"> An amount to displace the text past the start of the item (to center with backgrounds)</param>
		 ///<param name="centerItem"> Whether or not to center the item around the position.</param>
	     ///<param name="highlightable"> Whether or not this item is highlightable</param>
         ///</summary>
        public MenuItem(WorldUnit position, string text, bool hasSubmenu, WorldUnit textBuffer, bool highlightable, 
            bool centerItem = false, MenuCommands command = MenuCommands.Nothing) {
            Position      = position;
            Text          = text;
            HasSubmenu    = hasSubmenu;
            TextBuffer    = textBuffer;
            Highlightable = highlightable;
            CenterItem    = centerItem;
            Command       = command;
            Texture       = null;

            ContainedItems = new List<MenuItem>();
            DrawDown       = -1;
        }

         ///<summary>
         ///Add an item to display when this screen is being drawn.
		 ///<param name="position">The position of this item on the screen.</param>
	     ///<param name="hasSubmenu">Whether or not this item has a submenu</param>
		 ///<param name="text">The text to show on this item</param>
         ///</summary>
        public void AddItem(MenuItem addItem) {

            ContainedItems.Add(addItem);

        }


         ///<summary>
         ///Sets the font for all the items in this menu
         ///</summary>
        public void SetFontForAll(SpriteFont font) {

            Font = font;

            foreach (MenuItem i in ContainedItems) {
                i.SetFontForAll(font);
                i.Font = font;
            }

        }

         ///<summary>
         ///Add's a texture to the item
         ///<param name="texture">The texture of this item</param>
         ///<param name="size">The size of this item (in pixels)</param>
         ///</summary>
        public void SetTexture(Texture2D texture , Vector2 size) {

            Texture     = texture;
            TextureSize = new Vector2(size.X/Texture.Width, size.Y/Texture.Height);

        }

         ///<summary>
         ///Update control of the menu.
         ///<param name="controllingPlayer">The player to control the menu</param>
         ///<remarks>This is done to allow for enumerator values to be changed</remarks>
         ///</summary>
        public MenuCommands UpdateMenu(PlayerIndex controllingPlayer) {

            DateTime now = DateTime.Now;

            if (Math.Abs(now.Millisecond - LastTimeUpdated) <= 100) return MenuCommands.Nothing;

            LastTimeUpdated = now.Millisecond;

            if (DrawDown == -1) {
            /* Updates the current menu */

                ContainedItems[CurrentHighlightedItem].TextColor = Color.Black;

                //Goes down to the next menu
                if (GamePad.GetState(controllingPlayer).IsButtonDown(Buttons.A)) {

                    if (ContainedItems[CurrentHighlightedItem].HasSubmenu)
                        DrawDown = CurrentHighlightedItem;
                    else
                        return ContainedItems[CurrentHighlightedItem].Command;

                }

                //Highlightes one item up
                if (GamePad.GetState(controllingPlayer).IsButtonDown(Buttons.DPadDown))
                    for (int i = CurrentHighlightedItem + 1; i < ContainedItems.Count(); ++i) {

                        if (!ContainedItems[i].Highlightable) continue;

                        CurrentHighlightedItem = i;

                        break;

                    }
                        
                //Highlights one item down
                if (GamePad.GetState(controllingPlayer).IsButtonDown(Buttons.DPadUp))
                    for (int i = CurrentHighlightedItem - 1; i >= 0; --i) {

                        if (!ContainedItems[i].Highlightable) continue;

                        CurrentHighlightedItem = i;

                        break;

                    }

            ContainedItems[CurrentHighlightedItem].TextColor = Color.Red;

            } else 
                return ContainedItems[DrawDown].UpdateMenu(controllingPlayer);

            return MenuCommands.Nothing;

        }

         ///<summary>
         ///Display the items in ContainedItems and other menu items.
         ///This is for if this item has a menu that it can display.
         ///</summary>
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

         ///<summary>
         ///Display this item.
         ///This is for when this item is displayed (on a different menu).
         ///</summary>
         ///<param name="batch">The SpriteBatch to draw with</param>
         ///@warning This method assumes that the spritebatch has already been started.
        public void DisplayItem(ref SpriteBatch batch) {

            if (Texture != null) {

                if (CenterItem)
                    batch.Draw(Texture, Position.GetThisPosition(), null, Color.White, 0, Vector2.Zero, TextureSize,
                        SpriteEffects.None, 0);
                else
                    batch.Draw(Texture, Position.GetThisPosition() - new Vector2(Texture.Width/2F, Texture.Height/2F),
                        null, Color.White, 0, Vector2.Zero, TextureSize, SpriteEffects.None, 0);

            }


            if (CenterItem)
                batch.DrawString(Font, Text, 
                    Position.Add(TextBuffer).GetThisPosition() - Font.MeasureString(Text)/2, TextColor);
            else 
                batch.DrawString(Font, Text, Position.Add(TextBuffer).GetThisPosition(), TextColor);

        }

         ///<summary>
         ///Access function for members of DisplayItems
         ///TODO finish this
         ///</summary>
        public void AccessItem(MenuItem add, params int[] index) {

            MenuItem testthing;

            for (int i = 0; i < index.Length; ++i) {
                
                //for the things in index add a new thing each time

            }

        }

    }

}
