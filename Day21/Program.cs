using System.Text;

checked
{
    var data = File.ReadAllLines("example.txt");
    //data = File.ReadAllLines("ex3.txt");
    data = File.ReadAllLines("input.txt");
    int stepCount = 26501365;

    var start = GetStart(data);
    int[,] dt = ParseData(data);

    var reach = GetReachMap(dt, new List<Tuple<int, int, int>> { new Tuple<int, int, int>(start.Item1, start.Item2, 0) });
    var count = GetReachableCount(reach.reachMap, stepCount);
    Console.WriteLine("Task 1: " + count);

    /*var ressb = new StringBuilder();
    int[,] segs = GetReachSegments(reach.reachMap, 11, stepCount);
    for (int r = 0; r < segs.GetLength(0); r++) {
        for (int c = 0; c < segs.GetLength(1); c++)
            ressb.Append(segs[r, c] + "\t");
        ressb.AppendLine();
    }
    File.WriteAllText("C:/temp/aoc.txt", ressb.ToString());

    int[,] GetReachSegments(int[,] reachMap, int scnt, int stepCount)
    {
        var cnt = reachMap.GetLength(0) / scnt;
        var res = new int[cnt, cnt];
        for (int r = 0; r < cnt; r++)
            for (int c = 0; c < cnt; c++)
            {
                var subtile = new int[scnt, scnt];
                for (int rr = 0; rr < scnt; rr++)
                    for (int cc = 0; cc < scnt; cc++)
                        subtile[rr, cc] = reachMap[r * scnt + rr, c * scnt + cc];
                res[r, c] = GetReachableCount(subtile, stepCount);
            }
        return res;
    }*/

    bool isEven = (stepCount % 2) == 0;
    long res = stepCount > reach.MaxStep
        ? (isEven ? reach.EvenCount : reach.OddCount)
        : GetReachableCount(reach.reachMap, stepCount);

    //PrintReachMap(reach, 10);

    var rotated = reach;
    for (int rotate = 0; rotate < 4; rotate++)
    {
        Console.WriteLine("\nCalculating for position: " + rotate);
        //PrintReachMap(rotated);
        Console.WriteLine($"Start Map: st.row {rotated.MinLeftIndex}, RMin: {rotated.MinRight}, Max: {rotated.MaxStep}");

        List<Tuple<int, int, int>> prevStart;
        List<Tuple<Tile, int>> rightMaps = GetRightMaps(rotated);
        Console.WriteLine("Right maps: " + string.Join(" >>> ",
            rightMaps.Select(r => $"Offs {r.Item2}, st.row {r.Item1.MinLeftIndex}, RMin: {r.Item1.MinRight}, Max: {r.Item1.MaxStep}")));

        // Count tiles to the right
        long r = 0;
        var remRight = stepCount - rotated.MinRight - 1;
        int rnum = 0;
        while (rnum < rightMaps.Count - 1 && remRight > 0)
        {
            var t = rightMaps[rnum].Item1;
            var cnt = remRight > t.MaxStep
                ? (remRight % 2 == 0 ? t.EvenCount : t.OddCount)
                : GetReachableCount(t.reachMap, remRight);
            r += cnt;
            Console.WriteLine($"Tile {rnum + 1}: {cnt}.");
            remRight -= t.MinRight + 1;
            rnum++;
        }
        if (remRight > 0)
        {
            var tr = rightMaps[rightMaps.Count - 1].Item1;
            var cntRExtra = (remRight - tr.MaxStep) / (tr.MinRight + 1);
            if (cntRExtra > 0)
            {
                var fe = remRight % 2 == 0 ? cntRExtra : 0;
                var fo = remRight % 2 == 0 ? 0 : cntRExtra;
                if ((tr.MinRight + 1) % 2 != 0)
                {
                    fe = remRight % 2 == 1 ? (cntRExtra / 2) : ((cntRExtra / 2) + (cntRExtra % 2));
                    fo = cntRExtra - fe;
                }
                r += fe * tr.EvenCount + fo * tr.OddCount;
                Console.WriteLine($"Full last tiles to right: {fe} ({tr.EvenCount}) and {fo} ({tr.OddCount}).");
            }
            remRight -= cntRExtra * (tr.MinRight + 1);
            while (remRight > 0)
            {
                var ole = GetReachableCount(tr.reachMap, remRight);
                r += ole;
                Console.WriteLine($"Last tile remaining {remRight}, options {ole}");
                remRight -= tr.MinRight + 1;
            }
        }

        int remCorner = stepCount - 2 - rotated.reachMap[rotated.H - 1, rotated.W - 1];
        if (remCorner > 0)
        {
            var tCorner = GetReachMap(rotated.reachMap, new List<Tuple<int, int, int>> { new Tuple<int, int, int>(0, 0, 0) });
            if (tCorner.H != tCorner.W)
                throw new Exception("This algorythm doesn't cover non-square labyrinth.");
            long rCount = remCorner / (tCorner.W) - 1;

            var fullCount = rCount > 0 ? rCount * (rCount + 1) / 2 : 0;
            if (tCorner.W % 2 == 0)
            {
                var fullOpts = remCorner % 2 == 0 ? tCorner.EvenCount : tCorner.OddCount;
                Console.WriteLine($"Found {fullCount} fully filled {fullOpts} each");
                r += fullCount * fullOpts;
            }
            else
            {
                // 1 + 3 + 5 + 7 + 9 = (1 + 9) / 2 * ((9 - 1) / 2 + 1)
                var fmax = (rCount - 1) + (rCount % 2);
                var fCount = (1 + fmax) * (1 + fmax) / 4;

                // 2 + 4 + 6 + 8 + 10 = (10 + 2) / 2 * (10 / 2)
                var smax = rCount - (rCount % 2);
                var sCount = (smax + 2) * smax / 4;

                var fall = (remCorner % 2 == 0) ? tCorner.EvenCount : tCorner.OddCount;
                var sall = (remCorner % 2 == 1) ? tCorner.EvenCount : tCorner.OddCount;
                Console.WriteLine($"Found {fullCount} fully filled with {fCount}({fall}) and {sCount} ({sall}).");
                r += fall * fCount + sall * sCount;
            }
            remCorner = remCorner - (int)rCount * tCorner.W;
            var borderCnt = rCount > 0 ? rCount + 1 : 1;
            while (remCorner >= 0)
            {
                var cornOpts = GetReachableCount(tCorner.reachMap, remCorner);
                Console.WriteLine($"Found {borderCnt} borders {cornOpts} each");
                r += borderCnt * cornOpts;
                remCorner -= tCorner.W;
                borderCnt++;
            }
        }

        Console.WriteLine("Found: " + r);
        res += r;
        rotated = Rotate(rotated);
    }

    Console.WriteLine("Task 2: " + res);

    Console.WriteLine("Finished");
}

