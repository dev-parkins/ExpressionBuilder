using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ExpressionBuilder
{
    /// <summary>
    /// Contains list of available comparison operations
    /// </summary>
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
        public static Expression<Func<T, bool>> CreateNewStatement<T>(string fields) where T : class
        {
            //------Get all token arrays that have length == 3
            var bindingsList = SplitParameters(fields);

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
                    throw new ArgumentException($"Invalid property: {binding[0]}");

                Expression value = BuildValueExpression(prop.PropertyType, binding[2]);

                Expression comparison = BuildOperandExpression(Expression.Property(pe, prop.Name), binding[1].Trim(), value);
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
            return Expression.Lambda<Func<T, bool>>(filterExpression, pe);
        }

        public static IEnumerable<string[]> SplitParameters(string fields)
        {
            //------check for any AND statements and seperate from there
            var predicates = Regex.Split(fields, " AND", RegexOptions.IgnoreCase).Select(o => o.Trim()).Where(o => !String.IsNullOrWhiteSpace(o));

            //------Get all token arrays that have length == 3
            var inners = predicates.Select(o => o.Split());
            return inners.Where(o => o.Length == 3);
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

        /// <summary>
        /// Build Right-Hand expression for Query statement. Handles nullables.
        /// </summary>
        /// <param name="propType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression BuildValueExpression(Type propType, string value)
        {
            if (value == null || value.Equals("null")) //temp - magic string 
            {
                return Expression.Constant(null);
            }

            var boxedType = Nullable.GetUnderlyingType(propType);
            if (boxedType != null)
            {
                return Expression.Convert(Expression.Constant(Convert.ChangeType(value, boxedType)), propType);
            }

            return Expression.Constant(Convert.ChangeType(value, propType));
        }
    }
}
