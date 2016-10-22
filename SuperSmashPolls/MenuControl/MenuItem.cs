﻿/*******************************************************************************************************************//**
 * @file MenuItem.cs
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
     * Holds the different commands that the menu might need to send into the main game
     ******************************************************************************************************************/
    enum MenuCommands {
        
        Nothing,     //The game should do nothing
        StartGame,   //The game should start
        MultiplayerMenu,
        SingleplayerMenu,
        BackToMainMenu, //Go back to the previous menu
        ExitGame

    }

    /***************************************************************************************************************//**
     * TODO This class
     * This class handles all properties of an item on a menu.
     * One very important object is the void function reference to be ran when the item is selected. This function needs
     * to be declared and defined elsewhere, and when the item is constructed it must be passed as an argument.
     * @note To create multi-leveled menus you must construct items from bottom to top.
     ******************************************************************************************************************/
    class MenuItem {
        /** Position of this item on the screen */
        private WorldUnit Position;
        /** Buffer to move text over from Position */
        private readonly WorldUnit TextBuffer;
        /** Text for this item (drawn on top of the screen if displaying the item's menu */
        public string Text;
        /** Determines if the item is highlihtable or not */
        public readonly bool Highlightable;
        /** Whether or not to center the text */
        public readonly bool CenterText;
        /* The command to use if clicked on */
        public readonly MenuCommands Command;

        /* Anything below here must be loaded after the constructor */

        /** Color for this item's text */
        private Color TextColor { get; set; } = Color.Black;
        /** Font to use for this item */
        private SpriteFont Font { get; set; } = null;
        /** Texture to use as the background for this item (not drawn if displaying the item's menu) */
        private Texture2D Texture { get; set; } = null;
        /** Background for this menu item (if the item is selected). @note This will cover the entire screen
         * @warning This must be set during load content */
        private Texture2D Background { get; set; } = null;

        /* Anything below here is for if this item can be selected to display another menu */

        /** The item's to display on this page */
        public List<MenuItem> ContainedItems = new List<MenuItem>();
        /** Holds if the item has it's own menu that it can display */
        private readonly bool HasSubmenu;
        /** Overlay this item's sub-menu on top of the current menu */
        private bool SubOverlay { get; set; } = false;
        /* The item within ContainedItems to draw instead of this one (-1 means draw this one) */
        public int DrawDown = -1;

        /* The item on screen that is currently highlighted */
        private int CurrentHighlightedItem = 0;
        /* Holds the last time the menu was updates */
        private int LastTimeUpdated = 0;

        /***********************************************************************************************************//**
         * Constructor
         * @param position The position of the item on the screen.
         * @param text The text of this item.
         * @param hasSubmenu Whether or not this item has a menu below it to navigate to.
         * @param textBuffer An amount to displace the text past the start of the item (to center with backgrounds)
         * @param centerText Whether or not to center the text (if there is no texture it will center on the point, 
         * otherwise it will center on the texture).
         * @param highlightable Whether or not this item is highlightable
         **************************************************************************************************************/
        public MenuItem(WorldUnit position, string text, bool hasSubmenu, WorldUnit textBuffer, bool highlightable, 
            bool centerText = false, MenuCommands command = MenuCommands.Nothing) {
            Position      = position;
            Text          = text;
            HasSubmenu    = hasSubmenu;
            TextBuffer    = textBuffer;
            Highlightable = highlightable;
            CenterText = centerText;
            Command       = command;
        }

        /***********************************************************************************************************//**
         * Add an item to display when this screen is being drawn.
         * @param position The position of this item on the screen.
         * @param hasSubmenu Whether or not this item has a submenu
         * @param texture The texture of this item (to show behind text)
         * @param text The text to show on this item
         **************************************************************************************************************/
        public void AddItem(MenuItem addItem) {

            ContainedItems.Add(addItem);

        }


        /***********************************************************************************************************//**
         * Sets the font for all the items in this menu
         **************************************************************************************************************/
        public void SetFontForAll(SpriteFont font) {

            Font = font;

            foreach (MenuItem i in ContainedItems) {
                i.SetFontForAll(font);
                i.Font = font;
            }

        }

        /***********************************************************************************************************//**
         * Update control of the menu.
         * @param controllingPlayer The player to control the menu
         * @return If the A button was pressed and a next menu is not being navigated to.
         * @note This is done to allow for enumerator values to be changed
         **************************************************************************************************************/
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

        /***********************************************************************************************************//**
         * Display the items in ContainedItems and other menu items.
         * This is for if this item has a menu that it can display.
         **************************************************************************************************************/
        public void DisplayMenu(SpriteBatch batch) {

            //Will draw this menu alone or with another menu on top of it
            if (DrawDown == -1 || ContainedItems[DrawDown].SubOverlay) {

                if (Background != null)
                    batch.Draw(Background, new Vector2(0, 0), Color.White);

                foreach (MenuItem i in ContainedItems)
                    i.DisplayItem(batch);

            }

            if (DrawDown != -1) {
                
                ContainedItems[DrawDown].DisplayMenu(batch);

            }

        }


        /***********************************************************************************************************//**
         * Display this item.
         * This is for when this item is displayed (on a different menu).
         * @param batch The SpriteBatch to draw with
         * @warning This method assumes that the spritebatch has already been started.
         **************************************************************************************************************/
        public void DisplayItem(SpriteBatch batch) {
            
            if (Texture != null)
                batch.Draw(Texture, Position.GetThisPosition(), Color.White);

            if (CenterText)
                batch.DrawString(Font, Text, 
                    Position.Add(TextBuffer).GetThisPosition() - Font.MeasureString(Text)/2, TextColor);
            else 
                batch.DrawString(Font, Text, Position.Add(TextBuffer).GetThisPosition(), TextColor);

        }

    }

}
