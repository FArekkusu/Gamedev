using System.Linq;

namespace Antireversi {
    internal class Alphabeta {
        private const double VictoryPoints = 1_000_000;
        
        public static ((int, int), double) Search(Board s, int ply, CellValue player, double low, double high, int passes) {
            if (ply == 0 || passes == 2)
                return ((0, 0), EvaluateState(s, player, passes));
            
            var bestMove = (0, 0);
            var bestScore = low;
            var moves = s.FindMoves(player);
            var opponent = player.Opposite();
            
            if (moves.Count == 0)
                return Search(s, ply, opponent, -high, -low, passes + 1);
            
            var sortedMoves = s.ValidPositions.Count < 32 ?
                moves.OrderBy(m => SumDistances(new Board(s, m, player), player, opponent)) :
                moves.OrderBy(m => DistSquared(m.Item1 - 4.5, m.Item2 - 4.5));
            
            foreach (var m in sortedMoves) {
                var nextS = new Board(s, m, player);
                var score = Search(nextS, ply - 1, opponent, -high, -low, 0).Item2;
                
                if (-score > low) {
                    low = -score;
                    bestMove = m;
                    bestScore = low;
                }

                if (low >= high)
                    return (bestMove, bestScore);
            }

            return (bestMove, bestScore);
        }
        
        private static double EvaluateState(Board board, CellValue player, int passes) {
            var opponent = player.Opposite();
            var playerTotal = board.Count(player);
            var opponentTotal = board.Count(opponent);
            var positionsLeft = board.ValidPositions.Count;
            
            if (passes == 2 || positionsLeft == 0)
                return playerTotal < opponentTotal ? VictoryPoints : -VictoryPoints;
            
            if (positionsLeft < 20)
                return opponentTotal - playerTotal;
            
            return SumDistances(board, opponent, player);
        }

        private static double DistSquared(double x, double y) {
            return x * x + y * y;
        }

        private static double SumDistances(Board board, CellValue pos, CellValue neg) {
            double distanceTotal = 0;
            
            for (var i = 1; i <= 8; i++)
                for (var j = 1; j <= 8; j++)
                    if (board.State[i, j] == pos)
                        distanceTotal += DistSquared(i - 4.5, j - 4.5);
                    else if (board.State[i, j] == neg)
                        distanceTotal -= DistSquared(i - 4.5, j - 4.5);
            
            return distanceTotal;
        }
    }
}