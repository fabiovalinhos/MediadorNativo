namespace Mediator_Nativo.Dominio
{
    public class Produto
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public string LocalDeEstoque { get; private set; }

        public Produto(string nome, string descricao, string localDeEstoque)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Descricao = descricao;
            LocalDeEstoque = localDeEstoque;
        }

        // Para EF Core ou deserialização, se necessário
        private Produto() { }
    }
}
