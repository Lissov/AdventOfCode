using System.Diagnostics;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("example.txt");
            //input = File.ReadAllLines("input.txt");

            var allWorkflows = new List<Workflow>();
            var parts = new List<Part>();
            bool isWf = true;
            foreach (var line in input)
            {
                if (string.IsNullOrEmpty(line)) isWf = false;
                else if (isWf) AddWf(allWorkflows, line);
                else AddPart(parts, line);
            }

            Console.WriteLine("Min RL: " + allWorkflows.Min(x => x.Rules.Length));

            var nodes = new List<Node>();
            var terminalnodes = new List<Node>();
            GetNode("in", allWorkflows, nodes, new Descript(), terminalnodes);
            var aNodes = terminalnodes.Where(t => t.Name == "A").ToArray();
            aNodes = MergeNodes(aNodes).ToArray();
            Console.WriteLine("A-nodes: " + aNodes.Count());

            long res = 0;
            foreach (var part in parts)
            {
                if (aNodes.Any(a => a.Input.IsInside(part.Values)))
                {
                    res += part.Values.Sum();
                }
            }
            Console.WriteLine("Task 1: " + res);

            long r2 = GetCombinations(aNodes);
            Console.WriteLine("Task 2: " + r2);
        }

        private static long GetCombinations(Node[] aNodes)
        {
            var g = aNodes.Where(x => aNodes.Any(y => y != x
                && y.Input.OverlapsWith(x.Input)));

            Console.WriteLine("Overlaps found: " + g.Count());

            if (g.Count() == 0)
                return aNodes.Sum(n => n.Input.GetPower());

            return -1;
        }

        private static IEnumerable<Node> MergeNodes(IEnumerable<Node> aNodes)
        {
            var a = aNodes.ToList(); // clone
            var r = new List<Node>();
            while (a.Count > 0)
            {
                var n = a[0];
                var touching = a.FirstOrDefault(x => x != n && x.Input.TouchIndex(n.Input) > 0);
                if (touching != null)
                {
                    r.Add(new Node { Name = n.Name, Input = n.Input.Merge(touching.Input) });
                }
                else
                {
                    r.Add(n);
                }
                a.RemoveAll(x => x == touching || x == n);
            }
            if (r.Count() == aNodes.Count())
                return r;
            else
                return MergeNodes(r);
        }

        private static void AddWf(List<Workflow> workflows, string line)
        {
            var spl = line.Split(new[] { '{', '}', ',' }, StringSplitOptions.RemoveEmptyEntries);
            workflows.Add(new Workflow { Name = spl[0], Rules = spl.Skip(1).ToArray() });
        }

        //{x=787,m=2655,a=1222,s=2876}
        private static void AddPart(List<Part> parts, string line)
        {
            var spl = line.Split(new[] { '{', '}', ',' }, StringSplitOptions.RemoveEmptyEntries);
            var p = new Part();
            foreach (var s in spl)
            {
                var x = s.Split('=')[0];
                var v = int.Parse(s.Split('=')[1]);
                p.Values["xmas".IndexOf(x)] = v;
            }
            parts.Add(p);
        }

        private static Node GetNode(string name, List<Workflow> allWorkflows, List<Node> nodes, Descript input, List<Node> terminals)
        {
            var match = nodes.FirstOrDefault(n => n.Name == name && n.IsInside(input));
            if (match != null) return match;

            if (name == "A" || name == "R")
            {
                var t = new Node { Name = name, Input = input };
                terminals.Add(t);
                return t;
            }

            var w = allWorkflows.Single(w => w.Name == name);

            var node = new Node { Input = input, Name = name };
            nodes.Add(node);
            var fd = input;
            for (int i = 0; i < w.Rules.Length - 1; i++)
            {
                var s = w.Rules[i].Split(':');
                var c = new Condition(s[0]);
                var passDesc = fd.GetPass(c);
                fd = fd.GetFail(c);
                if (passDesc != null)
                    node.Map.Add(new Tuple<Descript, Node>(passDesc, GetNode(s[1], allWorkflows, nodes, passDesc, terminals)));
                if (fd == null)
                    return node;
            }
            if (fd != null)
                node.Map.Add(new Tuple<Descript, Node>(fd, GetNode(w.Rules[w.Rules.Length - 1], allWorkflows, nodes, fd, terminals)));

            return node;
        }
    }
}

//px{a<2006:qkq,m>2090:A,rfg}
class Workflow
{
    public string Name { get; set; }
    public string[] Rules { get; set; }
}

[DebuggerDisplay("{Name}: {Input} -> {Map.Count}")]
class Node
{
    public string Name { get; set; }
    public Descript Input { get; set; }

