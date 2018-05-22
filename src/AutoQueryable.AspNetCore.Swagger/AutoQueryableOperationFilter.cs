using System.Linq;
using AutoQueryable.AspNetCore.Filter.FilterAttributes;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AutoQueryable.AspNetCore.Swagger
{
    public class AutoQueryableOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            // Policy names map to scopes
            var controllerScopes = context.ApiDescription.ActionAttributes()
                .OfType<AutoQueryableAttribute>();

            if (controllerScopes.Any())
            {
                Schema schema = context.SchemaRegistry.GetOrRegister(typeof(string));
                operation.Parameters.Add(new NonBodyParameter
                {
                    In = "query",
                    Name = "select",
                    Type = schema.Type,
                    Format = schema.Format
                });
            }
        }
    }
}