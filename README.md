# TV Show

This program is a multi-player quiz with an interactive telecommand. You can build yourself the telecommand with an ESP8266 chip, a RGB Led, an OLED screen and three buttons gathered on a breadboard.

## Gameplay

Each player has his own telecommand and has 10 seconds to answer to a few questions that you can choose. The game begins by an init phase where all players have to click on a button to be registered as participant in the game.

Then the game begins and the questions are printed on a HTML page. You can also visualize the number of the current question and the remaining time on the OLED screen. At each end of question, the LED illuminates red, green or blue according to if you are wrong, right or if you did not answer.

When the game is over, you can visualize the score on the HTML page.

## Setting up

1. Create a new Constellation project in Visual Studio then copy the files Program.cs and QCM.js in your project. You can easily change the questions in QCM.js
* Upload the code found in ManageESP in the ESP from Arduino software.
* Launch the HTML page presenter.html. It has to be in the same folder than style.css and wallpaper.jpeg.
* Finally, run the project on Visual Studio and the game begins !
