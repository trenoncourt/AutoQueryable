using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using AutoQueryable.Extensions.Autofac;
using AutoQueryable.Sample.AspNetFramework.Models;
using Bogus;

namespace AutoQueryable.Sample.AspNetFramework
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            
            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            // OPTIONAL: Register the Autofac model binder provider.
            builder.RegisterWebApiModelBinderProvider();

            // Register AutoQueryable services
            builder.RegisterAutoQueryable(profile => profile.DefaultToTake = 10);


            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            var db = new AutoQueryableSampleAspNetFrameworkContext();
            db.Database.CreateIfNotExists();
            Seed(db);
        }


        private void Seed(AutoQueryableSampleAspNetFrameworkContext context)
        {
            if (context.Users.Any())
            {
                return;
            }
            var adressFaker = new Faker<Address>()
                .RuleFor(u => u.City, (f, address) => f.Address.City())
                .RuleFor(u => u.HouseNumber, (f, address) => f.Address.BuildingNumber())
                .RuleFor(u => u.PostalCode, (f, address) => f.Address.ZipCode())
                .RuleFor(u => u.Street, (f, address) => f.Address.StreetName())
                ;
            
            var userFaker = new Faker<User>()
                .RuleFor(u => u.Birthdate, (f, user) => f.Date.Past(30))
                .RuleFor(u => u.FirstName, (f, user) => f.Name.FirstName())
                .RuleFor(u => u.LastName, (f, user) => f.Name.LastName())
                .RuleFor(u => u.Username, (f, user) => f.Internet.UserName())
                .RuleFor(u => u.Address, () => adressFaker.Generate())
                ;
            for (var i = 0; i < 10000; i++)
            {
                context.Users.Add(userFaker.Generate());
            }
            context.SaveChanges();
        }
    }
}
