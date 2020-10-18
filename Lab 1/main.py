from board import Board
from ui import display_board, display_result, display_give_up
from exceptions import GiveUpException
from player import create_player, BLACK, WHITE, HUMAN
from gameplay import play_othello, winner_most as determine_winner
from main_menu import get_console_players as get_players

COMMANDS = {x[0].lower(): x for x in (
    ("Play PvP", "Multiplayer"),
    ("Play PvC", "Singleplayer (black)"),
    ("Play CvP", "Singleplayer (white)"),
    ("Play CvC", "AI game"),
    ("Quit", "Exit the game")
)}

def gameloop():
    while True:
        players = get_players(COMMANDS)
        if players is None:
            break
        player_1 = create_player(players[0], BLACK)
        player_2 = create_player(players[1], WHITE)
        board = Board(player_1.symbol, player_2.symbol)
        try:
            play_othello(board, player_1, player_2)
            if HUMAN in (player_1.type, player_2.type):
                display_board(board)
            display_result(determine_winner(board, player_1, player_2))
        except GiveUpException as e:
            if HUMAN in (player_1.type, player_2.type):
                display_board(board)
            display_give_up(e)

if __name__ == "__main__":
    gameloop()
