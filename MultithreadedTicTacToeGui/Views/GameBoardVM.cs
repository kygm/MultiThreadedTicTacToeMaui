using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MultiThreadedTicTacToeGui.ViewModels;

namespace MultiThreadedTicTacToeGui.Views
{
    public class GameBoardVM : ViewModelBase
    {

        private List<KeyValuePair<PlayerType, KeyValuePair<int, int>>> _populatedPlayerMatrix;
        private List<KeyValuePair<int, int>> _openBoardSlots;

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
            _isGameRunning = true;
            //First select a randomNumberGenerator player - X or O
            var randomNumberGenerator = new Random();
            PlayerType startingPlayer = (PlayerType)randomNumberGenerator.Next(2); // Generates either 0 or 1

            //Now select a first arbitrary randomNumberGenerator position
            int initialPosition = randomNumberGenerator.Next(0, 9);

            while(_isGameRunning)
            {
                await Application.Current.MainPage.DisplayAlert("Notice", $"Starting player {startingPlayer.ToString()}, starting position {initialPosition}", "Doge");
                _isGameRunning = HasGameBeenCompletedCheckState();
            }

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
