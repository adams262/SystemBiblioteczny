using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SystemBiblioteczny.Data;

var builder = WebApplication.CreateBuilder(args);

// ─── USŁUGI ──────────────────────────────────────────────────────

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BibliotekaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Uwierzytelnianie przez cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TylkoBibliotekarze", policy => policy.RequireRole("Bibliotekarz"));
    options.AddPolicy("TylkoCzytelnicy", policy => policy.RequireRole("Czytelnik"));
    options.AddPolicy("Zalogowani", policy => policy.RequireAuthenticatedUser());
});

// ─── PIPELINE ────────────────────────────────────────────────────

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();   // MUSI być przed UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
