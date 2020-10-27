using System;

namespace Antireversi {
    internal class Program {
        private static void Main(string[] args) {
            AbstractPlayer blackPlayer;
            AbstractPlayer whitePlayer;
            
            var blackHolePosition = Console.ReadLine();
            var color = Console.ReadLine();
            
            if (color == "black") {
                blackPlayer = new MinimaxPlayer(CellValue.Black);
                whitePlayer = new ConsolePlayer(CellValue.White);
            } else {
                blackPlayer = new ConsolePlayer(CellValue.Black);
                whitePlayer = new MinimaxPlayer(CellValue.White);
            }
            
            var board = new Board();
            board.AddBlackHole(Board.DecodeMove(blackHolePosition));
            
            Gameplay.PlayOthello(board, blackPlayer, whitePlayer);
        }
    }
}