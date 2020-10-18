def display_menu(options):
    print("Enter one of the following commands:")
    print("\n".join(f"  | {x} - {y}" for x, y in options))
    print()

def display_board(board, moves=None):
    print()
    print(board if moves is None else board.stringify_with_moves(moves))

def display_no_moves(side):
    print(f"\n{side} has no possible moves, skipping")

def display_result(winner):
    print()
    print(f"{winner.color} wins" if winner is not None else "Tie")
    print()

def display_give_up(exception):
    print()
    print(exception)
    print()