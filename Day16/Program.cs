var map = File.ReadAllLines("example.txt");
map = File.ReadAllLines("input.txt");
Dictionary<string, Dirs[]> mapping = new Dictionary<string, Dirs[]>
{
    //Dirs.Right
    { "0.", new[] { Dirs.Right } },
    { "0-", new[] { Dirs.Right } },
    { "0|", new[] { Dirs.Up, Dirs.Down } },
    { "0/", new[] { Dirs.Up } },
    { "0\\", new[] { Dirs.Down } },
    //Dirs.Down
    { "1.", new[] { Dirs.Down } },
    { "1-", new[] { Dirs.Right, Dirs.Left } },
    { "1|", new[] { Dirs.Down } },
    { "1/", new[] { Dirs.Left } },
    { "1\\", new[] { Dirs.Right } },
    //Dirs.Left
    { "2.", new[] { Dirs.Left } },
    { "2-", new[] { Dirs.Left } },
    { "2|", new[] { Dirs.Up, Dirs.Down } },
    { "2/", new[] { Dirs.Down } },
    { "2\\", new[] { Dirs.Up } },
    //Dirs.Up
    { "3.", new[] { Dirs.Up } },
    { "3-", new[] { Dirs.Left, Dirs.Right } },
    { "3|", new[] { Dirs.Up } },
    { "3/", new[] { Dirs.Right } },
    { "3\\", new[] { Dirs.Left } },
};

var cnt = GetCount(new Tuple<int, int, int>(0, 0, 0));
Console.WriteLine("Task 1: " + cnt);

var max = 0;
for (int c = 0; c < map[0].Length; c++)
    CheckMax(new Tuple<int, int, int>(0, c, (int)Dirs.Down));
for (int c = 0; c < map[0].Length; c++)
    CheckMax(new Tuple<int, int, int>(map.Length - 1, c, (int)Dirs.Up));
for (int r = 0; r < map.Length; r++)
    CheckMax(new Tuple<int, int, int>(r, 0, (int)Dirs.Right));
for (int r = 0; r < map.Length; r++)
    CheckMax(new Tuple<int, int, int>(r, map[0].Length-1, (int)Dirs.Left));

Console.WriteLine("Task 1: " + max);


void CheckMax(Tuple<int, int, int> start)
{
    var m = GetCount(start);
    if (m > max)
    {
        max = m;
        Console.WriteLine($"Found {max} for {start.Item1}:{start.Item2} {start.Item3}.");
    }
}

int GetCount(Tuple<int, int, int> start)
{
    var directions = new bool[map.Length, map[0].Length, 4];
    List<Tuple<int, int, int>> queue = new List<Tuple<int, int, int>> { start };
    var i = 0;
    while (i < queue.Count)
    {
        var pos = queue[i];
        directions[pos.Item1, pos.Item2, pos.Item3] = true;
        foreach (var next in GetNext(pos))
        {
            if (InField(next) && !directions[next.Item1, next.Item2, next.Item3])
                queue.Add(next);
        }
        i++;
    }

    var cnt = 0;

    for (int r = 0; r < map.Length; r++)
    {
        for (int c = 0; c < map[r].Length; c++)
            if (directions[r, c, 0] || directions[r, c, 1] || directions[r, c, 2] || directions[r, c, 3])
                cnt++;
    }
    return cnt;
}




bool InField(Tuple<int, int, int> next)
{
    return next.Item1 >= 0 && next.Item1 < map.Length
        && next.Item2 >= 0 && next.Item2 < map[0].Length;
}

IEnumerable<Tuple<int, int, int>> GetNext(Tuple<int, int, int> curr)
{
    var dir = curr.Item3;
    var cell = map[curr.Item1][curr.Item2];
    var dirs = mapping[dir.ToString() + cell];
    foreach ( var d in dirs)
    {
        yield return MoveTo((int)d, curr.Item1, curr.Item2);
    }
}

Tuple<int, int, int> MoveTo(int dir, int r, int c)
{
    switch (dir)
    {
        case (int)Dirs.Right: return new Tuple<int, int, int>(r, c+1, dir);
        case (int)Dirs.Down: return new Tuple<int, int, int>(r+1, c, dir);
        case (int)Dirs.Left: return new Tuple<int, int, int>(r, c-1, dir);
        case (int)Dirs.Up: return new Tuple<int, int, int>(r-1, c, dir);
    }
    throw new Exception();
}

enum Dirs { Right = 0, Down = 1, Left = 2, Up = 3 };
// 0 - right, 1 - down, 2 - left, 34 - up
