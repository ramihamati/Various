using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace ThirdGrpc.Services;

public class HubService : GRPCHubService.GRPCHubServiceBase
{
    private static ActivitySource source = new ActivitySource("ThirdGrpc", "1.0.0");

    private readonly ILogger<HubService> _logger;
    public HubService(ILogger<HubService> logger)
    {
        _logger = logger;
    }

    public override Task<rpcResponseCreateHub> CreateHub(
        rpcRequestCreateHub request,
        ServerCallContext context)
    {
        using Activity activity = source.StartActivity("CreateHub")!;
        _logger.LogInformation("The rpc service created a new hub");
        return HubCreateProcess(request);
    }

    private static Task<rpcResponseCreateHub> HubCreateProcess(rpcRequestCreateHub request)
    {
        using Activity activity = source.StartActivity("HubCreateProcess")!; // it's like starting again the parent activity with a different name
        // in zipkin we have 2 paralelel activities instead of nested ones

        return Task<rpcResponseCreateHub>.FromResult(new rpcResponseCreateHub
        {
            Hub = new rpcHubValue
            {
                HubValueValue = new rpcHub
                {
                    Name = request.HubName,
                    Id = Guid.NewGuid().ToString(),
                    CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                    Description = request.HubDescription
                }
            }
        });
    }
}