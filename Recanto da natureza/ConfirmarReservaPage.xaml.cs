using System;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Recanto_da_natureza.Data;
using Recanto_da_natureza.Models;

namespace Recanto_da_natureza
{
    public partial class ConfirmarReservaPage : ContentPage
    {
        private readonly decimal _pricePerNight;
        private readonly string _chaleName;
        private decimal _discountPercent = 0m;

        public ConfirmarReservaPage(string chaleName, decimal pricePerNight)
        {
            InitializeComponent();
            _chaleName = chaleName;
            _pricePerNight = pricePerNight;

            ChaleTitle.Text = _chaleName;
            PricePerNightLabel.Text = $"Valor por noite: R$ {_pricePerNight:0.00}";

            CheckInDate.Date = DateTime.Today;
            CheckOutDate.Date = DateTime.Today.AddDays(1);

            // Inicializar estado do switch de tema
            try
            {
                ThemeSwitch.IsToggled = Application.Current?.UserAppTheme == AppTheme.Dark;
            }
            catch
            {
                // ignore
            }

            CheckInDate.DateSelected += OnDateChanged;
            CheckOutDate.DateSelected += OnDateChanged;

            UpdateTotals();
        }

        private void OnDateChanged(object sender, DateChangedEventArgs e)
        {
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            // Calcular diferença de datas com segurança (Date pode ser nullable dependendo da plataforma)
            TimeSpan? diff = null;
            try
            {
                diff = CheckOutDate.Date - CheckInDate.Date;
            }
            catch
               {
                diff = null;
            }

            int nights = 1;
            if (diff.HasValue)
            {
                var days = (int)Math.Floor(diff.Value.TotalDays);
                nights = Math.Max(1, days);
                NightsLabel.Text = $"Quantidade de diárias: {nights}";
            }
            else
            {
                NightsLabel.Text = "Quantidade de diárias: 1";
            }

            var subtotal = nights * _pricePerNight;
            var discountAmount = Math.Round(subtotal * (_discountPercent / 100m), 2);
            var total = subtotal - discountAmount;

            DiscountLabel.Text = $"Desconto: R$ {discountAmount:0.00} ({_discountPercent:0}%)";
            TotalLabel.Text = $"Valor total: R$ {total:0.00}";
        }

        private async void OnConfirmarClicked(object sender, EventArgs e)
        {
            var payment = PaymentPicker.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(payment))
            {
                await DisplayAlert("Atenção", "Selecione uma forma de pagamento.", "OK");
                return;
            }

            // Calcular número de diárias com segurança
            TimeSpan? diff = null;
            try
            {
                diff = CheckOutDate.Date - CheckInDate.Date;
            }
            catch
            {
                diff = null;
            }

            int nights = 1;
            if (diff.HasValue)
            {
                nights = Math.Max(1, (int)Math.Floor(diff.Value.TotalDays));
            }

            var subtotal = nights * _pricePerNight;
            var discountAmount = Math.Round(subtotal * (_discountPercent / 100m), 2);
            var total = subtotal - discountAmount;

            var message = $"Reserva: {_chaleName}\nCheck-in: {CheckInDate.Date:d}\nCheck-out: {CheckOutDate.Date:d}\nDiárias: {nights}\nSubtotal: R$ {subtotal:0.00}\nDesconto: R$ {discountAmount:0.00} ({_discountPercent:0}%)\nTotal: R$ {total:0.00}\nPagamento: {payment}";

            await DisplayAlert("Reserva Confirmada", message, "OK");


            // Persistir reserva no banco, se disponível
            try
            {
                var services = Application.Current?.Handler?.MauiContext?.Services;
                var db = services?.GetService<AppDbContext>();
                if (db != null)
                {
                    var checkIn = CheckInDate?.Date ?? DateTime.Today;
                    var checkOut = CheckOutDate?.Date ?? DateTime.Today.AddDays(1);
                    var nightsCalc = Math.Max(1, (int)Math.Floor((checkOut - checkIn).TotalDays));
                    var subtotalCalc = nightsCalc * _pricePerNight;
                    var discountAmountCalc = Math.Round(subtotalCalc * (_discountPercent / 100m), 2);
                    var totalCalc = subtotalCalc - discountAmountCalc;

                    var reservation = new Reservation
                    {
                        ChaleName = _chaleName,
                        CheckIn = checkIn,
                        CheckOut = checkOut,
                        Nights = nightsCalc,
                        Total = totalCalc,
                        PaymentMethod = PaymentPicker.SelectedItem as string
                    };

                    db.Reservations.Add(reservation);
                    await db.SaveChangesAsync();
                }
            }
            catch
            {
                // falha ao salvar não impede a experiência do usuário
            }

            // Volta para página anterior
            await Navigation.PopAsync();
        }

        private void OnAplicarCupomClicked(object sender, EventArgs e)
        {
            var code = CouponEntry.Text?.Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(code))
            {
                _discountPercent = 0m;
                UpdateTotals();
                DisplayAlert("Cupom", "Nenhum cupom fornecido.", "OK");
                return;
            }

            // Exemplo: BROTAS10 -> 10% off
            if (code == "BROTAS10")
            {
                _discountPercent = 10m;
                UpdateTotals();
                DisplayAlert("Cupom", "Cupom aplicado: 10% OFF", "OK");
                return;
            }

            // Cupom inválido
            _discountPercent = 0m;
            UpdateTotals();
            DisplayAlert("Cupom", "Cupom inválido.", "OK");
        }

        private void OnThemeToggled(object sender, ToggledEventArgs e)
        {
            try
            {
                Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
            }
            catch
            {
                // ignoreo tach lq na tem nada a ver
            }
        }
    }
}
//codigo feito e refeito por LGAM e outros