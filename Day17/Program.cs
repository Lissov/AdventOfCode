var data = File.ReadAllLines("example.txt");
data = File.ReadAllLines("input.txt");

var lossMap = new int[data.Length, data[0].Length];
for (int i = 0; i < data.Length; i++)
    for (int j = 0; j < data[i].Length; j++)
        lossMap[i, j] = data[i][j] - '0';

var minLoss = GetMinLoss(lossMap, new Tuple<int, int>(0, 0), minMove: 1,maxMove: 3);
Console.WriteLine("Task 1: " + minLoss);

var minLoss2 = GetMinLoss(lossMap, new Tuple<int, int>(0, 0), minMove: 4, maxMove: 10);
Console.WriteLine("Task 2: " + minLoss2);

int GetMinLoss(int[,] data, Tuple<int, int> start, int minMove, int maxMove)
{
    int rowC = data.GetLength(0);
    int colC = data.GetLength(1);

    // 0 - horizontal, 1 - vertical
    var losses = new int[rowC, colC, 2];
    for (var x = 0;  x < rowC; x++)
        for (var y = 0; y < colC; y++)
            losses[x, y, 0] = losses[x, y, 1] = int.MaxValue;
    losses[0, 0, 0] = losses[0, 0, 1] = 0;

    var queue = new List<Tuple<int, int, int>>
    {
        new Tuple<int, int, int> ( start.Item1, start.Item2, 0 ),
        new Tuple<int, int, int> ( start.Item1, start.Item2, 1 )
    };
    var i = 0;

    while (i < queue.Count)
    {
        var q = queue[i];
        var dir = q.Item3;
        var dr = dir;
        var dc = 1 - dir;
        foreach (int delta in new[] {-1, 1})
        {
            var curr = losses[q.Item1, q.Item2, 1 - dir];
            for (var j = 1; j <= maxMove; j++)
            {
                var r = q.Item1 + j * dr * delta;
                var c = q.Item2 + j * dc * delta;
                if (r < 0 || r >= rowC || c < 0 || c >= colC) break;
                curr += data[r, c];
                if (j < minMove) continue;
                if (curr < losses[r, c, dir])
                {
                    losses[r, c, dir] = curr;
                    queue.Add(new Tuple<int, int, int>(r, c, 1 - dir));
                }
            }

        }
        i++;
    }

    return Math.Min(losses[rowC-1, colC-1, 0], losses[rowC - 1, colC - 1, 1]);
}
