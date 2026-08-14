using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace Recanto_da_natureza;

public partial class MainPage : ContentPage
{
    private readonly HttpClient _http = new HttpClient();

    // Coordenadas aproximadas de Brotas, SP
    private const double Latitude = -22.2869;
    private const double Longitude = -48.0289;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = LoadWeatherAsync();
    }

    private async System.Threading.Tasks.Task LoadWeatherAsync()
    {
        try
        {
            WeatherUpdatedLabel.Text = "Carregando clima...";

            var url = $"https://api.open-meteo.com/v1/forecast?latitude={Latitude}&longitude={Longitude}&current_weather=true&timezone=America/Sao_Paulo";
            var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode)
            {
                WeatherUpdatedLabel.Text = "Não foi possível carregar o clima.";
                return;
            }

            using var stream = await res.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            if (doc.RootElement.TryGetProperty("current_weather", out var cur))
            {
                var temp = cur.GetProperty("temperature").GetDouble();
                var code = cur.GetProperty("weathercode").GetInt32();
                var time = cur.GetProperty("time").GetString();

                TemperatureLabel.Text = $"{temp:0}°C";
                ConditionLabel.Text = WeatherCodeToText(code);
                WeatherUpdatedLabel.Text = $"Atualizado: {time}";
            }
            else
            {
                WeatherUpdatedLabel.Text = "Dados de clima indisponíveis.";
            }
        }
        catch (Exception ex)
        {
            WeatherUpdatedLabel.Text = "Erro ao carregar clima.";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    private static string WeatherCodeToText(int code)
    {
        // Mapear códigos simplificados para texto/emoji
        return code switch
        {
            0 => "Ensolarado ☀️",
            1 or 2 or 3 => "Parcialmente nublado ⛅",
            45 or 48 => "Névoa 🌫️",
            51 or 53 or 55 => "Chuvisco 🌦️",
            61 or 63 or 65 => "Chuva ☔",
            71 or 73 or 75 => "Neve ❄️",
            80 or 81 or 82 => "Chuva forte ⛈️",
            95 or 96 or 99 => "Tempestade ⚡",
            _ => "Temperatura estável"
        };
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
