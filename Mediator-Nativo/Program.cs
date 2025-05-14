using Mediator_Nativo.CQRS;
using Mediator_Nativo.Dominio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddTransient<IMediator, Mediator>();
builder.Services.AddTransient<IRequestHandler<GetProdutosQuery, List<Produto>>, GetProdutosQueryHandler>();
builder.Services.AddScoped<IRequestHandler<AddProdutoCommand, Produto>, AddProdutoCommandHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
