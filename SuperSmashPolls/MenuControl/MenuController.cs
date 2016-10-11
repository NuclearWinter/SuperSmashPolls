/*******************************************************************************************************************//**
 * @file MenuController.cs
 **********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperSmashPolls {

    /***************************************************************************************************************//**
     * Class to handle user interaction with menus.
     * TODO This class should only be handling instances of the MenuItem class and should not be declaring positions or
     * values of menu items.
     * @note This class will work best if menu items can themself contain sub-menus and can take over menu control when
     * selected. If it is done like this the only thing needed is to control an array of menu items and one 
     * (the title screen) is loaded by default, but is at its core the same as any sub-menus (with the obvious exception 
     * that sub-menus can move backward in the array and the main menu can't, which needs to be dealt with).
     * @note TL;DR This class displays stuff, handles selection of stuff, but should not contain any information of what
     * that stuff actually is.
     * @warning Right now this class is not set up this way
     ******************************************************************************************************************/
    class MenuController {

        /* The font to use on menus */
        private SpriteFont menuFont;
        /* Space between items on the menu */
        private readonly int itemSpacing;
        /* The space to use before writing words */
        private readonly int leftMargin;
        /* The height to start drawing items at */
        private readonly int startHeight;
        /* Color for the background */
        private Color backgroundColor;
        /* Color for text */
        private readonly Color fontColor;
        /* Color to use for text when that item is selected */
        private Color selectedColor;
        /* Holds the items to be placed on screen from the main menu */
        private readonly List<SelectableItem> mainMenuItems = new List<SelectableItem>();
            

        /** Controls which menu the player is currently on */
        enum Menus {

            MainScreen,       //The main screen of the game
            PauseScreen,      //The screen to use when the player pauses the game while playing
            OptionsMenu,      //Shows all the different options to be changed
            MultiplayerMenu,  //The multiplayer interface
            SinglePlayerMenu //The single player gamemode selection screen

        }
        /* Dictates which menu to display */
        private Menus currentMenu = Menus.MainScreen;

        /* Text to use to display the option to start single player */
        private static string singlePlayerOptionText = "Single Player";
        private static string highScoreMenuText = "High Scores";
        private static string quitText = "Exit";

        /************************************************************************************************************//**
         * Constructs the MenuController class.
         * @note Any items to menu lists need to be added here.
         **************************************************************************************************************/
        public MenuController(int itemSpacing, int leftMargin, int startHeight, Color backgroundColor, Color fontColor, 
            Color selectedColor) {
            this.itemSpacing = itemSpacing;
            this.leftMargin = leftMargin;
            this.startHeight = startHeight;
            this.backgroundColor = backgroundColor;
            this.fontColor = fontColor;
            this.selectedColor = selectedColor;

            mainMenuItems.Add(new SelectableItem(singlePlayerOptionText, selectedColor));
            mainMenuItems.Add(new SelectableItem("Multiplayer (not implimented)", fontColor));
            mainMenuItems.Add(new SelectableItem(highScoreMenuText, fontColor));
            mainMenuItems.Add(new SelectableItem("Options (not implimented)", fontColor));
            mainMenuItems.Add(new SelectableItem(quitText, fontColor));

        }

        /***********************************************************************************************************//**
         * Sets the selectable texture.
         * Intended to be used to set the texture from within content loading
         **************************************************************************************************************/
        public void SetMenuFont(SpriteFont newFont) {

            menuFont = newFont;

        }

        /***********************************************************************************************************//**
         * Updates the open menu.
         * @note Tested and working.
         **************************************************************************************************************/
        private void DrawList(List<SelectableItem> toDraw, SpriteBatch gameSpriteBatch) {

            int currentHeight = startHeight;

            foreach (var item in toDraw) {
                
                gameSpriteBatch.DrawString(menuFont, item.Text, new Vector2(leftMargin, currentHeight), item.TextColor);

                currentHeight += itemSpacing;

            }

        }

        /***********************************************************************************************************//**
         * Finds which item is currently selected.
         * @return The index of the currently selected item or -1 if a selected item could not be found.
         **************************************************************************************************************/
        private int FindSelected(List<SelectableItem> workList) {

            for (int i = 0; i < workList.Count; ++i) {
                //Finds which item is currently selected
                if (workList[i].TextColor != selectedColor) continue;

                return i;

            }

            return -1;

        }

        /***********************************************************************************************************//**
         * Updates the menus text color to be the newly selected item.
         **************************************************************************************************************/
        private void MoveSelectaed(bool up, List<SelectableItem> workList) {

            int currentSelected = FindSelected(workList); 

            if (up && currentSelected != 0) {

                workList[currentSelected].TextColor = fontColor;

                workList[currentSelected - 1].TextColor = selectedColor;

            } else if (!up && currentSelected != workList.Count - 1) {

                workList[currentSelected].TextColor = fontColor;

                workList[currentSelected + 1].TextColor = selectedColor;

            }

        }

        /***********************************************************************************************************//**
         * Determines if the desired item is currently selected.
         **************************************************************************************************************/
        private bool IsSelected(List<SelectableItem> workList, string find) {

            int currentlySelected = FindSelected(workList);

            return find == workList[currentlySelected].Text;

        }

        /***********************************************************************************************************//**
         * Updates the open menu.
         **************************************************************************************************************/
        private int lastSecondUpdated = 0;

        public void UpdateOpenMenu(PlayerIndex player) {

            DateTime now = DateTime.Now;

            if (Math.Abs(now.Millisecond - lastSecondUpdated) <= 100) return; //Only updates the menu ten times a second

            lastSecondUpdated = now.Millisecond;

            if (GamePad.GetState(player).DPad.Up == ButtonState.Pressed)
                MoveSelectaed(true, mainMenuItems);
            else if (GamePad.GetState(player).DPad.Down == ButtonState.Pressed)
                MoveSelectaed(false, mainMenuItems);

        }

        /***********************************************************************************************************//**
         * Determines if the game should be switched to single player mode.
         **************************************************************************************************************/
        public bool StartSinglePlayer(PlayerIndex player) {

            if (IsSelected(mainMenuItems, singlePlayerOptionText)) {

                return GamePad.GetState(player).Buttons.A == ButtonState.Pressed;

            }

            return false;

        }

        /***********************************************************************************************************//**
         * Determines if the game should be quit.
         * TODO make a child class that contains specific functions like these.
         **************************************************************************************************************/
        public bool QuitGame(PlayerIndex player) {

            if (IsSelected(mainMenuItems, quitText)) {

                return GamePad.GetState(player).Buttons.A == ButtonState.Pressed;

            }

            return false;

        }

        /***********************************************************************************************************//**
         * Finds if the player has selected to open the high score menu.
         * TODO make a child class that contains specific functions like these.
         **************************************************************************************************************/
        public bool OpenHighScore(PlayerIndex player) {

            if (IsSelected(mainMenuItems, highScoreMenuText)) {

                return GamePad.GetState(player).Buttons.A == ButtonState.Pressed;

            }

            return false;

        }

        /***********************************************************************************************************//**
         * Draws the open menu
         **************************************************************************************************************/
        public void DrawOpenMenu(SpriteBatch gameSpriteBatch) {

            switch (currentMenu) {
                    
                case Menus.MainScreen: {
                    
                    DrawList(mainMenuItems, gameSpriteBatch);

                    break;

                } case Menus.OptionsMenu: {
                    //TODO this
                    break;

                } case Menus.SinglePlayerMenu: {
                    //TODO this
                    break;

                } case Menus.MultiplayerMenu: {
                    //TODO this
                    break;

                } case Menus.PauseScreen: {
                    //TODO this
                    break;

                } default: {
                    //TODO this
                    break;

                }
                
            }

        }

    } //END MenuController

} //END namespace WindowsGameTest
