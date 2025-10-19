using Microsoft.AspNetCore.Mvc;
using TicTacToe.Core;

namespace TicTacToe.Web.Controllers;

public class GameController : Controller
{
    private readonly GameEngine _engine;

    public GameController(GameEngine engine)
    {
        _engine = engine;
    }

    public IActionResult New()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Start(string player1Name, string player2Name, int boardSize = 3)
    {
        var player1 = new Player(player1Name, 'X');
        var player2 = new Player(player2Name, 'O');
        _engine.SetPlayers(player1, player2);
        _engine.SetBoardSize(boardSize);
        return RedirectToAction("Play");
    }

    public IActionResult Play()
    {
        return View(_engine);
    }

    [HttpPost]
    public IActionResult MakeMove(int position)
    {
        if (_engine.Status != GameStatus.InProgress)
            return RedirectToAction("Play");

        _engine.TryPlayMove(position);

        return RedirectToAction("Play");
    }

    [HttpPost]
    public IActionResult Undo()
    {
        _engine.TryUndoLastMove();
        return RedirectToAction("Play");
    }
    
    [HttpGet]
    public IActionResult NewRound()
    {
        return View();
    }

    [HttpPost]
    public IActionResult StartNewRound(int boardSize)
    {
        _engine.SetBoardSize(boardSize);
        _engine.History.ClearHistory();
        return RedirectToAction("Play");
    }
}