List<Tuple<Tile, int>> GetRightMaps(Tile rotated)
{
    var res = new List<Tuple<Tile, int>>();
    var rightStart = GetStartByCol(rotated.reachMap, rotated.W - 1);
    var prevStart = rightStart;
    do
    {
        prevStart = rightStart;
        var stepMap = GetReachMap(rotated.reachMap, prevStart.Item1);
        res.Add(new Tuple<Tile, int>(stepMap, prevStart.Item2 % 2));
        rightStart = GetStartByCol(stepMap.reachMap, rotated.W - 1);
    } while (!Same(rightStart.Item1, prevStart.Item1) || rightStart.Item2 != prevStart.Item2);
    return res;
}


int[,] ParseData(string[] data)
{
    var res = new int[data.Length, data.Length];
    for (int r = 0; r < data.Length; r++)
        for (int c = 0; c < data.Length; c++)
            if (data[r][c] == '#') res[r, c] = -1;
    return res;
}


void PrintReachMap(Tile tile, int thr = -1)
{
    for (int r = 0; r < tile.H; r++)
    {
        for (int c = 0; c < tile.W; c++)
        {
            if (tile.reachMap[r, c] == -1)
            {
                Console.Write("X\t");
                continue;
            }
            Console.Write((thr == -1 ? tile.reachMap[r, c]
                : (tile.reachMap[r, c] <= thr && (tile.reachMap[r, c] % 2 == thr % 2) ? "." : " ")) + "\t");
        }
        Console.WriteLine();
    }
}



Tile Rotate(Tile reach)
{
    int[,] map = new int[reach.W, reach.H];
    for (int r = 0; r < reach.H; r++)
        for (int c = 0;  c < reach.W; c++)
            map[c, r] = reach.reachMap[r, reach.H - c - 1];
    return new Tile(map);
}

bool Same(List<Tuple<int, int, int>> one, List<Tuple<int, int, int>> two)
{
    for (int i = 0; i < one.Count; i++)
    {
        if (one[i].Item1 != two[i].Item1 || one[i].Item2 != two[i].Item2 || one[i].Item3 != two[i].Item3)
            return false;
    }
    return true;
}


Tuple<List<Tuple<int, int, int>>, int> GetStartByCol(int[,] reach, int col)
{
    var res = new List<Tuple<int, int, int>>();
    for (int r = 0; r < reach.GetLength(0); r++)
    {
        res.Add(new Tuple<int, int, int>(r, reach.GetLength(0) - col - 1, reach[r, col]));
    }
    var min = res.Min(x => x.Item3);
    for (int i = 0; i < res.Count; i++)
        res[i] = new Tuple<int, int, int>(res[i].Item1, res[i].Item2, res[i].Item3 - min);
    return new Tuple<List<Tuple<int, int, int>>, int>(res, min);
}

