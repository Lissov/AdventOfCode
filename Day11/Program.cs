using System.Diagnostics;

var data = File.ReadAllLines("example.txt");
data = File.ReadAllLines("input.txt");

var start = DateTime.Now;
var maxR = data.Length;
var maxC = data[0].Length;
var galaxies = new List<Point>();
for (int i = 0; i < maxR; i++)
{
    for (int j = 0; j < maxC; j++)
    {
        if (data[i][j] == '#')
            galaxies.Add(new Point { Row = i, Col = j });
    }
}
Console.WriteLine("Read complete");

var expandSize = 999999;

checked
{
    var r = 0;
    while (r < maxR)
    {
        if (!galaxies.Any(g => g.Row == r))
        {
            galaxies.Where(g => g.Row > r).ToList().ForEach(g => g.Row += expandSize);
            maxR += expandSize;
            r += expandSize;
        }
        r++;
    }
    var c = 0;
    while (c < maxC)
    {
        if (!galaxies.Any(g => g.Col == c))
        {
            galaxies.Where(g => g.Col > c).ToList().ForEach(g => g.Col += expandSize);
            maxC += expandSize;
            c += expandSize;
        }
        c++;
    }
    Console.WriteLine("Expand complete");

    long pathSum = 0;
    for (int i = 0; i < galaxies.Count; i++)
    {
        for (int j = i + 1; j < galaxies.Count; j++)
        {
            var dist = galaxies[i].DistanceTo(galaxies[j]);
            //Console.WriteLine($"{i + 1} to {j + 1}: {dist}");
            pathSum += dist;
        }
    }

    Console.WriteLine(pathSum);
}
Console.WriteLine((DateTime.Now - start).TotalMilliseconds + "ms");

[DebuggerDisplay("{Row}:{Col}")]
class Point
{
    public int Row;
    public int Col;

    public int DistanceTo(Point point)
    {
        return Math.Abs(point.Row - Row) + Math.Abs(point.Col - Col);
    }
}