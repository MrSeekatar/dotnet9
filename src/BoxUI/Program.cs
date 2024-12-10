using BoxUI.Components;
using BoxUI.Services;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var settings = new RefitSettings
{
    ContentSerializer = new NewtonsoftJsonContentSerializer()
};

builder.Services.AddRefitClient<IBoxService>(settings).ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri("http://localhost:5000/api/v1");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();