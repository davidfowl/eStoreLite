using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Frontend;

public static class YarpExtensions
{
    // Helper to wrap some ceremony for transforming the outgoing proxied request
    public static IEndpointConventionBuilder MapForwarder(this IEndpointRouteBuilder routes, string pattern, string destinationPrefix, string targetPath)
    {
        var transformBuilder = routes.ServiceProvider.GetRequiredService<ITransformBuilder>();

        var transform = transformBuilder.Create(c => c.AddPathRouteValues(targetPath));

        return routes.MapForwarder(pattern, destinationPrefix, new ForwarderRequestConfig(), transform);
    }
}
