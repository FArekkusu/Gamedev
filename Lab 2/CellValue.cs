namespace Antireversi {
    public enum CellValue {
        Black,
        White,
        Empty,
        Blackhole
    }

    public static class CellValueExtensions {
        public static CellValue Opposite(this CellValue value) {
            return value == CellValue.Black ? CellValue.White : CellValue.Black;
        }
    }
}