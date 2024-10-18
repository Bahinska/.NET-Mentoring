using System.Collections;
using System.Linq.Expressions;

namespace QueryProvider
{
    public class CustomQueryable<T> : IQueryable<T>
    {
        private readonly CustomQueryProvider _provider;
        private readonly Expression _expression;

        public CustomQueryable(CustomQueryProvider provider)
        {
            _provider = provider;
            _expression = Expression.Constant(this);
        }

        public CustomQueryable(CustomQueryProvider provider, Expression expression)
        {
            _provider = provider;
            _expression = expression;
        }

        public Type ElementType => typeof(T);
        public Expression Expression => _expression;
        public IQueryProvider Provider => _provider;

        public IEnumerator<T> GetEnumerator()
        {
            var result = _provider.Execute<IEnumerable<T>>(_expression);
            return result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public override string ToString()
        {
            var elementType = _expression.Type.GetGenericArguments().FirstOrDefault() ?? _expression.Type;
            return new ExpressionToSQLTranslator().Translate(elementType, _expression);
        }
    }
}
