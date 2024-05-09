using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Katastar.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}
   );
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddRazorPages();


builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
options.TokenLifespan = TimeSpan.FromHours(2));
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

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Guest}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "user",
    pattern: "{controller=User}/{action=Index}/{id?}");




async Task Main()
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new[] { "Admin", "Sluzbenik" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string email = "admin@admin.com";
        string password = "Admin123_";
        string emailSluzb = "sluzbenik@test.com";
        string passwordSluzb = "Sluzbenik123_";
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new ApplicationUser();

            user.FirstName = email;
            user.LastName = email;
            user.PhoneNumber = "061222333";
            user.UserName = email;
            user.Email = email;
            user.NormalizedEmail = userManager.NormalizeEmail(email);

            await userManager.CreateAsync(user, password);

            await userManager.AddToRoleAsync(user, "Admin");
        }
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var userSluzb = new ApplicationUser();

            userSluzb.FirstName = emailSluzb;
            userSluzb.LastName = emailSluzb;
            userSluzb.PhoneNumber = "061212333";
            userSluzb.UserName = emailSluzb;
            userSluzb.Email = emailSluzb;
            userSluzb.NormalizedEmail = userManager.NormalizeEmail(emailSluzb);

            await userManager.CreateAsync(userSluzb, passwordSluzb);

            await userManager.AddToRoleAsync(userSluzb, "Sluzbenik");
        }
    }

    app.Run();
}

await Main();