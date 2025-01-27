using MultiThreadedTicTacToeGui.Views;

namespace MultithreadedTicTacToeGui
{
    public partial class MainPage : ContentPage
    {
        private GameBoardPage _gameBoardPage;

        public MainPage()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
        }

        private async void OnStartButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_gameBoardPage ?? new GameBoardPage());
        }
    }

}
