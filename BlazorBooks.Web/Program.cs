using BlazorBooks.Shared.Interfaces;
using BlazorBooks.Web;
using BlazorBooks.Web.Components;
using BlazorBooks.Web.Data;
using BlazorBooks.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<BookContext>(options =>
{
    var connectionString = builder.Configuration.GetSection("pgSettings")["pgConnection"];
    options.UseNpgsql(connectionString);
});

// Add services to the container.
builder.Services.AddRazorComponents();

builder.Services.AddTransient<IBookService, BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(BlazorBooks.Shared.Components.Pages.Books).Assembly);

app.MapBookEndPoints();

app.Run();
