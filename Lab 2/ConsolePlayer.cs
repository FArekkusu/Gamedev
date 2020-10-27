using System;

namespace Antireversi {
    internal class ConsolePlayer : AbstractPlayer {
        public ConsolePlayer(CellValue symbol) : base(symbol) {}

        public override string Move(Board board) {
            return Console.ReadLine();
        }
    }
}