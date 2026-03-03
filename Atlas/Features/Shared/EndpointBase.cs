using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Atlas.Features.Shared;

[ApiController]
public abstract class EndpointBase : ControllerBase;

public sealed class EndpointBaseControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        var isEndpoint = !typeInfo.IsAbstract && typeof(EndpointBase).IsAssignableFrom(typeInfo);
        return isEndpoint || base.IsController(typeInfo);
    }
}

public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddEndpoints(this IMvcBuilder builder)
    {
        builder.ConfigureApplicationPartManager(manager =>
        {
            manager.FeatureProviders.Add(new EndpointBaseControllerFeatureProvider());
        });

        return builder;
    }
}