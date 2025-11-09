using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Добавляем фонового бота
builder.Services.AddHostedService<BotWorker>();

// Добавляем контроллеры и Razor Pages
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Настройка портов для Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

// Для проверки, что приложение запущено
app.MapGet("/", () => "Bot is running!");

// Настройка маршрутов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Можно закомментировать для Render
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages(); // <--- это обязательно для Razor Pages
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
