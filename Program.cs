// Projektarbete - Wordle
using System.Text.Json;

class Program
{
    // global properties / game settings
    const int maxGuesses = 6;
    const int wordLength = 5;

    static void Main(string[] args)
    {
        // properties
        string filePath = "wordle.json";
        List<string> words;
        
        // attempt to read the wordlist
        try {
            string jsonString = File.ReadAllText(filePath);
            words = JsonSerializer.Deserialize<List<string>>(jsonString) ?? new List<string>(); // if null return empty list

            // return if the list is empty
            if (words.Count == 0) {
                Console.WriteLine($"The wordlist in {filePath} appears to be empty.");
                return;
            }
        }
        catch (Exception)
        {
            Console.WriteLine($"Failed reading the {filePath} file.");
            return;
        }
        
        // instructions
        Console.Clear();
        Console.CursorVisible = false;
        
        Console.WriteLine("\nWelcome to WORDLE!\n");
        Console.WriteLine("Your goal is to guess a 5-letter word within 6 tries.");
        Console.WriteLine("To help you the letters will be colored green, yellow or white.");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("GREEN ");
        Console.ResetColor();
        Console.WriteLine("means that the letter is correct and in the correct position.");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("YELLOW ");
        Console.ResetColor();
        Console.WriteLine("means that the letter is in the answer but not in the correct position.");
        
        Console.WriteLine("WHITE means the letter is not in the answer at all.\n");

        Console.WriteLine("\nPS. the white color referenced above will be whatever default prompt text color you use.\n\n");
        Console.WriteLine("Press a button to start the game...");

        Console.ReadKey(true);
        game(words); // start the game
    }

    // main game method
    static void game(List<string> words)
    {
        // properties
        List<string> guessedWords = new List<string> {};
        int count = 0;

        // get a random word
        string answer = getWord(words);

        Console.Clear(); // development pre-clear
        renderGame(guessedWords, answer); // write the game board

        // main game loop
        while (true) {
            Console.CursorVisible = true;
            Console.Write("Guess: ");
            string? guess;
            // save the cursor position
            int startX = Console.CursorLeft;
            int startY = Console.CursorTop;

            do
            {
                Console.SetCursorPosition(startX, startY); // move the cursor to the saved position
                Console.Write(new string(' ', Console.WindowWidth)); // fill the entire width with empty spaces
                Console.SetCursorPosition(startX, startY); // move the cursor back to the saved position

                guess = Console.ReadLine();
            } while (guess == null || guess.Length != wordLength || !guess.All(char.IsLetter));

            guess = guess.ToUpper();
            guessedWords.Add(guess ?? "");
            count++;

            renderGame(guessedWords, answer); // write the game board 
            checkStatus(guess ?? "", answer, count, words); // check if the guess was correct
        }
    }

    static void renderGame(List<string> guessedWords, string answer)
    {
        Console.Clear();

        string[] letters = new string[30]; // initialize an array with 30 empty strings

        // fill the array with empty spaces
        Array.Fill(letters, " ");

        // replace empty spaces with letters of the guessed words
        int index = 0;
        foreach (var word in guessedWords)
        {
            foreach (var letter in word)
            {
                letters[index] = letter.ToString();
                index++;
            }
        }
        
        Console.WriteLine("╔═══╦═══╦═══╦═══╦═══╗");

        for (int i = 0; i < letters.Length; i++)
        {
            

            // if we're at the end of a word that isn't the last
            if (i % wordLength == 0 && i != 0) 
            {
                Console.WriteLine("║");
                Console.WriteLine("╠═══╬═══╬═══╬═══╬═══╣");
            }

            Console.Write("║ "); // before every letter

            // find all letters who exists in words
            if (answer.Contains(letters[i]))
            {
                Console.ForegroundColor = (letters[i] == answer[i % wordLength].ToString()) ? ConsoleColor.Green : ConsoleColor.Yellow;
            }
            
            Console.Write(letters[i]);
            Console.ResetColor();
            Console.Write(" ");
        }

        Console.WriteLine("║");
        Console.WriteLine("╚═══╩═══╩═══╩═══╩═══╝\n");
    }
    
    // check if the guessed word was correct or if the game is over
    static void checkStatus(string guess, string answer, int count, List<string> words)
    {
        if (guess == answer) Console.WriteLine("\nCongratulations you have won!\n");
        else if (count >= maxGuesses)
        {
            Console.WriteLine("\nYou're out of tries and have lost!\n");
            Console.WriteLine($"The correct answer was: {answer}\n");
        }
        else return;

        Console.WriteLine("1. Play again");
        Console.WriteLine("2. Exit\n");

        Console.CursorVisible = false;

        while (true)
        {
            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.D1:
                    game(words);
                    return;

                case ConsoleKey.D2:
                    Environment.Exit(0);
                    return;

                default:
                    Console.WriteLine("Please press '1' or '2'");
                    break;
            }
        }
    }

    // method to get a random word from a list of words
    static string getWord(List<string> words)
    {
        Random random = new Random();
        int randomNumber = random.Next(1, words.Count);
        return words[randomNumber].ToUpper();
    }
}