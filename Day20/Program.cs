using System.Diagnostics;

var data = File.ReadAllLines("example.txt");
data = File.ReadAllLines("input.txt");

List<Node> nodes = new List<Node>();
nodes.Add(new Node { Name = "button", OutputsStr = "broadcaster", Operation = 'b' });
foreach (var line in data)
{
    var s = line.Split("->").Select(x => x.Trim()).ToArray();
    var n = new Node();
    string name = s[0] == "broadcaster" ? s[0] : s[0].Substring(1);
    n.Operation = name != "broadcaster" ? s[0][0] : '=';
    n.Name = name;
    n.OutputsStr = s[1];
    nodes.Add(n);
}
nodes.Add(new Node { Name = "output" });
var nullNode = new Node { Name = "rx", Operation = 'Q' };
nodes.Add(nullNode);
// link
foreach (var node in nodes)
{
    var outputs = node.OutputsStr.Split(",", StringSplitOptions.RemoveEmptyEntries)
        .Select(x => x.Trim()).ToArray();
    foreach (var output in outputs)
    {
        var c = nodes.SingleOrDefault(n => n.Name == output) ?? nullNode;
        node.Outputs.Add(c);
        c.Inputs.Add(node);
    }
}

Console.WriteLine("Configuration map constructed");

//nodes = SortNodes(nodes);
nodes = nodes.OrderByDescending(x => PathToRx(x).Count()).ToList();
//var bottlenecks = GetBottlenecks(nodes);
/*foreach (var item in bottlenecks)
{
    Console.WriteLine("Bottleneck nodes: " + string.Join("; ", bottlenecks.Select(b => $"{b}: {nodes[b].Operation}{nodes[b].Name}")));
}*/
foreach (var node in nodes)
    Console.WriteLine(node.ToString());

var res = new Dictionary<bool, int> { { false, 0 }, { true, 0 } };
for (int i = 1; i <= 1000; i++)
{
    var v = MakePulse();
    res[false] = res[false] + v[false];
    res[true] = res[true] + v[true];
    /*foreach (var node in nodes) Console.Write(node.Value ? "1" : "_");
    Console.WriteLine();*/
    //Console.WriteLine($"Iteration {i}: low: {v[false]}, high {v[true]};  Total {res[false]} : {res[true]}");
}
Console.WriteLine("Task 1: " + res[false] * res[true]);

/*var complexity = nodes.Sum(x => x.Outputs.Count);
var reduced = GetRxNodes(nodes);
var complexity2 = reduced.Sum(x => x.Outputs.Count);*/

/*foreach (var n in nodes.OrderBy(x => x.Name))
{
    var inp = string.Join(", ", n.Inputs.Select(x => x.Name));
    var oup = string.Join(", ", n.Outputs.Select(x => x.Name));
    Console.WriteLine($"{n.Name}: {inp} -{n.Operation}-> {oup}");
}
*/

foreach (var node in GetCycles(nodes)) Console.WriteLine("Cycled: "  + node.ToString());

var cycles = GetCyclesRec(new List<Node> { nodes.Single(n => n.Name == "button") });
foreach (var path in cycles) Console.WriteLine("Cycle: " + string.Join("->", path.Select(x => x.Name)));
//no cycles!!!

var uncycled = nodes.Where(n => !cycles.Any(c => c.Contains(n)));
foreach (var n in uncycled) Console.WriteLine("Not in cycle: " + n.Name + " ... " + PathToRx(n).Count + " " + string.Join("->", PathToRx(n).Select(c => c.Name)));

//AnalyzeNodes(nodes);

var tnHist = new List<List<bool>>();
var nodeName = "fh";
var trackNode = nodes.Single(n => n.Name == nodeName);
for (int i = 1; i <= 10000; i++)
{
    trackNode.History = new List<bool>();
    var v = MakePulse();
    tnHist.Add(trackNode.History);
    /*foreach (var node in nodes) Console.Write(node.Value ? "1" : "_");
    Console.WriteLine();*/
    //Console.WriteLine($"Iteration {i}: low: {v[false]}, high {v[true]};  Total {res[false]} : {res[true]}");
}
var pattern = GetPattern(tnHist);
Console.WriteLine(string.Join("\n", pattern.Select(x => $"{x.Item1} ({x.Item2})")));