    public List<Tuple<Descript, Node>> Map = new List<Tuple<Descript, Node>>();

    internal bool IsInside(Descript input)
    {
        for (int i = 0; i < 4; i++)
            if (input.Bounds[i].Max > Input.Bounds[i].Max || input.Bounds[i].Min < Input.Bounds[i].Min)
                return false;
        return true;
    }

    public override string ToString()
    {
        return Name + ": " + Input.ToString();
    }
}

public class Condition
{
    public int Order { get; set; }
    public int Value { get; set; }
    public bool IsAbove { get; set; }

    public Condition(string line)
    {
        IsAbove = line.Contains('>');
        var s = line.Split(new[] { '<', '>' });
        Order = "xmas".IndexOf(s[0]);
        Value = int.Parse(s[1]);
    }
}

[DebuggerDisplay("x={Bounds[0]}, m={Bounds[1]}, a={Bounds[2]}, s={Bounds[3]}")]
class Descript
{
    public PropArea[] Bounds = new PropArea[4];
    public Descript()
    {
        Bounds = new[]
        {
            new PropArea(1, 4000),
            new PropArea(1, 4000),
            new PropArea(1, 4000),
            new PropArea(1, 4000)
        };
    }

    public override string ToString()
    {
        return $"x={Bounds[0]}, m={Bounds[1]}, a={Bounds[2]}, s={Bounds[3]}";
    }

    internal int TouchIndex(Descript anonter)
    {
        var r = -1;
        for (int i = 0; i < 4; i++)
            if (Bounds[i].Min != anonter.Bounds[i].Min || Bounds[i].Max != anonter.Bounds[i].Max)
            {
                if (r != -1) return -1;
                if (Bounds[i].Max == anonter.Bounds[i].Min - 1 || Bounds[i].Min == anonter.Bounds[i].Max + 1)
                    r = i;
                else
                    return -1;
            };
        return r;
    }

    internal Descript Merge(Descript another)
    {
        var d = new Descript();
        for (int i = 0; i < 4; i++)
        {
            d.Bounds[i] = new PropArea(Math.Min(Bounds[i].Min, another.Bounds[i].Min),
                Math.Max(Bounds[i].Max, another.Bounds[i].Max));
        }
        return d;
    }

    internal Descript GetPass(Condition condition)
    {
        var clone = new Descript { Bounds = this.Bounds.ToArray() };
        if (condition.IsAbove)
        {
            if (clone.Bounds[condition.Order].Min > condition.Value)
                return clone;
            if (clone.Bounds[condition.Order].Max <= condition.Value)
                return null;
            clone.Bounds[condition.Order].Min = condition.Value + 1;
            return clone;
        }
        else
        {
            if (clone.Bounds[condition.Order].Max < condition.Value)
                return clone;
            if (clone.Bounds[condition.Order].Min >= condition.Value)
                return null;
            clone.Bounds[condition.Order].Max = condition.Value - 1;
            return clone;
        }
    }
    internal Descript GetFail(Condition condition)
    {
        var clone = new Descript { Bounds = this.Bounds.ToArray() };
        if (condition.IsAbove)
        {
            if (clone.Bounds[condition.Order].Min > condition.Value)
                return null;
            if (clone.Bounds[condition.Order].Max <= condition.Value)
                return clone;
            clone.Bounds[condition.Order].Max = condition.Value;
            return clone;
        }
        else
        {
            if (clone.Bounds[condition.Order].Max < condition.Value)
                return null;
            if (clone.Bounds[condition.Order].Min >= condition.Value)
                return clone;
            clone.Bounds[condition.Order].Min = condition.Value;
            return clone;
        }
    }

    internal bool IsInside(int[] values)
    {
        for (int i = 0; i < 4; i++)
        {
            if (values[i] < Bounds[i].Min || values[i] > Bounds[i].Max)
                return false;
        }
        return true;
    }

    internal bool OverlapsWith(Descript input)
    {
        for (int i = 0; i < 4; i++)
        {
            if (Bounds[i].Max < input.Bounds[i].Min || Bounds[i].Min > input.Bounds[i].Max)
                return false;
        }
        return true;
    }

    public long GetPower()
    {
        checked
        {
            long r = 1;
            for (int i = 0; i < 4; i++)
                r = r * (long)((long)Bounds[i].Max - (long)Bounds[i].Min + (long)1);
            Console.WriteLine(r);
            return r;
        }
    }
}

[DebuggerDisplay("{Min}:{Max}")]
struct PropArea
{
    public int Min { get; set; }
    public int Max { get; set; }
    public PropArea(int min = int.MinValue, int max = int.MinValue)
    {
        this.Min = min;
        this.Max = max;
    }
}

class Part
{
    public int[] Values = new int[4];
}