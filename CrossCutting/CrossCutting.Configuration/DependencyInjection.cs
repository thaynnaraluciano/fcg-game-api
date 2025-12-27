using Infrastructure.Data.Repositories;
using Infrastructure.Data.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace CrossCutting.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddCrossCutting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddElasticsearch(services, configuration);

        return services;
    }

    private static void AddElasticsearch(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = new ElasticsearchClientSettings(
            new Uri(configuration["ElasticSearch:urlApi"]))
            .Authentication(new ApiKey(configuration["ElasticSearch:Key"]))
            .DefaultIndex("games");

        services.AddSingleton(new ElasticsearchClient(settings));
    }
}
