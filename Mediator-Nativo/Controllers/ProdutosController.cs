using Mediator_Nativo.CQRS;
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
    }
}
