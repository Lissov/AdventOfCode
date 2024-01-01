using System.Diagnostics;

var instructions = File.ReadLines("example.txt");
instructions = File.ReadLines("input.txt");

var field = GetField(1000, 1000);
var pos = new Tuple<int, int>(500, 500);

var directionSign = new Dictionary<string, char>
{
    { "SL", 'S' },
    { "SR", 'S' },
    { "SU", 'S' },
    { "SD", 'S' },
    { "LU", 'L' },
    { "LD", 'F' },
    { "RU", 'J' },
    { "RD", '7' },
    { "UR", 'F' },
    { "UL", '7' },
    { "DR", 'L' },
    { "DL", 'J' }
};
var pd = "S";

foreach (var line in instructions)
{
    var s = line.Split(' ');
    var d = s[0];
    var l = int.Parse(s[1]);
    field[pos.Item1, pos.Item2] = directionSign[pd + d];
    var dr = d == "U" ? -1 : d == "D" ? 1 : 0;
    var dc = d == "L" ? -1 : d == "R" ? 1 : 0;
    for (var i = 1; i <= l; i++)
    {
        pos = new Tuple<int, int>(pos.Item1 + dr, pos.Item2 + dc);
        field[pos.Item1, pos.Item2] = dr == 0 ? '-' : '|';
    }
    pd = d;
}
field[pos.Item1, pos.Item2] =
      (field[pos.Item1 - 1, pos.Item2] == '#' && field[pos.Item1, pos.Item2 - 1] == '#') ? 'J'
    : (field[pos.Item1 - 1, pos.Item2] == '#' && field[pos.Item1, pos.Item2 + 1] == '#') ? 'L'
    : (field[pos.Item1 + 1, pos.Item2] == '#' && field[pos.Item1, pos.Item2 - 1] == '#') ? '7'
    : 'F';
//PrintField(field);
var res = GetLagoonSize(field);
Console.WriteLine(res);


var realInstructs = instructions.Select(i => ParseRI(i));


var lines = new List<Line>();
var hlines = new List<HLine>();
pos = new Tuple<int, int>(0, 0);
foreach (var ri in realInstructs)
{
    Console.WriteLine(ri.Item1 + " " + ri.Item2);

    if (ri.Item1 == 'R' || ri.Item1 == 'L')
    {
        var np = pos.Item2 + (ri.Item1 == 'R' ? ri.Item2 : (-ri.Item2));
        var l = lines.Take(hlines.Count - 1)
            .FirstOrDefault(l => l.TopRow <= pos.Item1 && l.BottomRow >= pos.Item1
                && Math.Min(pos.Item2, np) <= l.Col && Math.Max(pos.Item2, np) >= l.Col);
        if (l != null && lines.IndexOf(l) > 0)
        {
            Debugger.Break(); // "Columns cross"
        }
        hlines.Add(new HLine { Row = pos.Item1, LeftCol = Math.Min(pos.Item2, np), RightCol = Math.Max(pos.Item2, np) });
        pos = new Tuple<int, int>(pos.Item1, np);
    } else
    {
        var np = pos.Item1 + (ri.Item1 == 'D' ? ri.Item2 : -ri.Item2);
        var h = hlines.Take(hlines.Count - 1)
            .FirstOrDefault(h => h.LeftCol <= pos.Item2 && h.RightCol >= pos.Item2
                && Math.Min(pos.Item1, np) <= h.Row && Math.Max(pos.Item1, np) >= h.Row);
        if (h != null && hlines.IndexOf(h) > 0)
        {
            Debugger.Break(); // "Rows cross"%
        }
        lines.Add(new Line { TopRow = Math.Min(pos.Item1, np), BottomRow = Math.Max(pos.Item1, np), Col = pos.Item2 });
        pos = new Tuple<int, int>(np, pos.Item2);
    }
    Console.WriteLine(pos.Item1 + " " + pos.Item2);
}

