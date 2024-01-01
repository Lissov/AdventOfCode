int a = 1500000000;
Console.WriteLine(a + a);
Console.WriteLine(checked(a + a));

var input = File.ReadAllLines("example.txt");
input = File.ReadAllLines("input.txt");

var directions = input[0];
var map = GetMap(input);
long n = GetPathLength("AAA", p => p == "ZZZ").Item1;
Console.WriteLine("Task 1: " + n);


input = File.ReadAllLines("example2.txt");
input = File.ReadAllLines("inputM.txt");
directions = input[0];
map =  GetMap(input);

n = 0;
string[] starts = map.Keys.Where(k => k.EndsWith("A")).ToArray();
var lengths = starts.Select(p => GetPathLength(p, x => x.EndsWith("Z")))
    .Select(x => new { toOne = x.Item1, circle = GetPathLength(x.Item2, q => q == x.Item2) })
    .ToList();

long x = MinCommonDividerE(lengths.Select(x => x.toOne));

Console.WriteLine("Task 2: " + x);

Dictionary<string, Tuple<string, string>> GetMap(string[] input)
{
    var map = new Dictionary<string, Tuple<string, string>>();
    for (int i = 2; i < input.Length; i++)
        map[input[i].Substring(0, 3)] = new Tuple<string, string>(input[i].Substring(7, 3), input[i].Substring(12, 3));
    return map;
}

Tuple<long, string> GetPathLength(string start, Func<string, bool> condition)
{
    long n = 0;
    string p = start;
    while (n == 0 || !condition(p))
    {
        var d = directions[(int)(n % directions.Length)];
        p = d == 'L' ? map[p].Item1 : map[p].Item2;
        n++;
    }
    return new Tuple<long, string>(n, p);
}


long MinCommonDividerE(IEnumerable<long> data)
{
    var dividers = data.Select(d => GetDividers(d));
    var all =
        dividers.SelectMany(d => d.Keys).Distinct()
            .ToDictionary(k => k,
                          k => dividers.Select(c => c.ContainsKey(k) ? 1 : 0).Max());
    long r = 1;
    foreach (var k in all.Keys)
    {
        r = r * (long)Math.Pow(k, all[k]);
    }
    return r;
}

Dictionary<long, int> GetDividers(long one)
{
    var x = one;
    var i = 2;
    var d = new Dictionary<long, int>();
    while (x > 1)
    {
        if (x % i == 0)
        {
            d[i] = d.ContainsKey(i) ? d[i] + 1 : 1;
            x = x / i;
        }
        else i++;
    }
    return d;
}
