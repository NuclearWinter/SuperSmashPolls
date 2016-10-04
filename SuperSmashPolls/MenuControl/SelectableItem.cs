﻿/********************************************************************************************************************************************//**
 * @file SelectableItem.cs
 ***********************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SuperSmashPolls {

/********************************************************************************************************************************************//**
 * Intended mainly to be used from the @see MenuController.cs class, the purpose of this class is to allow the printing of text with
 * colors to represent which item is selected.
 ***********************************************************************************************************************************************/
    class SelectableItem {

        /** The text held within this item */
        private string text;
        /** The color of the text */
        private Color textColor;

/********************************************************************************************************************************************//**
 * Constructs the SelectableItem class
 ***********************************************************************************************************************************************/
        public SelectableItem(string text, Color textColor) {
            this.text = text;
            this.textColor = textColor;
        }


/********************************************************************************************************************************************//**
 * Gets the text from this item
 ***********************************************************************************************************************************************/
        public string Text {

            get {
                return text;
            }

        }

/********************************************************************************************************************************************//**
 * Gets and sets the color of the text.
 ***********************************************************************************************************************************************/
        public Color TextColor {

            get {
                return textColor;
            }

            set {
                textColor = value;
            }

        }

    } //END class SelectableItem

}