var linesO = lines.OrderBy(x => x.TopRow).ToList();
var linesSplit = new List<Line>();
var linesProgr = new List<Line>();
var firstBottom = 0;
foreach (var l in linesO)
{
    while (linesProgr.Any() && firstBottom <= l.TopRow)
    {
        SplitBy(firstBottom);
        firstBottom = linesProgr.Any() ? linesProgr.Min(l => l.BottomRow) : 0;
    }
    SplitBy(l.TopRow);
    linesProgr.Add(l);
    firstBottom = linesProgr.Any() ? linesProgr.Min(l => l.BottomRow) : 0;
}
while (linesProgr.Any())
{
    SplitBy(firstBottom);
    firstBottom = linesProgr.Any() ? linesProgr.Min(l => l.BottomRow) : 0;
}
checked
{
    long l = 952408144115;
    var blocks = linesSplit.GroupBy(l => l.TopRow).OrderBy(g => g.Key).ToList();
    long res2 = 0;
    for (var bi = 0; bi < blocks.Count; bi++)
    {
        var block = blocks[bi];
        long rb = 0;
        var first = block.First();
        if (block.Any(b => b.BottomRow != first.BottomRow))
            throw new Exception("Not split!");
        
        var pairs = GetColPairs(block);
        foreach (var pair in pairs)
        {
            long oneB = (long)(first.BottomRow - first.TopRow + 1) * (pair.Item2 - pair.Item1 + 1);
            Console.WriteLine($"Rows {first.TopRow} to {first.BottomRow}, Col {pair.Item1} to {pair.Item2}: {oneB}");
            rb += oneB;
        }
        if (bi > 0)
        {
            // add overlapping line
            var top = GetColPairs(blocks[bi - 1]).ToList();
            foreach (var pair in pairs)
            {
                var over = top.Where(tb => tb.Item2 >= pair.Item1 && tb.Item1 <= pair.Item2);
                foreach (var item in over)
                {
                    long overCnt = Math.Min(pair.Item2, item.Item2) - Math.Max(pair.Item1, item.Item1) + 1;
                    Console.WriteLine($"Overlap with top: {overCnt}");
                    rb -= overCnt;
                }
            }
        }
        Console.WriteLine($"Block {rb}");
        res2 += rb;
    }
    Console.WriteLine("Task 2: " + res2);
}

IEnumerable<Tuple<int, int>> GetColPairs(IEnumerable<Line> block)
{
    var ordered = block.OrderBy(b => b.Col).ToList();
    for (int i = 0; i < ordered.Count; i += 2)
        yield return new Tuple<int, int>(ordered[i].Col, ordered[i + 1].Col);
}

void SplitBy(int row)
{
    var left = new List<Line>();
    foreach (var item in linesProgr)
    {
        if (item.TopRow >= row)
            left.Add(item);
        else
        {
            linesSplit.Add(new Line { Col = item.Col, TopRow = item.TopRow, BottomRow = row });
            if (item.BottomRow > row)
                left.Add(new Line { Col = item.Col, TopRow = row, BottomRow = item.BottomRow });
        }
    }
    linesProgr = left;
}


Tuple<char, int> ParseRI(string i)
{
    var p = i.Substring(i.IndexOf('#') + 1);
    p = p.Substring(0, p.Length - 1);
    var l = int.Parse(p.Substring(0, p.Length - 1), System.Globalization.NumberStyles.HexNumber);
    var d = p[p.Length - 1] == '0' ? 'R'
          : p[p.Length - 1] == '1' ? 'D'
          : p[p.Length - 1] == '2' ? 'L'
          : 'U';
    return new Tuple<char, int>(d, l);
}


int GetLagoonSize(char[,] field)
{
    var res = 0;
    for (int r = 0; r < field.GetLength(0); r++)
    {
        var inside = false;
        var opener = 'x';
        for (int c = 0; c < field.GetLength(1); c++)
        {
            switch (field[r, c])
            {
                case '|':
                    inside = !inside;
                    res++;
                    break;
                case '-':
                    res++;
                    break;
                case 'F':
                case 'L':
                case '7':
                case 'J':
                    var oc = "" + opener + field[r, c];
                    if (opener == 'x' || oc == "F7" || oc == "LJ")
                        inside = !inside;
                    opener = (opener == 'x') ? field[r, c] : 'x';
                    res++;
                    break;
                default:
                    if (inside) res++;
                    break;
            }
        }
    }
    return res;
}

static char[,] GetField(int rows, int cols)
{
    var r = new char[rows, cols];
    for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            r[i, j] = '.';
    return r;
}


void PrintField(char[,] field)
{
    for (int i = 0; i < field.GetLength(0); i++)
    {
        for (int j = 0; j < field.GetLength(1); j++)
            Console.Write(field[i, j]);
        Console.WriteLine();
    }
}

[DebuggerDisplay("{Col}, {TopRow} to {BottomRow}")]
class Line
{
    public int TopRow { get; set; }
    public int BottomRow { get; set; }
    public int Col { get; set; }
}

class HLine
{
    public int Row { get; set; }
    public int LeftCol { get; set; }
    public int RightCol { get; set; }
}

