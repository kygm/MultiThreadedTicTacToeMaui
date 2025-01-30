using MultiThreadedTicTacToeGui.ViewModels;
using System.Collections.ObjectModel;

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

        public string R1C1 => GameBoardLabels[0];
        public string R1C2 => GameBoardLabels[1];
        public string R1C3 => GameBoardLabels[2];
        public string R2C1 => GameBoardLabels[3];
        public string R2C2 => GameBoardLabels[4];
        public string R2C3 => GameBoardLabels[5];
        public string R3C1 => GameBoardLabels[6];
        public string R3C2 => GameBoardLabels[7];
        public string R3C3 => GameBoardLabels[8];

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
            for(int rowIndex = 1; rowIndex <= 3; rowIndex++)
            {
                for (int columnIndex = 1; columnIndex <= 3; columnIndex++)
                {
                    _openBoardSlots.Add(new KeyValuePair<int, int>(rowIndex, columnIndex));
                    GameBoardLabels.Add("");
                }
            }
        }

        public async Task StartGame()
        {
            IsGameRunning = true;

            InitializeGameBoard();

            PlayerType currentPlayer;

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
                currentPlayer = GetNextPlayer(currentPlayer);
                int nextBoardPosition = GetNextAvailablePosition();
                //No more available positions
                if(nextBoardPosition == -1)
                {
                    IsGameRunning = false;
                    await Application.Current.MainPage.DisplayAlert("Notice", "No remaining board slots available", "Ok");
                    return;
                }
                else
                {
                    GameBoardLabels[nextBoardPosition] = currentPlayer.ToString();
                    PopulatePlayerMatrixFromFlattenedBoardMatrixIndex(nextBoardPosition, currentPlayer);

                    UpdateRowColumnLabels(); //Need to invoke this for the UI to update
                }

                IsGameRunning = !HasGameBeenCompletedCheckState();
            }

        }

        public void UpdateDelay(int newDelayValue)
        {
            DelayBetweenMovesInMilliseconds = newDelayValue;
        }

        private void PopulatePlayerMatrixFromFlattenedBoardMatrixIndex(int nextBoardPosition, PlayerType currentPlayer )
        {
            _populatedPlayerMatrix.Add(new KeyValuePair<PlayerType, KeyValuePair<int, int>>
                (currentPlayer, GetMatrixPositionFromFlattenedBoardMatrixIndex(nextBoardPosition)));

            _openBoardSlots.Remove(GetMatrixPositionFromFlattenedBoardMatrixIndex(nextBoardPosition));
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
            while(_openBoardSlots.Count() != 0)
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
            foreach(var kvp in _populatedPlayerMatrix)//this should be _occupied board slots_ check
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
        /// This method should return the state of the game - when the game completes due to a draw, a win 
        /// by X, or a win by O, true should be returned
        /// </summary>
        /// <returns>State of the game - whether it's been completed or not</returns>
        private bool HasGameBeenCompletedCheckState()
        {
            //Perform logic here to determine wheter a winning condition is present in the PopulatedPlayerMatrix collection
            return false;
        }
    }

    public enum PlayerType
    {
        X = 0,
        O = 1
    }
}
