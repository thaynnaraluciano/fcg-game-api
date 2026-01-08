using Docker.DotNet;
using Docker.DotNet.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Api.Services
{
    public static class DockerMySqlManager
    {
        private const string ContainerName = "mysql_8_0_43_container";
        private const string ImageName = "mysql:8.0.43";

        private static DockerClient GetClient()
        {
            Uri dockerUri;

            if (OperatingSystem.IsWindows())
            {
                dockerUri = new Uri("npipe://./pipe/docker_engine");
            }
            else if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
            {
                dockerUri = new Uri("unix:///var/run/docker.sock");
            }
            else
            {
                throw new PlatformNotSupportedException("Sistema operacional não suportado para Docker.");
            }

            return new DockerClientConfiguration(dockerUri).CreateClient();
        }
        static string GetPassword(string conn)
        {
            var match = Regex.Match(conn, @"(?i)(?:password|pwd)\s*=\s*([^;]+)");
            return match.Success ? match.Groups[1].Value.Trim() : null??"";
        }

        public static async Task EnsureMySqlContainerRunningAsync(string conn)
        {
            string senhaDocker = GetPassword(conn);
            var client = GetClient();

            if (!await ImageExists(client))
                await PullImage(client);

            string containerId = await GetContainerId(client);

            if (containerId == null)
                containerId = await CreateContainer(client,senhaDocker);

            if (!await IsContainerRunning(client, containerId))
                await StartContainer(client, containerId);
        }

        private static async Task<bool> ImageExists(DockerClient client)
        {
            var images = await client.Images.ListImagesAsync(new ImagesListParameters
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    ["reference"] = new Dictionary<string, bool>
                    {
                        [ImageName] = true
                    }
                }
            });

            return images.Any();
        }

        private static async Task PullImage(DockerClient client)
        {
            Console.WriteLine("Baixando imagem MySQL...");
            await client.Images.CreateImageAsync(
                new ImagesCreateParameters { FromImage = ImageName },
                null,
                new Progress<JSONMessage>(m => Console.WriteLine(m.Status))
            );
        }

        private static async Task<string?> GetContainerId(DockerClient client)
        {
            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true,
            });

            return containers
                .FirstOrDefault(c => c.Names.Contains("/" + ContainerName))
                ?.ID;
        }

        private static async Task<bool> IsContainerRunning(DockerClient client, string containerId)
        {
            var container = await client.Containers.ListContainersAsync(new ContainersListParameters());

            return container.Any(c => c.ID == containerId);
        }

        private static async Task<string> CreateContainer(DockerClient client, string RootPassword)
        {
            Console.WriteLine("Criando container MySQL 8.0.43...");

            var response = await client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Name = ContainerName,
                Image = ImageName,
                Env = new List<string>
            {
                $"MYSQL_ROOT_PASSWORD={RootPassword}"
            },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        ["3306/tcp"] = new List<PortBinding>
                    {
                        new PortBinding { HostPort = "3306" }
                    }
                    }
                }
            });

            return response.ID;
        }

        private static async Task StartContainer(DockerClient client, string containerId)
        {
            Console.WriteLine("Iniciando container MySQL...");
            await client.Containers.StartContainerAsync(containerId, null);
        }
    }
}
