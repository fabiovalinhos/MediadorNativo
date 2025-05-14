using Mediator_Nativo.CQRS;

namespace Mediator_Nativo.Dominio
{
    public class AddProdutoCommand : IRequest<Produto>
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string LocalDeEstoque { get; set; } = string.Empty;
    }
}