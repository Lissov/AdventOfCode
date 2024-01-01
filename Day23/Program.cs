using System.Diagnostics;

Console.WriteLine("Day 23!");

var input = File.ReadAllLines("example.txt");
input = File.ReadAllLines("input.txt");

var dirs = "^>v<";

char[,] map = ParseMap(input);
//PrintMap(map);
Console.WriteLine("Filling dead ends...");
FillDeadEnds(map);
//PrintMap(map);
Console.WriteLine("Filling one ways...");
FillOneways(map);
//PrintMap(map);
Console.WriteLine("\nFilling nodes...");
var nodes = FindNodes(map);
FillConnections(map, nodes);
//PrintMap(map, nodes);
PrintNodes(nodes);

var path = GetLongestPath(nodes);
var pstr = string.Join(" -> ", path.Item1.Select(n => n.Name));
Console.WriteLine($"Part 1: {path.Item2} via {pstr}");

map = ParseMap(input);
ClearSlippery(map);
nodes = FindNodes(map);
FillConnections(map, nodes);
//PrintMap(map, nodes);
PrintNodes(nodes);
path = GetLongestPath(nodes);
pstr = string.Join(" -> ", path.Item1.Select(n => n.Name));
Console.WriteLine($"Part 2: {path.Item2} via {pstr}");

Console.WriteLine("Finished");

void ClearSlippery(char[,] map)
{
    for (int r = 0; r < map.GetLength(0) - 1; r++)
        for (int c = 0; c < map.GetLength(1); c++)
        {
            if (dirs.Contains(map[r, c]))
                map[r, c] = '.';
        }
}

Tuple<List<Node>, int> GetLongestPath(List<Node> nodes)
{
    var path = new List<Node>();
    var start = nodes[0];
    return GetLPRecursive(nodes, path, start);
}

Tuple<List<Node>, int> GetLPRecursive(List<Node> nodes, List<Node> path, Node start)
{
    if (start.Name == "End")
        return new Tuple<List<Node>, int>(new List<Node> { start }, 0);

    var paths = start.Connections
        .Where(c => !path.Contains(c.Item1))
        .Select(c =>
    {
        var np = path.ToList();
        np.Add(c.Item1);
        var p2 = GetLPRecursive(nodes, np, c.Item1);
        if (p2 == null) return null;
        return new Tuple<List<Node>, int>(p2.Item1, c.Item2 + p2.Item2);
    }).Where(x => x != null).ToList();
    if (!paths.Any()) return null;
    var max = paths.MaxBy(p => p.Item2);
    max.Item1.Insert(0, start);
    return max;
}

void PrintNodes(List<Node> nodes)
{
    foreach (Node node in nodes)
    {
        Console.WriteLine(node.Name+ " -> ");
        foreach (var c in node.Connections)
            Console.WriteLine($"\t{c.Item1.Name} in {c.Item2}");
    }
}


void FillOneways(char[,] map)
{
    map[0, 1] = 'v';
    map[map.GetLength(0)-1, map.GetLength(1)-2] = 'v';

    var q = new Queue<Tuple<int, int>>();
    for (int r = 0; r < map.GetLength(0) - 1; r++)
        for (int c = 0; c < map.GetLength(1); c++)
            if (dirs.Contains(map[r, c])) q.Enqueue(new Tuple<int, int>(r, c));

    while (q.Count > 0)
    {
        var n = q.Dequeue();
        var dir = dirs.IndexOf(map[n.Item1, n.Item2]);
        var next = GetCoords(n, dir);
        var nc = map[next.Item1, next.Item2];
        if ((dir + 2) % 4 == dirs.IndexOf(nc))
            throw new Exception("Opposite paths!");
        if (nc == '.') {
            var a = GetAround(map, next.Item1, next.Item2);
            if (a.Count(x => !IsWall(x)) == 2)
            {
                var nd = GetNextDir(map, next, dir);
                map[next.Item1, next.Item2] = dirs[nd];
                q.Enqueue(next);
            }
        }
    }
}

int GetNextDir(char[,] map, Tuple<int, int> next, int dir)
{
    var a = GetAround(map, next.Item1, next.Item2);
    var i = 0;
    while (i == (dir + 2) % 4 || IsWall(a[i])) i++;
    return i;
}

