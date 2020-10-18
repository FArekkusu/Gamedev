from collections import namedtuple
from board import Board
from ai import make_random_move as make_ai_move
from ui import display_menu, display_board, display_no_moves, display_result, display_give_up

class GiveUpException(Exception):
    pass

Player = namedtuple("Player", ("type", "symbol", "move"))

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

def create_player(type_code, color):
    type_, move_function = {"p": (HUMAN, make_human_move), "c": (COMPUTER, make_ai_move)}[type_code]
    return Player(type_, color, move_function)

def play_othello(board, player_1, player_2):
    current = player_1
    opponent = player_2
    passed = False
    pieces = 4
    while pieces < 64:
        move = current.move(board, current.symbol)
        if move is None:
            display_no_moves("Black" if current.symbol == BLACK else "White")
            if passed:
                break
            passed = True
        else:
            board.make_move(move, current.symbol)
            pieces += 1
            passed = False
        current, opponent = opponent, current
    return board.calculate_total(player_1.symbol, player_2.symbol)

def make_human_move(board, player_symbol):
    moves = board.find_moves(player_symbol)
    display_board(board, moves)
    if not moves:
        return
    print(f"\n{'Black' if player_symbol == BLACK else 'White'} possible moves: {', '.join(moves)}")
    while True:
        move = input("> ").upper()
        if move == "GIVE UP":
            raise GiveUpException(f"{'Black' if player_symbol == BLACK else 'White'} gave up")
        elif move in moves:
            return move
        print("Invalid move")

def gameloop():
    while True:
        display_menu(COMMANDS.values())
        command = input("> ").lower()
        if command == "quit":
            break
        elif not command in COMMANDS:
            print("Unknown command\n")
            continue
        board = Board(BLACK, WHITE)
        player_1 = create_player(command[-3], BLACK)
        player_2 = create_player(command[-1], WHITE)
        try:
            black_total, white_total = play_othello(board, player_1, player_2)
            if HUMAN in (player_1.type, player_2.type):
                display_board(board)
            display_result(black_total, white_total)
        except GiveUpException as e:
            if HUMAN in (player_1.type, player_2.type):
                display_board(board)
            display_give_up(e)

if __name__ == "__main__":
    gameloop()
