using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KTZSanatoriumServiceApi.Helpers.FilterHelper
{
    public class FilterHelperAtribute<T> : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var filter = context.HttpContext.Request.Query["filter"].ToString();
            var sort = context.HttpContext.Request.Query["sort"].ToString();
            var take = context.HttpContext.Request.Query["take"].ToString();
            var skip = context.HttpContext.Request.Query["skip"].ToString();
            if (string.IsNullOrWhiteSpace(filter)) return;
            var filters = ParseFilters(filter);
            // Применяем фильтры к результату запроса
            IQueryable result = ApplyFilters(context.Result as OkObjectResult, filters, skip, take, sort);

            // Заменяем результат запроса отфильтрованным результатом
            var executedResult = context.Result as ObjectResult;
            executedResult.Value = result;
        }


        private dynamic ParseFilters(string filter)
        {
            var regex = new Regex(@"([\w.]+)\s*(gt|ls|eq|nq)\s*(""([^""]|"""")*""|\w+)");

            var matches = regex.Matches(filter);

            var filters = matches.Select(match =>
            {
                var fieldName = match.Groups[1].Value;
                PropertyInfo propertyInfo;

                if (fieldName.Contains('.'))
                {
                    // Если название поля содержит точку, это вложенное свойство
                    var propertyNames = fieldName.Split('.');
                    propertyInfo = typeof(T).GetProperty(propertyNames[0]);
                    for (int i = 1; i < propertyNames.Length; i++)
                    {
                        propertyInfo = propertyInfo.PropertyType.GetProperty(propertyNames[i]);
                    }
                }
                else
                {
                    // Иначе это свойство верхнего уровня
                    propertyInfo = typeof(T).GetProperty(fieldName);
                }
                var propertyType = propertyInfo.PropertyType;
                var propertyValue = match.Groups[3].Value;

                if (propertyType.IsEnum)
                {
                    // Для перечислений
                    Enum.TryParse(propertyType, propertyValue, out var enumValue);
                    return new FieldFilter
                    {
                        Field = fieldName,
                        Operator = match.Groups[2].Value,
                        Value = enumValue
                    };
                }
                else if (propertyType == typeof(int))
                {
                    // Для целых чисел
                    var intValue = int.Parse(propertyValue);
                    return new FieldFilter
                    {
                        Field = fieldName,
                        Operator = match.Groups[2].Value,
                        Value = intValue
                    };
                }
                else if (propertyType == typeof(double))
                {
                    // Для чисел с плавающей точкой
                    var doubleValue = double.Parse(propertyValue);
                    return new FieldFilter
                    {
                        Field = fieldName,
                        Operator = match.Groups[2].Value,
                        Value = doubleValue
                    };
                }
                else if (propertyType == typeof(bool))
                {
                    // Для логических значений
                    var boolValue = bool.Parse(propertyValue);
                    return new FieldFilter
                    {
                        Field = fieldName,
                        Operator = match.Groups[2].Value,
                        Value = boolValue
                    };
                }
                else
                {
                    // Для всех остальных типов, включая строки
                    return new FieldFilter
                    {
                        Field = fieldName,
                        Operator = match.Groups[2].Value,
                        Value = Convert.ChangeType(propertyValue, propertyType)
                    };
                }

            }).ToList();

            return filters;
        }

        private IQueryable ApplyFilters(OkObjectResult result, List<FieldFilter> filters, string skip, string take, string sort)
        {


            var queryable = result.Value as IQueryable<T>;

            if (queryable != null)
            {
                var count = queryable.Count();
                // Применяем сортировку
                var results = queryable.ApplyFilter(filters);


                return results;
            }
            return queryable;
        }
    }
}
