using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MipWeb.Data;
using MipWeb.Models;
using MipWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
object value = builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Use session
builder.Services.AddSession();

// Add DbContext
builder.Services.AddDbContext<MipWebContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cookie Authentication
builder.Services.AddAuthentication("MipAuth")
    .AddCookie("MipAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "MipWebAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

// Secure cookies policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.Secure = CookieSecurePolicy.Always; // Only send cookies over HTTPS
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
});

builder.Services.Configure<AuthorizationOptions>(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder("MipAuth")
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<WhatsAppService>(); 
builder.Services.AddSingleton<WhatsAppService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseCookiePolicy();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Create DB if not exists (choose one)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MipWebContext>();
    db.Database.Migrate();
    db.Database.EnsureCreated();

    // Seed Roles
    if (!db.Roles.Any())
    {
        var roles = new[]
        {
            new Role { Name = "Admin", Description = "System administrator" },
            new Role { Name = "Faculty", Description = "Faculty member" },
            new Role { Name = "Partner", Description = "Partner of Marutham Isai Palli" }
        };

        db.Roles.AddRange(roles);
        db.SaveChanges();
    }

    // Seed admin user if not exists
        var adminUserId = "admin";
    if (!db.Users.Any(u => u.Username == adminUserId))
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123");
        db.Users.Add(new User
        {
            Username = adminUserId,
            PasswordHash = hashedPassword,
            Email = "maruthamisaipalli@gmail.com",
            IsActive = true,
            Name = "Administrator",
            WhatsAppNumber = "9789066079"
        });
        db.SaveChanges();
        Console.WriteLine("✅ Default admin user created: username='admin', password='admin123'");
    }

    // Seed default admin user
    var admin = db.Users.Include(u => u.UserRoles).FirstOrDefault(u => u.Username == adminUserId);

    if (admin != null)
    {
        var adminRole = db.Roles.First(r => r.Name == AppRoles.Admin);

        if (!admin.UserRoles.Any(ur => ur.RoleId == adminRole.Id))
        {
            db.UserRoles.Add(new UserRole
            {
                UserId = admin.Id,
                RoleId = adminRole.Id
            });

            db.SaveChanges();
        }
    }
}

app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming request: {context.Request.Path}");
    await next();
});

app.Run();

// Example seed inside Program.cs after EnsureCreated()

