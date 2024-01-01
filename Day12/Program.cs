checked
{
    var data = File.ReadAllLines("example.txt");
    //data = File.ReadAllLines("ex_one.txt");
    data = File.ReadAllLines("input.txt");

    DateTime start = DateTime.Now;
    var total = data.Sum(x => GetCombinations(x));
    Console.WriteLine(total);
    Console.WriteLine((DateTime.Now - start).TotalMilliseconds + "ms");
    Console.ReadLine();

    start = DateTime.Now;
    total = data.Sum(x => GetCombinations(x, unfolded: true));
    Console.WriteLine(total);
    Console.WriteLine((DateTime.Now - start).TotalMilliseconds + "ms");
}


long GetCombinations(string x, bool unfolded = false)
{
    Console.WriteLine("Processing: " + x);
    var pattern = x.Split(' ')[0];
    var numbers = x.Split(" ")[1].Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(x => int.Parse(x)).ToArray();

    if (unfolded)
    {
        var m = new[] { 0, 1, 2, 3, 4 };
        pattern = string.Join('?', m.Select(x => pattern));
        numbers = m.SelectMany(x => numbers).ToArray();
    }
    //while (pattern.Contains("..")) pattern = pattern.Replace("..", ".");

    var res = GetDynamic(pattern, numbers);

    Console.WriteLine("\tFound number: " + res);

    return res;
}

long GetDynamic(string pattern, int[] numbers)
{
    var opts = new long[pattern.Length+1, numbers.Length+1];
    for (int i = 0; i < numbers.Length; i++)
        opts[0, i] = 0;
    opts[0, 0] = 1;
    for (int patLen = 1; patLen <= pattern.Length; patLen++)
    {
        var pt = pattern.Substring(0, patLen);
        opts[patLen, 0] = pt.ToArray().All(x => x != '#') ? 1 : 0;
        for (int num = 1; num <= numbers.Length; num++) {
            var cn = numbers[num-1];
            switch (pattern[patLen-1])
            {
                case '.':
                    opts[patLen, num] = opts[patLen - 1, num];
                    break;
                case '#':
                case '?':
                    var canSet = cn <= patLen
                        && pt.Substring(patLen - cn, cn)
                            .ToArray().All(x => x != '.')
                        && (cn == patLen || pt[patLen - cn - 1] != '#');
                    var ifNot = pattern[patLen-1] == '?';

                    opts[patLen, num] =
                        (canSet ?  opts[patLen == cn ? 0 : patLen - cn - 1, num - 1] : 0)
                        + (ifNot ? opts[patLen - 1, num] : 0);
                    break;
            }
        }
    }
    return opts[pattern.Length, numbers.Length];
}

bool FillPositionsFrom(int[] positions, int[] numbers, int index, int startPos, string pattern)
{
    var strt = startPos;
    var s = strt;
    while (s + numbers[index] <= pattern.Length
        && (s == strt || pattern[s - 1] != '#')
        && !CanPlace(numbers[index], s, pattern)) s++;
    // check no # between start and placement
    if (s > strt && pattern[s - 1] == '#') return false;

    if (s + numbers[index] > pattern.Length) return false;

    positions[index] = s;

    if (index == positions.Length - 1)
    {
        var anyHash = pattern.Substring(positions[index] + numbers[index]).Contains('#');
        if (anyHash)
            return FillPositionsFrom(positions, numbers, index, positions[index] + 1, pattern);
        return true;
    }


    if (FillPositionsFrom(positions, numbers, index + 1, positions[index] + numbers[index] + 1, pattern))
        return true;

    return FillPositionsFrom(positions, numbers, index, positions[index] + 1, pattern);
}

bool CanPlace(int num, int s, string pattern)
{
    if (s > 0 && pattern[s - 1] == '#') return false;
    for (int i = s; i < s + num; i++)
        if (pattern[i] == '.') return false;
    if (s + num < pattern.Length && pattern[s + num] == '#') return false;
    return true;
}
