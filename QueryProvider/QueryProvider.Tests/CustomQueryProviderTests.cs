using QueryProvider.Entities;
using QueryProvider;

public class CustomQueryProviderTests
{
    private readonly string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=TestDatabase;MultipleActiveResultSets=True;Trusted_Connection=True;TrustServerCertificate=True;";
    
    [Fact]
    public void TestFromDBAnd()
    {
        var provider = new CustomQueryProvider(_connectionString);
        var productSet = new CustomQueryable<Products>(provider);

        var products = productSet.Where(p => p.UnitPrice > 100 && p.ProductType == "Customised Product").ToList();

        Assert.NotEmpty(products);
        foreach (var product in products)
        {
            Assert.True(product.UnitPrice > 100);
            Assert.Equal("Customised Product", product.ProductType);
        }
    }

    [Fact]
    public void TestFromDBLike()
    {
        var provider = new CustomQueryProvider(_connectionString);
        var productSet = new CustomQueryable<Products>(provider);

        var products = productSet.Where(p => p.ProductName.Contains("Pro"));

        Assert.NotEmpty(products);
        foreach (var product in products)
        {
            Assert.Contains("Pro", product.ProductType);
        }
    }

    [Fact]
    public void TestSimpleQuery()
    {
        var provider = new CustomQueryProvider("your_connection_string");
        var productSet = new CustomQueryable<Products>(provider);
        var query = productSet.Where(p => p.UnitPrice > 100 && p.ProductType == "Customised Product");

        var expectedSql = "SELECT * FROM [Products] WHERE (([UnitPrice] > 100) AND ([ProductType] = 'Customised Product'))";
        Assert.Equal(expectedSql, query.ToString());
    }

    [Fact]
    public void TestGreaterThanQuery()
    {
        var provider = new CustomQueryProvider("your_connection_string");
        var productSet = new CustomQueryable<Products>(provider);
        var query = productSet.Where(p => p.UnitPrice > 50);

        var expectedSql = "SELECT * FROM [Products] WHERE ([UnitPrice] > 50)";
        Assert.Equal(expectedSql, query.ToString());
    }

    [Fact]
    public void TestLessThanQuery()
    {
        var provider = new CustomQueryProvider("your_connection_string");
        var productSet = new CustomQueryable<Products>(provider);
        var query = productSet.Where(p => p.UnitPrice < 50);

        var expectedSql = "SELECT * FROM [Products] WHERE ([UnitPrice] < 50)";
        Assert.Equal(expectedSql, query.ToString());
    }

    [Fact]
    public void TestEqualQuery()
    {
        var provider = new CustomQueryProvider("your_connection_string");
        var productSet = new CustomQueryable<Products>(provider);
        var query = productSet.Where(p => p.ProductType == "Standard Product");

        var expectedSql = "SELECT * FROM [Products] WHERE ([ProductType] = 'Standard Product')";
        Assert.Equal(expectedSql, query.ToString());
    }

    [Fact]
    public void TestStartsWithQuery()
    {
        var provider = new CustomQueryProvider("your_connection_string");
        var productSet = new CustomQueryable<Products>(provider);
        var query = productSet.Where(p => p.ProductName.StartsWith("Pro"));

        var expectedSql = "SELECT * FROM [Products] WHERE ([ProductName] LIKE 'Pro%')";
        var res = query.ToString();
        Assert.Equal(expectedSql, res);
    }

    [Fact]
    public void TestEndsWithQuery()
    {
        var provider = new CustomQueryProvider("your_connection_string");
        var productSet = new CustomQueryable<Products>(provider);
        var query = productSet.Where(p => p.ProductName.EndsWith("101"));

        var expectedSql = "SELECT * FROM [Products] WHERE ([ProductName] LIKE '%101')";
        Assert.Equal(expectedSql, query.ToString());
    }

    [Fact]
    public void TestContainsQuery()
    {
        var provider = new CustomQueryProvider("your_connection_string");
        var productSet = new CustomQueryable<Products>(provider);
        var query = productSet.Where(p => p.ProductName.Contains("Pro"));

        var expectedSql = "SELECT * FROM [Products] WHERE ([ProductName] LIKE '%Pro%')";
        Assert.Equal(expectedSql, query.ToString());
    }

    [Fact]
    public void TestComplexAndQuery()
    {
        var provider = new CustomQueryProvider("your_connection_string");
        var productSet = new CustomQueryable<Products>(provider);
        var query = productSet.Where(p => p.UnitPrice > 100 && p.ProductName.Contains("Pro"));

        var expectedSql = "SELECT * FROM [Products] WHERE (([UnitPrice] > 100) AND ([ProductName] LIKE '%Pro%'))";
        Assert.Equal(expectedSql, query.ToString());
    }

    [Fact]
    public void TestComplexOrQuery()
    {
        var provider = new CustomQueryProvider("your_connection_string");
        var productSet = new CustomQueryable<Products>(provider);
        var query = productSet.Where(p => p.UnitPrice > 100 || p.ProductName.Contains("Pro"));

        var expectedSql = "SELECT * FROM [Products] WHERE (([UnitPrice] > 100) OR ([ProductName] LIKE '%Pro%'))";
        Assert.Equal(expectedSql, query.ToString());
    }
}