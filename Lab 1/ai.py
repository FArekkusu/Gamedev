from random import choice

def make_random_move(board, player_symbol):
    moves = board.find_moves(player_symbol)
    if moves:
        return choice(moves)