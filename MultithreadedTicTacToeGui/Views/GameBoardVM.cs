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

        public GameBoardVM()
        {
            InitializeGameBoard();
        }

        void InitializeGameBoard()
        {
            _openBoardSlots = new List<KeyValuePair<int, int>>();
            _populatedPlayerMatrix = new List<KeyValuePair<PlayerType, KeyValuePair<int, int>>>();
            _gameBoardLabels = new List<string>();

            //Fill open board slots list
            for(int rowIndex = 1; rowIndex <= 3; rowIndex++)
            {
                for (int columnIndex = 1; columnIndex <= 3; columnIndex++)
                {
                    _openBoardSlots.Add(new KeyValuePair<int, int>(rowIndex, columnIndex));
                    GameBoardLabels.Add("X");
                }
            }

        }
    }

    public enum PlayerType
    {
        X = 0,
        O = 1
    }
}
