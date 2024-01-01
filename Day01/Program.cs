var file = File.ReadAllLines("input.txt");

var res = 0;

foreach (var line in file)
{
    var one = line.SideNumber(isLeft: true);
    var two = line.SideNumber(isLeft: false);
    res += one * 10 + two;
}

Console.WriteLine(res);

public static class Extensions
{
    private static Dictionary<string, int> numbersMap = new Dictionary<string, int>() {
        { "0", 0 },
        { "zero", 0 },
        { "1", 1 },
        { "one", 1 },
        { "2", 2 },
        { "two", 2 },
        { "3", 3 },
        { "three", 3 },
        { "4", 4 },
        { "four", 4 },
        { "5", 5 },
        { "five", 5 },
        { "6", 6 },
        { "six", 6 },
        { "7", 7 },
        { "seven", 7 },
        { "8", 8 },
        { "eight", 8 },
        { "9", 9 },
        { "nine", 9 },
    };

    public static int SideNumber(this string line, bool isLeft)
    {
        var keys = numbersMap.Select(x => x.Key);

        var i = isLeft ? 0 : line.Length - 1;
        while (i < line.Length && i >= 0)
        {
            var rest = line.Substring(i);
            var key = keys.FirstOrDefault(k => rest.StartsWith(k));
            if (key != null)
            {
                return numbersMap[key];
            }
            i = isLeft ? i+1 : i-1;
        }
        throw new Exception("Not found");
    }
}