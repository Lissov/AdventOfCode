var data = File.ReadAllLines("example.txt");
data = File.ReadAllLines("input.txt");

var blocks = SplitData(data).ToArray();
var total = GetReflectionResult(0);
Console.WriteLine("Result for task 1: " + total);
Console.WriteLine("");

total = GetReflectionResult(1);
Console.WriteLine("Result for task 2: " + total);

int GetReflectionResult(int smudges)
{
    int total = 0;
    int n = 0;
    foreach (var block in blocks)
    {
        Console.WriteLine($"Block {++n}");
        var split = 100 * GetHSplit(block, smudges);
        if (split == 0)
        {
            split = GetVSplit(block, smudges);
        }
        Console.WriteLine("Reflection at: " + split);
        total += split;
    }
    return total;
}

int GetHSplit(string[] block, int smudges)
{
    var i = 0;
    while (i < block.Length-1)
    {
        if (GetLinesDiff(block[i], block[i+1]) <= smudges)
        {
            if (IsReflection(block, i, smudges))
                return i+1;
        }
        i++;
    }
    return 0;
}

int GetLinesDiff(string one, string two)
{
    int diff = 0;
    for (int i = 0; i < one.Length; i++)
    {
        if (one[i] != two[i])
        {
            // return if > 1?
            diff++;
        }
    }
    return diff;
}

bool IsReflection(string[] block, int i, int smudges)
{
    var u = i;
    var d = i + 1;
    var s = 0;
    while (u >= 0 && d < block.Length)
    {
        s += GetLinesDiff(block[u], block[d]);
        if (s > smudges) return false;
        u--;
        d++;
    }
    return s == smudges;
}

int GetVSplit(string[] block, int smudges)
{
    var blockR = new string[block[0].Length];
    for (int i = 0; i < block[0].Length; i++)
    {
        string s = "";
        for (int j = 0; j < block.Length; j++) { s += block[j][i]; }
        blockR[i] = s;
    }
    return GetHSplit(blockR, smudges);
}

IEnumerable<string[]> SplitData(string[] data)
{
    var d = new List<string>();
    foreach (string s in data)
    {
        if (string.IsNullOrEmpty(s))
        {
            yield return d.ToArray();
            d = new List<string>();
        }
        else
        {
            d.Add(s);
        }
    }
    yield return d.ToArray();
}