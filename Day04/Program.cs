var input = File.ReadAllLines("example.txt");
input = File.ReadAllLines("input.txt");

long res = 0;
foreach (var line in input)
{
    int wincount = GetWinCount(line);
    var pts = wincount == 0 ? 0 : (long)Math.Pow(2, wincount - 1);
    res += pts;
}
Console.WriteLine(res);

var cards = new int[input.Length];
for (int i = 0; i < input.Length; i++) cards[i] = 1;
for (int cn = 0; cn < input.Length; cn++)
{
    var winning = GetWinCount(input[cn]);
    for (var l = cn + 1; l <= cn + winning; l++)
        cards[l] += cards[cn];
}
var total = cards.Sum();
Console.WriteLine(total);

static int GetWinCount(string line)
{
    var winning = line.Split('|')[0]
            .Split(':')[1]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => int.Parse(x))
            .ToList();
    var selected = line.Split('|')[1]
        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Select(x => int.Parse(x))
        .ToList();
    var wincount = selected.Count(s => winning.Contains(s));
    return wincount;
}