List<long> presses = new List<long>();
foreach (var startNode in nodes.SingleOrDefault(n => n.Name == "rx").Inputs.Single().Inputs)
{
    nodes.ForEach(nd => nd.Value = false);
    var num = 0;
    var start = DateTime.Now;
    do
    {
        num++;
        startNode.History = new List<bool>();
        MakePulse();
        if (num % 100000 == 0)
        {
            Console.WriteLine($"{num}, in {(DateTime.Now - start).TotalMilliseconds}ms");
            num = 0;
            start = DateTime.Now;
        }
    } while (!startNode.History.Any(h => !h));
    Console.WriteLine($"Found Low signal on [{nodeName}] after {num} pushes in {(DateTime.Now - start).TotalMilliseconds}ms");
    presses.Add(num);
}

long r = 1;
presses.ForEach(p => r *= p);
Console.WriteLine("Task 2: " + r);

Console.WriteLine("Finished");

List<Tuple<string, int>> GetPattern(List<List<bool>> tnHist)
{
    var res = new List<Tuple<string, int>>();
    string prev = null;
    int cnt = 0;
    foreach (var n in tnHist)
    {
        var s = n.Count > 0 ? string.Join("", n.Select(x => x ? "1" : "0")) : "-";
        if (prev == s || (prev?.Contains("0") == false && !s.Contains("0"))) cnt++;
        else
        {
            res.Add(new Tuple<string, int>(prev, cnt));
            prev = s;
            cnt = 1;
        }
    }
    res.Add(new Tuple<string, int>(prev, cnt));
    return res;
}


List<Node> PathToRx(Node n)
{
    var visited = new List<Node>();
    var q = new Queue<Node>();
    q.Enqueue(n);
    while (q.Count > 0)
    {
        var nd = q.Dequeue();
        visited.Add(nd);
        if (nd.Name == "rx")
        {
            var p = nd;
            var path = new List<Node> { p };
            while (p != n)
            {
                p = visited.FirstOrDefault(v => p.Inputs.Contains(v));
                path.Insert(0, p);
            }
            return path;
        }
        foreach (var o in nd.Outputs.Where(x => !visited.Contains(x)))
        {
            q.Enqueue((Node)o);
        }
    }
    return new List<Node>();
    //throw new Exception("Path not found");
}


List<Node> SortNodes(List<Node> nodes)
{
    var res = new List<Node> { nodes.Single(n => n.Name == "button") };
    var i = 0;
    while (i < nodes.Count)
    {
        foreach (var item in nodes[i].Outputs.Where(x => !res.Contains(x)))
            res.Add(item);
        i++;
    }
    return res;

    /*foreach (var node in nodes) node.Order = -1;
    var q = new Queue<Node>();
    q.Enqueue(nodes[0]);
    while (q.Count > 0)
    {
        var n = q.Dequeue();
        n.Order = n.Inputs.Any() ? n.Inputs.Select(i => i.Order).Max() + 1 : 0;
        foreach (var item in n.Outputs)
        {
            if (!q.Contains(item) && item.Order <= n.Order)
            {
                q.Enqueue(item);
                if (item.Order > 0)
                    Console.WriteLine($"{n.Name} -> {item.Name}");
            }
        }
    }
    return nodes.OrderBy(i => i.Order).ToList();*/
}

/*List<int> GetBottlenecks(List<Node> nodes)
{
    var res = new List<int>();
    for (var i = 1; i < nodes.Count - 1; i++)
    {
        var prev = nodes.Take(i).ToList();
        var next = nodes.Skip(i+1).ToList();
        if (!prev.Any(n => n.Outputs.Any(o => next.Contains(o))))
            res.Add(i);
    }
    return res;
}*/



List<Node> GetCycles(List<Node> nodes)
{
    var res = new List<Node>();
    var q = new Queue<Node>();
    q.Enqueue(nodes.Single(n => n.Name == "button"));
    var reached = new List<Node>();
    while (q.Count > 0)
    {
        var next = q.Dequeue();
        reached.Add(next);
        foreach (var item in next.Outputs)
        {
            if (reached.Contains(item))
            {
                res.Add(item);
            }
            else
                q.Enqueue(item);
        }
    }
    return res.Distinct().ToList();
}

List<List<Node>> GetCyclesRec(List<Node> path)
{
    var res = new List<List<Node>>();
    var l = path.Last();
    foreach (var nx in l.Outputs)
    {
        if (path.Contains(nx))
        {
            res.Add(path.Skip(path.IndexOf(nx)).ToList());
        }
        else
        {
            path.Add(nx);
            res.AddRange(GetCyclesRec(path));
            path.Remove(nx);
        }
    }
    return res;
}

