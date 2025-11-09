using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SoftPac.Business
{
    /// <summary>
    /// Conversor genérico de DTOs usando reflexión
    /// </summary>
    public static class DTOConverter
    {
        /// <summary>
        /// Convierte de TSource a TDestination automáticamente
        /// </summary>
        public static TDestination Convertir<TSource, TDestination>(TSource source)
            where TDestination : new()
        {
            if (source == null)
                return default(TDestination);

            TDestination destination = new TDestination();
            ConvertirPropiedades(source, destination);
            return destination;
        }

        /// <summary>
        /// Convierte una lista de objetos
        /// </summary>
        public static List<TDestination> ConvertirLista<TSource, TDestination>(IEnumerable<TSource> sourceList)
            where TDestination : new()
        {
            if (sourceList == null)
                return new List<TDestination>();

            return sourceList.Select(item => Convertir<TSource, TDestination>(item)).ToList();
        }

        /// <summary>
        /// Convierte un array de objetos
        /// </summary>
        public static TDestination[] ConvertirArray<TSource, TDestination>(TSource[] sourceArray)
            where TDestination : new()
        {
            if (sourceArray == null)
                return Array.Empty<TDestination>();

            return sourceArray.Select(item => Convertir<TSource, TDestination>(item)).ToArray();
        }

        /// <summary>
        /// Copia las propiedades de source a destination basándose en nombres similares
        /// </summary>
        private static void ConvertirPropiedades(object source, object destination)
        {
            if (source == null || destination == null)
                return;

            Type sourceType = source.GetType();
            Type destType = destination.GetType();

            PropertyInfo[] sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] destProps = destType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo sourceProp in sourceProps)
            {
                if (!sourceProp.CanRead)
                    continue;

                // Buscar propiedad destino con nombre similar
                PropertyInfo destProp = BuscarPropiedadDestino(sourceProp, destProps);

                if (destProp != null && destProp.CanWrite)
                {
                    try
                    {
                        object valor = sourceProp.GetValue(source);

                        if (valor != null)
                        {
                            // Verificar si es un tipo "Specified"
                            if (sourceProp.Name.EndsWith("Specified"))
                                continue;

                            // Verificar si hay un campo "Specified" asociado
                            bool tieneSpecified = sourceProps.Any(p => p.Name == sourceProp.Name + "Specified");
                            if (tieneSpecified)
                            {
                                PropertyInfo specifiedProp = sourceType.GetProperty(sourceProp.Name + "Specified");
                                bool? isSpecified = specifiedProp?.GetValue(source) as bool?;
                                if (isSpecified == false)
                                    continue;
                            }

                            // Convertir el valor
                            object valorConvertido = ConvertirValor(valor, sourceProp.PropertyType, destProp.PropertyType);

                            if (valorConvertido != null)
                            {
                                destProp.SetValue(destination, valorConvertido);

                                // Si el destino tiene un campo "Specified", establecerlo en true
                                PropertyInfo destSpecifiedProp = destType.GetProperty(destProp.Name + "Specified");
                                if (destSpecifiedProp != null && destSpecifiedProp.CanWrite)
                                {
                                    destSpecifiedProp.SetValue(destination, true);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al convertir propiedad {sourceProp.Name}: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Busca una propiedad en el destino que coincida con el nombre de la propiedad origen
        /// </summary>
        private static PropertyInfo BuscarPropiedadDestino(PropertyInfo sourceProp, PropertyInfo[] destProps)
        {
            string sourceName = sourceProp.Name;

            // Busqueda exacta
            PropertyInfo exactMatch = destProps.FirstOrDefault(p =>
                p.Name.Equals(sourceName, StringComparison.OrdinalIgnoreCase));

            if (exactMatch != null)
                return exactMatch;

            // Convertir entre convenciones de nombres
            // snake_case ↔ PascalCase
            string convertedName = ConvertirNombrePropiedad(sourceName);

            PropertyInfo convertedMatch = destProps.FirstOrDefault(p =>
                p.Name.Equals(convertedName, StringComparison.OrdinalIgnoreCase));

            return convertedMatch;
        }

        /// <summary>
        /// Convierte entre convenciones de nombres (snake_case ↔ PascalCase)
        /// </summary>
        private static string ConvertirNombrePropiedad(string nombre)
        {
            // snake_case → PascalCase
            if (nombre.Contains("_"))
            {
                var parts = nombre.Split('_');
                return string.Join("", parts.Select(p =>
                    char.ToUpper(p[0]) + p.Substring(1).ToLower()));
            }

            // PascalCase → snake_case
            if (char.IsUpper(nombre[0]))
            {
                return string.Concat(nombre.Select((x, i) => i > 0 && char.IsUpper(x)
                    ? "_" + x.ToString()
                    : x.ToString())).ToLower();
            }

            return nombre;
        }

        /// <summary>
        /// Convierte un valor entre tipos diferentes
        /// </summary>
        private static object ConvertirValor(object valor, Type sourceType, Type destType)
        {
            if (valor == null)
                return null;

            // Mismo tipo
            if (sourceType == destType)
                return valor;

            // Tipos nullable
            Type underlyingDestType = Nullable.GetUnderlyingType(destType) ?? destType;
            Type underlyingSourceType = Nullable.GetUnderlyingType(sourceType) ?? sourceType;

            // Conversión de fechas (localDateTime ↔ DateTime)
            if (EsLocalDateTime(sourceType) && underlyingDestType == typeof(DateTime))
            {
                return ConvertirLocalDateTimeADateTime(valor);
            }

            if (underlyingSourceType == typeof(DateTime) && EsLocalDateTime(destType))
            {
                return ConvertirDateTimeALocalDateTime((DateTime)valor, destType);
            }

            // Tipos primitivos o estructuras
            if (underlyingDestType.IsPrimitive || underlyingDestType == typeof(string) ||
                underlyingDestType == typeof(decimal) || underlyingDestType == typeof(DateTime))
            {
                try
                {
                    return Convert.ChangeType(valor, underlyingDestType);
                }
                catch
                {
                    return null;
                }
            }

            // Arrays o listas
            if (typeof(IEnumerable).IsAssignableFrom(sourceType) &&
                typeof(IEnumerable).IsAssignableFrom(destType) &&
                sourceType != typeof(string) && destType != typeof(string))
            {
                return ConvertirColeccion(valor, sourceType, destType);
            }

            // Objetos complejos (recursión)
            if (sourceType.IsClass && destType.IsClass)
            {
                if (destType.GetConstructor(Type.EmptyTypes) != null)
                {
                    object destObject = Activator.CreateInstance(destType);
                    ConvertirPropiedades(valor, destObject);
                    return destObject;
                }
            }

            return null;
        }

        /// <summary>
        /// Convierte colecciones (arrays, listas)
        /// </summary>
        private static object ConvertirColeccion(object valor, Type sourceType, Type destType)
        {
            IEnumerable sourceCollection = valor as IEnumerable;
            if (sourceCollection == null)
                return null;

            Type sourceElementType = sourceType.IsArray
                ? sourceType.GetElementType()
                : sourceType.GetGenericArguments().FirstOrDefault();

            Type destElementType = destType.IsArray
                ? destType.GetElementType()
                : destType.GetGenericArguments().FirstOrDefault();

            if (sourceElementType == null || destElementType == null)
                return null;

            List<object> convertedList = new List<object>();
            foreach (object item in sourceCollection)
            {
                object convertedItem = ConvertirValor(item, sourceElementType, destElementType);
                if (convertedItem != null)
                {
                    convertedList.Add(convertedItem);
                }
            }

            // Convertir a array si el destino es array
            if (destType.IsArray)
            {
                Array destArray = Array.CreateInstance(destElementType, convertedList.Count);
                for (int i = 0; i < convertedList.Count; i++)
                {
                    destArray.SetValue(convertedList[i], i);
                }
                return destArray;
            }

            // Convertir a List<T>
            Type listType = typeof(List<>).MakeGenericType(destElementType);
            IList destList = (IList)Activator.CreateInstance(listType);
            foreach (object item in convertedList)
            {
                destList.Add(item);
            }
            return destList;
        }

        /// <summary>
        /// Verifica si un tipo es localDateTime
        /// </summary>
        private static bool EsLocalDateTime(Type type)
        {
            return type.Name.Contains("localDateTime") || type.Name.Contains("LocalDateTime");
        }

        /// <summary>
        /// Convierte localDateTime a DateTime
        /// </summary>
        private static DateTime? ConvertirLocalDateTimeADateTime(object localDateTime)
        {
            if (localDateTime == null)
                return null;

            try
            {
                var type = localDateTime.GetType();

                int? year = ObtenerPropiedad<int>(localDateTime, "year");
                int? month = ObtenerPropiedad<int>(localDateTime, "monthValue") ??
                            ObtenerPropiedad<int>(localDateTime, "month");
                int? day = ObtenerPropiedad<int>(localDateTime, "dayOfMonth") ??
                          ObtenerPropiedad<int>(localDateTime, "day");
                int? hour = ObtenerPropiedad<int>(localDateTime, "hour") ?? 0;
                int? minute = ObtenerPropiedad<int>(localDateTime, "minute") ?? 0;
                int? second = ObtenerPropiedad<int>(localDateTime, "second") ?? 0;

                if (year.HasValue && month.HasValue && day.HasValue)
                {
                    return new DateTime(year.Value, month.Value, day.Value,
                                       hour.Value, minute.Value, second.Value);
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Convierte DateTime a localDateTime
        /// </summary>
        private static object ConvertirDateTimeALocalDateTime(DateTime fecha, Type localDateTimeType)
        {
            try
            {
                object localDateTime = Activator.CreateInstance(localDateTimeType);

                EstablecerPropiedad(localDateTime, "year", fecha.Year);
                EstablecerPropiedad(localDateTime, "monthValue", fecha.Month);
                EstablecerPropiedad(localDateTime, "dayOfMonth", fecha.Day);
                EstablecerPropiedad(localDateTime, "hour", fecha.Hour);
                EstablecerPropiedad(localDateTime, "minute", fecha.Minute);
                EstablecerPropiedad(localDateTime, "second", fecha.Second);

                return localDateTime;
            }
            catch
            {
                return null;
            }
        }

        private static T? ObtenerPropiedad<T>(object obj, string nombre) where T : struct
        {
            try
            {
                var prop = obj.GetType().GetProperty(nombre);
                if (prop != null)
                {
                    var valor = prop.GetValue(obj);
                    if (valor != null)
                        return (T)Convert.ChangeType(valor, typeof(T));
                }
            }
            catch { }
            return null;
        }

        private static void EstablecerPropiedad(object obj, string nombre, object valor)
        {
            try
            {
                var prop = obj.GetType().GetProperty(nombre);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(obj, valor);
                }
            }
            catch { }
        }
    }
}