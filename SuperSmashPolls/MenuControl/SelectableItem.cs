/*******************************************************************************************************************//**
 * @file SelectableItem.cs
 **********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SuperSmashPolls {

    /***************************************************************************************************************//**
     * Intended mainly to be used from the @see MenuController.cs class, the purpose of this class is to allow the printing 
     * of text with colors to represent which item is selected.
     * @deprecated This class will soon not be used when the MenuItem class is implimented.
     ******************************************************************************************************************/
    class SelectableItem {

        /** The Text held within this item */
        private string Text;
        /** The color of the Text */
        public Color TextColor { get; set; }

        /***********************************************************************************************************//**
         * Constructs the SelectableItem class
         **************************************************************************************************************/
        public SelectableItem(string text, Color textColor) {
            this.Text = text;
            this.TextColor = textColor;
        }

    } //END class SelectableItem

}
