using System;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Recanto_da_natureza.Data;
using Recanto_da_natureza.Models;
using Recanto_da_natureza.Helpers;

namespace Recanto_da_natureza;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private AppDbContext GetDb()
    {
        try
        {
            var services = Application.Current?.Handler?.MauiContext?.Services;
            return services?.GetService<AppDbContext>();
        }
        catch
        {
            return null;
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var name = RegNameEntry.Text?.Trim();
        var email = RegEmailEntry.Text?.Trim()?.ToLowerInvariant();
        var phone = RegPhoneEntry.Text?.Trim();
        var password = RegPasswordEntry.Text;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Erro", "Preencha nome, e-mail e senha.", "OK");
            return;
        }

        var db = GetDb();
        if (db == null)
        {
            await DisplayAlert("Erro", "Banco de dados não disponível.", "OK");
            return;
        }

        var exists = await db.Users.FindAsync(email) ;
        // FindAsync by key won't work because key is Guid; use query
        var userExists = await db.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (userExists != null)
        {
            await DisplayAlert("Erro", "E-mail já cadastrado.", "OK");
            return;
        }

        var user = new User
        {
            Name = name,
            Email = email,
            Phone = phone,
            PasswordHash = PasswordHasher.Hash(password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        await DisplayAlert("Sucesso", "Cadastro realizado.", "OK");
        // Limpar campos
        RegNameEntry.Text = RegEmailEntry.Text = RegPhoneEntry.Text = RegPasswordEntry.Text = string.Empty;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = LoginEmailEntry.Text?.Trim()?.ToLowerInvariant();
        var password = LoginPasswordEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Erro", "Preencha e-mail e senha.", "OK");
            return;
        }

        var db = GetDb();
        if (db == null)
        {
            await DisplayAlert("Erro", "Banco de dados não disponível.", "OK");
            return;
        }

        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            await DisplayAlert("Erro", "Usuário não encontrado.", "OK");
            return;
        }

        if (!PasswordHasher.Verify(password, user.PasswordHash))
        {
            await DisplayAlert("Erro", "Senha incorreta.", "OK");
            return;
        }

        await DisplayAlert("Sucesso", $"Bem-vindo, {user.Name}!", "OK");
        // Aqui você pode navegar para MainPage ou armazenar sessão conforme necessário
    }
}
