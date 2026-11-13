//game config
using System.Text;

int confPlayersNum = 3;
int confTimer = 5000;
int confWordMinLength = 8;
int confWordMaxLength = 30;
char[] confForbiddenChars = [' ', ',', '.', '!', '?', '&'];

//read base word
Console.Write($"Enter the base word ({confWordMinLength}-{confWordMaxLength} chars): ");
var word = Console.ReadLine();

//start game if a word matches input criteria
if (!isBaseWordLengthCorrect(word, confWordMinLength, confWordMaxLength, confForbiddenChars))
{
    Console.WriteLine("The base word does not match the initial criteria or contains forbidden characters. The game did not start");
}
else
{
    Console.WriteLine($"Base word is '{word}'");
    Console.WriteLine($"Each player has {confTimer / 1000} seconds to enter a word containing letters from the base word only");
    gameStart();
}

void gameStart()
{
    var uniqueWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    uniqueWords.Add(word);

    var letterCombinations = splitBaseWord(word.ToLower());

    int currentPlayer = 0;
    bool flag = true;

    //play until a user enters an incorrect word or the timer is up
    while (flag)
    {
        currentPlayer = selectPlayer(currentPlayer);
        Console.WriteLine($"Player {currentPlayer}, your turn!");

        string userWord = wordEnter();

        flag = canBuildNewWord(letterCombinations, userWord)
            && isUniqueWord(userWord, uniqueWords)
            && isCorrectWord(userWord);
    }

    Console.WriteLine($"Player {currentPlayer}, you lost!");
}

bool isCorrectWord(string userWord)
{
    return userWord.Length > 1;
}

int selectPlayer(int currentPlayer)
{
    if (currentPlayer < confPlayersNum)
    {
        currentPlayer++;
    }
    else
    {
        currentPlayer = 1;
    }

    return currentPlayer;
}

static bool canBuildNewWord(List<char> letterCombinations, String userWord)
{
    var letterCounts = new Dictionary<char, int>();//create HashMap

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
            return false;

        letterCounts[letter]--;
    }

    return true;
}

string wordEnter()
{
    var timer = new System.Timers.Timer(confTimer);
    bool isElapsed = false;
    timer.AutoReset = false;

    timer.Elapsed += (sender, e) =>
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

static bool isUniqueWord(String userWord, HashSet<string> uniqueWords)
{
    return uniqueWords.Add(userWord);
}

List<char> splitBaseWord(String word)
{
    List<char> charList = new List<char>(30);
    charList = word.ToList();

    return charList;
}

static bool isBaseWordLengthCorrect(String word, int minChars, int maxChars, char[] forbiddenChars)
{
    return word.Length >= minChars && word.Length <= maxChars && !word.AsSpan().ContainsAny(forbiddenChars);
}
