using Swashbuckle.AspNetCore.SwaggerGen;

namespace AutoQueryable.AspNetCore.Swagger
{
    public static class SwaggerGenOptionsExtensions
    {
        public static void AddAutoQueryable(this SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.OperationFilter<AutoQueryableOperationFilter>();
        }
    }
}