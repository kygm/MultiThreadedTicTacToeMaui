using MultiThreadedTicTacToeGui.Views;

namespace MultithreadedTicTacToeGui
{
    public partial class MainPage : ContentPage
    {
        private HomePage _homePage;

        public MainPage()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
        }

        private async void OnStartButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_homePage ?? new HomePage());
        }
    }

}
