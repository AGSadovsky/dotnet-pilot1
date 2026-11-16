//game config
using dotnet_pilot1;
using System.Globalization;
using System.Text;

const int PlayersNumber = 2;
const int TimerDurationMs = 5000;
const int MinWordLength = 8;
const int MaxWordLength = 30;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

bool isLanguageSelected = false;
while (!isLanguageSelected)
{
    Console.WriteLine(Messages.ChooseLang);
    Console.Write("> ");
    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ChangeLanguage("en-US");
            isLanguageSelected = true;
            break;
        case "2":
            ChangeLanguage("ru-RU");
            isLanguageSelected = true;
            break;
        default:
            Console.WriteLine("Invalid input\n");
            continue;
    }

    Console.WriteLine("-----------------------------------------");
    Console.WriteLine(string.Format(Messages.Greeting));
    Console.WriteLine(string.Format(Messages.GameRules, TimerDurationMs / 1000));
    Console.WriteLine("-----------------------------------------\n");
}
        
//read base word
Console.WriteLine(string.Format(Messages.BaseWordEnter, MinWordLength, MaxWordLength));
string baseWord = Console.ReadLine() ?? string.Empty;

//start game if a word matches input criteria
if (!IsBaseWordValid(baseWord))
{
    Console.WriteLine(string.Format(Messages.InitialCriteriaError));
}
else
{
    Console.WriteLine(string.Format(Messages.BaseWordInfo, baseWord));
    StartGame(baseWord);
}

static void ChangeLanguage(string cultureName)
{
    var culture = new CultureInfo(cultureName);

    Thread.CurrentThread.CurrentCulture = culture;
    Thread.CurrentThread.CurrentUICulture = culture;

    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
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
        Console.WriteLine(string.Format(Messages.PlayerTurn, currentPlayer));

        string userWord = EnterWord();

        if (userWord != "-1")
            canContinueGame = CanBuildNewWord(baseWordLetters, userWord)
                && IsUniqueWord(userWord, uniqueWords)
                && IsWordLengthValid(userWord);
        else
            canContinueGame = false;
    }

    Console.WriteLine(string.Format(Messages.PlayerLost, currentPlayer));
}

bool IsWordLengthValid(string userWord)
{
    if (userWord.Length <= 1)
    {
        Console.WriteLine(string.Format(Messages.InvalidWordLength));
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

bool CanBuildNewWord(List<char> baseWordLetters, string userWord)
{
    var letterCounts = new Dictionary<char, int>();

    foreach (char letter in baseWordLetters)
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
            Console.WriteLine(string.Format(Messages.IncorrectWord));
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
        Console.WriteLine($"\n{string.Format(Messages.TimeIsUp)}");
        return "-1";
    }

    timer.Dispose();

    return inputBuffer.ToString();
}

bool IsUniqueWord(string userWord, HashSet<string> uniqueWords)
{
    bool result = uniqueWords.Add(userWord);

    if (!result)
    {
        Console.WriteLine(string.Format(Messages.UserWordIsNotUnique));
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