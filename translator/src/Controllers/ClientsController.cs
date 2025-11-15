using Microsoft.AspNetCore.Mvc;
using censudex_api_gateway.src.Protos.Clients;
using Grpc.Core;
using translator.src.Dtos.Clients;
using translator.src.Extensions;

[ApiController]
[Route("clients")]
public class ClientsController : ControllerBase
{
    private readonly IClientsGrpcClient _grpcClient;

    public ClientsController(IClientsGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    // ------------------ CREATE ------------------
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequestDto dto)
    {
        var req = new CreateUserRequestProto
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Username = dto.Username,
            BirthDate = dto.BirthDate.ToString("yyyy-MM-dd"),
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber,
            Password = dto.Password
        };

        try
        {
            var client = _grpcClient.GetClient();
            var res = await client.CreateAsync(req);

            return Ok(new
            {
                Client = res.Client
            });
        }
        catch (RpcException ex)
        {
            return new ObjectResult(ex.Status.Detail)
            {
                StatusCode = (int)ClientsController.MapGrpcToHttp(ex.StatusCode)

            };

        }
    }


    // ------------------ GET ALL ------------------
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? fullName = null,
        [FromQuery] string? email = null,
        [FromQuery] string? username = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool isDescending = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
        )
    {
        var req = new ClientQueryProto
        {
            FullName = fullName ?? "",
            Email = email ?? "",
            Username = username ?? "",
            IsActive = isActive ?? false,
            SortBy = sortBy ?? "",
            IsDescending = isDescending,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var client = _grpcClient.GetClient();
        var res = await client.GetAllAsync(req);

        return Ok(new
        {
            Items = res.Items,
            TotalCount = res.TotalCount,
            TotalPages = res.TotalPages
        });
    }


    // ------------------ GET BY ID ------------------
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var req = new GetClientByIdRequestProto { Id = id.ToString() };

        try
        {
            var client = _grpcClient.GetClient();
            var res = await client.GetByIdAsync(req);
            return Ok(res.Client);
        }
        catch (RpcException ex)
        {
            return new ObjectResult(ex.Status.Detail)
            {
                StatusCode = (int)ClientsController.MapGrpcToHttp(ex.StatusCode)

            };

        }
    }


    // ------------------ UPDATE ------------------
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateUserRequestDto dto)
    {
        var req = new UpdateUserRequestProto
        {
            Id = id.ToString(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Username = dto.Username,
            BirthDate = dto.BirthDate.ToString("yyyy-MM-dd"),
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber
        };

        try
        {
            var client = _grpcClient.GetClient();
            var res = await client.UpdateAsync(req);
            return Ok(new { Success = res.Success });
        }
        catch (RpcException ex)
        {
            return new ObjectResult(ex.Status.Detail)
            {
                StatusCode = (int)ClientsController.MapGrpcToHttp(ex.StatusCode)

            };

        }
    }


    // ------------------ SOFT DELETE ------------------
    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> SoftDelete(Guid id, [FromHeader(Name = "Authorization")] string? auth)
    {
        if (string.IsNullOrEmpty(auth))
            return Unauthorized("Missing token.");

        var req = new SoftDeleteRequestProto
        {
            Id = id.ToString(),
            Token = auth.Replace("Bearer ", "").Trim()
        };

        try
        {
            var client = _grpcClient.GetClient();
            var res = await client.SoftDeleteAsync(req);
            return Ok(new { Success = res.Success });
        }
        catch (RpcException ex)
        {
            return new ObjectResult(ex.Status.Detail)
            {
                StatusCode = (int)ClientsController.MapGrpcToHttp(ex.StatusCode)

            };

        }
    }


    // ------------------ VERIFY CREDENTIALS ------------------
    [HttpPost("credentials")]
    public async Task<IActionResult> VerifyCredentials([FromBody] VerifyCredentialsDto dto)
    {
        var req = new VerifyCredentialsRequestProto
        {
            Email = dto.Email ?? "",
            Username = dto.Username ?? "",
            Password = dto.Password
        };

        try
        {
            var client = _grpcClient.GetClient();
            var res = await client.VerifyCredentialsAsync(req);

            return Ok(new
            {
                res.Id,
                res.Role,
                res.Username,
                res.Email,
                res.IsActive
            });
        }
        catch (RpcException ex)
        {
            return new ObjectResult(ex.Status.Detail)
            {
                StatusCode = (int)ClientsController.MapGrpcToHttp(ex.StatusCode)

            };

        }
    }

    // ------------------ HELPER ------------------
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
