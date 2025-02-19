using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using K9.SharedLibrary.Exceptions;
using K9.SharedLibrary.Extensions;

namespace K9.SharedLibrary.Helpers
{
    public class ObjectMapperSharedProperty
    {
        public PropertyInfo MappedFromProperty { get; set; }
        public PropertyInfo MappedToProperty { get; set; }
    }

    public interface ICustomMap
    {
        Object MapFromObject(object item);
    }

    /// <summary>
    /// Maps properties from one class to another using a custom mapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class CustomMapBase<T, T2> : ICustomMap
        where T : class
        where T2 : class
    {
        public object MapFromObject(object item)
        {
            return MapFrom(item as T);
        }

        public abstract T2 MapFrom(T item);
    }

    public static class ObjectMapper
    {
        private static readonly Dictionary<Type, List<PropertyInfo>> ObjectProperties;
        private static List<Tuple<Assembly, Tuple<Type, Type, ICustomMap>>> _maps;
        private static List<Tuple<Assembly, Tuple<Type, Type, ICustomMap>>> Maps => _maps;

        static ObjectMapper()
        {
            ObjectProperties = new Dictionary<Type, List<PropertyInfo>>();
        }

        public static T MapTo<T>(this object item, bool ignoreCustomMap = false)
            where T : class
        {
            if (item == null)
            {
                return (T)null;
            }

            var mapFromType = item.GetType();
            var mapToType = typeof(T);

            if (!ignoreCustomMap)
            {
                var customMap = GetCustomMap(mapFromType, mapToType);
                if (customMap != null)
                {
                    return customMap.MapFromObject(item) as T;
                }
            }

            var result = Activator.CreateInstance<T>();

            DeepMapSharedProperties(item, result, GetSharedProperties(mapFromType, typeof(T)));

            return result;
        }

        private static void DeepMapSharedProperties(object mapFrom, object mapTo, List<ObjectMapperSharedProperty> sharedProperties)
        {
            foreach (var sharedProperty in sharedProperties)
            {
                var mapFromProperty = sharedProperty.MappedFromProperty;
                var mapToProperty = sharedProperty.MappedToProperty;
                var value = mapFrom.GetProperty(mapFromProperty);

                if (value != null && mapToProperty.CanWrite)
                {
                    try
                    {
                        SetValue(mapTo, sharedProperty.MappedFromProperty.PropertyType, sharedProperty.MappedToProperty, value);
                    }
                    catch (Exception ex)
                    {
                        throw new FailedToMapPropertyException(mapToProperty, ex);
                    }
                }
            }
        }

        private static List<ObjectMapperSharedProperty> GetSharedProperties(Type mappedFromType, Type mappedToType)
        {
            var mapFromProperties = GetProperties(mappedFromType);
            var mapToProperties = GetProperties(mappedToType);

            return mapToProperties
                .Where(mapTo => mapFromProperties.Select(mapFrom => mapFrom.Name).Contains(mapTo.Name))
                .Select(mapTo => new ObjectMapperSharedProperty
                {
                    MappedFromProperty = mapFromProperties.First(p => p.Name == mapTo.Name),
                    MappedToProperty = mapTo
                }).ToList();
        }

        private static void SetValue(Object mapTo, Type mapFromPropertyType, PropertyInfo mapToPropertyInfo, object value, bool ignorePropertiesOfDifferentType = false)
        {
            if (mapToPropertyInfo.CanWrite)
            {
                if (mapFromPropertyType != mapToPropertyInfo.PropertyType)
                {
                    if (ignorePropertiesOfDifferentType)
                    {
                        return;
                    }

                    if (typeof(Enum).IsAssignableFrom(mapToPropertyInfo.PropertyType))
                    {
                        mapTo.SetProperty(
                            mapToPropertyInfo,
                            Enum.Parse(mapToPropertyInfo.PropertyType, value.ToString()));

                        return;
                    }

                    if (typeof(IList).IsAssignableFrom(mapFromPropertyType) &&
                        typeof(IList).IsAssignableFrom(mapToPropertyInfo.PropertyType))
                    {
                        mapTo.MapListProperty(mapFromPropertyType, mapToPropertyInfo, value);
                        return;
                    }

                    if (mapFromPropertyType.GetTypeInfo().IsClass && mapToPropertyInfo.PropertyType.GetTypeInfo().IsClass)
                    {
                        mapTo.MapClassProperty(mapFromPropertyType, mapToPropertyInfo, value);
                        return;
                    }
                }

                mapTo.SetProperty(mapToPropertyInfo, value);
            }
        }

