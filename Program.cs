using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemHunter1
{
    public class Cell
    {
        public string Occupant { get; set; }

        public Cell(string occupant)
        {
            Occupant = occupant;
        }
    }

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public class Player
    {
        public string Name { get; }
        public Position Position { get; set; }
        public int GemCount { get; set; }

        public Player(string name, Position initialPosition)
        {
            Name = name;
            Position = initialPosition;
            GemCount = 0;
        }

        public void MovePlayer(Player player, string direction)
        {
            var position = player.Position;
            var x = position.X;
            var y = position.Y;

            switch (direction)
            {
                case "U":
                    x--;
                    if (x < 0) x = 0;
                    break;
                case "D":
                    x++;
                    if (x > 5) x = 5;
                    break;
                case "L":
                    y--;
                    if (y < 0) y = 0;
                    break;
                case "R":
                    y++;
                    if (y > 5) y = 5;
                    break;
                default:
                    Console.WriteLine("\nInvalid move. Please enter U, D, L, or R.");
                    break;
            }

            Position = new Position(x, y);

        }


    }

    public class Game
    {
        public Board Board { get; }

        public Player Player1 { get; }
        public Player Player2 { get; }
        public Player CurrentTurn { get; set; } // Add this property

        public Game()
        {
            Board = new Board();

            Player1 = new Player("P1", new Position(0, 0));
            Player2 = new Player("P2", new Position(5, 5));
            CurrentTurn = Player1; // Initialize the current turn to Player1
        }

        public void Start()
        {
            Console.WriteLine("\nWelcome to Gem Hunters! \n");

            for (int turn = 1; turn <= 30; turn++)
            {
                Console.WriteLine($"Turn {turn}:");

                DisplayBoard();
                TakeTurn();

                if ((Player1.GemCount + Player2.GemCount) >= 9 || Player1.GemCount >= 5 || Player2.GemCount >= 5)
                {
                    break;
                }

                SwitchTurn();
            }

            AnnounceWinner();
        }

        public void DisplayBoard()
        {
            Console.WriteLine("Current Board:");
            Board.Display();
            Console.WriteLine($"{Player1.Name}: {Player1.GemCount} gems");
            Console.WriteLine($"{Player2.Name}: {Player2.GemCount} gems");
        }
        private void TakeTurn()
        {
            Console.WriteLine($"{CurrentTurn.Name}'s turn. Enter move (U/D/L/R): ");
            string move = Console.ReadLine().ToUpper() ?? "";


            if (Board.IsValidMove(CurrentTurn, move))
            {
                CurrentTurn.MovePlayer(CurrentTurn, move);
                Board.CollectGem(CurrentTurn);
            }
            else
                Console.WriteLine("\nError: That's an Invalid Move!");

            // Check for obstacles
            if (Board.Grid[CurrentTurn.Position.X, CurrentTurn.Position.Y].Occupant == "X")
            {
                Console.WriteLine($"Oops! {CurrentTurn.Name} hit an obstacle and cannot move.");
                // You might want to handle this differently, e.g., skip the turn or penalize the player
            }
        }


        private void SwitchTurn()
        {
            CurrentTurn = (CurrentTurn == Player1) ? Player2 : Player1;
        }

        private void AnnounceWinner()
        {
            Console.WriteLine("Game over!");
            Console.WriteLine($"{Player1.Name}: {Player1.GemCount} gems");
            Console.WriteLine($"{Player2.Name}: {Player2.GemCount} gems");

            if (Player1.GemCount > Player2.GemCount)
                Console.WriteLine($"{Player1.Name} wins!");
            else if (Player2.GemCount > Player1.GemCount)
                Console.WriteLine($"{Player2.Name} wins!");
            else
                Console.WriteLine("It's a tie!");
        }

    }


    public class Board
    {
        public Cell[,] Grid { get; }


        public Board()
        {
            Grid = new Cell[6, 6];
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    Grid[i, j] = new Cell("-");
                }
            }

            Grid[0, 0] = new Cell("P1");
            Grid[5, 5] = new Cell("P2");

            PlaceGems(9);
            PlaceObstacles(7);

        }
        private void PlaceGems(int numberOfGems)
        {
            Random random = new Random();

            for (int i = 0; i < numberOfGems; i++)
            {
                int x, y;
                do
                {
                    x = random.Next(0, Grid.GetLength(0));
                    y = random.Next(0, Grid.GetLength(1));
                } while (Grid[x, y].Occupant != "-");

                Grid[x, y].Occupant = "G";
            }
        }
        private void PlaceObstacles(int numberOfObstacles)
        {
            Random random = new Random();

            for (int i = 0; i < numberOfObstacles; i++)
            {
                int x, y;
                do
                {
                    x = random.Next(1, 5);
                    y = random.Next(1, 5);
                } while (Grid[x, y].Occupant != "-");

                Grid[x, y].Occupant = "X";
            }
        }

        public void Display()
        {
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    Console.Write(string.Format("{0, -5}", Grid[i, j].Occupant));
                }
                Console.WriteLine();
            }
        }

        public bool IsValidMove(Player player, string direction)
        {
            var position = player.Position;
            var x = position.X;
            var y = position.Y;

            switch (direction)
            {
                case "U":
                    x--;
                    if (x < 0) x = 0;
                    break;
                case "D":
                    x++;
                    if (x > 5) x = 5;
                    break;
                case "L":
                    y--;
                    if (y < 0) y = 0;
                    break;
                case "R":
                    y++;
                    if (y > 5) y = 5;
                    break;
                default:
                    Console.WriteLine("Invalid move. Please enter U, D, L, or R.");
                    break;
            }

            bool isValidMove = Grid[x, y].Occupant == "X" ? false : true;

            if (isValidMove)
            {
                Grid[position.X, position.Y].Occupant = "-";
            }

            return isValidMove;
        }

        public void CollectGem(Player player)
        {
            int x = player.Position.X;
            int y = player.Position.Y;

            if (Grid[x, y].Occupant == "G")
            {
                // The player's new position contains a gem
                player.GemCount++;

            }
            Grid[x, y].Occupant = player.Name; // Remove the gem from the board

        }


    }

    public class Program
    {
        static void Main()
        {
            Game gemHunters = new Game();
            gemHunters.Start();
            Console.ReadLine();
        }
    }
}
