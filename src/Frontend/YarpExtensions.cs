using Yarp.ReverseProxy.Forwarder;

namespace Frontend;

public static class YarpExtensions
{
    // Helper to wrap some ceremony for transforming the outgoing proxied request
    public static IEndpointConventionBuilder MapForwarder(this IEndpointRouteBuilder routes, string pattern, string destinationPrefix, Func<HttpContext, HttpRequestMessage, string, CancellationToken, ValueTask> transform)
    {
        return routes.MapForwarder(pattern, destinationPrefix, new ForwarderRequestConfig(), new DelegateTransformer(transform));
    }

    private class DelegateTransformer(Func<HttpContext, HttpRequestMessage, string, CancellationToken, ValueTask> transform) : HttpTransformer
    {
        public override async ValueTask TransformRequestAsync(HttpContext httpContext, HttpRequestMessage proxyRequest, string destinationPrefix, CancellationToken cancellationToken)
        {
            await transform(httpContext, proxyRequest, destinationPrefix, cancellationToken);

            await base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix, cancellationToken);
        }
    }
}