        private static void MapClassProperty(this object item, Type mapFromPropertyType, PropertyInfo mapToProperty,
            object value)
        {
            var mappedToPropertyValue = Activator.CreateInstance(mapToProperty.PropertyType);
            var sharedProperties = GetSharedProperties(mapFromPropertyType, mapToProperty.PropertyType);

            foreach (var sharedPropertyInfo in sharedProperties)
            {
                SetValue(
                    mappedToPropertyValue,
                    sharedPropertyInfo.MappedFromProperty.PropertyType,
                    sharedPropertyInfo.MappedToProperty,
                    value.GetProperty(sharedPropertyInfo.MappedFromProperty));
            }
            item.SetProperty(mapToProperty, mappedToPropertyValue);
        }

        private static void MapListProperty(this object item, Type mapFromPropertyType, PropertyInfo mapToProperty,
            object value)
        {
            var fromGenericType = typeof(Array).IsAssignableFrom(mapFromPropertyType.GetTypeInfo().BaseType)
                ? mapFromPropertyType.GetElementType()
                : mapFromPropertyType.GetGenericArguments()[0];
            var isTargetAnArray = typeof(Array).IsAssignableFrom(mapToProperty.PropertyType.GetTypeInfo().BaseType);
            var arrayElementType = mapToProperty.PropertyType.GetElementType();
            var toGenericType = isTargetAnArray
                ? arrayElementType
                : mapToProperty.PropertyType.GetGenericArguments()[0];

            var sourceList = value as IList;
            var targetList = isTargetAnArray
                ? Array.CreateInstance(arrayElementType, sourceList.Count)
                : Activator.CreateInstance(mapToProperty.PropertyType) as IList;

            var i = 0;
            foreach (var listItem in sourceList)
            {
                var newListItem = Activator.CreateInstance(toGenericType);
                var sharedProperties = GetSharedProperties(fromGenericType, toGenericType);

                foreach (var sharedPropertyInfo in sharedProperties)
                {
                    SetValue(
                        newListItem,
                        sharedPropertyInfo.MappedFromProperty.PropertyType,
                        sharedPropertyInfo.MappedToProperty,
                        listItem.GetProperty(sharedPropertyInfo.MappedFromProperty));
                }
                if (isTargetAnArray)
                {
                    targetList[i] = newListItem;
                }
                else
                {
                    targetList.Add(newListItem);
                }
                i++;
            }

            item.SetProperty(mapToProperty, targetList);
        }

        private static void LoadMapsFromAssembly(Assembly assembly)
        {
            if (_maps == null)
            {
                _maps = new List<Tuple<Assembly, Tuple<Type, Type, ICustomMap>>>();
            }

            if (!_maps.Exists(m => m.Item1 == assembly))
            {
                var types = assembly.GetTypes()
                    .Where(p =>
                        typeof(ICustomMap).IsAssignableFrom(p)
                        && !p.GetTypeInfo().IsAbstract);

                foreach (var t in types)
                {
                    var map = Activator.CreateInstance(t) as ICustomMap;
                    var baseType = t.GetTypeInfo().BaseType;
                    var mapFromType = baseType.GenericTypeArguments[0];
                    var mapToType = baseType.GenericTypeArguments[1];

                    _maps.Add(new Tuple<Assembly, Tuple<Type, Type, ICustomMap>>(assembly, new Tuple<Type, Type, ICustomMap>(mapFromType, mapToType, map)));
                }
            }
        }

        private static ICustomMap GetCustomMap(Type mapFromType, Type mapToType)
        {
            var assembly = mapFromType.GetTypeInfo().Assembly;
            LoadMapsFromAssembly(assembly);
            var match = Maps.FirstOrDefault(
                m => m.Item1 == assembly &&
                     m.Item2.Item1 == mapFromType &&
                     m.Item2.Item2 == mapToType);
            return match?.Item2.Item3;
        }

        private static List<PropertyInfo> GetProperties(Type type)
        {
            List<PropertyInfo> properties;
            if (ObjectProperties.ContainsKey(type))
            {
                properties = ObjectProperties[type];
            }
            else
            {
                properties = new List<PropertyInfo>();
                foreach (PropertyInfo property in type.GetProperties())
                    properties.Add(property);
                ObjectProperties.Add(type, properties);
            }
            return properties;
        }

    }
}