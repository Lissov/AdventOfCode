var input = File.ReadAllLines("example.txt");
input = File.ReadAllLines("input.txt");

long minX = 7; long maxX = 27;
minX = 200000000000000; maxX = 400000000000000;
long minY = minX; long maxY = maxX;

checked
{
    var lines = input.Select(x => new Line(x)).ToList();
    var pairs = GetCrossingLinePairs(lines);
    Console.WriteLine("Task 1: " + pairs.Count());

    var xCommon = GetRockVelocity(lines, p => p.X);
    var yCommon = GetRockVelocity(lines, p => p.Y);
    var zCommon = GetRockVelocity(lines, p => p.Z);

    Console.WriteLine($"Velocity: {xCommon}, {yCommon}, {zCommon}");
    var p = CanStart(xCommon, yCommon, zCommon, lines);
    Console.WriteLine($"Task 2: {p.X} + {p.Y} + {p.Z} = {p.X + p.Y + p.Z}");

    var bound = 1000;
    var xs = lines.Select(x => x.Velocity.X).OrderBy(x => x).Distinct().ToList();
    var ys = lines.Select(x => x.Velocity.Y).OrderBy(x => x).Distinct().ToList();
    var zs = lines.Select(x => x.Velocity.Z).OrderBy(x => x).Distinct().ToList();
    // assume velocity in range -500..500
    for (int vx = 0; vx <= bound; vx++)
    {
        if (vx % 10 == 0)
            Console.WriteLine("For vx: " + vx);
        for (int vy = 0; vy <= bound; vy++)
            for (int vz = 0; vz <= bound; vz++)
            {
                PointD s = CanStart(vx, vy, vz, lines);
                if (s != null)
                {
                    if (decimal.Floor(s.X) == s.X && decimal.Floor(s.Y) == s.Y && decimal.Floor(s.Z) == s.Z)
                    {
                        Console.WriteLine($"Solution for {vx}:{vy}:{vz} > {s.X} * {s.Y} * {s.Z} == {s.X * s.Y * s.Z}");
                        //return;
                    }
                    else
                        Console.WriteLine($"Non-integer solution found for {vx}:{vy}:{vz}: {s.X} * {s.Y} * {s.Z}");
                }
            }
    }
}

int GetRockVelocity(List<Line> lines, Func<Point, long> getter)
{
    var sameVx = lines.GroupBy(l => getter(l.Velocity)).Where(c => c.Count() > 1)
        .OrderByDescending(x => x.Count());
    List<int> xvel = null;
    foreach (var g in sameVx.Where(x => x.Key > 0))
    {
        var p = GetVelocities(g.Skip(1).First(), g.First(), getter);
        if (xvel == null)
            xvel = p;
        else
            xvel = xvel.Where(x => p.Contains(x)).ToList();
    }
    return xvel.Single();
}

List<int> GetVelocities(Line line1, Line line2, Func<Point, long> getter)
{
    var res = new List<int>();
    for (var i = -1000; i <= 1000; i++)
        if (i != getter(line1.Velocity)
            && (getter(line2.Start) - getter(line1.Start)) % (getter(line1.Velocity) - i) == 0)
            res.Add(i);
    return res;
}

List<long> GetPrimeFact(long value)
{
    var res = new List<long>();
    if (value < 2)
        return res;
    long x = 2;
    while (value % x == 0)
    {
        res.Add(x);
        value /= x;
    }
    x = 3;
    while (value > 1 && value > x * x)
    {
        if (value % x == 0)
        {
            value /= x;
            res.Add(x);
        }
        else
            x = x + 2;
    }
    if (value > 1)
        res.Add(value);
    return res;
}

Console.WriteLine("Finished");

PointD CanStart(int vx, int vy, int vz, List<Line> hailstones)
{
    var m = new List<decimal[]>();
    // x>>   rx + vx*t0 = sx0 + vx0*t0
    // t0*(vx0 - vx) + sx0 - rx = 0

    // t0 * (vx0 - vx) + 0 * t1 - rx - 0 - 0 + sx0 = 0
    m.Add(new decimal[] { (hailstones[0].Velocity.X - vx), 0, -1, 0, 0, hailstones[0].Start.X });
    m.Add(new decimal[] { (hailstones[0].Velocity.Y - vy), 0, 0, -1, 0, hailstones[0].Start.Y });
    m.Add(new decimal[] { (hailstones[0].Velocity.Z - vz), 0, 0, 0, -1, hailstones[0].Start.Z });

    //.0*t0 + t1*(vx1 - vx) + - rx - 0 - 0 + sx1 = 0
    m.Add(new decimal[] { 0, (hailstones[1].Velocity.X - vx), -1, 0, 0, hailstones[1].Start.X });
    m.Add(new decimal[] { 0, (hailstones[1].Velocity.Y - vy), 0, -1, 0, hailstones[1].Start.Y });
    m.Add(new decimal[] { 0, (hailstones[1].Velocity.Z - vz), 0, 0, -1, hailstones[1].Start.Z });

    var r = Solve(m);
    if (r == null)
        return null;
    return new PointD(r[2], r[3], r[4]);
}

