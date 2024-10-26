using QueryProvider;
using System.Data.SqlClient;
using System.Linq.Expressions;

public class CustomQueryProvider : IQueryProvider
{
    private readonly string _connectionString;

    public CustomQueryProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        var elementType = expression.Type.GetGenericArguments().FirstOrDefault() ?? expression.Type;
        return (IQueryable)Activator.CreateInstance(typeof(CustomQueryable<>).MakeGenericType(elementType), new object[] { this, expression });
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new CustomQueryable<TElement>(this, expression);
    }

    public object Execute(Expression expression)
    {
        var elementType = expression.Type.GetGenericArguments().FirstOrDefault() ?? expression.Type;
        var translator = new ExpressionToSQLTranslator();
        var sqlQuery = translator.Translate(elementType, expression);
        Console.WriteLine($"Generated SQL: {sqlQuery}");
        return ExecuteQuery(sqlQuery, elementType);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        var elementType = typeof(TResult).GetGenericArguments().FirstOrDefault() ?? typeof(TResult);
        var sqlQuery = new ExpressionToSQLTranslator().Translate(elementType, expression);
        Console.WriteLine($"Generated SQL: {sqlQuery}");
        return (TResult)ExecuteQuery(sqlQuery, elementType);
    }

    private object ExecuteQuery(string sqlQuery, Type elementType)
    {
        var results = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
        var addMethod = results.GetType().GetMethod("Add");

        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand(sqlQuery, connection))
        {
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                var properties = elementType.GetProperties();
                while (reader.Read())
                {
                    var element = Activator.CreateInstance(elementType);
                    foreach (var property in properties)
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                        {
                            property.SetValue(element, reader.GetValue(reader.GetOrdinal(property.Name)));
                        }
                    }
                    addMethod.Invoke(results, new[] { element });
                }
            }
        }

        return results;
    }
}