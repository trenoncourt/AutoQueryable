namespace AutoQueryable.UnitTest
{
    public class AutoQueryableProfileTest
    {
        //[Fact]
        //public void AllowOnlyOneClause()
        //{
        //    using (var context = new Mock.AutoQueryableContext())
        //    {
        //        DataInitializer.InitializeSeed(context);
        //        var query = (context.Product.AutoQueryable("select=name&top=10", new AutoQueryableProfile
        //        {
        //            AllowedClauses = ClauseType.Select
        //        }) as IEnumerable<dynamic>).ToList();

        //        query.Count.Should().Be(DataInitializer.ProductSampleCount);
        //        var first = query.First();

        //        var propertiesCount = ((Type)first.GetType()).GetProperties().Length;
        //        propertiesCount.Should().Be(1);

        //        string name = first.GetType().GetProperty("name").GetValue(first);
        //        name.Should().NotBeNull();
        //    }
        //}

        //[Fact]
        //public void AllowMultipleClauses()
        //{
        //    using (var context = new Mock.AutoQueryableContext())
        //    {
        //        //var autoQuery = new AutoQuery();
        //        DataInitializer.InitializeSeed(context);
        //        var query = (context.Product.AutoQueryable("select=productId&top=10&skip=100", new AutoQueryableProfile
        //        {
        //            AllowedClauses = ClauseType.Select | ClauseType.Top
        //        }) as IEnumerable<dynamic>).ToList();

        //        query.Count.Should().Be(10);
        //        var first = query.First();

        //        var propertiesCount = ((Type)first.GetType()).GetProperties().Length;
        //        propertiesCount.Should().Be(1);

        //        int productid = first.GetType().GetProperty("productId").GetValue(first);
        //        productid.Should().Be(101);
        //    }
        //}
    }
}
