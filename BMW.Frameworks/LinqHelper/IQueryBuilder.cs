using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

namespace BMW.Frameworks.LinqHelper
{
    public static class QueryBuilder
    {
        public static IQueryBuilder<T> Create<T>()
        {
            return new QueryBuilder<T>();
        }
    }

    class QueryBuilder<T> : IQueryBuilder<T>
    {
        Expression<Func<T, bool>> IQueryBuilder<T>.Expression
        {
            get
            {
                return RetrieveExpression();
            }
        }

        public ParameterExpression[] Parameters { get; set; }

        protected List<Expression> Expressions = new List<Expression>();

        public QueryBuilder()
        {

        }

        public void AppendExpression(Expression expression)
        {
            this.Expressions.Add(expression);
        }

        protected virtual Expression<Func<T, bool>> RetrieveExpression()
        {
            if (this.Expressions.Count == 0)
            {
                return (Expression<Func<T, bool>>)(f => true);
            }

            Expression expression = this.Expressions.Aggregate((e1, e2) => Expression.AndAlso(e1, e2));

            return Expression.Lambda<Func<T, bool>>(expression, this.Parameters);
        }
    }

    /// <summary>
    /// 动态查询条件创建者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueryBuilder<T>
    {
        Expression<Func<T, bool>> Expression { get; }

        ParameterExpression[] Parameters { get; set; }

        void AppendExpression(Expression expression);
    }

    public static class IQueryBuilderExtensions
    {
        #region Between

        private static readonly DateTime minDate = new DateTime(1800, 1, 1);
        private static readonly DateTime maxDate = new DateTime(2999, 1, 1);

        /// <summary>
        /// 建立 Between 查询条件
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="q">动态查询条件创建者</param>
        /// <param name="property">属性</param>
        /// <param name="from">开始值</param>
        /// <param name="to">结束值</param>
        /// <returns></returns>
        public static IQueryBuilder<T> Between<T, P>(this IQueryBuilder<T> q, Expression<Func<T, P>> property, P from, P to)
        {
            Type type = typeof(P);
            var constantFrom = Expression.Constant(from);
            var constantTo = Expression.Constant(to);

            var propertyBody = GetMemberExpression(q, property);

            Expression nonNullProperty = propertyBody;

            //如果是Nullable<X>类型，则转化成X类型
            if (type.IsNullableType())
            {
                type = type.GetNonNullableType();
                nonNullProperty = Expression.Convert(propertyBody, type);
            }
            var c1 = Expression.GreaterThanOrEqual(nonNullProperty, constantFrom);
            var c2 = Expression.LessThanOrEqual(nonNullProperty, constantTo);
            var c = Expression.AndAlso(c1, c2);

            q.AppendExpression(c);

            return q;
        }

        /// <summary>
        /// 建立 Between 查询条件
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="q">动态查询条件创建者</param>
        /// <param name="property">属性</param>
        /// <param name="from">开始值</param>
        /// <param name="to">结束值</param>
        /// <returns></returns>
        public static IQueryBuilder<T> Between<T>(this IQueryBuilder<T> q, Expression<Func<T, string>> property, string from, string to)
        {
            from = from.Trim();
            to = to.Trim();

            if (from != string.Empty && to != string.Empty)
            {
                var propertyBody = GetMemberExpression(q, property);
                var constantFrom = Expression.Constant(from);
                var constantTo = Expression.Constant(to);
                var constantZero = Expression.Constant(0);
                var compareMethod = typeof(string).GetMethod("Compare", new Type[] { typeof(string), typeof(string) });

                MethodCallExpression methodExp1 = Expression.Call(null, compareMethod, propertyBody, constantFrom);
                var c1 = Expression.GreaterThanOrEqual(methodExp1, constantZero);
                MethodCallExpression methodExp2 = Expression.Call(null, compareMethod, propertyBody, constantTo);
                var c2 = Expression.LessThanOrEqual(methodExp2, constantZero);
                var c = Expression.AndAlso(c1, c2);

                q.AppendExpression(c);
            }

            return q;
        }

        #endregion

        #region Like

        /// <summary>
        /// 建立 Like ( 模糊 ) 查询条件
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="q">动态查询条件创建者</param>
        /// <param name="property">属性</param>
        /// <param name="value">查询值</param>
        /// <returns></returns>
        public static IQueryBuilder<T> Like<T>(this IQueryBuilder<T> q, Expression<Func<T, string>> property, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim();

                var propertyBody = GetMemberExpression(q, property);

                MethodCallExpression methodExpr = Expression.Call(propertyBody,
                    typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                    Expression.Constant(value)
                );

                q.AppendExpression(methodExpr);
            }

            return q;
        }

        #endregion

        private static Expression GetMemberExpression<T, P>(IQueryBuilder<T> q, Expression<Func<T, P>> property)
        {
            if (q.Parameters == null || q.Parameters.Length == 0)
            {
                q.Parameters = property.GetParameters();
                return property.Body;
            }

            ParameterExpressionVisitor visitor = new ParameterExpressionVisitor(q.Parameters[0]);

            Expression memberExpr = visitor.ChangeParameter(property.Body);

            return memberExpr;
        }

        #region Equals

        /// <summary>
        /// 建立 Equals ( 相等 ) 查询条件
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="q">动态查询条件创建者</param>
        /// <param name="property">属性</param>
        /// <param name="value">查询值</param>
        /// <param name="exclude">排除值（意味着如果value==exclude，则当前条件不被包含到查询中）</param>
        /// <returns></returns>
        public static IQueryBuilder<T> Equals<T, P>(this IQueryBuilder<T> q, Expression<Func<T, P>> property, P value, P exclude)
        {
            if (value.Equals(exclude))
            {
                return q;
            }

            return Equals(q, property, value);
        }

