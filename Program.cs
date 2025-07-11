using EmployeeDepartmentCRUDApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("db_con"));
});

builder.Services.AddSession();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await SeedAdminUser(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred seeding the DB: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();

static async Task SeedAdminUser(ApplicationDbContext context)
{
    if (!context.Users.Any(u => u.Username == "admin"))
    {
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "admin")
                        ?? new Role { Name = "admin" };

        if (adminRole.Id == 0)
            await context.Roles.AddAsync(adminRole);

        var passwordHasher = new PasswordHasher<User>();
        var adminUser = new User
        {
            Username = "admin",
            Password = passwordHasher.HashPassword(null, "admin@123")
        };

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();

        await context.UserRoles.AddAsync(new UserRole
        {
            UserId = adminUser.Id,
            RoleId = adminRole.Id
        });

        await context.SaveChangesAsync();
    }
}