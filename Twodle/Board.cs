using System.Media;
using System.Windows;

namespace Twodle
{
    // This is the Board class
    internal class Board
    {
        //-------- varibles --------
        private string word;
        enum Color {Gray, Yellow, Green};
        private bool solved = false;
        private int rowSolved = 10;
        
        //------- Constructor --------
        public Board(string w)
        {
            word = w.ToUpper();
           //MessageBox.Show(word); //for testing purpose to display the correct words
        }

        //------- Getter functions for the varibales ----------
        public bool Solved { get { return solved; } }

        public string Word { get { return word; } }
        public int RowSolved { get { return rowSolved; } }

        //------- Methods ---------

        // This function check if the user input is equal to the generated word and store the information into an
        // integer array
        public int[] check(string guess, int row)
        {
            int[] acccuracy = { 0, 0, 0, 0, 0 };
            // The guess is fully equal to the word
            if (guess.Equals(this.word))
            {
                solved= true;
                rowSolved = row;
                return new int[5] { 2, 2, 2, 2, 2 };
			}
            else // Guess is paritaly equal to the word
            {
                for (int i = 0; i < 5; i++)
                {
                    // If letter is in the correct position
                    if (guess[i] == word[i])
                    {
                        acccuracy[i] = 2;
                    }
                    // If letter is in the word but not correct position
                    else if (word.Contains(guess[i].ToString()))
                    {
                        acccuracy[i] = 1;
                    }
                    // If letter is not in the word
                    else
                    {
                        acccuracy[i] = 0;
                    }
                }
            }
            return acccuracy;
        }
    }
}