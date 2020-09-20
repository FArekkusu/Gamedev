EMPTY = " "
MARK = "x"

DIRECTIONS = [(dx, dy) for dx in range(-1, 2) for dy in range(-1, 2) if dx or dy]

class Board:
    def __init__(self, black_symbol, white_symbol):
        self.state = [[EMPTY] * 10 for _ in range(10)]
        self.state[4][5] = self.state[5][4] = black_symbol
        self.state[4][4] = self.state[5][5] = white_symbol
        self.valid_positions = [(x, y) for x in range(1, 9) for y in range(1, 9) if self.state[x][y] == EMPTY]
    
    def __str__(self):
        top = "    " + "   ".join("ABCDEFGH")
        line = "-" * 33
        rows = f"\n  {line}\n".join(f"{i} | {' | '.join(x[1:-1])} |" for i, x in enumerate(self.state[1:-1], 1))
        return f"{top}\n  {line}\n{rows}\n  {line}"
    
    def stringify_with_moves(self, moves):
        moves = [(int(x), ord(y) - 64) for y, x in moves]
        for x, y in moves:
            self.state[x][y] = MARK
        result = str(self)
        for x, y in moves:
            self.state[x][y] = EMPTY
        return result

    def find_moves(self, player_symbol):
        moves = []
        for x, y in self.valid_positions:
            for dx, dy in DIRECTIONS:
                if self.state[x][y] == EMPTY and self.line_exists(player_symbol, x, y, dx, dy):
                    moves.append(f"{chr(y + 64)}{x}")
                    break
        return moves
    
    def line_exists(self, player_symbol, x, y, dx, dy):
        X = x
        Y = y
        while True:
            X += dx
            Y += dy
            if self.state[X][Y] == EMPTY:
                return False
            if self.state[X][Y] == player_symbol:
                return X - x != dx or Y - y != dy
    
    def make_move(self, move, player_symbol):
        x = int(move[1])
        y = ord(move[0]) - 64
        self.state[x][y] = player_symbol
        for dx, dy in DIRECTIONS:
            if self.line_exists(player_symbol, x, y, dx, dy):
                X = x
                Y = y
                while True:
                    X += dx
                    Y += dy
                    if self.state[X][Y] == player_symbol:
                        break
                    self.state[X][Y] = player_symbol
    
    def calculate_total(self, black_symbol, white_symbol):
        black_total = sum(x.count(black_symbol) for x in self.state)
        white_total = sum(x.count(white_symbol) for x in self.state)
        return (black_total, white_total)
