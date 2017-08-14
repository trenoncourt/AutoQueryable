using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoQueryable.Core.Extensions
{
    public static class ExpressionExtension
    {
        /// <summary>
        /// Create a select lambda expression called from another expression.
        /// <ExistingExpression>.Select(x => x.Body)
        /// </summary>
        /// <param name="from">Existing expression to call select method on</param>
        /// <param name="body">The body inside the select expression: .Select(x => x.Body)</param>
        /// <param name="param">Optional, the parameter of the Select expression (often x =>)</param>
        /// <returns>the Call of the created Select expression</returns>
        public static MethodCallExpression CreateSelect(this Expression from, Expression body, ParameterExpression param = null)
        {
            // input eg: Product.SalesOrderDetail (type IList<SalesOrderDetail>), output: type SalesOrderDetail
            Type enumerableType = from.Type.GetGenericArguments().SingleOrDefault();

            if (param == null)
            {
                // declare parameter for the lambda expression of SalesOrderDetail.Select(x => x.LineTotal)
                param = Expression.Parameter(enumerableType, "x"); 
            }

            // Lambda is of type Func<Order, int> in the case of x => x.LineTotal
            var funcType = typeof(Func<,>).MakeGenericType(enumerableType, body.Type);

            var lambda = Expression.Lambda(funcType, body, param);

            var selectMethod = (from m in typeof(Enumerable).GetMethods()
                where m.Name == "Select"
                      && m.IsGenericMethod
                let parameters = m.GetParameters()
                where parameters.Length == 2
                      && parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)
                select m).Single().MakeGenericMethod(enumerableType, body.Type);

            // Do SalesOrderDetail.Select(x => x.LineTotal)
            return Expression.Call(null, selectMethod, from, lambda);
        }

        /// <summary>
        /// Create a parameter expression from another expression wich have a generic type.
        /// </summary>
        /// <param name="from">Existing expression to create the parameter on<</param>
        /// <param name="parameterName">Optional, the name of the parameter</param>
        /// <returns>The created parameter expression</returns>
        public static ParameterExpression CreateParameterFromGenericType(this Expression from, string parameterName = "x")
        {
            // input eg: Product.SalesOrderDetail (type IList<SalesOrderDetail>), output: type SalesOrderDetail
            var enumerableType = from.Type.GetGenericArguments().SingleOrDefault();
            // declare parameter for the lambda expression of SalesOrderDetail.Select(x => x.LineTotal)
            return Expression.Parameter(enumerableType, parameterName);
        }
    }
}