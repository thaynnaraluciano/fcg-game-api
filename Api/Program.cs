using Api.Utils;
using CrossCutting.Exceptions.Middlewares;
using Domain.Commands.v1.Biblioteca.ComprarJogo;
using Domain.Commands.v1.Biblioteca.ConsultaBiblioteca;
using Domain.Commands.v1.Jogos.AtualizarJogo;
using Domain.Commands.v1.Jogos.BuscarJogoPorId;
using Domain.Commands.v1.Jogos.CriarJogo;
using Domain.Commands.v1.Jogos.ListarJogos;
using Domain.Commands.v1.Jogos.RemoverJogo;
using Domain.Commands.v1.Promocoes.AtualizarPromocao;
using Domain.Commands.v1.Promocoes.BuscarPromocaoPorId;
using Domain.Commands.v1.Promocoes.CriarPromocao;
using Domain.Commands.v1.Promocoes.ListarPromocoes;
using Domain.Commands.v1.Promocoes.RemoverPromocao;
using Domain.MapperProfiles;
using DotNetEnv;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Prometheus.DotNetRuntime;
using Prometheus;
using Api.Extensions;
using CrossCutting.Monitoring;
using CrossCutting.Configuration;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FIAP Games API",
        Version = "v1",
        Description = "API para gerenciamento de jogos FIAP"
    });

    c.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

#region MediatR
// Jogos
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CriarJogoCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(AtualizarJogoCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(RemoverJogoCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ListarJogosCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(BuscarJogoPorIdCommandHandler).Assembly));
// Promoções
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CriarPromocaoCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(AtualizarPromocaoCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(RemoverPromocaoCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ListarPromocoesCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(BuscarPromocaoPorIdCommandHandler).Assembly));
// Biblioteca
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ConsultaBibliotecaCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ComprarJogoCommandHandler).Assembly));

#endregion

#region AutoMapper
builder.Services.AddAutoMapper(typeof(JogoProfile));
builder.Services.AddAutoMapper(typeof(PromocaoProfile));
builder.Services.AddAutoMapper(typeof(BibliotecaProfile));
#endregion

#region Validators
// Jogos
builder.Services.AddScoped<IValidator<CriarJogoCommand>, CriarJogoCommandValidator>();
builder.Services.AddScoped<IValidator<AtualizarJogoCommand>, AtualizarJogoCommandValidator>();
builder.Services.AddScoped<IValidator<RemoverJogoCommand>, RemoverJogoCommandValidator>();
builder.Services.AddScoped<IValidator<BuscarJogoPorIdCommand>, BuscarJogoPorIdCommandValidator>();
builder.Services.AddScoped<IValidator<ListarJogosCommand>, ListarJogosCommandValidator>();
// Promoções
builder.Services.AddScoped<IValidator<CriarPromocaoCommand>, CriarPromocaoCommandValidator>();
builder.Services.AddScoped<IValidator<AtualizarPromocaoCommand>, AtualizarPromocaoCommandValidator>();
builder.Services.AddScoped<IValidator<RemoverPromocaoCommand>, RemoverPromocaoCommandValidator>();
builder.Services.AddScoped<IValidator<ListarPromocoesCommand>, ListarPromocoesCommandValidator>();
builder.Services.AddScoped<IValidator<BuscarPromocaoPorIdCommand>, BuscarPromocaoPorIdCommandValidator>();
// Biblioteca
builder.Services.AddScoped<IValidator<ConsultaBibliotecaCommand>, ConsultaBibliotecaCommandValidator>();
builder.Services.AddScoped<IValidator<ComprarJogoCommand>, ComprarJogoCommandValidator>();
#endregion

#region Interfaces
builder.Services.AddScoped<IJogoRepository, JogoRepository>();
builder.Services.AddScoped<IPromocaoRepository, PromocaoRepository>();
builder.Services.AddScoped<IBibliotecaRepository, BibliotecaRepository>();
#endregion

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddSingleton<IMetricsService, MetricsService>();

try
{
    Env.Load();
}
catch
{
    //Caso do deploy.
    Console.WriteLine(".env não encontrado, usando apenas variáveis de ambiente...");
}

// Le variáveis de ambiente (do SO, .env ou secrets)
string host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
string db = Environment.GetEnvironmentVariable("DB_NAME") ?? "Bd_Games";
string user = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
string pass = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "root";

string connString = $"Server={host};Database={db};User={user};Password={pass};";


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connString,
        new MySqlServerVersion(new Version(8, 0, 43))
    ));

var app = builder.Build();


    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = new List<OpenApiServer>
            {
                new() { Url = "/game" }
            };
        });
    });
    app.UseSwaggerUI();



app.UseHttpsRedirection();

DotNetRuntimeStatsBuilder.Default().StartCollecting();

app.UseRouting();

app.UseRequestMetrics();
app.MapMetrics();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok("Healthy"));

app.UseMiddleware<MiddlewareTratamentoDeExcecoes>();

app.Run();
