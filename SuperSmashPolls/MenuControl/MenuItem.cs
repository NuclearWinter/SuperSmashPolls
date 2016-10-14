/*******************************************************************************************************************//**
 * @file MenuItem.cs
 **********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperSmashPolls.MenuControl {

    /***************************************************************************************************************//**
     * TODO This class
     * This class handles all properties of an item on a menu.
     * One very important object is the void function reference to be ran when the item is selected. This function needs
     * to be declared and defined elsewhere, and when the item is constructed it must be passed as an argument.
     ******************************************************************************************************************/
    class MenuItem {
        /** The position on the screen of the item */
        private Point Position;
        /** The texture to use for the background on this item (not drawn when the menu is selected) */
        private Texture2D Texture;
        /** The text for this item */
        private string Text;
        /** The color for this item's text */
        private Color TextColor { get; set; } = Color.Black;
        /** Overlay this item's sub-menu on top of the current menu */
        private bool SubOverlay { get; set; } = false;
        /** The background for this menu item (if the item is selected) */
        private Texture2D Background { get; set; } = null;
        /** The item's to display on this page */
        private List<MenuItem> ContainedItems;

        /***********************************************************************************************************//**
         * Constructor
         **************************************************************************************************************/
        public MenuItem(Point position, Texture2D texture, string text) {

            Position = position;
            Texture = texture;
            Text = text;

        }

        /***********************************************************************************************************//**
         * Add an item to display when this screen is being drawn.
         * @param position The position of this item on the screen.
         * @note Position must be in terms of WorldUnit Ratio.
         * @param texture The texture of this item (to show behind text)
         * @param text The text to show on this item
         **************************************************************************************************************/
        public void AddItem(Point position, string text = "", Texture2D texture = null) {

            ContainedItems.Add(new MenuItem(position, texture, text));

        }

        /***********************************************************************************************************//**
         * Display the items in ContainedItems and other menu items.
         * This is for if this item has a menu that it can display.
         **************************************************************************************************************/
        public void DisplayMenu() {
            


        }


        /***********************************************************************************************************//**
         * Display this item.
         * This is for when this item is displayed (on a different menu).
         **************************************************************************************************************/
        public void DisplayItem() {
            


        }

    }

}
