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

        /* TODO An object to contain a reference to a void function that handles what to do on selection. */
        public delegate void SelectMethod();
        /* The method to run when the item is selected */
        private SelectMethod OnClick;
        /* The position on the screen of the item */
        private Point Position;
        /* The texture to use for this item (not drawn when the menu is selected) */
        private Texture2D Texture;
        /* The background for this menu item */
        private Texture2D Background;
        /* The text for this item */
        private string Text;
        /* The color for this item's text */
        private Color TextColor { get; set; } = Color.Black;
        /* The item's to display on this page */


        /***********************************************************************************************************//**
         * Constructor
         **************************************************************************************************************/
        public MenuItem(Point position, Texture2D texture, string text, SelectMethod selectMethod) {

            Position = position;
            Text = text;
            OnClick = selectMethod;

        }

    }

}
