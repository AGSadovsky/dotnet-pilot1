//game config
using System.Text;

const int PlayersNumber = 2;
const int TimerDurationMs = 5000;
const int MinWordLength = 8;
const int MaxWordLength = 30;

//read base word
Console.Write($"Enter the base word ({MinWordLength}-{MaxWordLength} chars): ");
string baseWord = Console.ReadLine() ?? string.Empty;

//start game if a word matches input criteria
if (!IsBaseWordValid(baseWord))
{
    Console.WriteLine("The base word does not match the initial criteria or contains forbidden characters. The game did not start");
}
else
{
    Console.WriteLine($"Base word is '{baseWord}'");
    Console.WriteLine($"Each player has {TimerDurationMs / 1000} seconds to enter a word containing letters from the base word only");
    StartGame(baseWord);
}

void StartGame(string validBaseWord)
{
    var uniqueWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { validBaseWord };
        
    var baseWordLetters = SplitBaseWord(validBaseWord.ToLower());

    int currentPlayer = 0;
    bool canContinueGame = true;

    //play until a user enters an incorrect word or the timer is up
    while (canContinueGame)
    {
        currentPlayer = GetNextPLayer(currentPlayer);
        Console.WriteLine($"Player {currentPlayer}, your turn!");

        string userWord = EnterWord();

        canContinueGame = CanBuildNewWord(baseWordLetters, userWord)
            && IsUniqueWord(userWord, uniqueWords)
            && IsWordLengthValid(userWord);
    }

    Console.WriteLine($"Player {currentPlayer}, you lost!");
}

bool IsWordLengthValid(string userWord)
{
    if (userWord.Length <= 1)
    {
        Console.WriteLine("The minimum word length should be more than 1 symbol");
    }

    return userWord.Length > 1;
}

int GetNextPLayer(int currentPlayer)
{
    if (currentPlayer < PlayersNumber)
    {
        currentPlayer++;
    }
    else
    {
        currentPlayer = 1;
    }

    return currentPlayer;
}

bool CanBuildNewWord(List<char> letterCombinations, string userWord)
{
    var letterCounts = new Dictionary<char, int>();

    foreach (char letter in letterCombinations)
    {
        if (letterCounts.ContainsKey(letter))
            letterCounts[letter]++;
        else
            letterCounts[letter] = 1;
    }

    foreach (char letter in userWord.ToLower())
    {
        if (!letterCounts.ContainsKey(letter) || letterCounts[letter] == 0)
        {
            Console.WriteLine("The entered word is incorrect!");
            return false;
        }

        letterCounts[letter]--;
    }

    return true;
}

string EnterWord()
{
    var timer = new System.Timers.Timer(TimerDurationMs);
    bool isElapsed = false;
    timer.AutoReset = false;

    timer.Elapsed += (_, _) =>
    {
        isElapsed = true;
    };

    var inputBuffer = new StringBuilder();

    timer.Start();

    while (!isElapsed)
    {

        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                timer.Stop();
                Console.WriteLine();
                break;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace && inputBuffer.Length > 0)
            {
                inputBuffer.Remove(inputBuffer.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                inputBuffer.Append(keyInfo.KeyChar);
                Console.Write(keyInfo.KeyChar);
            }
        }

        Thread.Sleep(10);
    }

    if (isElapsed)
    {
        Console.WriteLine("Time is up");
    }

    timer.Dispose();

    return inputBuffer.ToString();
}

bool IsUniqueWord(string userWord, HashSet<string> uniqueWords)
{
    bool result = uniqueWords.Add(userWord);

    if (!result)
    {
        Console.WriteLine("The word is not unique!");
    }

    return result;
}

List<char> SplitBaseWord(string word)
{
    return word.ToList();
}

bool IsBaseWordValid(string word)
{
    return word.Length >= MinWordLength
        && word.Length <= MaxWordLength
        && word.All(char.IsLetter);
}
