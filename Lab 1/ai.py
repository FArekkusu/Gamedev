from random import choice

def make_random_move(player, board):
    moves = board.find_moves(player.symbol)
    if moves:
        return choice(moves)