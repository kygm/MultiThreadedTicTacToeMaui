using MultiThreadedTicTacToeGui.Helpers;

namespace MultiThreadedTicTacToeGui.Views;

public partial class GameBoardPage : ContentPage
{
	private readonly GameBoardVM _gameBoardVM;
	public GameBoardPage()
	{
		InitializeComponent();
		_gameBoardVM = ServiceHelper.GetService<GameBoardVM>();
		BindingContext = _gameBoardVM;
	}

	private bool _isLoaded = false;
    protected async override void OnAppearing()
    {
        base.OnAppearing();
		if(!_isLoaded)
		{
			await _gameBoardVM.StartGame();
			_isLoaded = true;
		}
    }

	private async void OnStartGamePressed(object sender, EventArgs e)
	{
		if(!_gameBoardVM.IsGameRunning)
		{
            await _gameBoardVM.StartGame();
        }
	}
}