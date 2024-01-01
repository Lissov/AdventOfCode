using System.Diagnostics;

var lines = File.ReadAllLines("example.txt");
lines = File.ReadAllLines("example2.txt");
lines = File.ReadAllLines("example3.txt");
lines = File.ReadAllLines("input.txt");

//var field = new char[lines.Length, lines[0].Length];
var start = GetStart(lines);
var visited = new bool[lines.Length, lines[0].Length];
visited[start.Item1, start.Item2] = true;

var pos = GetStartDirection();

while (lines[pos.r][pos.c] != 'S')
{
    pos = MakeNextStep(pos);
}
Console.WriteLine(pos.length / 2);



// todo: write algorythm here
lines[start.Item1] = lines[start.Item1].Replace('S', 'L');

var res = new List<string>();
for (int i = 0; i < lines.Length; i++)
{
    var s = "";
    for (int j = 0; j < lines[i].Length; j++)
        s += visited[i, j] ? lines[i][j] : ' ';
    res.Add(s);
}
File.WriteAllLines("ex3.txt", res);

int inside = 0;
foreach (var line in res)
{
    string x = line;
    x = ReplaceAll(x, "-", "");
    x = ReplaceAll(x, "FJ", "|");
    x = ReplaceAll(x, "L7", "|");
    x = ReplaceAll(x, "F7", "");
    x = ReplaceAll(x, "LJ", "");
    var isInside = false;
    for (int j = 0;j < x.Length; j++)
    {
        if (x[j] == '|') isInside = !isInside;
        if (x[j] == ' ' && isInside) inside++;
        if (x[j] != '|' && x[j] != ' ')
            throw new Exception("Unexpected");
    }
}
Console.WriteLine(inside);

inside = 0;
for (int r = 0; r < lines.Length; r++)
{
    var isinside = false;
    var opener = 'x';
    for (int c = 0; c < lines[r].Length; c++)
    {
        if (visited[r, c])
        {
            if ("-".Contains(lines[r][c]))
                continue;
            var t = lines[r][c];
            if (("" + opener + t) == "FJ" || ("" + opener + t) == "L7")
            {
                opener = 'x';
                // keep isinside, because we didn't cross the line
            }
            else
            {
                isinside = !isinside;
                opener = t;
            }
        } else
        {
            if (isinside)
            {
                inside++;
            }
        }
    }
}
Console.WriteLine(inside);

string ReplaceAll(string source, string toReplace, string with)
{
    while (source.Contains(toReplace))
        source = source.Replace(toReplace, with);
    return source;
}
Step MakeNextStep(Step pos)
{
    visited[pos.r, pos.c] = true;
    switch (pos.direction)
    {
        case 'R':
            switch (lines[pos.r][pos.c])
            {
                case '-': return new Step (pos.r, pos.c + 1, 'R', pos.length + 1);
                case 'J': return new Step (pos.r - 1, pos.c, 'U', pos.length + 1);
                case '7': return new Step (pos.r + 1, pos.c, 'D', pos.length + 1);
            }
            break;
        case 'L':
            switch (lines[pos.r][pos.c])
            {
                case '-': return new Step (pos.r, pos.c - 1, 'L', pos.length + 1);
                case 'L': return new Step (pos.r - 1, pos.c, 'U', pos.length + 1);
                case 'F': return new Step (pos.r + 1, pos.c, 'D', pos.length + 1);
            }
            break;
        case 'U':
            switch (lines[pos.r][pos.c])
            {
                case '|': return new Step(pos.r - 1, pos.c, 'U', pos.length + 1);
                case '7': return new Step(pos.r, pos.c - 1, 'L', pos.length + 1);
                case 'F': return new Step(pos.r, pos.c + 1, 'R', pos.length + 1);
            }
            break;
        case 'D':
            switch (lines[pos.r][pos.c])
            {
                case '|': return new Step(pos.r + 1, pos.c, 'D', pos.length + 1);
                case 'J': return new Step(pos.r, pos.c - 1, 'L', pos.length + 1);
                case 'L': return new Step(pos.r, pos.c + 1, 'R', pos.length + 1);
            }
            break;
        default:
            break;
    }
    throw new Exception("Inconsistent");
}