List<Tuple<int, int, int>> GetStartByRow(int[,] reach, int row)
{
    var res = new List<Tuple<int, int, int>>();
    for (int c = 0; c < reach.GetLength(0); c++)
    {
        res.Add(new Tuple<int, int, int>(reach.GetLength(0) - row - 1, c, reach[row, c]));
    }
    var min = res.Min(x => x.Item3);
    for (int i = 0; i < res.Count; i++)
        res[i] = new Tuple<int, int, int>(res[i].Item1, res[i].Item2, res[i].Item3 - min);
    return res;
}



/*Tuple<int, int> GetColOptimum(int[,] reach, int col)
{
    var ri = 0;
    for (int i = 1; i < reach.GetLength(0); i++)
    {
        if (reach[i, col] < reach[ri, col]) ri = i;
    }
    return new Tuple<int, int>(ri, reach[ri, col]);
}

Tuple<int, int> GetRowOptimum(int[,] reach, int row)
{
    var ci = 0;
    for (int i = 1; i < reach.GetLength(1); i++)
    {
        if (reach[row, i] < reach[row, ci]) ci = i;
    }
    return new Tuple<int, int>(ci, reach[row, ci]);
}*/

Tile GetReachMap(int[,] source, List<Tuple<int, int, int>> startPositions)
{
    var res = new int[source.GetLength(0), source.GetLength(1)];
    for (int i = 0; i < source.GetLength(0); i++)
        for (int j = 0; j < source.GetLength(1); j++)
            res[i, j] = (source[i, j] == -1) ? -1 : int.MaxValue;

    var q = new Queue<Tuple<int, int, int>>();
    foreach (var start in startPositions)
        //q.Enqueue(new Tuple<int, int, int>(start.Item1, start.Item2, start.Item3));
        CheckPut(res, q, start.Item1, start.Item2, start.Item3);

    while (q.Count > 0)
    {
        var item = q.Dequeue();
        CheckPut(res, q, item.Item1, item.Item2 + 1, item.Item3 + 1);
        CheckPut(res, q, item.Item1, item.Item2 - 1, item.Item3 + 1);
        CheckPut(res, q, item.Item1 + 1, item.Item2, item.Item3 + 1);
        CheckPut(res, q, item.Item1 - 1, item.Item2, item.Item3 + 1);
    }

    return new Tile(res);
}

int GetReachableCount(int[,] steps, int max)
{
    var cnt = 0;
    for (int r = 0; r < steps.GetLength(0); r++)
        for (int c = 0; c < steps.GetLength(1); c++)
        {
            if (steps[r, c] >= 0 && steps[r, c] <= max && steps[r, c] % 2 == (max %2))
                cnt++;
        }

    return cnt;
}

void CheckPut(int[,] reach, Queue<Tuple<int, int, int>> q, int r, int c, int n)
{
    if (r < 0 || r >= reach.GetLength(0) || c < 0 || c >= reach.GetLength(1))
        return;
    if (/*data[r][c] != '#' && */reach[r, c] != -1 && reach[r, c] > n)
    {
        reach[r, c] = n;
        q.Enqueue(new Tuple<int, int, int>(r, c, n));
    }
}

Tuple<int, int> GetStart(string[] data)
{
    for (int i = 0; i < data.Length; i++)
        for (int j = 0; j < data[i].Length; j++)
            if (data[i][j] == 'S') return new Tuple<int, int>(i, j);
    throw new Exception();
}

class Tile
{
    public int[,] reachMap;

    public int MaxStep;
    public int OddCount;
    public int EvenCount;
    public int MinRight;
    public int MinLeft;
    public int MinLeftIndex;
    //public int MinTop;
    //public int MinBottom;

    public int H;
    public int W;

    public Tile(int[,] map)
    {
        reachMap = map;
        MaxStep = 0;
        OddCount = 0;
        EvenCount = 0;
        H = map.GetLength(0);
        W = map.GetLength(1);
        for (int r = 0; r < H; r++)
            for (int c = 0; c < W; c++)
                if (map[r, c] >= 0 && map[r,c] < int.MaxValue)
                {
                    EvenCount += (map[r, c] + 1) % 2;
                    OddCount += map[r, c] % 2;
                    if (map[r, c] > MaxStep)
                        MaxStep = map[r, c];
                }
        MinRight = int.MaxValue;
        MinLeft = int.MaxValue;
        for (int r = 0; r < H; r++)
        {
            if (MinLeft > map[r, 0])
            {
                MinLeft = map[r, 0];
                MinLeftIndex = r;
            }
            if (MinRight > map[r, W-1]) MinRight = map[r, W-1];
        }
        /*MinTop = int.MaxValue;
        MinBottom = int.MaxValue;
        for (int c = 0; c < W; c++)
        {
            if (MinTop > map[0, c]) MinTop = map[0, c];
            if (MinBottom > map[H-1, c]) MinBottom = map[H-1, c];
        }*/
    }
}