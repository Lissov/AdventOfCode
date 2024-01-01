using System.Diagnostics;

Console.WriteLine("Day 25");

var input = File.ReadAllLines("example.txt");
input = File.ReadAllLines("input.txt");

var net = ParseNet(input);

Tuple<string, string>[] links = GetLinks(net);
var la = links.Select(l => new LinkD { From = l.Item1, To = l.Item2, UseCnt = 0 }).ToList();

var r = new Random();
for (int i = 1; i <= 1000000; i++)
{
    RandomTry();

    if (i % 100 == 0)
    {
        var skipped = la.OrderByDescending(x => x.UseCnt)
            .Take(3)
            .Select(exl => Array.IndexOf(links, links
            .Single(l => l.Item1 == exl.From && l.Item2 == exl.To
                || l.Item1 == exl.To && l.Item2 == exl.From)))
            .ToArray();
        GetParts(net, links, skipped[0], skipped[1], skipped[2]);
        var v1 = net.Count(x => x.Visited);
        var u1 = net.Count(x => !x.Visited);
        if (u1 > 0)
        {
            Console.WriteLine($"Task 25/1: solved after {i} tries: {v1}*{u1} = {v1 * u1}");
            break;
        }
    }
    if (i % 1000 == 0)
    {
        Console.WriteLine("Continuing try: " + i);
        PrintTryRes();
    }
}

Console.WriteLine("Finished");


void PrintTryRes()
{
    la.OrderByDescending(x => x.UseCnt)
        .Take(10).ToList()
        .ForEach(x => Console.WriteLine($"{x.From}:{x.To} = {x.UseCnt}"));
}

void RandomTry()
{
    var f = net[r.Next(net.Count())];
    var t = net[r.Next(net.Count())];
    var p = GetPath(f, t);
    if (p != null)
    {
        for (int i = 1; i < p.Count; i++)
        {
            var fr = p[i-1].Name;
            var tt = p[i].Name;
            var l = la.Single(x => x.From == fr && x.To == tt || x.From == tt && x.To == fr);
            l.UseCnt++;
        }
    }
}

List<Node> GetPath(Node f, Node t)
{
    if (f == t)
        return null;
    net.ForEach(x => x.Distance = int.MaxValue);
    f.Distance = 0;
    var q = new Queue<Node>();
    q.Enqueue(f);
    while (q.Count > 0)
    {
        var n = q.Dequeue();
        foreach (var x in n.Links)
        {
            if (x == t)
            {
                var r = new List<Node>();
                r.Add(x);
                var p = n;
                while (p != f) {
                    r.Add(p);
                    p = p.CameFrom;
                }
                r.Add(f);
                return r;
            }
            if (x.Distance > n.Distance + 1)
            {
                x.Distance = n.Distance + 1;
                x.CameFrom = n;
                q.Enqueue(x);
            }
        }
    }
    return null;
}



/*
for (int i = 0; i < links.Length; i++)
{
    Console.WriteLine(i);
    for (int j = i + 1; j < links.Length; j++)
        for (int k = j + 1; k < links.Length; k++)
        {
            var netUpd = GetParts(net, links, i, j, k);
            if (netUpd.Any(n => !n.Visited))
            {
                var v = net.Count(x => x.Visited);
                var u = net.Count(x => !x.Visited);
                Console.WriteLine($"Task 25/1: {v}*{u} = {v * u}");
                return;
            }
        }
}*/

Console.WriteLine("Finished");

List<Node> GetParts(List<Node> net, Tuple<string, string>[] links, int i, int j, int k)
{
    net.ForEach(n => n.Visited = false);
    var q = new Queue<Node>();
    q.Enqueue(net[0]);
    net[0].Visited = true;
    var forbidden = new[] { links[i], links[j], links[k] };
    while (q.Count > 0)
    {
        var n = q.Dequeue();
        foreach (var l in n.Links) {
            if (!l.Visited &&
                !forbidden.Any(f => f.Item1 == n.Name && f.Item2 == l.Name
                                    || f.Item1 == l.Name && f.Item2 == n.Name))
            {
                l.Visited = true;
                q.Enqueue(l);
            }
        }
    }
    return net;
}


Tuple<string, string>[] GetLinks(List<Node> net)
{
    var res = new List<Tuple<string, string>>();
    foreach (var node in net)
    {
        foreach (var link in node.Links) {
            var f = node.Name.CompareTo(link.Name) < 0 ? node.Name : link.Name;
            var t = node.Name == f ? link.Name : node.Name;
            if (!res.Any(x => x.Item1 == f && x.Item2 == t))
                res.Add(new Tuple<string, string>(f, t));
        }
    }
    return res.ToArray();
}

List<Node> ParseNet(string[] input)
{
    var res = new List<Node>();
    foreach (var line in input)
    {
        var l = line.Split(':')[0];
        var c = line.Split(":")[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var ln = GetOrAddNode(res, l);
        foreach (var item in c)
        {
            var cn = GetOrAddNode(res, item);
            ln.Links.Add(cn);
            cn.Links.Add(ln);
        }
    }
    return res;
}

Node GetOrAddNode(List<Node> res, string l)
{
    var ln = res.SingleOrDefault(n => n.Name == l);
    if (ln == null)
    {
        ln = new Node { Name = l };
        res.Add(ln);
    }
    return ln;
}


[DebuggerDisplay("{Name}")]
class Node
{
    public string Name { get; set; }
    public List<Node> Links { get; set; } = new List<Node>();
    public Node CameFrom { get; set; }

    public int Distance { get; set; }
    
    public List<Node> KeepLinks { get; set; } = new List<Node>();

    public bool Visited;
    public int VisCount;
}

class Link
{
    public Node From;
    public Node To;
    public bool Used;
}

class LinkD
{
    public string From;
    public string To;
    public int UseCnt;
}