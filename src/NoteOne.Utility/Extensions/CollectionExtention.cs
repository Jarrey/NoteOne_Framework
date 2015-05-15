using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NoteOne_Utility.Attributes;

namespace NoteOne_Utility.Extensions
{
    public static class CollectionExtention
    {
        public static IEnumerable<OperationPropertyInfo> GetCollectionOperationProperties<T>(
            this IEnumerable<T> collection)
        {
            var operationProperties = new List<OperationPropertyInfo>();
            IEnumerable<PropertyInfo> properties = typeof (T).GetRuntimeProperties();
            foreach (PropertyInfo property in properties)
            {
                var attributes =
                    (CollectionOperationAttribute[]) property.GetCustomAttributes(
                        typeof (CollectionOperationAttribute), false);

                if (attributes.Length > 0)
                {
                    operationProperties.Add(new OperationPropertyInfo
                        {
                            Operation = attributes[0].Operation,
                            PropertyInfo = property,
                            PropertyName = property.Name,
                            PropertyDescritpion = property.GetDescription()
                        });
                }
            }
            return operationProperties;
        }

        public static IEnumerable<OperationPropertyInfo> GetCollectionOperationProperties<T>(
            this IEnumerable<T> collection, CollectionOperate operate)
        {
            return from OperationPropertyInfo o in collection.GetCollectionOperationProperties()
                   where (o.Operation & operate) == operate
                   select o;
        }

        public static object Operate<T>(this IEnumerable<T> collection,
                                        OperationPropertyInfo operationPropertyInfo,
                                        CollectionOperate operate,
                                        Func<T, bool> filterFunc = null)
        {
            object result;
            switch (operate)
            {
                case CollectionOperate.Sort:
                    result = collection.OrderBy(p => operationPropertyInfo.PropertyInfo.GetValue(p));
                    return result as IOrderedEnumerable<T>;
                case CollectionOperate.Group:
                    result = collection.GroupBy(p => operationPropertyInfo.PropertyInfo.GetValue(p));
                    return result as IEnumerable<IGrouping<object, T>>;
                case CollectionOperate.Filter:
                    if (filterFunc != null)
                        result = from T p in collection
                                 where (bool) operationPropertyInfo.PropertyInfo.GetValue(p) == filterFunc(p)
                                 select p;
                    else
                        result = from T p in collection
                                 where (bool) operationPropertyInfo.PropertyInfo.GetValue(p)
                                 select p;
                    return result as IEnumerable<T>;
                default:
                    return collection;
            }
        }

        public class OperationPropertyInfo
        {
            public CollectionOperate Operation { get; set; }
            public string PropertyName { get; set; }
            public string PropertyDescritpion { get; set; }
            public PropertyInfo PropertyInfo { get; set; }

            public override string ToString()
            {
                return PropertyDescritpion;
            }
        }
    }
}