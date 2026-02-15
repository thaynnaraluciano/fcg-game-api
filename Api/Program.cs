using Api.Extensions;
using Api.Services;
using Api.Utils;
using CrossCutting.Configuration;
using CrossCutting.Exceptions.Middlewares;
using CrossCutting.Monitoring;
using Domain.Commands.v1.Biblioteca.ComprarJogo;
using Domain.Commands.v1.Biblioteca.ConsultaBiblioteca;
using Domain.Commands.v1.Jogos.AtualizarJogo;
using Domain.Commands.v1.Jogos.BuscarJogoeSugestoes;
using Domain.Commands.v1.Jogos.BuscarJogoPorId;
using Domain.Commands.v1.Jogos.CriarJogo;
using Domain.Commands.v1.Jogos.JogosPopulares;
using Domain.Commands.v1.Jogos.ListarJogos;
using Domain.Commands.v1.Jogos.RemoverJogo;
using Domain.Commands.v1.Jogos.SugerirJogos;
using Domain.Commands.v1.Promocoes.AtualizarPromocao;
using Domain.Commands.v1.Promocoes.BuscarPromocaoPorId;
using Domain.Commands.v1.Promocoes.CriarPromocao;
using Domain.Commands.v1.Promocoes.ListarPromocoes;
using Domain.Commands.v1.Promocoes.RemoverPromocao;
using Domain.MapperProfiles;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.Repositories;
using Infrastructure.Messaging.Configuration;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.Publishers;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Prometheus;
using Prometheus.DotNetRuntime;
using System.Text.Json.Serialization;

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
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(JogosPopularesCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(SugerirJogosCommandHandler).Assembly));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(BuscaJogoeSugestoesCommandHandler).Assembly));
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
builder.Services.AddScoped<IValidator<JogosPopularesCommand>, JogosPopularesCommandValidator>();
builder.Services.AddScoped<IValidator<SugerirJogosCommand>, SugerirJogosCommandValidator>();
builder.Services.AddScoped<IValidator<BuscaJogoeSugestoesCommand>, BuscaJogoeSugestoesCommandValidator>();
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
builder.Services.AddScoped<IJogoESRepository, JogoESRepository>();
builder.Services.AddScoped<IPromocaoRepository, PromocaoRepository>();
builder.Services.AddScoped<IBibliotecaRepository, BibliotecaRepository>();
builder.Services.AddScoped<IGameAvailableEventPublisher, GameAvailableEventPublisher>();
#endregion

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddSingleton<IMetricsService, MetricsService>();
//Le variáveis do .env para o elasticSearch
string elasticUrl = Environment.GetEnvironmentVariable("ELASTICSEARCH_URLAPI") ?? builder.Configuration["ElasticSearch:urlApi"] ?? "";
string elasticKey = Environment.GetEnvironmentVariable("ELASTICSEARCH_KEY") ?? builder.Configuration["ElasticSearch:Key"] ?? "";
builder.Configuration["ElasticSearch:urlApi"] = elasticUrl;
builder.Configuration["ElasticSearch:Key"] = elasticKey;

#if DEBUG
//Chama o gerenciador do docker ANTES da aplicação iniciar
string connString = builder.Configuration.GetConnectionString(name: "DefaultConnection") ?? "";
await DockerMySqlManager.EnsureMySqlContainerRunningAsync(connString);
#else

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

#endif

#region RABBIT MQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentConfirmedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        MassTransitConfiguration.Configure(context, cfg);
    });
});

#endregion

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connString,
        new MySqlServerVersion(new Version(8, 0, 43))
    ));

builder.Services.AddCrossCutting(builder.Configuration);

var app = builder.Build();
#if DEBUG
//Aguardando docker subir.
await Infrastructure.Data.MigrationHelper.WaitForMySqlAsync(connString);
//Aplica migrations se não estiver atualizado
Infrastructure.Data.MigrationHelper.ApplyMigrations(app);

#endif


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
