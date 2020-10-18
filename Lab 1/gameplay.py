from ui import display_no_moves

def play_othello(board, player_1, player_2):
    current = player_1
    opponent = player_2
    passed = False
    pieces = 4
    while pieces < 64:
        move = current.move(current, board)
        if move is None:
            display_no_moves(current.color)
            if passed:
                break
            passed = True
        else:
            board.make_move(move, current.symbol)
            pieces += 1
            passed = False
        current, opponent = opponent, current

def winner_most(board, player_1, player_2):
    total_1, total_2 = board.calculate_total(player_1.symbol, player_2.symbol)
    if total_1 != total_2:
        return player_1 if total_1 > total_2 else player_2