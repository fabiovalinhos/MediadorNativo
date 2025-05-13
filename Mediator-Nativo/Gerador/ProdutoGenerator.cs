using Mediator_Nativo.Dominio;
using System.Text.Json;

namespace Mediator_Nativo.Gerador
{
    public static class ProdutoGenerator
    {
        public static List<Produto> GerarProdutos()
        {
            var json = @"
        [
            { ""Nome"": ""Produto A"", ""Descricao"": ""Descrição A"", ""LocalDeEstoque"": ""Estoque A"" },
            { ""Nome"": ""Produto B"", ""Descricao"": ""Descrição B"", ""LocalDeEstoque"": ""Estoque B"" },
            { ""Nome"": ""Produto C"", ""Descricao"": ""Descrição C"", ""LocalDeEstoque"": ""Estoque C"" },
            { ""Nome"": ""Produto D"", ""Descricao"": ""Descrição D"", ""LocalDeEstoque"": ""Estoque D"" },
            { ""Nome"": ""Produto E"", ""Descricao"": ""Descrição E"", ""LocalDeEstoque"": ""Estoque E"" },
            { ""Nome"": ""Produto F"", ""Descricao"": ""Descrição F"", ""LocalDeEstoque"": ""Estoque F"" },
            { ""Nome"": ""Produto G"", ""Descricao"": ""Descrição G"", ""LocalDeEstoque"": ""Estoque G"" },
            { ""Nome"": ""Produto H"", ""Descricao"": ""Descrição H"", ""LocalDeEstoque"": ""Estoque H"" },
            { ""Nome"": ""Produto I"", ""Descricao"": ""Descrição I"", ""LocalDeEstoque"": ""Estoque I"" },
            { ""Nome"": ""Produto J"", ""Descricao"": ""Descrição J"", ""LocalDeEstoque"": ""Estoque J"" }
        ]";

            var produtos = JsonSerializer.Deserialize<List<Produto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Atribuir Guid manualmente porque o JSON não tem ID
            foreach (var p in produtos!)
            {
                typeof(Produto).GetProperty("Id")!
                    .SetValue(p, Guid.NewGuid());
            }

            return produtos;
        }
    }
}
