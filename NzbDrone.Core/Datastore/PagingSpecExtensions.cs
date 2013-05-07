﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace NzbDrone.Core.Datastore
{
    public static class PagingSpecExtensions
    {
        public static Expression<Func<TModel, object>> OrderByClause<TModel>(this PagingSpec<TModel> pagingSpec)
        {
            return CreateExpression<TModel>(pagingSpec.SortKey);
        }

        public static int PagingOffset<TModel>(this PagingSpec<TModel> pagingSpec)
        {
            return (pagingSpec.Page - 1)*pagingSpec.PageSize;
        }

        private static Expression<Func<TModel, object>> CreateExpression<TModel>(string propertyName)
        {
            Type type = typeof(TModel);
            ParameterExpression parameterExpression = Expression.Parameter(type, "x");
            Expression expressionBody = parameterExpression;

            var splitPropertyName = propertyName.Split('.').ToList();

            foreach (var property in splitPropertyName)
            {
                expressionBody = Expression.Property(expressionBody, property);
            }

            expressionBody = Expression.Convert(expressionBody, typeof(object));
            return Expression.Lambda<Func<TModel, object>>(expressionBody, parameterExpression);
        }
    }
}
    