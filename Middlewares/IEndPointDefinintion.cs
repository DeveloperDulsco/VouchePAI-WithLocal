using Microsoft.AspNetCore.Builder;
namespace Middlewares;

public interface IEndPointDefinintion
{
    public void RegisterEndPoints(WebApplication application);
}
