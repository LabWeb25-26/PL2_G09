using DCarMarketplace.Data;
using DCarMarketplace.Models; // <--- OBRIGAT�RIO: Para encontrar a classe Utilizador
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar a Base de Dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. CONFIGURA��O DO IDENTITY (CORRIGIDA)
// Mud�mos <IdentityUser> para <Utilizador>
// Adicion�mos .AddRoles<IdentityRole>()
builder.Services.AddDefaultIdentity<Utilizador>(options =>
{
    // Facilita a vida em desenvolvimento (n�o pede confirma��o de email nem passwords complexas)
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
    .AddRoles<IdentityRole>() // <--- IMPORTANTE: Ativa os Perfis (Admin, Vendedor)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. BLOCO DE SEEDING (POVOAMENTO)
// Este c�digo corre o DbInitializer para criar o Admin e as Marcas
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<Utilizador>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Chama a classe que criaste na pasta Data
        await DbInitializer.Initialize(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao povoar a base de dados.");
    }
}

// Configurar o pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 4. ATIVAR AUTENTICA��O (Antes da Autoriza��o!)
app.UseAuthentication(); // <--- OBRIGAT�RIO: "Quem �s tu?"
app.UseAuthorization();  // <--- OBRIGAT�RIO: "O que podes fazer?"

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();