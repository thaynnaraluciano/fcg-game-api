using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace Infrastructure.Data
{
    public static class MigrationHelper
    {
        public static void ApplyMigrations(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();   //  Executa migrations pendentes
        }
        //Necessário esperar o docker subir para aplicar as migrations automaticamente.
        public static async Task WaitForMySqlAsync(string connectionString)
        {
            var maxRetries = 30;
            var delaySeconds = 5;

            string[] strConn = connectionString.Split(";");
            for (int i = 1; i <= maxRetries; i++)
            {
                try
                {
                    //RETIRANDO O NOME DO DATABASE POIS ELE PODE NÃO EXISTIR
                    connectionString = $"{strConn[0]};{strConn[1]};{strConn[3]};{strConn[4]};";
                    using var connection = new MySqlConnection(connectionString);
                    await connection.OpenAsync();
                    Console.WriteLine("MySQL está pronto!");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} Tentativa {i}/{maxRetries} falhou: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }

            throw new Exception("MySQL não ficou pronto dentro do tempo esperado.");
        }
    }
}
