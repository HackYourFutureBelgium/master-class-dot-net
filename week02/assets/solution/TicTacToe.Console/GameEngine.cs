namespace TicTacToe.Console;

class GameEngine
{
    private readonly Board _board;
    private readonly Player _player1;
    private readonly Player _player2;
    private Player _currentPlayer;
    private GameStatus _status;

    public GameEngine(Player p1, Player p2, int boardSize)
    {
        _board = new Board(boardSize);
        _player1 = p1;
        _player2 = p2;
        _currentPlayer = _player1;
        _status = GameStatus.InProgress;
    }

    public void Start()
    {
        while (_status == GameStatus.InProgress)
        {
            System.Console.Clear();
            _board.Display();
            System.Console.Write($"{_currentPlayer.Name} ({_currentPlayer.Symbol}), enter a position (1-{_board.MaxPosition()}): ");
            var input = System.Console.ReadLine();

            if (!int.TryParse(input, out var position) || position < 1 || position > _board.MaxPosition())
            {
                System.Console.WriteLine("❌ Invalid input. Press Enter to try again.");
                System.Console.ReadLine();
                continue;
            }

            if (!_board.IsMoveValid(position))
            {
                System.Console.WriteLine("🚫 Position already taken. Press Enter to try again.");
                System.Console.ReadLine();
                continue;
            }

            _board.PlaceMove(position, _currentPlayer.Symbol);

            if (_board.CheckWin(_currentPlayer.Symbol))
            {
                _status = GameStatus.Win;
            }
            else if (_board.IsDraw())
            {
                _status = GameStatus.Draw;
            }
            else
            {
                SwitchPlayer();
            }
        }

        System.Console.Clear();
        _board.Display();

        if (_status == GameStatus.Win)
        {
            System.Console.WriteLine($"🎉 {_currentPlayer.Name} wins!");
            System.Console.Beep();
            _currentPlayer.AddWin();
        }
        else
        {
            System.Console.WriteLine("🤝 It's a draw!");
        }
    }

    private void SwitchPlayer()
    {
        _currentPlayer = _currentPlayer == _player1 ? _player2 : _player1;
    }
}
