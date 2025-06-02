using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Timers;
using System.Media;

namespace Twodle
{
    public partial class MainWindow : Window
    {   
        //-------- declaring variables ----------
        // The word list that are being generated for the game
        string[] words= { "apple","bento","cries","dials","eagle","flail","goose","happy","ideal", "junky","knife","legal","mouse","noise",
                          "ocean","prong","quart","radio","snuck","thumb","udder","viola","wager","xerox","yummy","zilch", 
                          "brisk", "charm", "drill", "event", "frost", "glide", "hatch", "inbox", "jaunt", "kneel",
                          "latch", "mirth", "noble", "orbit", "piano", "quilt", "raven", "slope", "trace", "unite",
                          "vivid", "whale", "xenon", "yacht", "zebra", "actor", "blush", "climb", "drape", "ember",
                          "fetch", "grasp", "hinge", "ivory", "jelly", "knack", "lemon", "mango", "nudge", "opine",
                          "plume", "quick", "rumor", "spend", "troop", "unzip", "vouch", "wrist", "yield", "zesty"};
        Board board1;
        Board board2;
        string guess="";
        TextBox[,] attempts;
        TextBox[,] attempts2;
        int row = 0;
        Thread t1;
        Thread t2;
        SoundPlayer woo;
        SoundPlayer clap;
        SoundPlayer boo;
        public MainWindow()
        {
            // Initializing declared vairables
            InitializeComponent();
            board1 = new Board(words[new Random().Next(words.Length)]);
            board2 = new Board(words[((new Random().Next(words.Length))+30)%words.Length]);

            attempts = new TextBox[7,5] { {One1, One2, One3, One4, One5 }, 
                                          {Two1, Two2, Two3, Two4, Two5 },
                                          {Three1, Three2, Three3, Three4, Three5 },
                                          {Four1, Four2, Four3, Four4, Four5 },
                                          {Five1, Five2, Five3, Five4, Five5 },
                                          {Six1, Six2, Six3, Six4, Six5 },
                                          {Seven1, Seven2, Seven3, Seven4, Seven5} };
            attempts2 = new TextBox[7, 5] { {One1_2, One2_2, One3_2, One4_2, One5_2 },
                                          {Two1_2, Two2_2, Two3_2, Two4_2, Two5_2 },
                                          {Three1_2, Three2_2, Three3_2, Three4_2, Three5_2 },
                                          {Four1_2, Four2_2, Four3_2, Four4_2, Four5_2 },
                                          {Five1_2, Five2_2, Five3_2, Five4_2, Five5_2 },
                                          {Six1_2, Six2_2, Six3_2, Six4_2, Six5_2 },
                                          {Seven1_2, Seven2_2, Seven3_2, Seven4_2, Seven5_2} };
            woo = new SoundPlayer("woohoo.wav");
            clap = new SoundPlayer("clap.wav");
            boo = new SoundPlayer("boo.wav"); 
        }

        // This function is used to update the guess string with the letters that being pressed/clicked by the user
        // and display the letter to the textbox for that attempt
        private void letterClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string letter = (string)(sender as Button).Content;
                //if the length of guess is over 5 letters, nothing will be updated to the guess string.
                if (guess.Length >= 5 || (ehMode.IsChecked == true && (sender as Button).Background == Brushes.Gray))
                {
                    return;
                }
                //check if the board have been solved or not, if not update the textbox with the letter
                if (!board1.Solved) { attempts[row, guess.Length].Text = letter; }
                if (!board2.Solved) { attempts2[row, guess.Length].Text = letter; }

                guess = guess.Insert(guess.Length, letter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // When enter is clicked this function will update the UI with the correct information for the letters
        // that were used in the attempt
        private async void enter_Click(object sender, RoutedEventArgs e)
        {
            //check if the length of the word is equal to 5, if not tell the user to enter a 5 letter word
            if (guess.Length < 5)
            {
                MessageBox.Show("Please enter a 5 letter word");
                return;
            }

            // Threading start to excute the two boards concurrently
            int[] result, result2 = new int[5];
            if (!board1.Solved)
            {
                result = board1.check(guess,row);
                t1 = new Thread(() => interpret(result, attempts));
                t1.Start();
            }
            else
            {
                result = new int[5];
            }

            if (!board2.Solved)
            {
                result2 = board2.check(guess,row);
                t2 = new Thread(() => interpret(result2, attempts2));
                t2.Start();
            }
            
            // If one board is solved, play a sound effects to tell the player that they have solve a board
            if ((board1.Solved && board1.RowSolved==row)||( board2.Solved && board2.RowSolved == row))
            {
                woo.Play();
            }
            keyboardboth(result, result2);
            row++;
            guess = "";
            
            await Task.Delay(5 * 300);

            // End the game if both boards are solved or they reached to the last attempet to make guess
            if ((board1.Solved && board2.Solved) || row == 7) // want this to run after the animation is done
            {
                endGame();
            }
        }

        // This function delete a letter at the end of the word with Delete/Backspace is pressed/clicked
        private void delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if the guess length is more than 0, if not nothing can happen
                if (guess.Length > 0)
                {
                    guess = guess.Remove(guess.Length - 1);
                    attempts[row, guess.Length].Text = "";
                    attempts2[row, guess.Length].Text = "";
                }
            }
            catch (IndexOutOfRangeException ex) // Handle error if delete an empty string
            {
                MessageBox.Show($"Index Error: {ex.Message}");
            } 
            catch (Exception ex) // Handle any other errors
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // This function take an integer array to check if the accuracy of each letters, and change the 
        // background color of the textbox to the associated color and position
        private void interpret(int[] accuracy, TextBox[,] a)
        {
            SolidColorBrush b;
            // Go through the array to change the color of each textbox
            for(int i = 0; i < 5; i++)
            {
                if (accuracy[i] == 2)
                {
                    b = Brushes.Green;
                }
                else if (accuracy[i] == 1)
                {
                    b = Brushes.Yellow;
                }
                else
                {
                    b = Brushes.Gray;
                }
                animate(i,b,a);
                Thread.Sleep(300);
                Dispatcher.Invoke(() =>
                {
                    a[row - 1, i].Margin = new Thickness(2);
                });
            }
            
            // End the threads
            t1.Abort();
            t2.Abort();
        }

