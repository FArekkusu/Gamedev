using System.Collections.Generic;

namespace Antireversi {
    internal class Board {
        private static readonly List<(int, int)> Directions = new List<(int, int)> {
            (-1, -1), (-1, 0), (-1, 1),
            ( 0, -1), ( 0, 1),
            ( 1, -1), ( 1, 0), ( 1, 1)
        };

        public readonly CellValue[,] State;
        public readonly List<(int, int)> ValidPositions;

        public Board() {
            State = new CellValue[10, 10];
            for (var i = 0; i < 10; i++)
                for (var j = 0; j < 10; j++)
                    State[i, j] = CellValue.Empty;
            
            State[4, 5] = CellValue.Black;
            State[5, 4] = CellValue.Black;
            
            State[4, 4] = CellValue.White;
            State[5, 5] = CellValue.White;
            
            ValidPositions = new List<(int, int)>();
            for (var i = 1; i <= 8; i++)
                for (var j = 1; j <= 8; j++)
                    if (State[i, j] == CellValue.Empty)
                        ValidPositions.Add((i, j));
        }

        public Board(Board board, (int, int) move, CellValue c) {
            State = new CellValue[10, 10];
            for (var i = 0; i < 10; i++)
                for (var j = 0; j < 10; j++)
                    State[i, j] = board.State[i, j];
            
            ValidPositions = new List<(int, int)>(board.ValidPositions);
            
            MakeMove(move, c);
        }

        public static string EncodeMove((int, int) move) {
            var (x, y) = move;
            return $"{(char) (y + 64)}{x}";
        }

        public static (int, int) DecodeMove(string move) {
            var x = move[1] - '0';
            var y = move[0] - 64;
            return (x, y);
        }

        public List<(int, int)> FindMoves(CellValue c) {
            var moves = new List<(int, int)>();
            
            foreach (var (x, y) in ValidPositions)
                foreach (var (dx, dy) in Directions)
                    if (LineExists(c, x, y, dx, dy)) {
                        moves.Add((x, y));
                        break;
                    }
            
            return moves;
        }

        private bool LineExists(CellValue c, int x, int y, int dx, int dy) {
            var xx = x;
            var yy = y;
            
            while (true) {
                xx += dx;
                yy += dy;
                
                if (State[xx, yy] == CellValue.Empty || State[xx, yy] == CellValue.Blackhole)
                    return false;
                if (State[xx, yy] == c)
                    return xx - x != dx || yy - y != dy;
            }
        }

        public void MakeMove((int, int) move, CellValue c) {
            var (x, y) = move;
            State[x, y] = c;
            ValidPositions.Remove(move);
            
            foreach (var (dx, dy) in Directions)
                if (LineExists(c, x, y, dx, dy)) {
                    var xx = x;
                    var yy = y;
                    
                    while (true) {
                        xx += dx;
                        yy += dy;
                        
                        if (State[xx, yy] == c)
                            break;
                        
                        State[xx, yy] = c;
                    }
                }
        }

        public int Count(CellValue c) {
            var result = 0;
            
            for (var i = 1; i <= 8; i++)
                for (var j = 1; j <= 8; j++)
                    if (State[i, j] == c)
                        result++;
            
            return result;
        }

        public void AddBlackHole((int, int) pos) {
            var (x, y) = pos;
            State[x, y] = CellValue.Blackhole;
            ValidPositions.Remove(pos);
        }
    }
}