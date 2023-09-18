using System;
using System.Linq;

namespace wsServer
{
    public class Room
    {
        private readonly string[] _board = new string[9];
        private static readonly int[][] WinPatterns = {
            new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8}, // Rows
            new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8}, // Columns
            new[] {0, 4, 8}, new[] {2, 4, 6} // Diagonals
        };

        public Guid Id { get; set; }
        public string FirstPlayerMove { get; set; }
        public string SecondPlayerMove { get; set; }
        public string FirstPlayerId { get; set; }
        public string SecondPlayerId { get; set; }
        public string FirstPlayerSessionId { get; set; }
        public string SecondPlayerSessionId { get; set; }

        public bool MakeMove(int cellIndex, string move, out string winnerId, out int[] winPattern)
        {
            _board[cellIndex] = move;

            return IsGameFinished(out winnerId, out winPattern);
        }
        
        private bool IsGameFinished(out string winnerId, out int[] winningPattern) {
            winnerId = null;
            winningPattern = null;
            
            foreach (var winPattern in WinPatterns)
            {
                int a = winPattern[0];
                int b = winPattern[1];
                int c = winPattern[2];

                if (!string.IsNullOrEmpty(_board[a]) &&
                    _board[a] == _board[b] &&
                    _board[a] == _board[c])
                {
                    if (FirstPlayerMove == _board[a])
                        winnerId = FirstPlayerId;
                    else
                        winnerId = SecondPlayerId;

                    winningPattern = winPattern;
                    return true;
                }
            }

            if (_board.Any(string.IsNullOrEmpty))
            {
                return false;
            }

            return true;
        }

        public void RandomizePlayers()
        {
            if (new Random().Next(2) == 1)
            {
                FirstPlayerMove = "x";
                SecondPlayerMove = "o";
            }
            else
            {
                FirstPlayerMove = "o";
                SecondPlayerMove = "x";
            }
        }
    }
}