namespace Antireversi {
    internal class Gameplay {
        public static void PlayOthello(Board board, AbstractPlayer black, AbstractPlayer white) {
            var current = black;
            var opponent = white;
            var passed = false;
            
            while (board.ValidPositions.Count > 0) {
                var move = current.Move(board);
                
                if (move == "pass") {
                    if (passed)
                        break;
                    passed = true;
                } else {
                    board.MakeMove(Board.DecodeMove(move), current.Symbol);
                    passed = false;
                }
                
                var temp = current;
                current = opponent;
                opponent = temp;
            }
        }
    }
}