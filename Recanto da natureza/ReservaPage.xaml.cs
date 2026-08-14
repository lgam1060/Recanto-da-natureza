namespace Recanto_da_natureza;

public partial class ReservaPage : ContentPage
{
    public ReservaPage()
    {
        InitializeComponent();
    }

    private async void OnReservarClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string param)
        {
            var parts = param.Split(';');
            if (parts.Length == 2 && decimal.TryParse(parts[1], out var price))
            {
                var title = parts[0];
                await Navigation.PushAsync(new ConfirmarReservaPage(title, price));
            }
        }
    }
}