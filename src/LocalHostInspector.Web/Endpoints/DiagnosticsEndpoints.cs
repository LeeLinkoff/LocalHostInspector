using LocalHostInspector.Core.Engine;


namespace LocalHostInspector.Web.Endpoints;


public static class DiagnosticsEndpoints
{
    public static IEndpointRouteBuilder MapDiagnosticsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/info",
                         async (HttpContext context,
                                DiagnosisEngine engine) =>
                                {
                                    try
                                    {
                                        var result = await engine.InspectAsync(context.Request.Host.Host);

                                        result.Url = $"{context.Request.Scheme}://{context.Request.Host}";

                                        return Results.Json(result);
                                    }
                                    catch (Exception ex)
                                    {
                                        return Results.Problem(detail: ex.ToString(),title: ex.Message,statusCode: 500);
                                    }
                                });

        return endpoints;
    }
}