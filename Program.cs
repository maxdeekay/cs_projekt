// Projektarbete - Wordle
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        // properties
        string filePath = "wordle.json";
        List<string> words;
        List<string> guessedWords = new List<string> {};
        int count = 0;
        
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

        // randomze a correct word
        Random random = new Random();
        int randomNumber = random.Next(1, words.Count);
        string answer = words[randomNumber].ToUpper();

        Console.Clear(); // development pre-clear
        Console.WriteLine("Correct Answer: " + answer);
        renderGame(guessedWords); // write the game board

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
            } while (guess == null || guess.Length != 5 || !guess.All(char.IsLetter));

            guess = guess.ToUpper();
            guessedWords.Add(guess ?? "");
            count++;

            renderGame(guessedWords); // write the game board 
            Console.WriteLine("Answer: " + answer);
            Console.WriteLine("Count: " + count);
            checkStatus(guess ?? "", answer, count); // check if the guess was correct
        }
    }

    static void renderGame(List<string> guessedWords)
    {
        Console.Clear();

        string[] letters = new string[30]; // initialize an array with 30 empty strings

        // fill the array with empty spaces
        for (int i = 0; i < letters.Length; i++)
        {
            letters[i] = " ";
        }

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
            if (i % 5 == 0 && i != 0) 
            {
                Console.WriteLine("║");
                Console.WriteLine("╠═══╬═══╬═══╬═══╬═══╣");
            }
            
            Console.Write($"║ {letters[i]} ");
        }

        Console.WriteLine("║");
        Console.WriteLine("╚═══╩═══╩═══╩═══╩═══╝");
    }
    
    // check if the guessed word was correct or if the game is over
    static void checkStatus(string guess, string answer, int count)
    {
        if (guess != answer && count < 6) return;

        if (guess == answer) Console.WriteLine("\nCongratulations you have won!\n");
        if (guess != answer && count == 6) Console.WriteLine("\nYou're out of tries and have lost!\n");
        
        Console.WriteLine($"The correct answer was: {answer}\n");
        Console.WriteLine("1. Play again");
        Console.WriteLine("2. Exit");

        Console.CursorVisible = false;
        int input = (int) Console.ReadKey(true).Key;
    }
}