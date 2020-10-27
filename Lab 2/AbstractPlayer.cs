namespace Antireversi {
    internal abstract class AbstractPlayer {
        public readonly CellValue Symbol;

        protected AbstractPlayer(CellValue symbol) {
            Symbol = symbol;
        }

        public abstract string Move(Board board);
    }
}