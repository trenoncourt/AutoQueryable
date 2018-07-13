using System;
using AutoQueryable.AspNetCore.Filter.FilterAttributes;

namespace AutoQueryable.AspNetCore.Swagger
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoQueryableSwaggerAttribute : Attribute, IAutoQueryableAttribute
    {
    }
}