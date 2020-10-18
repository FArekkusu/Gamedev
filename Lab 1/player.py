from collections import namedtuple
from ai import make_random_move as make_ai_move
from human_controller import make_console_move as make_human_move

Player = namedtuple("Player", ("type", "symbol", "move", "color"))

BLACK = "Black"
WHITE = "White"

HUMAN = "human"
COMPUTER = "computer"

SYMBOLS = {
    BLACK: "■",
    WHITE: "□"
}

MOVE_OPTIONS = {
    HUMAN: make_human_move,
    COMPUTER: make_ai_move
}

def create_player(type_, color):
    move_function = MOVE_OPTIONS[type_]
    return Player(type_, SYMBOLS[color], move_function, color)