namespace Recanto_da_natureza;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new MainPage())
        {
            BarBackgroundColor = Color.FromArgb("#7BAE7F"),
            BarTextColor = Colors.White
        };
    }
}