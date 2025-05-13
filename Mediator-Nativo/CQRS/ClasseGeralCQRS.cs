using Mediator_Nativo.Dominio;
using Mediator_Nativo.Gerador;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

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
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));//IRequestHandler<GetProdutosQuery, List<Produto>>
            var handler = _provider.GetService(handlerType); // GetProdutosQueryHandler
            // poderia ter usado o .GetRequiredService pois ele lança uma exceção se não encontrar o serviço

            if (handler == null)
                throw new InvalidOperationException($"Handler não encontrado para {request.GetType().Name}");

            var method = handlerType.GetMethod("Handle")!;
            //Por que usar o handlerType e não handler?
            // Porque:
            //Garante que você está pegando o método da interface esperada.
            //Evita pegar acidentalmente uma implementação fora do contrato.
            //Funciona bem quando você quer chamar algo dinamicamente e com segurança.

            return (Task<TResponse>)method.Invoke(handler, new object[] { request, cancellationToken })!;
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
}
