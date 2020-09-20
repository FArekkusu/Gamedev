from collections import namedtuple
from board import Board
from ai import make_random_move as make_ai_move

class ForfeitException(Exception):
    pass

Player = namedtuple("Player", ("type", "symbol"))

BLACK = "■"
WHITE = "□"

HUMAN = "human"
COMPUTER = "computer"

COMMANDS = {x[0].lower(): x for x in (
    ("Play PvP", "Multiplayer"),
    ("Play PvC", "Singleplayer (black)"),
    ("Play CvP", "Singleplayer (white)"),
    ("Play CvC", "AI game"),
    ("Quit", "Exit the game")
)}

def print_menu():
    print("Enter one of the following commands:")
    print("\n".join(f"  | {x} - {y}" for x, y in COMMANDS.values()))
    print()

def play_othello(board, player_1, player_2):
    current = player_1
    opponent = player_2
    passed = False
    while True:
        move = (make_ai_move if current.type == COMPUTER else make_human_move)(board, current.symbol)
        if move is None:
            if passed:
                break
            passed = True
        else:
            board.make_move(move, current.symbol)
        current, opponent = opponent, current
    print()
    black_total, white_total = board.calculate_total(player_1.symbol, player_2.symbol)
    print(f"Black: {black_total}, White: {white_total}")
    print(f"{'Black' if black_total > white_total else 'White'} wins" if black_total != white_total else "Tie")
    print()

def make_human_move(board, player_symbol):
    moves = board.find_moves(player_symbol)
    print()
    if not moves:
        print(f"{'Black' if player_symbol == BLACK else 'White'} has no possible moves, skipping")
        return
    print(board)
    print(f"\n{player_symbol} Possible moves: {', '.join(moves)}")
    while True:
        move = input("> ").upper()
        if move == "FORFEIT":
            raise ForfeitException(f"{'Black' if player_symbol == BLACK else 'White'} gave up")
        elif move in moves:
            return move
        print("Invalid move")

def gameloop():
    while True:
        print_menu()
        command = input("> ").lower()
        if command == "quit":
            break
        elif not command in COMMANDS:
            print("Unknown command\n")
            continue
        board = Board(BLACK, WHITE)
        player_1 = Player(HUMAN if command[-3] == "p" else COMPUTER, BLACK)
        player_2 = Player(HUMAN if command[-1] == "p" else COMPUTER, WHITE)
        try:
            play_othello(board, player_1, player_2)
        except ForfeitException as e:
            print(e)
            print()

if __name__ == "__main__":
    gameloop()
