using System;
using System.Collections.Generic;
using System.Linq;


namespace LEA
{
    public class Player : Participant
    {
        private List<Competitor> _competitors;
        private int              _curErrors;
        private Stack<char>      _typedText;

        #region Properties

        public int CurErrors
        {
            get => _curErrors;
            set => _curErrors = value;
        }

        public Stack<char> TypedText
        {
            get => _typedText;
            set => _typedText = value;
        }

        #endregion


        public Player(string name, string color, Race currentRace) : base(name, color, currentRace)
        {
            CurErrors = 0;
            TypedText = new Stack<char>(CurrentRace.Text.Length);
        }


        public override int GetProgress()
        {
            double correctChars = TypedText.Count;

            return Convert.ToInt32(100 * (correctChars / CurrentRace.Text.Length));
        }


        /// <summary>
        /// Add the char to TypedText and write the char to the console in the appropriate color
        /// </summary>
        /// <param name="enteredChar">The char read from the players console</param>
        private void HandleCorrectChar(char enteredChar)
        {
            TypedText.Push(enteredChar);
            Console.Write($"{Fg.White}{enteredChar}{Fg.Reset}");
        }


        /// <summary>
        /// Add the char to TypedText, increment CurErrors and TotalErrors and write the char to the console in the appropriate color
        /// </summary>
        /// <param name="enteredChar">The char read from the players console</param>
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


        /// <summary>
        /// Delete the most recently entered char from TypedText and the console while decrementing CurErrors if the removed char was an error
        /// </summary>
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


        public override bool HasCompletedText()
        {
            return TypedText.Count == CurrentRace.Text.Length && CurErrors == 0;
        }


        /// <summary>
        /// Read a char from the console and handle it appropriately
        /// </summary>
        /// <param name="enteredKey">The key read from the players console</param>
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


        /// <summary>
        /// A frame fragment is a 'race lane' in the client's view of the race<para />
        /// 
        /// <para>Returns:</para>
        ///  A frame fragment to include in a clients view
        /// </summary>
        /// <param name="progress">The progress as an integer percentage</param>
        /// <param name="color">A string containing the escape sequence for the players color</param>
        /// <param name="name">A players name</param>
        /// <returns>A frame fragment to include in a clients view</returns>
        private static string CreateFrameFragment(int progress, string color, string name)
        {
            var indentation = new string(' ', progress);

            var indentedCar = string.Join(Environment.NewLine,
                                          new[] {indentation, indentation, indentation}
                                             .Zip(Car.Model.Split('\n'),
                                                  (a, b) => string.Join("", a, b)
                                                 )
                                         );

            return $"{name}\n{color}{indentedCar}{Fg.Reset}";
        }


        /// <summary>
        /// Collect all participants frame data, construct the frame fragments and merge them<para/>
        /// <para>Returns:</para>
        /// A constructed frame ready to be drawn
        /// </summary>
        /// <param name="participantData">A participants current race data encoded into a string</param>
        /// <returns>A constructed frame ready to be drawn</returns>
        private string CreateCompleteRaceFrame(List<string> participantData)
        {
            string frame = CreateFrameFragment(GetProgress(), Color, Name);

            foreach (string dataPoint in participantData)
            {
                List<string> data     = dataPoint.Split(";").ToList();
                int          progress = Convert.ToInt32(data[0]);
                string       color    = data[1];
                string       name     = data[2];
                frame += CreateFrameFragment(progress, color, name) + "\n";
            }

            return frame;
        }


        /// <summary>
        /// Let a player type the given text and handle their keypresses
        /// </summary>
        public override void TypeText()
        {
            Console.Write($"{Fg.BrightBlack}{CurrentRace.Text}\r");

            while (!HasCompletedText())
            {
                var enteredKey = Console.ReadKey(true);
                HandleKeyPress(enteredKey);
                Console.WriteLine(CreateCompleteRaceFrame(CurrentRace.CollectPlayerData()));
            }

            CurrentRace.CompletionOrder.Add(this);
        }
    }
}