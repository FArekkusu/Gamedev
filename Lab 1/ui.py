def display_menu(options):
    print("Enter one of the following commands:")
    print("\n".join(f"  | {x} - {y}" for x, y in options))
    print()

def display_board(board, moves=None):
    print()
    print(board if moves is None else board.stringify_with_moves(moves))

def display_no_moves(side):
    print(f"\n{side} has no possible moves, skipping")

def display_result(black, white):
    print()
    print(f"Black: {black}, White: {white}")
    print(f"{'Black' if black > white else 'White'} wins" if black != white else "Tie")
    print()