namespace AutoQueryable.Sample.AspNetFramework.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AutoQueryable.Sample.AspNetFramework.Models.AutoQueryableSampleAspNetFrameworkContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "AutoQueryable.Sample.AspNetFramework.Models.AutoQueryableSampleAspNetFrameworkContext";
        }

        protected override void Seed(AutoQueryable.Sample.AspNetFramework.Models.AutoQueryableSampleAspNetFrameworkContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
