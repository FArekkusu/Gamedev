from ui import display_menu
from player import HUMAN, COMPUTER

PLAYER_OPTIONS = {
    "p": HUMAN,
    "c": COMPUTER
}

def get_console_players(commands):
    display_menu(commands.values())
    while True:
        command = input("> ").lower()
        if command == "quit":
            break
        elif command not in commands:
            print("Unknown command\n")
            continue
        player_1_type, player_2_type = command.split()[-1].split("v")
        return (PLAYER_OPTIONS[player_1_type], PLAYER_OPTIONS[player_2_type])