using System.Collections.Generic;
using Nancy;
using Nancy.Bootstrapper;

namespace AutoQueryable.Nancy.Filter
{
    public interface INancyContextWrapper
    {
        NancyContext Context { get; set; }
    }

    public class NancyContextWrapper : INancyContextWrapper
    {
        public NancyContext Context { get; set; }
    }

    public class NancyContextWrapperRegistrations : IRegistrations
    {
        public IEnumerable<TypeRegistration> TypeRegistrations 
        {
            get 
            { 
                return new[]
                {
                    new TypeRegistration(typeof(INancyContextWrapper), typeof(NancyContextWrapper), Lifetime.PerRequest),
                    //new TypeRegistration(typeof(IUrlString .... per request
                };
                // or you can use AssemblyTypeScanner, etc here to find
            }    

            //make the other 2 interface properties to return null
        }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; }
        public IEnumerable<InstanceRegistration> InstanceRegistrations { get; }
    }
    public class PrepareNancyContextWrapper : IRequestStartup
    {
        private readonly INancyContextWrapper _nancyContext;
        public PrepareNancyContextWrapper(INancyContextWrapper nancyContext)
        {
            _nancyContext = nancyContext;
        }

        public void Initialize(IPipelines piepeLinse, NancyContext context)
        {
            _nancyContext.Context = context;
        }
    }
}
