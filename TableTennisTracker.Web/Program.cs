using TableTennisTracker.Web.Infrastructure;
using TableTennisTracker.Web.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Register mock repositories
builder.Services.AddScoped<ITournamentRepository, MockTournamentRepository>();
builder.Services.AddScoped<IVenueRepository, MockVenueRepository>();
builder.Services.AddScoped<IPlayerRepository, MockPlayerRepository>();
builder.Services.AddScoped<IMatchRepository, MockMatchRepository>();
builder.Services.AddScoped<IRegistrationRepository, MockRegistrationRepository>();
builder.Services.AddScoped<IMatchParticipantRepository, MockMatchParticipantRepository>();
builder.Services.AddScoped<IMatchSetResultRepository, MockMatchSetResultRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
