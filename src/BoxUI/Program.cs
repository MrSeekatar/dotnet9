using BoxUI.Components;
using BoxUI.Services;
using Refit;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();  // Aspire extension method in Box.ServiceDefaults

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var url = builder.Configuration.GetValue<string>("BoxServerUrl");
if (string.IsNullOrEmpty(url))
{
    throw new InvalidOperationException("BoxServerUrl is required");
}

var settings = new RefitSettings
{
    ContentSerializer = new NewtonsoftJsonContentSerializer()
};

builder.Services.AddRefitClient<IBoxService>(settings).ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri(url);
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

app.MapStaticAssets(); // ASP.NET 9

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();