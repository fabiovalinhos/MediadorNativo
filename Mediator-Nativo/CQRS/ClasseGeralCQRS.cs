using System.Reflection;
using Mediator_Nativo.Dominio;
using Mediator_Nativo.Gerador;

namespace Mediator_Nativo.CQRS
{
    public interface IRequest<TResponse> { }

    public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    }

    public class Mediator : IMediator
    {
        private readonly IServiceProvider _provider;

        public Mediator(IServiceProvider provider)
        {
            _provider = provider;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            using var scope = _provider.CreateScope(); // escopo criado 
            var scopedProvider = scope.ServiceProvider;

            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            //IRequestHandler<GetProdutosQuery, List<Produto>>


            try
            {
                var handler = scopedProvider.GetRequiredService(handlerType);
                // posso usar o GetService, mas ele nao lança exceção se não encontrar o handler.
                // Apenas retorna null

                var method = handlerType.GetMethod("Handle")!;
                //Por que usar o handlerType e não handler?
                // Porque:
                //Garante que você está pegando o método da interface esperada.
                //Evita pegar acidentalmente uma implementação fora do contrato.
                //Funciona bem quando você quer chamar algo dinamicamente e com segurança.

                return (Task<TResponse>)method.Invoke(handler, new object[] { request, cancellationToken })!;
            }
            catch (InvalidOperationException ex)
            {
                // Handler não registrado no DI
                throw new InvalidOperationException($"Handler não encontrado para {request.GetType().Name}", ex);
            }
            catch (TargetInvocationException ex)
            {
                // Exceção lançada dentro do método Handle
                throw new Exception($"Erro ao executar o handler para {request.GetType().Name}: {ex.InnerException?.Message}", ex.InnerException);
            }
        }
    }

    public class GetProdutosQuery : IRequest<List<Produto>> { }


    public class GetProdutosQueryHandler : IRequestHandler<GetProdutosQuery, List<Produto>>
    {
        public Task<List<Produto>> Handle(GetProdutosQuery request, CancellationToken cancellationToken)
        {
            var produtos = ProdutoGenerator.GerarProdutos();
            return Task.FromResult(produtos);
        }
    }

    public class AddProdutoCommandHandler : IRequestHandler<AddProdutoCommand, Produto>
    {
        // Lista estática simulando um "banco em memória"
        private static readonly List<Produto> _produtos = new();

        public Task<Produto> Handle(AddProdutoCommand request, CancellationToken cancellationToken)
        {
            var produto = new Produto(request.Nome, request.Descricao, request.LocalDeEstoque);
            _produtos.Add(produto);
            return Task.FromResult(produto);
        }
    }


    //    [Request HTTP] GET /api/produtos
    //        ↓
    //    [Controller] chama _mediator.Send(new GetProdutosQuery())
    //        ↓
    //    [Mediator] identifica o tipo de handler esperado
    //        ↓
    //    [DI] encontra o GetProdutosQueryHandler
    //        ↓
    //    [Reflection] chama o método Handle()
    //        ↓
    //    [Handler] retorna List<Produto> com dados
    //        ↓
    //    [Controller] retorna 200 OK com os dados

    // ######

    //[HTTP POST] → Controller
    //               ↓
    //         AddProdutoCommand
    //               ↓
    //          Mediator.Send()
    //               ↓
    //   AddProdutoCommandHandler.Handle()
    //               ↓
    //         Retorna Produto
    //               ↓
    //        Controller → 200 OK

}
