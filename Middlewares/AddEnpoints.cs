using Microsoft.AspNetCore.Builder;
using System.Reflection;


namespace Middlewares
{
    public static class AddEnpoints
    {
        public static void AddApiEndPoints
        (this WebApplication app, Assembly assembly)
        {
            var endpointDefinitions=assembly.GetTypes().Where(t=>t.IsAssignableTo(typeof(IEndPointDefinintion))
            && !t.IsAbstract && !t.IsInterface).Select(Activator.CreateInstance).Cast<IEndPointDefinintion>();

            foreach (var endpointDefinition in endpointDefinitions) endpointDefinition.RegisterEndPoints(app);
        }
    }
}
