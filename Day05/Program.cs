using System.Diagnostics;

var input = File.ReadAllLines("example.txt");
input = File.ReadAllLines("input.txt");

var seedsNums = input[0].Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)).ToList();
var seeds = new List<StepMap>();

for (int i = 0; i < seedsNums.Count(); i+=2)
{
    var r = new SeedRange
    {
        Start = seedsNums[i],
        End = seedsNums[i] + seedsNums[i+1]-1
    };
    seeds.Add(new StepMap { Source = r, Destination = r });
}

for  (var i = 1; i < input.Length; i++)
{
    if (string.IsNullOrWhiteSpace(input[i]))
    {
        ApplyMap();
        continue;
    }
    if (input[i].Contains(':')) continue;
    var onemap = ParseMap(input, i);

    var n = 0;
    while (n < seeds.Count)
    {
        var mapped = onemap.GetMapped(seeds[n]);
        seeds[n] = mapped.First();
        seeds.AddRange(mapped.Skip(1));
        n++;
    }
}

var minIndex = seeds.Min(x => x.Destination.Start);
Console.WriteLine(minIndex);

void ApplyMap()
{
    foreach (var item in seeds)
    {
        item.Source = item.Destination;
    }
}

static StepMap ParseMap(string[] input, int i)
{
    var x = input[i].Split(' ').Select(x => long.Parse(x)).ToList();
    var l = x[2];
    return new StepMap {
        Source = new SeedRange { Start = x[1], End = x[1] + l - 1 },
        Destination = new SeedRange { Start = x[0], End = x[0] + l - 1 },
    };
}

[DebuggerDisplay("{Start} - {End}")]
struct SeedRange
{
    public long Start;
    public long End;
}

[DebuggerDisplay("{Source} -> {Destination}")]
class StepMap
{
    public SeedRange Source;
    public SeedRange Destination;

    internal List<StepMap> GetMapped(StepMap seed)
    {
        var overlaps = seed.Source.End >= Source.Start && seed.Source.Start <= Source.End;
        if (!overlaps) return new List<StepMap>(new[] { seed });
        var res = new List<StepMap>();
        var curr = new SeedRange {
            Start = Math.Max(seed.Source.Start, Source.Start),
            End = Math.Min(seed.Source.End, Source.End)
        };
        var strt = Destination.Start + curr.Start - Source.Start;
        var mpd = new SeedRange
        {
            Start = strt,
            End = strt + curr.End - curr.Start
        };

        res.Add(new StepMap { Source = curr, Destination = mpd });
        if (Source.Start > seed.Source.Start)
        {
            var range = new SeedRange { Start = seed.Source.Start, End = Source.Start - 1 };
            res.Add(new StepMap { Source = range, Destination = range });
        }

        if (Source.End < seed.Source.End)
        {
            var range = new SeedRange { Start = Source.End + 1, End = seed.Source.End };
            res.Add(new StepMap { Source = range, Destination = range });
        }

        return res;
    }
}