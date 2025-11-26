using DCARMarketplace.Models;

namespace DCARMarketplace.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Se já houver anúncios, não faz nada
            if (context.Anuncios.Any())
            {
                return;
            }

            // 1. Criar Marcas e Combustíveis
            var bmw = new Marca { Nome = "BMW" };
            var mercedes = new Marca { Nome = "Mercedes-Benz" };
            context.Marcas.AddRange(bmw, mercedes);

            var gasolina = new Combustivel { Tipo = "Gasolina" };
            var diesel = new Combustivel { Tipo = "Diesel" };
            context.Combustiveis.AddRange(gasolina, diesel);
            context.SaveChanges();

            // 2. Criar Modelos
            var m3 = new Modelo { Nome = "M3", Marca = bmw };
            var cClass = new Modelo { Nome = "Classe C", Marca = mercedes };
            context.Modelos.AddRange(m3, cClass);
            context.SaveChanges();

            // 3. Criar um Vendedor Falso
            var vendedor = new Vendedor
            {
                Nome = "Stand Auto",
                Email = "stand@auto.com",
                Username = "standauto",
                Password = "123",
                Tipo = "Empresa",
                NIF = "999888777"
            };
            context.Vendedores.Add(vendedor);
            context.SaveChanges();

            // 4. Criar Carros
            var carro1 = new Carro { VIN = "123BMW", Matricula = "AA-00-BB", Ano = 2021, Quilometragem = 45000, Caixa = "Automática", Modelo = m3, Combustivel = gasolina };
            var carro2 = new Carro { VIN = "456MERC", Matricula = "CC-11-DD", Ano = 2019, Quilometragem = 80000, Caixa = "Manual", Modelo = cClass, Combustivel = diesel };
            context.Carros.AddRange(carro1, carro2);
            context.SaveChanges();

            // 5. Criar Anúncios
            var anuncio1 = new Anuncio
            {
                Titulo = "BMW M3 Competition",
                Descricao = "Carro desportivo em estado irrepreensível.",
                Preco = 85000,
                DataInicio = DateTime.Now,
                Localizacao = "Lisboa",
                Estado = "ativo",
                Vendedor = vendedor,
                Carro = carro1,
                Fotos = "https://images.unsplash.com/photo-1555215695-3004980adade?w=500&q=80"
            };

            var anuncio2 = new Anuncio
            {
                Titulo = "Mercedes Classe C Económico",
                Descricao = "Ótimo citadino a diesel.",
                Preco = 32500,
                DataInicio = DateTime.Now,
                Localizacao = "Porto",
                Estado = "ativo",
                Vendedor = vendedor,
                Carro = carro2,
                Fotos = "https://images.unsplash.com/photo-1617788138017-80ad40651399?w=500&q=80"
            };

            context.Anuncios.AddRange(anuncio1, anuncio2);
            context.SaveChanges();
        }
    }
}