from ui import display_board
from exceptions import GiveUpException

def make_console_move(player, board):
    moves = board.find_moves(player.symbol)
    display_board(board, moves)
    if not moves:
        return
    print(f"\n{player.color} possible moves: {', '.join(moves)}")
    while True:
        move = input("> ").upper()
        if move == "GIVE UP":
            raise GiveUpException(f"{player.color} gave up")
        elif move in moves:
            return move
        print("Invalid move")