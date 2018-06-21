using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.AspNetCore.Filter.FilterAttributes;
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
                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<IParameter>();
                }
                operation.Parameters.Add(CreateParameter(context, "select"));
                operation.Parameters.Add(CreateParameter(context, "take", typeof(int)));
                operation.Parameters.Add(CreateParameter(context, "skip", typeof(int)));
                operation.Parameters.Add(CreateParameter(context, "first"));
                operation.Parameters.Add(CreateParameter(context, "last"));
                operation.Parameters.Add(CreateParameter(context, "orderby"));
                operation.Parameters.Add(CreateParameter(context, "orderbydesc"));
                operation.Parameters.Add(CreateParameter(context, "wrapwith"));
                operation.Parameters.Add(CreateParameter(context, "pagesize"));
            }
        }

        private NonBodyParameter CreateParameter(OperationFilterContext context, string name, Type type = null)
        {
            type = type ?? typeof(string);
            Schema schema = context.SchemaRegistry.GetOrRegister(type);
            return new NonBodyParameter
            {
                In = "query",
                Name = name,
                Type = schema?.Type,
                Format = schema?.Format
            };
        }
    }
}