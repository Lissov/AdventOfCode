var input = File.ReadAllLines("input.txt");

var res = 0;
var res2 = 0;

var maxesRGB = new[] { 12, 13, 14 }; // "12 red cubes, 13 green cubes, and 14 blue"
var colors = new[] { "red", "green", "blue" };
for (int i = 0; i < input.Length; i++)
{
    var games = ParseLine(input[i]);
    if (games.All(g => GameMatches(g)))
        res += (i + 1);
    var game = MinGame(games);
    res2 += game[0] * game[1] * game[2];
}

Console.WriteLine("Task 1: " + res);
Console.WriteLine("Task 2: " + res2);

// methods

int[] ParseGame(string game)
{
    var balls = game.Split(",");
    var res = new[] { 0, 0, 0 };
    foreach (var b in balls)
    {
        var s = b.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        int pos = Array.IndexOf(colors, s[1]);
        int count = int.Parse(s[0]);
        res[pos] = count;
    }
    return res;
}

IEnumerable<int[]> ParseLine(string line)
{
    var games = line.Split(':')[1].Split(";");
    return games.Select(g => ParseGame(g));
}

bool GameMatches(int[] game) {
    for (int i = 0; i < 3; i++)
    {
        if (game[i] > maxesRGB[i]) return false;
    }
    return true;
}

int[] MinGame(IEnumerable<int[]> games)
{
    var res = new int[3] { 0, 0, 0 };
    foreach (var game in games)
    {
        for (int i = 0; i < game.Length; i++)
        {
            if (res[i] < game[i])
                res[i] = game[i];
        }
    }
    return res;
}
