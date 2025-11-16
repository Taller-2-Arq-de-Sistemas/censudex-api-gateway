using Grpc.Net.Client;
using censudex_api_gateway.src.Protos.Clients;

namespace translator.src.Extensions
{
    public static class GrpcClientsConfig
    {
        public static IServiceCollection AddGrpcClients(this IServiceCollection services)
        {
            var channels = new List<GrpcChannel>();
            string url = "http://clients-service-1:5003";
            Console.WriteLine($"[gRPC] Registering instance: {url}");

            // Create a custom HttpClient that allows HTTP/1.1
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var handler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true,
                AllowAutoRedirect = false
            };

            handler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (_, _, _, _) => true
            };

            var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
            {
                HttpHandler = handler
            });

            channels.Add(channel);


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