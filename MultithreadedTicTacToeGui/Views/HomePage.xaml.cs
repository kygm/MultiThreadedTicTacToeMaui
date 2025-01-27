using MultiThreadedTicTacToeGui.Helpers;

namespace MultiThreadedTicTacToeGui.Views;

public partial class HomePage : ContentPage
{
	private readonly HomePageVM _homePageVM;
	public HomePage()
	{
		InitializeComponent();
		_homePageVM = ServiceHelper.GetService<HomePageVM>();
		BindingContext = _homePageVM;
	}
}