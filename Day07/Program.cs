using System.Diagnostics;

var input = File.ReadAllLines("example.txt");
input = File.ReadAllLines("input.txt");

var cards = input.Select(i => new Hand(i))
    .ToList();
cards.Sort(CompareHands);
long res = 0;
for (int i = 0; i < cards.Count; i++)
    res += (i + 1) * cards[i].bet;
Console.WriteLine(res);

const string order = "J23456789TJQKA";

int CompareHands(Hand x, Hand y)
{
    if (x == y) return 0;
    if (x.type != y.type) return x.type - y.type;
    for (int i = 0; i < cards.Count; i++)
    {
        if (x.cards[i] != y.cards[i]) return order.IndexOf(x.cards[i]) - order.IndexOf(y.cards[i]);
    }
    return 0;
}

[DebuggerDisplay("{ToString()}")]
class Hand
{
    public int bet;
    public char[] cards;
    public HandType type;
    public Hand(string input)
    {
        var s = input.Split(' ');
        bet = int.Parse(s[1]);
        cards = s[0].ToCharArray();
        type = GetType(cards);
    }

    public override string ToString()
    {
        return string.Join("", cards) + "   " + type.ToString() + "   " + bet;
    }

    private HandType GetType(char[] cards)
    {
        var s = cards.OrderBy(x => x).ToArray();
        var n = -1;
        var c = new int[5];
        var cn = new char[5];
        for (int i = 0; i < 5; i++)
        {
            if (i == 0 || s[i] != s[i - 1])
            {
                n++;
            }
            cn[n] = s[i];
            c[n]++;
        }
        var joker = Array.IndexOf(cn, 'J');
        var jcount = 0;
        if (joker >= 0)
        {
            jcount = c[joker];
            c[joker] = 0;
        }

        c = c.OrderByDescending(c => c).ToArray();
        c[0] += jcount;
        if (c[0] == 5) return HandType.Five;
        if (c[0] == 4) return HandType.Four;
        if (c[0] == 3 && c[1] == 2) return HandType.FullHouse;
        if (c[0] == 3) return HandType.Three;
        if (c[0] == 2 && c[1] == 2) return HandType.TwoPair;
        if (c[0] == 2) return HandType.Pair;
        return HandType.HighCard;
    }
}

enum HandType
{
    HighCard = 0,
    Pair = 1,
    TwoPair = 2,
    Three = 3,
    FullHouse = 4,
    Four = 5,
    Five = 6,
}