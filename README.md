# SuperSmashPolls

## Dependencies
In order to build this program and have the result look as intended, please install the font 8 Bit Wonder located [here](http://www.dafont.com/8bit-wonder.font).
Please also install DirectX and XNA Game studio 4.0 if you do not already have those on your system.

## The Game
Super smash polls is a platform fighter in the style of Nintendo's Super Smash Bros, but with the characters from the 2016 election season taking Mario's place. This game was created by William Kluge and Joe Brooksbank.

## How it Works
Much of this project is still experimental, but all collision is done through the Farseer Physics Engine that is currently packaged with this game. The bodies of the levels (that is what player's actually collide with) are created from the vertices of textures, so levels have almost perfect hitboxes. Currently character bodies are created as a rectangle that get's its dimentions from a texture, but that is a temporary fix. Character moves are all handled through delegate functions. For more details on how any of this works, see the comments in the section fo the code you are interested in.
