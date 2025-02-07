using MultiThreadedTicTacToeGui.ViewModels;
using System.Threading;

namespace MultiThreadedTicTacToeGui.Views
{
    public class GameBoardVM : ViewModelBase
    {
        //WARNING - Beginning index = 1
        private List<KeyValuePair<PlayerType, KeyValuePair<int, int>>> _populatedPlayerMatrix = new List<KeyValuePair<PlayerType, KeyValuePair<int, int>>>();
        private List<KeyValuePair<int, int>> _openBoardSlots = new List<KeyValuePair<int, int>>();

        private List<string> _gameBoardLabels = new List<string>();
        public List<string> GameBoardLabels
        {
            get => _gameBoardLabels; 
            set => SetProperty(ref _gameBoardLabels, value);
        }

        #region Row Column Game Labels
        public string R1C1 => GameBoardLabels[0];
        public string R1C2 => GameBoardLabels[1];
        public string R1C3 => GameBoardLabels[2];
        public string R2C1 => GameBoardLabels[3];
        public string R2C2 => GameBoardLabels[4];
        public string R2C3 => GameBoardLabels[5];
        public string R3C1 => GameBoardLabels[6];
        public string R3C2 => GameBoardLabels[7];
        public string R3C3 => GameBoardLabels[8];
        #endregion

        #region Winning Line Visibilities

        private bool _isR1WinLineVisible = false;
        public bool IsR1WinLineVisible 
        {
            get => _isR1WinLineVisible;
            set => SetProperty(ref _isR1WinLineVisible, value);
        }
        private bool _isR2WinLineVisible = false;
        public bool IsR2WinLineVisible
        {
            get => _isR2WinLineVisible;
            set => SetProperty(ref _isR2WinLineVisible, value);
        }
        private bool _isR3WinLineVisible = false;
        public bool IsR3WinLineVisible
        {
            get => _isR3WinLineVisible;
            set => SetProperty(ref _isR3WinLineVisible, value);
        }
        private bool _isC1WinLineVisible = false;
        public bool IsC1WinLineVisible
        {
            get => _isC1WinLineVisible;
            set => SetProperty(ref _isC1WinLineVisible, value);
        }
        private bool _isC2WinLineVisible = false;
        public bool IsC2WinLineVisible
        {
            get => _isC2WinLineVisible;
            set => SetProperty(ref _isC2WinLineVisible, value);
        }
        private bool _isC3WinLineVisible = false;
        public bool IsC3WinLineVisible
        {
            get => _isC3WinLineVisible;
            set => SetProperty(ref _isC3WinLineVisible, value);
        }
        private bool _isNegativeDiagonalWinLineVisible = false;
        public bool IsNegativeDiagonalWinLineVisible
        {
            get => _isNegativeDiagonalWinLineVisible;
            set => SetProperty(ref _isNegativeDiagonalWinLineVisible, value);
        }

        private bool _isPositiveDiagonalWinLineVisible = false;
        public bool IsPositiveDiagonalWinLineVisible
        {
            get => _isPositiveDiagonalWinLineVisible;
            set => SetProperty(ref _isPositiveDiagonalWinLineVisible, value);
        }

        #endregion

        private bool _isGameRunning = false;
        public bool IsGameRunning
        {
            get => _isGameRunning;
            set => SetProperty(ref _isGameRunning, value);
        }

        private int _delayBetweenMovesInMilliseconds = 500;
        public int DelayBetweenMovesInMilliseconds
        {
            get => _delayBetweenMovesInMilliseconds;
            set => SetProperty(ref _delayBetweenMovesInMilliseconds, value);
        }

        public static Mutex mutex = new Mutex();


        public GameBoardVM()
        {
            InitializeGameBoard();
        }

        void InitializeGameBoard()
        {
            _openBoardSlots.Clear();
            _populatedPlayerMatrix.Clear();
            GameBoardLabels.Clear();

            //Fill open board slots list and board labels list
            for (int rowIndex = 1; rowIndex <= 3; rowIndex++)
            {
                for (int columnIndex = 1; columnIndex <= 3; columnIndex++)
                {
                    _openBoardSlots.Add(new KeyValuePair<int, int>(rowIndex, columnIndex));
                    GameBoardLabels.Add("");
                }
            }

            ResetWinningLines();
        }

