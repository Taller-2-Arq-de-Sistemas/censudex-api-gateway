using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using censudex_api_gateway.src.Protos.Product;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using translator.src.Dtos.Clients.Product;
using translator.src.Extensions;


namespace translator.src.Controllers
{
    [ApiController]
    [Route("product")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsGrpcClient _grpcClient;

        public ProductsController(IProductsGrpcClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var client = _grpcClient.GetClient();
                var response = await client.GetAllProductsAsync(new Empty());

                return Ok(new
                {
                    Products = response.Products
                });
            }
            catch (RpcException ex)
            {
                return StatusCode((int)MapGrpcToHttp(ex.StatusCode), ex.Status.Detail);
            }
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var req = new ProductIdRequest { Id = id };

                var client = _grpcClient.GetClient();
                var response = await client.GetProductByIdAsync(req);

                return Ok(new { Product = response });
            }
            catch (RpcException ex)
            {
                return StatusCode((int)MapGrpcToHttp(ex.StatusCode), ex.Status.Detail);
            }
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            try
            {
                var bytes = await dto.Image.CopyToByteArrayAsync();

                var req = new NewProductRequest
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    Category = dto.Category,
                    Image = Google.Protobuf.ByteString.CopyFrom(bytes)
                };

                var client = _grpcClient.GetClient();
                var response = await client.CreateProductAsync(req);

                return Ok(new { Product = response.Product });
            }
            catch (RpcException ex)
            {
                return StatusCode((int)MapGrpcToHttp(ex.StatusCode), ex.Status.Detail);
            }
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] UpdateProductDto dto)
        {
            try
            {
                var req = new UpdateProductRequest
                {
                    Id = id,
                    Name = dto.Name ?? "",
                    Description = dto.Description ?? "",
                    Category = dto.Category ?? "",
                    Price = dto.Price ?? 0
                };

                if (dto.Image != null)
                {
                    var bytes = await dto.Image.CopyToByteArrayAsync();
                    req.Image = Google.Protobuf.ByteString.CopyFrom(bytes);
                }

                var client = _grpcClient.GetClient();
                var response = await client.UpdateProductAsync(req);

                return Ok(new { Product = response.Product });
            }
            catch (RpcException ex)
            {
                return StatusCode((int)MapGrpcToHttp(ex.StatusCode), ex.Status.Detail);
            }
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(string id)
        {
            try
            {
                var req = new ProductIdRequest { Id = id };

                var client = _grpcClient.GetClient();
                var response = await client.SoftDeleteProductAsync(req);

                return Ok(new { Product = response.Product });
            }
            catch (RpcException ex)
            {
                return StatusCode((int)MapGrpcToHttp(ex.StatusCode), ex.Status.Detail);
            }
        }

        private static System.Net.HttpStatusCode MapGrpcToHttp(StatusCode grpcStatusCode)
        {
            return grpcStatusCode switch
            {
                Grpc.Core.StatusCode.NotFound => System.Net.HttpStatusCode.NotFound,
                Grpc.Core.StatusCode.AlreadyExists => System.Net.HttpStatusCode.Conflict,
                Grpc.Core.StatusCode.InvalidArgument => System.Net.HttpStatusCode.BadRequest,
                Grpc.Core.StatusCode.Unauthenticated => System.Net.HttpStatusCode.Unauthorized,
                Grpc.Core.StatusCode.PermissionDenied => System.Net.HttpStatusCode.Forbidden,
                _ => System.Net.HttpStatusCode.InternalServerError
            };
        }
    }

    public static class FormFileExtensions
    {
        public static async Task<byte[]> CopyToByteArrayAsync(this IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }
    }

    
}