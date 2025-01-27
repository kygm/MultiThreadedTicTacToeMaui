using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MultiThreadedTicTacToeGui.ViewModels;

namespace MultiThreadedTicTacToeGui.Views
{
    public class GameBoardVM : ViewModelBase
    {

        private List<KeyValuePair<PlayerType, KeyValuePair<int, int>>> _populatedPlayerMatrix; //WARNING - Beginning index = 1
        private List<KeyValuePair<int, int>> _openBoardSlots; //WARNING - Beginning index = 1

        private List<string> _gameBoardLabels;
        public List<string> GameBoardLabels
        {
            get => _gameBoardLabels;
            set => SetProperty(ref _gameBoardLabels, value);
        }

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
            _openBoardSlots = new List<KeyValuePair<int, int>>();
            _populatedPlayerMatrix = new List<KeyValuePair<PlayerType, KeyValuePair<int, int>>>();
            _gameBoardLabels = new List<string>();

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
            PlayerType currentPlayer;

            //First select a randomNumberGenerator player - X or O
            var randomNumberGenerator = new Random();
            currentPlayer = (PlayerType)randomNumberGenerator.Next(2); // Generates either 0 or 

            //Now select a first arbitrary randomNumberGenerator position
            int initialPosition = GetNextAvailablePosition();

            //Load the initial config into the board labels collection
            GameBoardLabels[initialPosition] = currentPlayer.ToString();


            //Main game loop - do threading in here
            while(IsGameRunning)
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

                GameBoardLabels[nextBoardPosition] = currentPlayer.ToString();

                IsGameRunning = !HasGameBeenCompletedCheckState();
            }

        }

        public void UpdateDelay(int newDelayValue)
        {
            DelayBetweenMovesInMilliseconds = newDelayValue;
        }

        private PlayerType GetNextPlayer(PlayerType currentPlayer)
        {
            return (currentPlayer == PlayerType.X) ? PlayerType.O : PlayerType.X;
        }

        private int GetNextAvailablePosition()
        {
            var randomNumberGenerator = new Random();
            bool isComputedPositionTaken = false;
            int computedNextPosition = -1;
            while(_openBoardSlots.Count() != 0 && !isComputedPositionTaken)
            {
                computedNextPosition = randomNumberGenerator.Next(0, 9);
                var flattenedBoardMatrix = FlattenBoardMatrix();
                if (!flattenedBoardMatrix.Contains(computedNextPosition))
                {
                    _openBoardSlots.Remove(GetMatrixPositionFromFlattenedBoardMatrix(computedNextPosition));
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
                        flattenedBoardMatrix.Add(kvp.Value.Value);
                        break;
                    case 2: //row 2
                        flattenedBoardMatrix.Add(kvp.Value.Value + 3);
                        break;
                    case 3:
                        flattenedBoardMatrix.Add(kvp.Value.Value + 6);
                        break;
                }
            }

            return flattenedBoardMatrix;
        }

        //TEST THIS METHOD! UNTESTED!
        private KeyValuePair<int, int> GetMatrixPositionFromFlattenedBoardMatrix(int index)
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
