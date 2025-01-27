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
}