List<Node> FindNodes(char[,] map)
{
    var res = new List<Node>
    {
        new Node{ Name = "Start", Row = 0, Col = 1 }
    };

    int i = 0;

    for (int r = 1; r < map.GetLength(0) - 1; r++)
        for (int c = 1; c < map.GetLength(1) - 1; c++)
        {
            if (IsWall(map[r, c])) continue;
            var around = GetAround(map, r, c);
            if (around.Count(x => !IsWall(x)) > 2)
                res.Add(new Node { Name = $"Node {++i}", Row = r, Col = c });
        }
    res.Add(new Node { Name = "End", Row = map.GetLength(0) - 1, Col = map.GetLength(1) - 2 });
    return res;
}

void FillConnections(char[,] map, List<Node> nodes)
{
    foreach (var node in nodes)
    {
        var around = GetAround(map, node.Row, node.Col);
        var outgoing = GetPossibilities(around);
        foreach (var neighbor in outgoing)
        {
            var d = 1;
            var dir = neighbor;
            var pos = GetCoords(new Tuple<int, int>(node.Row, node.Col), dir);
            Node dest;
            while ((dest = nodes.FirstOrDefault(n => n.Row == pos.Item1 && n.Col == pos.Item2)) == null)
            {
                d++;
                dir = GetNextDir(map, pos, dir);
                pos = GetCoords(pos, dir);
            }
            node.Connections.Add(new Tuple<Node, int>(dest, d));
        }
    }
}

List<int> GetPossibilities(string around)
{
    var res = new List<int>();
    for (int i = 0; i < around.Length; i++)
    {
        if (IsWall(around[i])) continue;
        var opp = (i + 2) % 4;
        if (around[i] == '.')
        {
            res.Add(i);
            Console.WriteLine("Two way found");
            continue;
        }
        if (around[i] != dirs[opp])
            res.Add(i);
    }
    return res;
}

char[,] ParseMap(string[] input)
{
    var res = new char[input.Length, input[0].Length];
    for (int i = 0; i < input.Length; i++)
        for (int j = 0; j < input[i].Length; j++)
            res[i, j] = input[i][j];
    return res;
}

void FillDeadEnds(char[,] map)
{
    var q = new Queue<Tuple<int, int>>();
    for (int r = 1; r < map.GetLength(0) - 1; r++)
        for (int c = 1; c < map.GetLength(1) - 1; c++)
        {
            if (map[r, c] != '#')
            {
                var around = GetAround(map, r, c);
                if (around.Count(c => c == '#') == 3)
                    q.Enqueue(new Tuple<int, int>(r, c));
            }
        }

    while (q.Count > 0)
    {
        var n = q.Dequeue();
        var around = GetAround(map, n.Item1, n.Item2);
        if (around.Count(x => IsWall(x)) >= 3)
        {
            map[n.Item1, n.Item2] = 'X';
            var next = Array.FindIndex(around.ToCharArray(), x => !IsWall(x));
            if (next >= 0)
                q.Enqueue(GetCoords(n, next));
        }
    }
}

void PrintMap(char[,] map, List<Node> nodes = null)
{
    for (int r = 0; r < map.GetLength(0); r++)
    {
        for (int c = 0; c < map.GetLength(1); c++)
        {
            var n = nodes?.FirstOrDefault(n => n.Row == r && n.Col == c);
            Console.Write(n?.Name[0] ?? map[r, c]);
        }
        Console.WriteLine();
    }
}


Tuple<int, int> GetCoords(Tuple<int, int> n, int next)
{
    switch (next)
    {
        case 0: return new Tuple<int, int>(n.Item1 - 1, n.Item2);
        case 1: return new Tuple<int, int>(n.Item1, n.Item2 + 1);
        case 2: return new Tuple<int, int>(n.Item1 + 1, n.Item2);
        case 3: return new Tuple<int, int>(n.Item1, n.Item2 - 1);
        default:
            throw new NotImplementedException();
    }
}

string GetAround(char[,] map, int r, int c)
{
    if (r == 0) return "#"  + map[r, c + 1] + map[r + 1, c] + map[r, c - 1];
    if (r == map.GetLength(0) - 1) return "" + map[r - 1, c] + map[r, c + 1] + "#" + map[r, c - 1];
    return "" + map[r - 1, c] + map[r, c + 1] + map[r + 1, c] + map[r, c - 1];
}

static bool IsWall(char x)
{
    return x == '#' || x == 'X';
}

[DebuggerDisplay("{Name}")]
class Node
{
    public string Name = "Cross";
    public int Row;
    public int Col;
    public List<Tuple<Node, int>> Connections = new List<Tuple<Node, int>>();
}