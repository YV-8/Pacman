using PacmanSolution.ViewModels;

namespace PacmanSolution.Models;

public class Board:Entity
{
    public Board(int row, int col, EntityType type) 
        : base(row, col, type, GameConfig.CellWidth, GameConfig.CellHeight, 1) 
    { }

    public override void Update(double deltaTime) 
    { 
        // Las celdas del tablero normalmente no cambian cada frame
    }
    public readonly string[] Layout = {
        "WWWWWWWWWWWWWWWWWWWWWWWWWWWW",
        "W............WW............W",
        "W.WWWW.WWWWW.WW.WWWWW.WWWW.W",
        "WoWWWW.WWWWW.WW.WWWWW.WWWWoW",
        "W.WWWW.WWWWW.WW.WWWWW.WWWW.W",
        "W..........................W",
        "W.WWWW.WW.WWWWWWWW.WW.WWWW.W",
        "W.WWWW.WW.WWWWWWWW.WW.WWWW.W",
        "W......WW....WW....WW......W",
        "WWWWWW.WWWWW WW WWWWW.WWWWWW",
        "EEEEEW.WWWWW WW WWWWW.WEEEEE",
        "EEEEEW.WW     G    WW.WEEEEE",
        "EEEEEW.WW WWW--WWW WW.WEEEEE",
        "WWWWWW.WW W      W WW.WWWWWW",
        "E     .EE W GGG  W EE.     E",
        "WWWWWW.WW W      W WW.WWWWWW",
        "EEEEEW.WW WWWWWWWW WW.WEEEEE",
        "EEEEEW.WW          WW.WEEEEE",
        "EEEEEW.WW WWWWWWWW WW.WEEEEE",
        "WWWWWW.WW WWWWWWWW WW.WWWWWW",
        "W............WW............W",
        "W.WWWW.WWWWW.WW.WWWWW.WWWW.W",
        "W.WWWW.WWWWW.WW.WWWWW.WWWW.W",
        "Wo..WW.......P........WW..oW",
        "WWW.WW.WW.WWWWWWWW.WW.WW.WWW",
        "WWW.WW.WW.WWWWWWWW.WW.WW.WWW",
        "W......WW....WW....WW......W",
        "W.WWWWWWWWWW.WW.WWWWWWWWWW.W",
        "W.WWWWWWWWWW.WW.WWWWWWWWWW.W",
        "W..........................W",
        "WWWWWWWWWWWWWWWWWWWWWWWWWWWW"
    };
}