bool HasCycle(Node n)
{
    var q = new Queue<Node>();
    q.Enqueue(n);
    while (q.Count > 0)
    {
        var next = q.Dequeue();
        foreach (var item in next.Outputs)
        {
            if (item == n) return true;
            q.Enqueue(item);
        }
    }
    return false;
}



/*void AnalyzeNodes(List<Node> nodes)
{
    var q = new Queue<Node>();
    q.Enqueue(nodes.Single(x => x.Name == "button"));

    while (q.Count > 0)
    {
        AnalyzeNode(q, q.Dequeue());
    }
}

void AnalyzeNode(Queue<Node> q, Node node)
{
    switch (node.Operation)
    {
        case 'b':
            node.schedule.Schedules.Add(new ScheduleItem
            {
                Repetitions = 1,
                Value = false
            });
            break;
        default:
            break;
    }

}*/

/*List<Node> GetRxNodes(List<Node> nodes)
{
    var res = new List<Node>();
    var q = new Queue<Node>();

    var rx = nodes.Single(x => x.Name == "rx");
    res.Add(rx);

    q.Enqueue(rx);

    while (q.Count > 0)
    {
        var n = q.Dequeue();
        foreach (var input in n.Inputs)
        {
            var e = res.SingleOrDefault(x => x.Name == input.Name);
            if (e == null)
            {
                e = new Node
                {
                    Name = input.Name,
                    Inputs = input.Inputs.ToList(),
                    Operation = input.Operation,
                    Outputs = new List<Node>() // no tracking of unused inputs
                };
                res.Add(e);
                q.Enqueue(e);
            }
            e.Outputs.Add(n);
        }
    }

    return res;
}*/

Dictionary<bool, int> MakePulse()
{
    var res = new Dictionary<bool, int> { { false, 0 }, { true, 0 } };
    var q = new Queue<Signal>();
    var start = nodes.Single(n => n.Name == "button");
    var inits = new Signal { From = start, To = start.Outputs[0], Value = false };
    res[inits.Value] = res[inits.Value] + 1;
    q.Enqueue(inits);
    var i = 0;
    while (q.Count > 0)
    {
        var s = q.Dequeue();
        var signals = s.To.ProcessSignal(s);
        foreach (var sign in signals)
        {
            res[sign.Value] = res[sign.Value] + 1;
            q.Enqueue(sign);
        }
    }
    return res;
}

[DebuggerDisplay("{ToString()}")]
class Node
{
    public string Name { get; set; }
    public char Operation = '-';
    public List<Node> Inputs = new List<Node>();
    public bool Value = false;
    public List<Node> Outputs = new List<Node>();
    public string OutputsStr = "";

    public Schedule schedule = new Schedule();

    public List<bool> History = new List<bool>();
    public int Order;

    public IEnumerable<Signal> ProcessSignal(Signal s) {
        //Console.WriteLine(s.ToString());
        History.Add(s.Value);
        switch (Operation)
        {
            case '-':
                return new List<Signal>();
            case '=':
                Value = s.Value;
                break;
            case '%':
                if (s.Value) return new List<Signal>();
                Value = !Value;
                break;
            case '&':
                var inputs = Inputs.Select(i => i.Value);
                Value = !inputs.All(i => i);
                break;
            case 'Q':
                if (!s.Value)
                {
                    Console.WriteLine("rx received LOW");
                }
                //Console.WriteLine("rx received HIGH");
                return new List<Signal>();
            default:
                throw new NotImplementedException();
        }
        return Outputs.Select(x => new Signal { From = this, To = x, Value = Value });
    }

    public override string ToString()
    {
        var ins = string.Join(", ", Inputs.Select(x => x.Name));
        var outs = string.Join(", ", Outputs.Select(x => x.Name));
        return $"{Order}: {Name}: {ins} --{Operation}-> {outs}";
    }
}

class Schedule
{
    public List<ScheduleItem> Schedules = new List<ScheduleItem>();
}
class ScheduleItem
{
    public int Repetitions;
    public Schedule Pattern = null;
    public bool? Value = null;
}

class Signal
{
    public Node From;
    public Node To;
    public bool Value;
    public override string ToString()
    {
        var v = Value ? "high" : "low";
        return $"{From.Name} -{v}-> {To.Name}";
    }
}