decimal[] Solve(List<decimal[]> m)
{
    var sz = m.Count;
    for (int c = 0; c < sz-1; c++)
    {
        if (m[c][c] == 0)
        {
            var l = m.Skip(c).FirstOrDefault(x => x[c] != 0);
            if (l == null)
                return null;
            m.Remove(l);
            m.Insert(c, l);
        }
        for (int r = 0; r < sz; r++)
        {
            if (r != c)
            {
                var k = m[r][c] / m[c][c];
                for (int cl = c; cl < sz; cl++)
                {
                    m[r][cl] = m[r][cl] - k * m[c][cl];
                }
            }
        }
    }
    if (m[sz - 1][sz - 1] > 0.0000001m || m[sz - 1][sz - 2] > 0.0000001m)
        return null;//not solvable

    var res = new decimal[sz];
    for (int c = 0; c < sz-1; c++)
        res[c] = Math.Round(-m[c][sz - 1] / m[c][c], 6);
    return res;
}


/*Tuple<Line, Line> GetFarthestHStones(List<Line> lines, Func<Point, decimal> f)
{
    decimal maxXD = 0;
    Tuple<int, int> res = new Tuple<int, int>(0, 0);
    for (int i = 0; i < lines.Count; i++)
        for (int j = i; j < lines.Count; j++)
        {
            if (f(lines[i].Velocity) * f(lines[j].Velocity) < 0)
            {
                if (maxXD < Math.Abs(f(lines[i].Start) - f(lines[j].Start)))
                {
                    maxXD = Math.Abs(f(lines[i].Start) - f(lines[j].Start));
                    res = new Tuple<int, int>(i, j);
                }
            }
        }
    return new Tuple<Line, Line>(lines[res.Item1], lines[res.Item2]);
}*/



List<Tuple<Line, Line>> GetCrossingLinePairs(List<Line>? lines)
{
    var res = new List<Tuple<Line, Line>>();

    for (int i = 0; i < lines.Count; i++)
        for (int j = i + 1; j < lines.Count; j++)
        {
            var cross = lines[i].GetCrossing(lines[j]);
            if (cross != null && cross.X >= minX && cross.X <= maxX 
                && cross.Y >= minY && cross.Y <= maxY)
            {
                var t1 = (cross.X - lines[i].Start.X) * lines[i].Velocity.X;
                var t2 = (cross.X - lines[j].Start.X) * lines[j].Velocity.X;
                if (t1 == 0 && t2 == 0)
                {
                    t1 = (cross.Y - lines[i].Start.Y) * lines[i].Velocity.Y;
                    t2 = (cross.Y - lines[j].Start.Y) * lines[j].Velocity.Y;
                }
                if (t1 > 0 && t2 > 0)
                {
                    res.Add(new Tuple<Line, Line>(lines[i], lines[j]));
                    //Console.WriteLine($"Crossing {i} and {j}");
                }
            }
        }
    return res;
}



class Point
{
    public long X; public long Y; public long Z;
    public Point(long x, long y, long z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}
class PointD
{
    public decimal X; public decimal Y; public decimal Z;
    public PointD(decimal x, decimal y, decimal z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}
class Line
{
    public Point Start;
    public Point Velocity;

    public decimal K;
    public decimal L;

    public Line(string str)
    {
        var pt = str.Split("@")[0]
            .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => long.Parse(x)).ToList();
        Start = new Point(pt[0], pt[1], pt[2]);

        var vel = str.Split("@")[1]
            .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => int.Parse(x)).ToList();
        Velocity = new Point(vel[0], vel[1], vel[2]);

        K = (decimal)Velocity.Y / (decimal)Velocity.X;
        L = (decimal)Start.Y - K * Start.X;
    }

    public PointD GetCrossing(Line another)
    {
        if (K == another.K)
        {
            return L == another.L
                ? new PointD(0, 0, 0)
                : null;
        }

        var x = (another.L - L) / (K - another.K);
        var y = x * K + L;
        return new PointD(x, y, 0);

/*        if (Velocity.X == another.Velocity.X)
        {
            return Start.X == another.Start.X && Start.Y == another.Start.Y && Start.Z == another.Start.Z
                ? new Tuple<PointD, decimal>(new PointD(Start.X, Start.Y, Start.Z), 0)
                : null;
        }
        var t = (Start.X - another.Start.X) / (decimal)(another.Velocity.X -  Velocity.X);
        var y1 = Start.Y + Velocity.Y * t;
        var y2 = another.Start.Y + another.Velocity.Y * t;
        if (y1 != y2)
        {
            if (Math.Abs(y1 - y2) < 0.001m)
                throw new Exception("Delta?");
            return null;
        }

        var z1 = Start.Z + Velocity.Z * t;
        var z2 = another.Start.Z + another.Velocity.Z * t;
        if (z1 != z2)
        {
            if (Math.Abs(z1 - z2) < 0.001m)
                throw new Exception("Delta?");
            return null;
        }
        return new Tuple<PointD, decimal>(new PointD(Start.X + Velocity.X * t, y1, 0), t);*/
    }
}