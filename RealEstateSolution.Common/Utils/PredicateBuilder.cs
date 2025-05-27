using System;
using System.Linq;
using System.Linq.Expressions;

namespace RealEstateSolution.Common.Utils
{
    /// <summary>
    /// 表达式组合构建器
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// 创建始终为真的表达式
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>表达式</returns>
        public static Expression<Func<T, bool>> True<T>() => param => true;

        /// <summary>
        /// 创建始终为假的表达式
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>表达式</returns>
        public static Expression<Func<T, bool>> False<T>() => param => false;

        /// <summary>
        /// 组合两个表达式（AND）
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expr1">表达式1</param>
        /// <param name="expr2">表达式2</param>
        /// <returns>组合后的表达式</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);
            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        /// <summary>
        /// 组合两个表达式（OR）
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expr1">表达式1</param>
        /// <param name="expr2">表达式2</param>
        /// <returns>组合后的表达式</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);
            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left, right), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }
    }
} 