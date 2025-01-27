using MultiThreadedTicTacToeGui.Helpers;

namespace MultiThreadedTicTacToeGui.Views;

public partial class GameBoardPage : ContentPage
{
	private readonly GameBoardVM ViewModel;
	public GameBoardPage()
	{
		InitializeComponent();
		ViewModel = ServiceHelper.GetService<GameBoardVM>();
		BindingContext = ViewModel;
	}

	private bool _isLoaded = false;
    protected async override void OnAppearing()
    {
        base.OnAppearing();
		if(!_isLoaded)
		{
			await ViewModel.StartGame();
			_isLoaded = true;
		}
    }

	private async void OnStartGamePressed(object sender, EventArgs e)
	{
		if(!ViewModel.IsGameRunning)
		{
            await ViewModel.StartGame();
        }
	}

    private async void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
		try
		{
			ViewModel.UpdateDelay((int.Parse(e.NewValue.ToString("F0"))));
        }
		catch(Exception ex)
		{
			await Application.Current.MainPage.DisplayAlert("Error!", "An error occurred while setting the slider value", "Ok");
		}
    }
}