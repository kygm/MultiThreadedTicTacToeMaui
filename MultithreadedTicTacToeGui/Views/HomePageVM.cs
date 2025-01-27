using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MultiThreadedTicTacToeGui.ViewModels;

namespace MultiThreadedTicTacToeGui.Views
{
    public class HomePageVM : ViewModelBase
    {
        public HomePageVM() 
        {
            ; //Add constructor logic if necessary;
        }

        private string _editorText;
        public string EditorText
        {
            get => _editorText;
            set
            {
                if(_editorText != value)
                {
                    SetProperty(ref _editorText, value);
                }
            }
        }

        private string _processTextResult;
        public string ProcessTextResult
        {
            get => _processTextResult;
            set
            {
                if (_processTextResult != value)
                {
                    SetProperty(ref _processTextResult, value);
                }
            }
        }

        private bool _isProcessedTextVisible = false;
        public bool IsProcessedTextVisible
        {
            get => _isProcessedTextVisible;
            set
            {
                if (_isProcessedTextVisible != value)
                {
                    SetProperty(ref _isProcessedTextVisible, value);
                }
            }
        }

        private ICommand _processTextCommand = null;
        public ICommand ProcessTextCommand
        {
            get
            {
                if(_processTextCommand == null)
                {
                    _processTextCommand = new Command(() =>
                    {
                        ProcessText();
                    });
                }
                return _processTextCommand;
            }
        }

        private async void ProcessText()
        {
            ProcessTextResult = EditorText + " - processed";
            IsProcessedTextVisible = true;
        }
    }
}
