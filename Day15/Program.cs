var input = File.ReadAllLines("example.txt")[0];
input = File.ReadAllLines("input.txt")[0];

var lenses = input.Split(',');

var res = lenses.Select(GetHash).Sum();

Console.WriteLine("Task 1: " + res);

var lens = lenses.Select(l => new Lens(l));
var groups = lens.GroupBy(x => GetHash(x.Key));

var totalFocal = 0;
foreach (var group in groups)
{
    //Console.Write(group.Key + ": ");

    //Console.WriteLine(string.Join(',', group));
    var slots = new List<Lens>();
    foreach (var item in group)
    {
        if (item.FocalLength < 0)
            slots.RemoveAll(x => x.Key == item.Key);
        else
        {
            var ex = slots.SingleOrDefault(x => x.Key == item.Key);
            if (ex != null)
                ex.FocalLength = item.FocalLength; // we don't need this item, can easily modify
            else
                slots.Add(item);
        }
    }

    var sum = 0;
    //Console.WriteLine("\t" + string.Join(", ", slots.Select(s => s.Key + " " + s.FocalLength)));
    for (var i = 0; i < slots.Count; i++)
        sum += (group.Key + 1) * (i + 1) * slots[i].FocalLength;
    //Console.WriteLine(sum);
    totalFocal += sum;
}

Console.WriteLine("Result task 2: " + totalFocal);

int GetHash(string l)
{
    int hashOne = 0;
    foreach (char c in l)
    {
        hashOne = ((hashOne + c) * 17) % 256;
    }
    return hashOne;
}

internal class Lens
{
    public string Key { get; set; }
    public int FocalLength { get; set; }

    public Lens(string input)
    {
        if (input.EndsWith('-')) {
            Key = input.Substring(0, input.Length - 1);
            FocalLength = -1;
            return;
        }
        var s = input.Split('=');
        Key = s[0];
        FocalLength += int.Parse(s[1]);
    }
}
