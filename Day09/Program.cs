var input = File.ReadAllLines("example.txt");
input = File.ReadAllLines("input.txt");

var res = input.Select(ParseLine).Select(GetDiffs).Select(x => x.Sum()).Sum();

Console.WriteLine(res);

var rx = input.Select(ParseLine).Select(GetLeftDiffs).Select(RevDiff);
var res2 = input.Select(ParseLine).Select(GetLeftDiffs).Select(RevDiff).Sum();

Console.WriteLine(res2);

int[] ParseLine(string line)
{
    return line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
}

int[] GetDiffs(int[] line)
{
    var res = new int[line.Length];
    Array.Copy(line, res, line.Length);
    for (int max = line.Length - 1; max >= 0; max--)
    {
        for (int i = 0; i < max; i ++)
        {
            res[i] = res[i+1] - res[i];
        }
    }
    return res;
}
int[] GetLeftDiffs(int[] line)
{
    var res = new int[line.Length];
    Array.Copy(line, res, line.Length);
    for (int min = 1; min < line.Length; min++)
    {
        for (int i = line.Length - 1; i >= min; i--)
        {
            res[i] = res[i] - res[i-1];
        }
    }
    return res;
}

int RevDiff(int[] x)
{
    var m = 1;
    var e = 0;
    for (int i = 0; i < x.Length; i++)
    {
        e += x[i] * m;
        m *= -1;
    }
    return e;
}

