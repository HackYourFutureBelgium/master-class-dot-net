namespace TicTacToe.Console;

class Player
{
    public string Name { get; }
    public char Symbol { get; }
    public int Wins { get; private set; }

    public Player(string name, char symbol)
    {
        Name = name;
        Symbol = symbol;
        Wins = 0;
    }

    public void AddWin() => Wins++;
}
