using System.Linq.Expressions;
using System.Text;

public class ExpressionToSQLTranslator : ExpressionVisitor
{
    private StringBuilder _sql;

    public string Translate(Type entityType, Expression expression)
    {
        _sql = new StringBuilder();
        _sql.Append($"SELECT * FROM [{entityType.Name}] WHERE ");
        Visit(expression);
        return _sql.ToString();
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        _sql.Append("(");
        Visit(node.Left);
        _sql.Append($" {GetSqlOperator(node.NodeType)} ");
        Visit(node.Right);
        _sql.Append(")");
        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Value is IQueryable)
        {
            // Skip visiting the root queryable object itself
            return node;
        }

        switch (Type.GetTypeCode(node.Type))
        {
            case TypeCode.String:
                _sql.Append($"'{node.Value as string}'");
                break;

            case TypeCode.Boolean:
                _sql.Append((bool)node.Value ? 1 : 0);
                break;

            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                _sql.Append(Convert.ToDouble(node.Value).ToString(System.Globalization.CultureInfo.InvariantCulture));
                break;

            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.DateTime:
                _sql.Append(node.Value);
                break;

            case TypeCode.Object when node.Type.IsEnum:
                _sql.Append(Convert.ToInt32(node.Value));
                break;

            default:
                throw new NotSupportedException($"Unsupported constant type: {node.Type}");
        }

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        _sql.Append($"[{node.Member.Name}]");
        return node;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType == typeof(string) && node.Object != null)
        {
            _sql.Append("(");
            switch (node.Method.Name)
            {
                case "StartsWith":
                    Visit(node.Object);
                    _sql.Append(" LIKE '");
                    Visit(node.Arguments[0]);
                    RemoveQuotes(_sql);
                    _sql.Append("%')");
                    return node;

                case "EndsWith":
                    Visit(node.Object);
                    _sql.Append(" LIKE '%");
                    Visit(node.Arguments[0]);
                    RemoveQuotes(_sql);
                    _sql.Append("')");
                    return node;

                case "Contains":
                    Visit(node.Object);
                    _sql.Append(" LIKE '%");
                    Visit(node.Arguments[0]);
                    RemoveQuotes(_sql);
                    _sql.Append("%')");
                    return node;
            }
        }

        if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "Where")
        {
            Visit(node.Arguments[1]);
            return node;
        }

        return base.VisitMethodCall(node);
    }

    public static void RemoveQuotes(StringBuilder sb)
    {
        sb.Remove(sb.ToString().LastIndexOf("'"), 1);
        sb.Remove(sb.ToString().LastIndexOf("'"), 1);
    }

    private string GetSqlOperator(ExpressionType type)
    {
        switch (type)
        {
            case ExpressionType.Equal:
                return "=";
            case ExpressionType.AndAlso:
                return "AND";
            case ExpressionType.OrElse:
                return "OR";
            case ExpressionType.GreaterThan:
                return ">";
            case ExpressionType.LessThan:
                return "<";
            default:
                throw new NotSupportedException($"Unsupported operator: {type}");
        }
    }
}