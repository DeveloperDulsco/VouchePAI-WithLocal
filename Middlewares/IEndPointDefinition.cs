using Microsoft.AspNetCore.Builder;
namespace Middlewares;

public interface IEndPointDefinition
{
    public void RegisterEndPoints(WebApplication application);
}
