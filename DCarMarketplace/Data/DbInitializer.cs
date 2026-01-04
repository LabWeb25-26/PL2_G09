using Microsoft.AspNetCore.Identity;
using DCarMarketplace.Models;
using System.Linq; // Necessário para usar .Any() e .FirstOrDefault()

namespace DCarMarketplace.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<Utilizador> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // 1. Criar Perfis (Roles)
            string[] roles = { "Administrador", "Vendedor", "Comprador" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Criar Admin
            if (userManager.FindByEmailAsync("admin@utad.pt").Result == null)
            {
                var adminUser = new Utilizador
                {
                    UserName = "admin@utad.pt",
                    Email = "admin@utad.pt",
                    Nome = "Super Administrador",
                    EstadoConta = "ativo",
                    DataRegisto = DateTime.Now,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                    var adminProfile = new Administrador { Id = adminUser.Id };
                    context.Administradores.Add(adminProfile);
                }
            }
                                                                                                                                                                                                         
            // 3. Criar Marcas (Se não existirem)
            if (!context.Marcas.Any())
            {
                var marcas = new Marca[]
                {
                    new Marca { Nome = "BMW" },
                    new Marca { Nome = "Mercedes-Benz" },
                    new Marca { Nome = "Audi" },
                    new Marca { Nome = "Renault" },
                    new Marca { Nome = "Tesla" },
                    new Marca { Nome = "Peugeot" },
                    new Marca { Nome = "Volkswagen" }
                };
                context.Marcas.AddRange(marcas);
                await context.SaveChangesAsync(); // Gravar já para gerar os IDs
            }

            // 4. Criar Combustíveis
            if (!context.Combustiveis.Any())
            {
                var combustiveis = new Combustivel[]
                {
                    new Combustivel { Tipo = "Gasolina" },
                    new Combustivel { Tipo = "Diesel" },
                    new Combustivel { Tipo = "Elétrico" },
                    new Combustivel { Tipo = "Híbrido" }
                };
                context.Combustiveis.AddRange(combustiveis);
            }

            // 5. CRIAR MODELOS (NOVO BLOCO)
            if (!context.Modelos.Any())
            {
                // Vamos buscar as marcas à BD para ter os IDs corretos
                var bmw = context.Marcas.FirstOrDefault(m => m.Nome == "BMW");
                var mercedes = context.Marcas.FirstOrDefault(m => m.Nome == "Mercedes-Benz");
                var audi = context.Marcas.FirstOrDefault(m => m.Nome == "Audi");
                var renault = context.Marcas.FirstOrDefault(m => m.Nome == "Renault");
                var tesla = context.Marcas.FirstOrDefault(m => m.Nome == "Tesla");

                var modelos = new List<Modelo>();

                if (bmw != null)
                {
                    modelos.Add(new Modelo { Nome = "Série 3", MarcaId = bmw.Id });
                    modelos.Add(new Modelo { Nome = "Série 5", MarcaId = bmw.Id });
                    modelos.Add(new Modelo { Nome = "X5", MarcaId = bmw.Id });
                }
                if (mercedes != null)
                {
                    modelos.Add(new Modelo { Nome = "Classe A", MarcaId = mercedes.Id });
                    modelos.Add(new Modelo { Nome = "Classe C", MarcaId = mercedes.Id });
                }
                if (audi != null)
                {
                    modelos.Add(new Modelo { Nome = "A3", MarcaId = audi.Id });
                    modelos.Add(new Modelo { Nome = "A4", MarcaId = audi.Id });
                }
                if (renault != null)
                {
                    modelos.Add(new Modelo { Nome = "Clio", MarcaId = renault.Id });
                    modelos.Add(new Modelo { Nome = "Megane", MarcaId = renault.Id });
                }
                if (tesla != null)
                {
                    modelos.Add(new Modelo { Nome = "Model 3", MarcaId = tesla.Id });
                    modelos.Add(new Modelo { Nome = "Model Y", MarcaId = tesla.Id });
                }

                context.Modelos.AddRange(modelos);
            }

            await context.SaveChangesAsync();
        }
    }
}