        /// <summary>
        /// 建立 Equals ( 相等 ) 查询条件
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="q">动态查询条件创建者</param>
        /// <param name="property">属性</param>
        /// <param name="value">查询值</param>
        /// <returns></returns>
        public static IQueryBuilder<T> Equals<T, P>(this IQueryBuilder<T> q, Expression<Func<T, P>> property, P value)
        {
            Expression right = Expression.Constant(value);
            Type type = typeof(P);

            var propertyBody = GetMemberExpression(q, property);

            Expression left = propertyBody;

            //如果是Nullable类型，则把value转化成Nullable类型
            if (type.IsNullableType())
            {
                right = Expression.Convert(right, type);
            }

            var methodExpr = Expression.Equal(left, right);

            q.AppendExpression(methodExpr);

            return q;
        }

        #endregion

        #region In

        //TODO:无法直接获取重载的泛型方法？
        //var method = typeof(Enumerable).GetMethod("Contains");
        private static MethodInfo method_Contains =
                        (from m in typeof(Enumerable).GetMethods()
                         where m.Name.Equals("Contains")
                             && m.IsGenericMethod
                             && m.GetGenericArguments().Length == 1
                             && m.GetParameters().Length == 2
                         select m
                        ).First();

        /// <summary>
        /// 建立 In 查询条件
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="q">动态查询条件创建者</param>
        /// <param name="property">属性</param>
        /// <param name="valuse">查询值</param> 
        /// <returns></returns>
        public static IQueryBuilder<T> In<T, P>(this IQueryBuilder<T> q, Expression<Func<T, P>> property, params P[] values)
        {
            if (values != null && values.Length > 0)
            {
                Type type = typeof(P);

                object nonNullValues = values;

                //如果是Nullable<X>类型，则转化成X类型
                if (type.IsNullableType())
                {
                    //type = type.GetNonNullableType();

                    //nonNullValues = values.Select(v => Convert.ChangeType(v, type)).ToArray();
                }

                var method = method_Contains.MakeGenericMethod(new Type[] { type });

                var propertyBody = GetMemberExpression(q, property);

                MethodCallExpression methodExpr = Expression.Call(null,
                    method,
                    Expression.Constant(nonNullValues),
                    propertyBody
                );

                q.AppendExpression(methodExpr);
            }

            return q;
        }

        #endregion

        #region Fuzzy

        /// <summary>
        /// 建立 Fuzzy 查询条件
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="q">动态查询条件创建者</param>
        /// <param name="property">属性</param>
        /// <param name="expression">查询表达式（支持：1,2,3 或 1-3；如果不符合前面规则，即认为模糊查询；忽略空格；）</param>
        /// <returns></returns>
        public static IQueryBuilder<T> Fuzzy<T>(this IQueryBuilder<T> q, Expression<Func<T, string>> property, string expression)
        {
            if (string.IsNullOrEmpty(expression)) return q;
            expression = expression.Trim();
            List<string> valueList = new List<string>();
            string[] splits = expression.Split(',');
            if (splits.Length > 1)
            {
                splits.ForEach(c =>
                {
                    string el = c.Trim();
                    if (el.Length > 0)
                    {
                        valueList.Add(el);
                    }
                });

                return q.In<T, string>(property, valueList.ToArray());
            }

            int index_minus = expression.IndexOf('-');
            if (index_minus > 0 && index_minus < expression.Length - 1)
            {
                string left = expression.Substring(0, index_minus).Trim();
                string right = expression.Substring(index_minus + 1).Trim();
                return q.Between<T>(property, left, right);
            }

            return q.Like(property, expression);
        }

        #endregion

        #region GreaterThan

        /// <summary>
        /// 大于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> GreaterThan<T, P>(this IQueryBuilder<T> q, Expression<Func<T, P>> property, P value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Expression right = Expression.Constant(value);
            Type type = typeof(P);

            var propertyBody = GetMemberExpression(q, property);

            Expression left = propertyBody;

            var methodExpr = Expression.GreaterThan(left, right);

            q.AppendExpression(methodExpr);

            return q;
        }

        /// <summary>
        /// 大于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> GreaterThanOrEqual<T, P>(this IQueryBuilder<T> q, Expression<Func<T, P>> property, P value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Expression right = Expression.Constant(value);
            Type type = typeof(P);

            var propertyBody = GetMemberExpression(q, property);

            Expression left = propertyBody;

            var methodExpr = Expression.GreaterThanOrEqual(left, right);

            q.AppendExpression(methodExpr);

            return q;
        }

        #endregion

        #region LessThan

        /// <summary>
        /// 小于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> LessThan<T, P>(this IQueryBuilder<T> q, Expression<Func<T, P>> property, P value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Expression right = Expression.Constant(value);
            Type type = typeof(P);

            var propertyBody = GetMemberExpression(q, property);

            Expression left = propertyBody;

            var methodExpr = Expression.LessThan(left, right);

            q.AppendExpression(methodExpr);

            return q;
        }

        /// <summary>
        /// 小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="q"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryBuilder<T> LessThanOrEqual<T, P>(this IQueryBuilder<T> q, Expression<Func<T, P>> property, P value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Expression right = Expression.Constant(value);
            Type type = typeof(P);

            var propertyBody = GetMemberExpression(q, property);

            Expression left = propertyBody;

            var methodExpr = Expression.LessThanOrEqual(left, right);

            q.AppendExpression(methodExpr);

            return q;
        }

        #endregion
    }
}

