using Core.DTOs.GeneralDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public static class FilterBuilder
    {
        public static IQueryable<T> ApplyFilter<T> (IQueryable<T> source,List<FilterCondition> filterConditions)
        {
            if(filterConditions == null || !filterConditions.Any())
                return source;

            foreach(var filter in filterConditions)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var member = Expression.PropertyOrField(parameter, filter.Field);


                Type targetType = Nullable.GetUnderlyingType(member.Type) ?? member.Type;
                var typedValue = Convert.ChangeType(filter.Value, targetType);
                var constant = Expression.Constant(typedValue, member.Type);

                Expression? body = filter.Operator.ToLower() switch
                {
                    "equals" => Expression.Equal(member, constant),
                    "notequals" => Expression.NotEqual(member, constant),
                    "greaterthan" => Expression.GreaterThan(member, constant),
                    "lessthan" => Expression.LessThan(member, constant),
                    "contains" => Expression.Call(member, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, constant),
                    "startswith" => Expression.Call(member, typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!, constant),
                    "endswith" => Expression.Call(member, typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!, constant),
                    _ => null
                };

                if (body != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
                    source = source.Where(lambda);
                }
            }

            return source;
        }
    }
}