Step GetStartDirection()
{
    if (start.Item2 < lines[0].Length - 1 &&
        "-J7".Contains(lines[start.Item1][start.Item2 + 1]))
        return new Step(start.Item1, start.Item2 + 1, 'R', 1);
    if (start.Item2 > 0 &&
        "-FL".Contains(lines[start.Item1][start.Item2 - 1]))
        return new Step(start.Item1, start.Item2 - 1, 'L', 1);
    // no matter
    return new Step (start.Item1 - 1, start.Item2, 'U', 1 );
}



// Don't need :)

/*while (qi < queue.Count)
{
    var pos = queue[qi];
    var p = lines[pos.Item1][pos.Item2];
    var curL = lengths[pos.Item1, pos.Item2];
    switch (p)
    {
        case '|':
            GoTo(pos, curL, "U");
            GoTo(pos, curL, "D");
            break;
        case '-':
            GoTo(pos, curL, "R");
            GoTo(pos, curL, "L");
            break;
        case 'J':
            GoTo(pos, curL, "U");
            GoTo(pos, curL, "L");
            break;
        case 'L':
            GoTo(pos, curL, "U");
            GoTo(pos, curL, "R");
            break;
        case 'F':
            GoTo(pos, curL, "R");
            GoTo(pos, curL, "D");
            break;
        case '7':
            GoTo(pos, curL, "L");
            GoTo(pos, curL, "D");
            break;
        case 'S':
            GoTo(pos, curL, "U");
            GoTo(pos, curL, "D");
            GoTo(pos, curL, "R");
            GoTo(pos, curL, "L");
            break;
        default:
            break;
    }
    qi++;
}

void GoTo(Tuple<int, int> pos, int curL, string direction)
{
    Console.WriteLine($"Going from {pos} to {direction}.");
    switch (direction)
    {
        case "U":
            AddToQueue(pos.Item1 - 1, pos.Item2, curL, "7F|");
            return;
        case "D":
            AddToQueue(pos.Item1 + 1, pos.Item2, curL, "JL|");
            return;
        case "R":
            AddToQueue(pos.Item1, pos.Item2 + 1, curL, "-7J");
            return;
        case "L":
            AddToQueue(pos.Item1, pos.Item2 - 1, curL, "-LF");
            return;
        default:
            throw new NotImplementedException();
    }
}

void AddToQueue(int r, int c, int curL, string v2)
{
    if (lengths[r, c] != 0)
    {
        if (lengths[r, c] == curL + 1)
        {
            Console.WriteLine($"{lengths[r, c]} at {r}:{c}");
        } else
        {
            Console.WriteLine($"Inconsistent {lengths[r, c]} or {curL + 1} at {r}:{c}");
        }
        return;
    }
    var newC = lines[r][c];
    if (v2.Contains(newC))
    {
        lengths[r, c] = curL + 1;
        queue.Add(new Tuple<int, int> ( r, c ) );
        Console.WriteLine($"Went to {r}:{c} in {curL + 1}.");
    } else
    {
        Console.WriteLine($"Can't go to {r}:{c}");
    }
}*/

Tuple<int, int> GetStart(string[] lines)
{
    for (int i = 0; i < lines.Length; i++)
    {
        var c = lines[i].IndexOf("S");
        if (c >= 0)
            return new Tuple<int, int>(i, c);
    }
    throw new Exception("Start not found");
}

// classes 
[DebuggerDisplay("{r}:{c} going {direction} in {length}")]
class Step
{
    public int r;
    public int c;
    public char direction;
    public int length;
    public Step(int r, int c, char direction, int length)
    {
        this.r = r;
        this.c = c;
        this.direction = direction;
        this.length = length;
    }
}
