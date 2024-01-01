var raw = File.ReadAllLines("input.txt");
//var raw = File.ReadAllLines("example.txt");
var dataL = new List<string>();

var width = raw[0].Length + 2;
var height = raw.Length + 2;
string border = "";
for (int i = 0; i < width; i++) border += ".";
dataL.Add(border);
dataL.AddRange(raw.Select(x => "." + x + "."));
dataL.Add(border);

var data = dataL.ToArray();

var result = 0;

for (var row = 1; row < height-1; row++)
{
    var val = 0;
    var hasSym = false;
    for (var col = 0; col < width; col++)
    {
        var thisSym = (GetTypeI(row - 1, col) == SymType.Symbol || GetTypeI(row + 1, col) == SymType.Symbol);
        if (thisSym)
            hasSym = true;
        switch (GetTypeI(row, col))
        {
            case SymType.Symbol:
            case SymType.Gear:
                hasSym = true;
                result += val;
                val = 0;
                break;
            case SymType.Dot:
                if (hasSym) result += val;
                val = 0;
                hasSym = thisSym;
                break;
            case SymType.Digit:
                val = val * 10 + (data[row][col] - '0');
                break;
            default:
                break;
        }
    }
}

Console.WriteLine(result);

long resGears = 0;
var dataC = new char[height, width];
for (int r = 0; r < height; r++)
{
    for (int c = 0; c < height; c++)
        dataC[r, c] = data[r][c];
}

for (var row = 1; row < height-1; row++)
{
    for (var col = 1; col < width - 1; col++)
    {
        if (GetTypeC(row, col) == SymType.Gear)
        {
            resGears += GetGear(row, col);
        }
    }
}

Console.WriteLine(resGears);

long GetGear(int row, int col)
{
    int qIndex = 0;
    var queue = new List<Tuple<int, int>> { new Tuple<int, int>(row, col) };
    while (qIndex < queue.Count)
    {
        ProcessNext(queue, ref qIndex);
    }
    var area = ExtractArea(queue);
    if (area.gearCount != 1) return 0; // invalid gears

    var numbers = GetNumbers(area).ToList();
    return numbers.Count == 2
        ? numbers[0]* numbers[1]
        : 0;
}

IEnumerable<long> GetNumbers(AreaInfo area)
{
    var num = -1;
    for (int row = area.minRow; row <= area.maxRow; row++)
    {
        for (int col = area.minCol; col <= area.maxCol; col++)
        {
            var c = area.area[row, col];
            switch (GetType(c))
            {
                case SymType.Digit:
                    if (num == -1) num = 0;
                    num = 10 * num + (c - '0');
                    break;
                case SymType.Dot:
                case SymType.Symbol:
                case SymType.Gear:
                    if (num > 0)
                        yield return num;
                    num = 0;
                    break;

            }
        }
    }
}

AreaInfo ExtractArea(List<Tuple<int, int>> queue)
{
    var area = new char[height, width];
    var gears = 0;
    foreach (var item in queue)
    {
        if (GetTypeC(item.Item1, item.Item2) == SymType.Gear) gears++;
        area[item.Item1, item.Item2] = dataC[item.Item1, item.Item2];
        dataC[item.Item1, item.Item2] = '.'; // to avoid reprocessing
    }
    return new AreaInfo
    {
        area = area,
        gearCount = gears,
        minRow = queue.Min(x => x.Item1),
        maxRow = queue.Max(x => x.Item1),
        minCol = queue.Min(x => x.Item2),
        maxCol = queue.Max(x => x.Item2)
    };
}

void ProcessNext(List<Tuple<int, int>> queue, ref int qIndex)
{
    var c = queue[qIndex];
    switch (GetTypeC(c.Item1, c.Item2)) {
        case SymType.Digit:
        case SymType.Symbol: // how should it be?
        case SymType.Gear: // we will just clear the area
            PushAround(queue, c.Item1, c.Item2);
            break;
        case SymType.Dot: // dots break the area
            break;
    }
    qIndex++;
}

void PushAround(List<Tuple<int, int>> queue, int row, int col)
{
    Action<int, int> push = (r, c) =>
    {
        if (!queue.Any(item => item.Item1 == r && item.Item2 == c))
            queue.Add(new Tuple<int, int>(r, c));
    };
    push(row - 1, col-1);
    push(row - 1, col);
    push(row - 1, col+1);
    push(row, col-1);
    push(row, col+1);
    push(row + 1, col-1);
    push(row + 1, col);
    push(row + 1, col+1);
}

SymType GetTypeI(int row, int col)
{
    var c = data[row][col];
    return GetType(c);
}
SymType GetTypeC(int row, int col)
{
    var c = dataC[row, col];
    return GetType(c);
}

SymType GetType(char c)
{
    if (char.IsDigit(c)) return SymType.Digit;
    if (c == '.') return SymType.Dot;
    if (c == '*') return SymType.Gear;
    return SymType.Symbol;
}

enum SymType { Digit, Dot, Symbol, Gear }

struct AreaInfo
{
    public char[,] area;
    public int minRow, maxRow, minCol, maxCol;
    public int gearCount;
}
