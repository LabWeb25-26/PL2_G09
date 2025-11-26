using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using aula8_eweb.Models; // <<< ISTO É O QUE ESTÁ FALTANDO!

namespace aula8_eweb.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Adicione este DbSet para a nova tabela Perfils
        public DbSet<Perfil> Perfils { get; set; }
    }
}