        public async Task StartGame()
        {
            IsGameRunning = true;

            InitializeGameBoard();

            PlayerType currentPlayer;
            var gameWinState = new GameWinStructure();

            //First select a randomNumberGenerator player - X or O
            var randomNumberGenerator = new Random();
            currentPlayer = (PlayerType)randomNumberGenerator.Next(2); // Generates either 0 or 

            //Now select a first arbitrary randomNumberGenerator position
            int initialPosition = GetNextAvailablePosition();

            //Load the initial config into the board labels collection
            GameBoardLabels[initialPosition] = currentPlayer.ToString();
            PopulatePlayerMatrixFromFlattenedBoardMatrixIndex(initialPosition, currentPlayer);
            UpdateRowColumnLabels();



            //Main game loop - do threading in here
            while (IsGameRunning)
            {
                await Task.Delay(DelayBetweenMovesInMilliseconds);

                //Generate a new random position for the subsequent player
                currentPlayer = GetNextPlayer(currentPlayer);                   //Completely synchronous - no need to multithread
                int nextBoardPosition = -2;
                var nextAvailablePositionProcess = new Thread(() =>
                {
                    nextBoardPosition = GetNextAvailablePosition();
                });
                nextAvailablePositionProcess.Name = "Next available position";
                nextAvailablePositionProcess.Start();
                nextAvailablePositionProcess.Join();

                //No more available positions
                if (nextBoardPosition == -1)
                {
                    IsGameRunning = false;
                    await Application.Current.MainPage.DisplayAlert("Notice", "No remaining board slots available", "Ok");
                    return;
                }
                else
                {
                    var setGameBoardLabelsThread = new Thread(() =>
                    {
                        GameBoardLabels[nextBoardPosition] = currentPlayer.ToString();
                    });
                    setGameBoardLabelsThread.Name = "Set game board labels";

                    var populatePlayerMatrixThread = new Thread(() =>
                    {
                        PopulatePlayerMatrixFromFlattenedBoardMatrixIndex(nextBoardPosition, currentPlayer);
                    });

                    populatePlayerMatrixThread.Name = "Populate player matrix";

                    setGameBoardLabelsThread.Start();
                    populatePlayerMatrixThread.Start();

                    setGameBoardLabelsThread.Join();
                    populatePlayerMatrixThread.Join();


                    UpdateRowColumnLabels(); //Need to invoke this for the UI to update     //Multithread eligible - what if this blocks the UI thread?
                }

                gameWinState = HasGameBeenCompletedCheckState();
                if((gameWinState.ResultOfGame == GameResult.XWins) || (gameWinState.ResultOfGame == GameResult.OWins))
                {
                    SetWinnerLine(gameWinState); //Multithread elgibile
                }

                IsGameRunning = (gameWinState.ResultOfGame == GameResult.NoWinYet);
                
            }

        }

        public void UpdateDelay(int newDelayValue)
        {
            DelayBetweenMovesInMilliseconds = newDelayValue;
        }

