using System;
using System.Collections.Generic;


namespace LEA
{
    public class Player
    {
        #region Properties

        private string Name { get; }

        private string Color { get; }

        private Stack<char> TypedText { get; }

        private int CurErrors { get; set; }

        private int TotalErrors { get; set; }

        private Race CurrentRace { get; }


        #endregion


        public Player(string name, string color, Race currentRace)
        {
            Name        = name;
            Color       = color;
            CurrentRace = currentRace;
            TypedText   = new Stack<char>(CurrentRace.Text.Length);
            TotalErrors = 0;
            CurErrors   = 0;
            
        }


        private void HandleCorrectChar(char enteredChar)
        {
            TypedText.Push(enteredChar);
            Console.Write($"{Fg.White}{enteredChar}{Fg.Reset}");
        }

        private int WordsPerMinute(Race currentRace){
            DateTime endOfRace = DateTime.Now;
            int timeInSeconds = Convert.ToInt32((endOfRace - CurrentRace.StartOfRace).TotalSeconds);
            int wordsPerMinute = (((currentRace.Text.Length/5)/timeInSeconds)/60);
            
            return wordsPerMinute;
        }

        private void HandleFalseChar(char enteredChar)
        {
            TypedText.Push(enteredChar);

            if (enteredChar == ' ')
            {
                Console.Write($"{Bg.Red}{enteredChar}{Bg.Reset}");
            }
            else
            {
                Console.Write($"{Fg.Red}{enteredChar}{Fg.Reset}");
            }
            ++CurErrors;
            ++TotalErrors;
        }


        private void HandleBackspace()
        {
            if (TypedText.Count == 0)
            {
                return;
            }

            if (TypedText.Peek() != CurrentRace.Text[TypedText.Count - 1])
            {
                --CurErrors;
            }

            TypedText.Pop();

            try
            {
                Console.Write($"\b{Fg.BrightBlack}{CurrentRace.Text[TypedText.Count]}{Fg.Reset}\b");
            }
            catch (IndexOutOfRangeException)
            {
                Console.Write("\b \b");
            }
        }


        private void HandleKeyPress(ConsoleKeyInfo enteredKey)
        {
            var enteredChar = enteredKey.KeyChar;

            if (enteredKey.Key == ConsoleKey.Backspace)
            {
                HandleBackspace();
            }
            // Fix: Exception when typing past the end of the text
            else if (enteredChar == CurrentRace.Text[TypedText.Count])
            {
                HandleCorrectChar(enteredChar);
            }
            else
            {
                HandleFalseChar(enteredChar);
            }
        }


        private bool HasCompletedText()
        {
            return TypedText.Count == CurrentRace.Text.Length && CurErrors == 0;
        }


        public void TypeText()
        {
            Console.Write($"{Fg.BrightBlack}{CurrentRace.Text}\r");

            while (!HasCompletedText())
            {
                var enteredKey = Console.ReadKey(true);
                HandleKeyPress(enteredKey);
            }

            Console.WriteLine(WordsPerMinute(CurrentRace));
        }
    }
}