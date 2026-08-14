using System;
using Microsoft.Maui.Controls;

namespace Recanto_da_natureza.Controls
{
    public partial class NavBarView : ContentView
    {
        public NavBarView()
        {
            InitializeComponent();
            // Não realiza hook de pointer para manter compatibilidade entre plataformas.
        }

        private async void OnHomeClicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        private async void OnReservasClicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Recanto_da_natureza.ReservaPage());
        }

        private async void OnSobreClicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Recanto_da_natureza.SobrePage());
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Recanto_da_natureza.LoginPage());
        }
    }
}
