using System;

namespace Antireversi {
    internal class MinimaxPlayer : AbstractPlayer {
        private const int MaxDepth = 7;
        private const double Inf = double.PositiveInfinity;

        public MinimaxPlayer(CellValue symbol) : base(symbol) {}

        public override string Move(Board board) {
            var moves = board.FindMoves(Symbol);
            var move = "pass";
            
            if (moves.Count == 1)
                move = Board.EncodeMove(moves[0]);
            else if (moves.Count > 1)
                move = Board.EncodeMove(Alphabeta.Search(board, MaxDepth, Symbol, -Inf, Inf, 0).Item1);
            
            Console.WriteLine(move);
            return move;
        }
    }
}