        // This fucntion animate the textbox, when they change color enlarge the box
        private void animate(int i, SolidColorBrush b, TextBox[,] a)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {

                    a[row - 1, i].Background = b;
                    a[row - 1, i].Margin = new Thickness(0);
                });
            }
            catch (IndexOutOfRangeException ex) // Handle Error for Index out of Bound
            {
                MessageBox.Show($"Index Error: {ex.Message}");
            }
            catch (Exception ex) // Handle any other error
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // This fucntion change the background color of the keyboard to show the user the accuracy of the letters
        private void keyboardboth(int[] accuracy, int[] accuracy2)
        {
            for (int i = 0; i < 5; i++)
            {
                string btnName = guess[i].ToString().ToLower();
                Button btn = (Button)this.FindName(btnName);

                // Change the background color of the keyboard accordingly 
                if ((!board1.Solved && accuracy[i] == 2) || (!board2.Solved && accuracy2[i]==2))
                {
                    btn.Background = Brushes.Green;
                }
                else if (((!board1.Solved && accuracy[i] == 1) || (!board2.Solved && accuracy2[i]==1)) && (btn.Background!=Brushes.Green))
                {
                    btn.Background = Brushes.Yellow;
                }
                else if (((!board1.Solved && accuracy[i] == 0) || (!board2.Solved && accuracy2[i] == 0)) && (btn.Background != Brushes.Green && btn.Background !=Brushes.Yellow))
                {
                    btn.Background = Brushes.Gray;
                }
            }
        }

        // This function end the game when either both boards are solved or the user hit the last attempt to guess
        private void endGame()
        {
            // Plays a clap sound effects that tells the user they have solved both boards
            if(board1.Solved && board2.Solved)
            {
                clap.Play();
            }
            else // Plays a boo sound effects if the user have not solve both boards 
            {
                boo.Play();
            }
            
            // Shows the result of the game and display the correct words
            MessageBoxResult result=MessageBox.Show("You " + ((board1.Solved && board2.Solved) ? "won!" : $"lost :( \nThe correct words were \'{board1.Word}\' and '{board2.Word}'")+"\n Would you like to play again?", "Game over!", MessageBoxButton.YesNo);
            
            // Ask the user if they want to play again or not
            if (result == MessageBoxResult.Yes)
            {
                newGame();
            }
            else
            {
                MessageBox.Show("Thank you for playing!");
                System.Windows.Application.Current.Shutdown();
            }
        }

        // This function generate a new game for the player
        private void newGame()
        {
            // Generate new words
            board1 = new Board(words[new Random().Next(words.Length)]);
            board2 = new Board(words[((new Random().Next(words.Length)) + 30) % words.Length]);
            row = 0;

            // Clear the background color of the keyboard
            for (int i = 0; i < attempts.GetLength(0); i++)
            {
                for (int j = 0; j < attempts.GetLength(1); j++)
                {
                    string btnName = attempts[i,j].Text.ToLower();
                    Button btn = (Button)this.FindName(btnName);
                    if (btn != null)
                    {
                        btn.ClearValue(Button.BackgroundProperty);
                    }

                    attempts[i,j].Text = "";
                    attempts[i,j].ClearValue(TextBox.BackgroundProperty);
                }
            }
            for (int i = 0; i < attempts2.GetLength(0); i++)
            {
                for (int j = 0; j < attempts2.GetLength(1); j++)
                {
                    string btnName = attempts2[i, j].Text.ToLower();
                    Button btn = (Button)this.FindName(btnName);
                    if (btn != null)
                    {
                        btn.ClearValue(Button.BackgroundProperty);
                    }

                    attempts2[i, j].Text = "";
                    attempts2[i, j].ClearValue(TextBox.BackgroundProperty);
                }
            }
        }

        // This function find the button that are pressed on the physical keyboard and return the name of the button
        private Button Find_Button(Key key)
        {
            Button btn = null;
            if (key >= Key.A && key <= Key.Z)
            {
                btn = (Button)this.FindName($"{key.ToString().ToLower()}");
            }
            else if (key == Key.Back)
            {
                btn = (Button)this.FindName("back");
            }
            else if (key == Key.Enter)
            {
                btn = (Button)this.FindName("enter");
            }
            return btn;
        }

        // This function calls the Click functions according to which button is click on the keyboard
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Button btn = Find_Button(e.Key); ;
            if (btn != null)
            {
                if (e.Key == Key.Back)
                {
                    delete_Click(btn, e);
                }
                else if (e.Key == Key.Enter)
                {
                    enter_Click(btn, e);
                }
                else
                {
                    letterClick(btn, e);
                }
            }
        }
    }
}
