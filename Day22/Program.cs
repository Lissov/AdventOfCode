using System.Diagnostics;

var data = File.ReadAllLines("example.txt");
data = File.ReadAllLines("input.txt");

var bricks = ParseBricks(data).ToList();
LetBricksDrop(bricks);

var safeToDesintegrate = bricks.Where(b => !b.Above.Any(a => a.Below.Count == 1)).ToList();

Console.WriteLine("Task 1: " + safeToDesintegrate.Count());

var unsafeB = bricks.Where(b => !safeToDesintegrate.Contains(b)).ToList();
SetDesintegrateCounts(bricks, unsafeB);

Console.WriteLine("Task 2: " + bricks.Sum(b => b.DesintegrateCount));


void SetDesintegrateCounts(List<Brick> bricks, List<Brick> unsafeBricks)
{
    foreach (var brick in unsafeBricks)
    {
        var removedBricks = new List<Brick> { brick };
        var bq = brick.Above.ToList();
        while (bq.Count > 0)
        {
            var cb = bq[0];
            bq.RemoveAt(0);
            if (cb.Below.All(bel => removedBricks.Contains(bel)))
                removedBricks.Add(cb);
            foreach (var a in cb.Above) if (!bq.Contains(a)) bq.Add(a);
            bq = bq.OrderBy(b => b.Z.Item1).ToList();
        }
        brick.DesintegrateCount = removedBricks.Count - 1; // exclude this
    }
}

void LetBricksDrop(List<Brick> bricks)
{
    var sortedB = bricks.OrderBy(b => b.Z.Item1);
    var xM = sortedB.Max(b => b.X.Item2);
    var yM = sortedB.Max(b => b.Y.Item2);

    var floor = new int[xM+1, yM+1];
    var brickRef = new Brick[xM+1, yM+1];

    foreach (var b in sortedB)
    {
        var maxH = 0;
        List<Brick> supporting = new List<Brick>();
        for (var x = b.X.Item1; x <= b.X.Item2; x++)
            for (var y = b.Y.Item1; y <= b.Y.Item2; y++)
            {
                if (floor[x, y] == maxH)
                {
                    supporting.Add(brickRef[x, y]);
                }
                if (floor[x, y] > maxH)
                {
                    maxH = floor[x, y];
                    supporting = new List<Brick> { brickRef[x, y] };
                }
            }

        supporting = supporting.Where(br => br != null).Distinct().ToList();
        b.Below = supporting;
        foreach (var sb in supporting) sb.Above.Add(b);

        if (maxH >= b.Z.Item1)
            throw new Exception("Overlapping brick!");
        // move down
        b.Z = new Tuple<int, int>(maxH + 1, b.Z.Item2 - b.Z.Item1 + maxH + 1);

        for (var x = b.X.Item1; x <= b.X.Item2; x++)
            for (var y = b.Y.Item1; y <= b.Y.Item2; y++)
            {
                floor[x, y] = b.Z.Item2;
                brickRef[x, y] = b;
            }

    }
}

IEnumerable<Brick> ParseBricks(string[] data)
{
    var l = 'A';
    foreach (var line in data)
    {
        var s0 = line.Split('~')[0].Split(',').Select(x => int.Parse(x)).ToArray();
        var s1 = line.Split('~')[1].Split(',').Select(x => int.Parse(x)).ToArray();
        yield return new Brick(s0[0], s1[0], s0[1], s1[1], s0[2], s1[2], "" + (l++));
    }
}

[DebuggerDisplay("{Name}: {X.Item1},{Y.Item1},{Z.Item1}~{X.Item2},{Y.Item2},{Z.Item2}")]
class Brick
{
    string Name { get; set; }
    public int DesintegrateCount { get; set; }
    public Brick(int x0, int x1, int y0, int y1, int z0, int z1, string name)
    {
        X = new Tuple<int, int>(Math.Min(x0, x1), Math.Max(x0, x1));
        Y = new Tuple<int, int>(Math.Min(y0, y1), Math.Max(y0, y1));
        Z = new Tuple<int, int>(Math.Min(z0, z1), Math.Max(z0, z1));
        Below = new List<Brick>();
        Above = new List<Brick>();
        Name = name;
    }

    public Tuple<int, int> X;
    public Tuple<int, int> Y;
    public Tuple<int, int> Z;

    public List<Brick> Below { get; set; } // bricks directly supporting current one
    public List<Brick> Above { get; set; } // bricks directly supported by the current one
}