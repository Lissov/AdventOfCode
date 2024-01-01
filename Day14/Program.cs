var data = File.ReadAllLines("example.txt");
data = File.ReadAllLines("input.txt");

/*int total = 0;
for (int col = 0; col < data[0].Length; col++)
{
    int colT = 0;
    int mx = data.Length;
    for (int row = 0; row < data.Length; row++)
    {
        switch (data[row][col])
        {
            case 'O':
                colT += mx;
                mx--;
                break;
            case '#':
                mx = data.Length - row - 1;
                break;
        }
    }
//    Console.WriteLine($"Col {col}: " + colT);
    total += colT;
}*/
var dt = data.Select(x => x.ToArray()).ToArray();
MoveUp(dt);
var total = GetLoad(dt);

Console.WriteLine(total);

var totalRotations = 1000000000;

dt = data.Select(x => x.ToArray()).ToArray();
var moved = new List<char[][]>();
moved.Add(dt);
char[][] cycled = dt;
var cont = true;
PrintArr(dt); 
while (cont)
{
    Console.WriteLine(moved.Count);
    cycled = Cycle(cycled);
    PrintArr(cycled);
    var sameIndex = GetSameIndex(moved, cycled);
    if (sameIndex != -1)
    {
        Console.WriteLine($"Found cycle {sameIndex} - {moved.Count}");
        var cycleLength = moved.Count - sameIndex;
        var index = (totalRotations - sameIndex) % (cycleLength) + sameIndex;
        var load = GetLoad(moved[index]);
        Console.WriteLine("Final:");
        PrintArr(moved[index]);
        Console.WriteLine(load);
        return;
    }
    moved.Add(cycled);
}
Console.WriteLine(moved.Count);

int GetSameIndex(List<char[][]> moved, char[][] cycled)
{
    return moved.FindIndex(x =>
    {
        for (int r = 0; r < cycled.Length; r++)
            for (int c = 0; c < cycled[0].Length; c++)
                if (x[r][c] != cycled[r][c]) return false;
        return true;
    });
}

PrintArr(cycled);

char[][] Cycle(char[][] input)
{
    var clone = input.Select(x => x.ToArray()).ToArray();
    MoveUp(clone);
    MoveLeft(clone);
    MoveDown(clone);
    MoveRight(clone);
    return clone;
}
void MoveUp(char[][] input)
{
    for (int col = 0; col < input[0].Length; col++)
    {
        var p = 0;
        for (int row = 0; row < input.Length; row++)
        {
            switch (input[row][col])
            {
                case '#': p = row+1; break;
                case 'O':
                    if (p < row)
                    {
                        input[p][col] = 'O';
                        input[row][col] = '.';
                    }
                    p++;
                    break;
            }
        }
    }
}

void MoveDown(char[][] input)
{
    for (int col = 0; col < input[0].Length; col++)
    {
        var p = input.Length - 1;
        for (int row = input.Length - 1; row >= 0; row--)
        {
            switch (input[row][col])
            {
                case '#': p = row - 1; break;
                case 'O':
                    if (p > row)
                    {
                        input[p][col] = 'O';
                        input[row][col] = '.';
                    }
                    p--;
                    break;
            }
        }
    }
}

void MoveRight(char[][] input)
{
    for (int row = 0; row < input.Length; row++)
    {
        var p = input[row].Length-1;
        for (int col = input[row].Length-1; col >= 0; col--)
        {
            switch (input[row][col])
            {
                case '#': p = col - 1; break;
                case 'O':
                    if (p > col)
                    {
                        input[row][p] = 'O';
                        input[row][col] = '.';
                    }
                    p--;
                    break;
            }
        }
    }
}

void MoveLeft(char[][] input)
{
    for (int row = 0; row < input.Length; row++)
    {
        var p = 0;
        for (int col = 0; col < input[row].Length; col++)
        {
            switch (input[row][col])
            {
                case '#': p = col + 1; break;
                case 'O':
                    if (p < col)
                    {
                        input[row][p] = 'O';
                        input[row][col] = '.';
                    }
                    p++;
                    break;
            }
        }
    }
}

int GetLoad(char[][] dt)
{
    var t = 0;
    var rc = dt.Length;
    for (int col = 0; col < dt[0].Length; col++)
    {
        for (int row = 0; row < rc; row++)
            if (dt[row][col] == 'O') t += (rc - row);
    }
    return t;
}

void PrintArr(char[][] dt)
{
    Console.WriteLine();
    for (int i = 0; i < dt.Length; i++)
        Console.WriteLine(string.Join("", dt[i]));
}