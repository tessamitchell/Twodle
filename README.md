<h3>Overview</h3>
We created a game called Twodle inspired by Wordle. Twodle is a word-guessing application that we implemented in C# with WPF, and it allows the player to play two Wordle games concurrently. It randomly selects two 5-letter words for the word lists, which allows the player to guess the secret words with a maximum of 8 attempts. After each guess, it provides visual feedback to the player to show which letters are correct based on the existence and positions of those letters in the secret words. 

<h3>Usage Instructions</h3>
To run the game, you can simply run it through Visual Studio, or by opening the game application in Twodle/bin/Debug/Twodle.exe. 

Once the player opens the game, they can use either the keyboard on the screen or the physical keyboard on their device to enter the word they want to guess. The player then clicks enter, and each letter of the guess will be revealed as green, yellow, or gray.  Green means the letter is correct and in the right spot, yellow means the letter is correct but in the wrong spot, and gray means the letter is not in the word at all.  Use this information to influence your next guess.  Each guess will be played on both boards at the same time. Once one of the two boards is solved there will be a visual display on board with all boxes in green and a “woohoo” sound effect that tells the player that they have solved one of the words. When both boards are solved, there will be clap sound effects that congratulate the player for solving both boards, and an option to play again. 

There are two modes for the game: Easy mode and Hard mode. The game defaults to the easy mode. To enable the hard mode, players simply click the checkbox at the top right corner of the screen. 
<ul>
<li>Easy: This mode allows the player to play Twodle normally, and be able to use the letters that are not in the words in previous guesses. </li>
<li>Hard: This mode gives the player some challenges by not allowing the player to use any of the letters that were used in previous guesses and not in the words. </li>
</ul>

<h3>Test cases</h3>
<h5>Two positive test cases:</h5>
<ul>
<li>The player correctly entered a 5-letter word and clicked enter to check whether the guess is correct or not.
  <ul>
    <li>The word should appear on the screen, with each letter filling a separate box</li>
    <li>After ‘enter’ is pressed, each box will change color to green, yellow, or gray, depending on its location/existence in the target word.</li>
    <li>Row is incremented and any further typing will be inputted in the next row down, and a new guess is started.</li>
  </ul>
</li>
<li>The player correctly guessed both words and ended the game before they reached the last attempt. 
  <ul>
  <li>The endGame function is called, with a celebratory clapping sound being played.</li>
  <li>The player now has the choice to play a new game or close the application.</li>
  <li>If a new game is selected, then the board and keyboard gets reset, new target words are generated and the game starts over.</li>
  <li>If the player chooses not to play again, a popup thanks the player, and upon closing the popup, the game window exits.</li>
  </ul>
  </li>
</ul>

<h5>Two negative test cases:</h5>
<ul>
<li>The player enters a word that is less than 5 letters. 
  <ul>
    <li>The application will have the message box pop up to tell the player to correctly enter a 5-letter word guess. The player can select ‘Okay’ to close the message box and nothing else will happen until the player presses a button other than ‘Enter’
    </li>
  </ul>
</li>
<li>The player can not delete when there are not more letters in the attempt, and the player is not allowed to press more letters when there are 5 letters in the attempt. 
  <ul>
    <li>Nothing happens.  The function call does nothing and the game remains idle until a different button is pressed.</li>
    <li>Any letters pressed when the guess is already five letters are ignored and do not alter the input.  The game remains idle until a different, non-letter button is pressed.</li>
  </ul>
</li>
</ul>

<h3>Acknowledgements</h3>
<h6>Library used</h6>
<ul>
<li>Thread (System.Threading)</li>
<li>Media (System.Media)</li>
<li>Timer (System.Timers)  </li>
</ul>

