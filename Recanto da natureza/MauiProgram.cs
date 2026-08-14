namespace Recanto_da_natureza;

using System.IO;
using Microsoft.EntityFrameworkCore;
using Recanto_da_natureza.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Configurar DbContext SQLite com arquivo local
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        var app = builder.Build();

        // Garantir que banco de dados exista
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }
        catch
        {
            // Não falhar a inicialização do app se houver problema ao criar o DB
        }

        return app;
    }
}
