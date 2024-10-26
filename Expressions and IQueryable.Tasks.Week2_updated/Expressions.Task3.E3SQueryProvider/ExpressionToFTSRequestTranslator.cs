using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Expressions.Task3.E3SQueryProvider
{
    public class ExpressionToFtsRequestTranslator : ExpressionVisitor
    {
        readonly StringBuilder _resultStringBuilder;

        public ExpressionToFtsRequestTranslator()
        {
            _resultStringBuilder = new StringBuilder();
        }

        public string Translate(Expression exp)
        {
            Visit(exp);

            return _resultStringBuilder.ToString();
        }

        #region protected methods

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable)
                && node.Method.Name == "Where")
            {
                var predicate = node.Arguments[1];
                Visit(predicate);

                return node;
            }

            if (node.Method.DeclaringType == typeof(string) && node.Object != null)
            {
                switch (node.Method.Name)
                {
                    case "StartsWith":
                        Visit(node.Object);
                        _resultStringBuilder.Append("(");
                        Visit(node.Arguments[0]);
                        _resultStringBuilder.Append("*)");
                        return node;

                    case "EndsWith":
                        Visit(node.Object);
                        _resultStringBuilder.Append("(*");
                        Visit(node.Arguments[0]);
                        _resultStringBuilder.Append(")");
                        return node;

                    case "Contains":
                        Visit(node.Object);
                        _resultStringBuilder.Append("(*");
                        Visit(node.Arguments[0]);
                        _resultStringBuilder.Append("*)");
                        return node;

                    case "Equals":
                        Visit(node.Object);
                        _resultStringBuilder.Append("(");
                        Visit(node.Arguments[0]);
                        _resultStringBuilder.Append(")");
                        return node;
                }
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    if (node.Left.NodeType == ExpressionType.MemberAccess && node.Right.NodeType == ExpressionType.Constant)
                    {
                        Visit(node.Left);
                        _resultStringBuilder.Append("(");
                        Visit(node.Right);
                        _resultStringBuilder.Append(")");
                    }
                    else if (node.Left.NodeType == ExpressionType.Constant && node.Right.NodeType == ExpressionType.MemberAccess)
                    {
                        Visit(node.Right);
                        _resultStringBuilder.Append("(");
                        Visit(node.Left);
                        _resultStringBuilder.Append(")");
                    }
                    else
                    {
                        throw new NotSupportedException($"Operands must be field and constant for Equal operation: {node.NodeType}");
                    }
                    break;

                case ExpressionType.AndAlso:
                    _resultStringBuilder.Append("(");
                    Visit(node.Left);
                    _resultStringBuilder.Append(") AND (");
                    Visit(node.Right);
                    _resultStringBuilder.Append(")");
                    break;

                default:
                    throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
            }

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _resultStringBuilder.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _resultStringBuilder.Append(node.Value);

            return node;
        }

        #endregion
    }
}
