using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using censudex_api_gateway.src.Protos.Clients;
using System.Net.Http;

namespace translator.src.Extensions
{
    public static class GrpcClientsConfig
    {
        private const string BaseName = "clients-service";
        private const int Port = 5003;
        private const int InstanceCount = 5;

        public static IServiceCollection AddGrpcClients(this IServiceCollection services)
        {
            var channels = new List<GrpcChannel>();

            for (int i = 1; i <= InstanceCount; i++)
            {
                string url = $"http://{BaseName}-{i}:{Port}";
                Console.WriteLine($"[gRPC] Registering instance: {url}");

                // Create a custom HttpClient that allows HTTP/1.1
                var httpClientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                var httpClient = new HttpClient(httpClientHandler);

                var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
                {
                    HttpClient = httpClient,
                    DisposeHttpClient = true
                });

                channels.Add(channel);
            }

            services.AddSingleton(channels);
            services.AddSingleton<IClientsGrpcClient, ClientsGrpcLoadBalancer>();

            return services;
        }
    }

    public interface IClientsGrpcClient
    {
        ClientsService.ClientsServiceClient GetClient();
    }

    public class ClientsGrpcLoadBalancer : IClientsGrpcClient
    {
        private readonly List<GrpcChannel> _channels;
        private int _index = 0;

        public ClientsGrpcLoadBalancer(List<GrpcChannel> channels)
        {
            _channels = channels;
        }

        public ClientsService.ClientsServiceClient GetClient()
        {
            int pos = Interlocked.Increment(ref _index) % _channels.Count;
            return new ClientsService.ClientsServiceClient(_channels[pos]);
        }
    }
}