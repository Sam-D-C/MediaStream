using MediaStream.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<MediaService>();
builder.Services.AddSingleton<PlaylistService>();
builder.Services.AddSingleton<SettingsService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("VueFrontend");
app.UseRouting();

// Serveer de gebouwde Vue bestanden
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

// Alle onbekende routes gaan naar index.html (Vue Router)
app.MapFallbackToFile("index.html");

app.Run("http://localhost:5000");