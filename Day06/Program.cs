var input = File.ReadAllLines("example.txt");
input = File.ReadAllLines("input.txt");
input = File.ReadAllLines("input2.txt");

var times = input[0].Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)).ToArray();
var dists = input[1].Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)).ToArray();
var roots = new double[times.Length, 2];
long res = 1;
// h(t-h)=l
// ht-h2=l
// h2 -ht + l = 0
for (var i = 0; i < times.Length; i++)
{
    var t = times[i];
    var l = dists[i];
    double r1 = (t - Math.Sqrt(t * t - 4 * l)) / 2;
    double r2 = (t + Math.Sqrt(t * t - 4 * l)) / 2;
    roots[i, 0] = Math.Ceiling(r1) + (Math.Round(r1) == r1 ? 1 : 0);
    roots[i, 1] = Math.Floor(r2) - (Math.Round(r2) == r2 ? 1 : 0);
    var ways = (long)(roots[i, 1] - roots[i, 0]) + 1;
    res *= ways;
}

Console.WriteLine(res);

/*
 * Since the current record for this race is 9 millimeters, 
 * there are actually 4 different ways you could win: you could hold the button for 2, 3, 4, or 5 milliseconds at the start of the race.

In the second race, you could hold the button for at least 4 milliseconds and at most 11 milliseconds and beat the record, a total of 8 different ways to win.

In the third race, you could hold the button for at least 11 milliseconds and no more than 19 milliseconds and still beat the record, a total of 9 ways you could win.
 * */