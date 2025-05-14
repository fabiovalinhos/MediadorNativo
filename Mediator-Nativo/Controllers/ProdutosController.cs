using Mediator_Nativo.CQRS;
using Mediator_Nativo.Dominio;
using Mediator_Nativo.Gerador;
using Microsoft.AspNetCore.Mvc;

namespace Mediator_Nativo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProdutosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var produtos = await _mediator.Send(new GetProdutosQuery());
            return Ok(produtos);
        }

        [HttpPost]
        public async Task<ActionResult<Produto>> Add([FromBody] AddProdutoCommand command)
        {
            var produtoCriado = await _mediator.Send(command);

            //deveria apontar para outro endpoint
            return CreatedAtAction(nameof(Add), new { id = produtoCriado.Id }, produtoCriado);
        }
    }
}
