namespace AutoQueryable.Sample.EfCore.Dtos
{
    public class ProductDto
    {
        public string Name { get; set; }

        public ProductCategoryDto Category { get; set; }
    }
}