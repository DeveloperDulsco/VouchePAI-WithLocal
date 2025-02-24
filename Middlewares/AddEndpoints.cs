﻿using Microsoft.AspNetCore.Builder;
using System.Reflection;


namespace Middlewares
{
    public static class AddEndpoints
    {
        public static void AddApiEndPoints
        (this WebApplication app, Assembly assembly)
        {
            var endpointDefinitions=assembly.GetTypes().Where(t=>t.IsAssignableTo(typeof(IEndPointDefinition))
            && !t.IsAbstract && !t.IsInterface).Select(Activator.CreateInstance).Cast<IEndPointDefinition>();

            foreach (var endpointDefinition in endpointDefinitions) endpointDefinition.RegisterEndPoints(app);
        }
    }
}
