using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionBuilder
{
    enum ops
    {
        lt,
        gt,
        eq,
        ne,
        ge,
        le  
    }

    public static class ExpressionHelper
    {
        public static Func<T, bool> CreateNewStatement<T>(string[] fields) where T : class
        {
            //------Get all token arrays that have length == 3
            var bindingsList = fields.ToList().Where(o => o.Split().Length == 3).Select(o => o.Split());

            //------Boilerplate items

            var type = typeof(T);
            var pe = Expression.Parameter(type, "p");
            Expression selectLeft = null;
            Expression selectRight = null;
            Expression filterExpression = null;

            //------Loop through the bindingslist and build Expression list (e.g. value1 = a AND value2 = b ...)

            foreach (var binding in bindingsList)
            {
                var prop = type.GetProperty(binding[0]);
                if (prop == null)
                    throw new ArgumentException("Property not found");

                Expression value = BuildValueExpression(prop.PropertyType, binding[2]);

                Expression comparison = BuildOperandExpression(Expression.Property(pe, prop.Name), binding[1], value);
                if (selectLeft == null)
                {
                    selectLeft = comparison;
                    filterExpression = selectLeft;
                    continue;
                }
                if (selectRight == null)
                {
                    selectRight = comparison;
                    filterExpression = Expression.AndAlso(selectLeft, selectRight);
                    continue;
                }
                filterExpression = Expression.AndAlso(filterExpression, comparison);
            }

            return Expression.Lambda<Func<T, bool>>(filterExpression, pe).Compile();
        }

        public static Expression BuildOperandExpression(Expression left, string op, Expression right)
        {
            switch (op.ToLower())
            {
                case nameof(ops.lt):
                    return Expression.LessThan(left, right);
                case nameof(ops.gt):
                    return Expression.GreaterThan(left, right);
                case nameof(ops.eq):
                    return Expression.Equal(left, right);
                case nameof(ops.ne):
                    return Expression.NotEqual(left, right);
                case nameof(ops.ge):
                    return Expression.GreaterThanOrEqual(left, right);
                case nameof(ops.le):
                    return Expression.LessThanOrEqual(left, right);
                default:
                    throw new InvalidOperationException("Query operand not supported");
            };
        }

        public static Expression BuildValueExpression(Type propType, string value)
        {
            if(value == null || value.Equals("null")) //temp - magic string 
            {
                return Expression.Constant(null);
            }

            if (propType.IsEquivalentTo(typeof(int)))
            {
                return Expression.Constant(Int32.Parse(value));
            }
            else if (propType.IsEquivalentTo(typeof(bool)))
            {
                return Expression.Constant(Boolean.Parse(value));
            }
            else if (propType.IsEquivalentTo(typeof(DateTime)))
            {
                return Expression.Constant(DateTime.Parse(value));
            }
            else if (propType.IsEquivalentTo(typeof(DateTime?)))
            {
                return Expression.Convert(Expression.Constant(Convert.ToDateTime(value)), typeof(DateTime?));
            }
            
            return Expression.Constant(value);
        }
    }
}
