using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VRZ.Infra.Database.Serialization;

namespace VRZ.Infra.Database.Utils
{
    public static class PropertyInfoExtensions
    {
        /// <summary>
        ///     Get class/struct properties without <see cref="DatabaseIgnoreMember" /> attribute.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="ignoreId"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetMappedProperties(this IEnumerable<PropertyInfo> properties,
            bool ignoreId = false)
        {
            var mappedProperties = new List<PropertyInfo>();
            foreach (var property in properties)
            {
                if (ignoreId && (property.Name.ToLower() == "id" || property.Name.ToLower() == "_id"))
                {
                    continue;
                }

                var attributes = property.GetCustomAttributes(typeof(DatabaseIgnoreMember), false);
                if (attributes.Any())
                {
                    continue;
                }

                mappedProperties.Add(property);
            }

            return mappedProperties;
        }


        public static bool IsStatic(this PropertyInfo property)
        {
            return (property.GetMethod ?? property.SetMethod).IsStatic;
        }

        public static bool IsCandidateProperty(this PropertyInfo propertyInfo, bool needsWrite = true,
            bool publicOnly = true)
        {
            return !propertyInfo.IsStatic()
                   && propertyInfo.CanRead
                   && (!needsWrite || propertyInfo.FindSetterProperty() != null)
                   && propertyInfo.GetMethod != null
                   && (!publicOnly || propertyInfo.GetMethod.IsPublic)
                   && propertyInfo.GetIndexParameters().Length == 0;
        }

        public static bool IsIndexerProperty(this PropertyInfo propertyInfo)
        {
            var indexParams = propertyInfo.GetIndexParameters();
            return indexParams.Length == 1
                   && indexParams[0].ParameterType == typeof(string);
        }

        public static PropertyInfo FindGetterProperty(this PropertyInfo propertyInfo)
        {
            return propertyInfo.DeclaringType
                .GetPropertiesInHierarchy(propertyInfo.GetSimpleMemberName())
                .FirstOrDefault(p => p.GetMethod != null);
        }

        public static PropertyInfo FindSetterProperty(this PropertyInfo propertyInfo)
        {
            return propertyInfo.DeclaringType
                .GetPropertiesInHierarchy(propertyInfo.GetSimpleMemberName())
                .FirstOrDefault(p => p.SetMethod != null);
        }

        public static IEnumerable<PropertyInfo> GetPropertiesInHierarchy(this Type type, string name)
        {
            do
            {
                var typeInfo = type.GetTypeInfo();
                foreach (var propertyInfo in typeInfo.DeclaredProperties)
                {
                    if (propertyInfo.Name.Equals(name, StringComparison.Ordinal)
                        && !(propertyInfo.GetMethod ?? propertyInfo.SetMethod).IsStatic)
                    {
                        yield return propertyInfo;
                    }
                }

                type = typeInfo.BaseType;
            } while (type != null);
        }

        public static string GetSimpleMemberName(this MemberInfo member)
        {
            var name = member.Name;
            var index = name.LastIndexOf('.');
            return index >= 0 ? name.Substring(index + 1) : name;
        }
    }
}