        private void PopulatePlayerMatrixFromFlattenedBoardMatrixIndex(int nextBoardPosition, PlayerType currentPlayer)
        {
            try
            {
                mutex.WaitOne();

                _populatedPlayerMatrix.Add(new KeyValuePair<PlayerType, KeyValuePair<int, int>>
                (currentPlayer, GetMatrixPositionFromFlattenedBoardMatrixIndex(nextBoardPosition)));

                _openBoardSlots.Remove(GetMatrixPositionFromFlattenedBoardMatrixIndex(nextBoardPosition));
            }
            catch (Exception e)
            {
                Console.WriteLine($"A threading error occurred - {e.Message}");
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public void UpdateRowColumnLabels()
        {
            OnPropertyChanged(nameof(R1C1));
            OnPropertyChanged(nameof(R1C2));
            OnPropertyChanged(nameof(R1C3));
            OnPropertyChanged(nameof(R2C1));
            OnPropertyChanged(nameof(R2C2));
            OnPropertyChanged(nameof(R2C3));
            OnPropertyChanged(nameof(R3C1));
            OnPropertyChanged(nameof(R3C2));
            OnPropertyChanged(nameof(R3C3));
        }

        private PlayerType GetNextPlayer(PlayerType currentPlayer)
        {
            return (currentPlayer == PlayerType.X) ? PlayerType.O : PlayerType.X;
        }

        private int GetNextAvailablePosition()
        {
            var randomNumberGenerator = new Random();
            int computedNextPosition = -1;
            while (_openBoardSlots.Count() != 0)
            {
                computedNextPosition = randomNumberGenerator.Next(0, 9);
                var flattenedBoardMatrix = FlattenBoardMatrix();
                if (!flattenedBoardMatrix.Contains(computedNextPosition))
                {
                    return computedNextPosition;
                }
            }

            return -1;
        }

        /// <summary>
        /// This takes the board matrix and converts it to a flattened list such that position
        /// [1,1] => 0, [2,3] -> 5
        /// </summary>
        /// <returns></returns>
        private List<int> FlattenBoardMatrix()
        {
            var flattenedBoardMatrix = new List<int>();
            foreach (var kvp in _populatedPlayerMatrix)//this should be _occupied board slots_ check
            {
                switch (kvp.Value.Key)
                {
                    case 1: //row 1
                        flattenedBoardMatrix.Add(kvp.Value.Value - 1);
                        break;
                    case 2: //row 2
                        flattenedBoardMatrix.Add(kvp.Value.Value + 2);
                        break;
                    case 3:
                        flattenedBoardMatrix.Add(kvp.Value.Value + 5);
                        break;
                }
            }
            flattenedBoardMatrix.Sort();
            return flattenedBoardMatrix;
        }

        /// <summary>
        /// This assumes a zero indexed flattened board matrix
        /// </summary>
        /// <param name="index"></param>
        /// <returns>A 1 indexed KeyValuePair representing a row, column index</returns>
        private KeyValuePair<int, int> GetMatrixPositionFromFlattenedBoardMatrixIndex(int index)
        {
            // Map the index to matrix position [row, column]
            int row = (index / 3) + 1;
            int col = (index % 3) + 1;
            return new KeyValuePair<int, int>(row, col);
        }

        /// <summary>
        /// This method checks if the game has been completed by either a win (by Player X or Player O) or a draw.
        /// </summary>
        /// <returns>A GameWinStructure object indicating the result of the game.</returns>
        private GameWinStructure HasGameBeenCompletedCheckState()
        {
            var gameWinStructure = new GameWinStructure();

            // Helper function to check for a win for a specific player
            bool CheckWinCondition(PlayerType player, out int? winningRow, out int? winningColumn, out DiagonalType? winningDiagonal)
            {
                winningRow = null;
                winningColumn = null;
                winningDiagonal = null;

                // Initialize counts for rows, columns, and diagonals using KeyValuePair
                List<KeyValuePair<int, int>> rowCounts = new List<KeyValuePair<int, int>>();
                List<KeyValuePair<int, int>> colCounts = new List<KeyValuePair<int, int>>();
                int mainDiagonalCount = 0;
                int antiDiagonalCount = 0;

                for (int i = 1; i <= 3; i++)
                {
                    rowCounts.Add(new KeyValuePair<int, int>(i, 0));
                    colCounts.Add(new KeyValuePair<int, int>(i, 0));
                }

                foreach (var kvp in _populatedPlayerMatrix)
                {
                    if (kvp.Key != player)
                        continue;

                    int row = kvp.Value.Key;
                    int col = kvp.Value.Value;

                    // Increment the counts for rows and columns
                    rowCounts[row - 1] = new KeyValuePair<int, int>(row, rowCounts[row - 1].Value + 1);
                    colCounts[col - 1] = new KeyValuePair<int, int>(col, colCounts[col - 1].Value + 1);

                    // Check main diagonal (top-left to bottom-right)
                    if (row == col)
                        mainDiagonalCount++;

                    // Check anti-diagonal (top-right to bottom-left)
                    if (row + col == 4) // Since row and col are 1-indexed
                        antiDiagonalCount++;
                }

                // Check if any row, column, or diagonal count equals 3
                foreach (var rowCount in rowCounts)
                {
                    if (rowCount.Value == 3)
                    {
                        winningRow = rowCount.Key;
                        return true;
                    }
                }
                foreach (var colCount in colCounts)
                {
                    if (colCount.Value == 3)
                    {
                        winningColumn = colCount.Key;
                        return true;
                    }
                }
                if (mainDiagonalCount == 3)
                {
                    winningDiagonal = DiagonalType.NegativeSlopeDiagonal;
                    return true;
                }
                if (antiDiagonalCount == 3)
                {
                    winningDiagonal = DiagonalType.PositiveSlopeDiagonal;
                    return true;
                }

                return false;
            }

            // Variables to store the winning conditions for both players
            int? xWinningRow = null, oWinningRow = null;
            int? xWinningColumn = null, oWinningColumn = null;
            DiagonalType? xWinningDiagonal = null, oWinningDiagonal = null;

            // Check for a win condition for both players
            bool xWins = CheckWinCondition(PlayerType.X, out xWinningRow, out xWinningColumn, out xWinningDiagonal);
            bool oWins = CheckWinCondition(PlayerType.O, out oWinningRow, out oWinningColumn, out oWinningDiagonal);

            if (!xWins && !oWins)
            {
                gameWinStructure.ResultOfGame = GameResult.NoWinYet;
            }
            else if (xWins)
            {
                gameWinStructure.ResultOfGame = GameResult.XWins;
                gameWinStructure.WinningPlayer = PlayerType.X;
                gameWinStructure.WinningRowNumber = xWinningRow;
                gameWinStructure.WinningColumnNumber = xWinningColumn;
                gameWinStructure.WinningDiagonalType = xWinningDiagonal;
            }
            else
            {
                gameWinStructure.ResultOfGame = GameResult.OWins;
                gameWinStructure.WinningPlayer = PlayerType.O;
                gameWinStructure.WinningRowNumber = oWinningRow;
                gameWinStructure.WinningColumnNumber = oWinningColumn;
                gameWinStructure.WinningDiagonalType = oWinningDiagonal;
            }

            return gameWinStructure;
        }

        private void ResetWinningLines()
        {
            IsR1WinLineVisible = false;
            IsR2WinLineVisible = false;
            IsR3WinLineVisible = false;
            IsC1WinLineVisible = false;
            IsC2WinLineVisible = false;
            IsC3WinLineVisible = false;
            IsPositiveDiagonalWinLineVisible = false;
            IsNegativeDiagonalWinLineVisible = false;
        }

        private void SetWinnerLine(GameWinStructure gameWinStructure)
        {
            if(gameWinStructure.WinningRowNumber != null)
            {
                switch(gameWinStructure.WinningRowNumber)
                {
                    case 1:
                        IsR1WinLineVisible = true; 
                        break;
                    case 2:
                        IsR2WinLineVisible = true;
                        break;
                    case 3:
                        IsR3WinLineVisible = true;
                        break;
                }
            }
            else if(gameWinStructure.WinningColumnNumber != null)
            {
                switch (gameWinStructure.WinningColumnNumber)
                {
                    case 1:
                        IsC1WinLineVisible = true;
                        break;
                    case 2:
                        IsC2WinLineVisible = true;
                        break;
                    case 3:
                        IsC3WinLineVisible = true;
                        break;
                }
            }
            else if(gameWinStructure.WinningDiagonalType != null)
            {
                if (gameWinStructure.WinningDiagonalType == DiagonalType.PositiveSlopeDiagonal)
                    IsPositiveDiagonalWinLineVisible = true;
                else
                    IsNegativeDiagonalWinLineVisible = true;
            }
        }


    }

    public enum PlayerType
    {
        X = 0,
        O = 1
    }

    public enum GameResult
    {
        XWins = 0,
        OWins,
        NoWinYet
    }

    public enum DiagonalType
    {
        PositiveSlopeDiagonal = 0,
        NegativeSlopeDiagonal
    }

    public class GameWinStructure
    {
        public GameResult ResultOfGame { get; set; }
        public PlayerType WinningPlayer { get; set; }
        public int? WinningRowNumber { get; set; }
        public int? WinningColumnNumber { get; set; }
        public DiagonalType? WinningDiagonalType { get; set; }
    }
}
