using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace aula8_eweb.Data
{
    public static class SeedRoles
    {
        public static void Seed(RoleManager<IdentityRole> roleManager)
        {
            // Verifica se alguma Role já existe
            if (roleManager.Roles.Any() == false)
            {
                // Cria as roles "Admin" e "Client" se não existirem
                roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                roleManager.CreateAsync(new IdentityRole("Client")).Wait();
            }
        }
    }
}