namespace Recanto_da_natureza;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void AbrirLogin(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }

    private async void AbrirReserva(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ReservaPage());
    }

    private async void AbrirSobre(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SobrePage());
    }
}