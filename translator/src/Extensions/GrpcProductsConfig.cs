using censudex_api_gateway.src.Protos.Product;
using Grpc.Net.Client;


namespace translator.src.Extensions
{
    public static class GrpcProductsConfig
    {
        public static IServiceCollection AddGrpcProductsClient(this IServiceCollection services)
        {
            var channels = new List<GrpcChannel>();
            string url = "http://products-service-1:5004";
            Console.WriteLine($"[gRPC] Registering products instance: {url}");

            
            var handler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true,
                AllowAutoRedirect = false,
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (_, _, _, _) => true
                }
            };

            // Create channel
            var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
            {
                HttpHandler = handler
            });

            channels.Add(channel);

            
            services.AddSingleton(channels);
            services.AddSingleton<IProductsGrpcClient, ProductsGrpcLoadBalancer>();

            return services;
        }
    }

    public interface IProductsGrpcClient
    {
        ProductService.ProductServiceClient GetClient();
    }

    public class ProductsGrpcLoadBalancer : IProductsGrpcClient
    {
        private readonly List<GrpcChannel> _channels;
        private int _index = 0;

        public ProductsGrpcLoadBalancer(List<GrpcChannel> channels)
        {
            _channels = channels;
        }

        public ProductService.ProductServiceClient GetClient()
        {
            int pos = Interlocked.Increment(ref _index) % _channels.Count;
            return new ProductService.ProductServiceClient(_channels[pos]);
        }
    }
}
