using DotnetPilot1;
using System.Globalization;
using System.Text;

const int PlayersNumber = 2;
const int TimerDurationMs = 5000;
const int MinWordLength = 8;
const int MaxWordLength = 30;
const string TimeoutSignal = "-1";

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
    Console.WriteLine(Messages.Greeting);
    Console.WriteLine(string.Format(Messages.GameRules, TimerDurationMs / 1000));
    Console.WriteLine("-----------------------------------------\n");
}
        
//read base word
Console.WriteLine(string.Format(Messages.BaseWordEnter, MinWordLength, MaxWordLength));
string baseWord = Console.ReadLine() ?? string.Empty;

//start game if a word matches input criteria
if (!IsBaseWordValid(baseWord))
    Console.WriteLine(Messages.InitialCriteriaError);
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
    bool canBuildNewWord;
    bool isUniqueWord;
    bool isWordLengthValid;


    //play until a user enters an incorrect word or the timer is up
    while (canContinueGame)
    {
        currentPlayer = GetNextPlayer(currentPlayer);
        Console.WriteLine(string.Format(Messages.PlayerTurn, currentPlayer));

        string userWord = EnterWord();

        canBuildNewWord = CanBuildNewWord(baseWordLetters, userWord);
        isUniqueWord = IsUniqueWord(userWord, uniqueWords);
        isWordLengthValid = IsWordLengthValid(userWord);
        
        if (userWord != TimeoutSignal)
        {
            canContinueGame = canBuildNewWord
                && isUniqueWord
                && isWordLengthValid;

            if (!canBuildNewWord)
                Console.WriteLine(Messages.IncorrectWord);
            else if (!isUniqueWord)
                Console.WriteLine(Messages.DuplicateUserWord);
            else if (!isWordLengthValid)
                Console.WriteLine(Messages.InvalidWordLength);
        }
        else
        {
            canContinueGame = false;
            Console.WriteLine($"\n{Messages.TimeIsUp}");
        }
    }

    Console.WriteLine(string.Format(Messages.PlayerLost, currentPlayer));
}

bool IsWordLengthValid(string userWord)
{
    return userWord.Length > 1;
}

int GetNextPlayer(int currentPlayer)
{
    if (currentPlayer < PlayersNumber)
        currentPlayer++;
    else
        currentPlayer = 1;

    return currentPlayer;
}

bool CanBuildNewWord(List<char> baseWordLetters, string userWord)
{
    var letterCounts = new Dictionary<char, int>(30);
    
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
            return false;

        letterCounts[letter]--;
    }

    return true;
}

string EnterWord()
{
    using var timer = new System.Timers.Timer(TimerDurationMs);
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
        return TimeoutSignal;

    return inputBuffer.ToString();
}

bool IsUniqueWord(string userWord, HashSet<string> uniqueWords)
{
    return uniqueWords.Add(userWord);
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