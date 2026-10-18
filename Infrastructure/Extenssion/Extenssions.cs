using Model.PresentationModels;
using MWSMan.Core.Model.Events;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;


namespace MWSMan.Core.Infrastructure.Extenssion
{
    public static class Extenssions
    {

        private const string KeyValueSeparator = "%%%";
        private const string ItemSeparator = "@@@";
        public static void TakeAllServerOn(this IList<ServerSelected> serverSelecteds, ResponseState responseState, Func<ServerSelected, bool> func)
        {

            if (responseState is ResponseState.Finish || responseState is ResponseState.FinishWithError)
                foreach (ServerSelected server in serverSelecteds.Where(func))
                {
                    server.responseInfo.responseState = responseState;
                    server.isSelected = false;
                }

            else if (responseState is ResponseState.None)
            {
                foreach (ServerSelected server in serverSelecteds.Where(func))
                {
                    server.responseInfo.responseState = responseState;
                    server.isSelected = false;
                }
            }
            else if (responseState is ResponseState.Wait)
            {
                foreach (ServerSelected server in serverSelecteds.Where(func))
                {
                    server.responseInfo.responseState = responseState;
                }
            }
            else
                foreach (ServerSelected server in serverSelecteds.Where(func))
                {
                    server.responseInfo.responseState = responseState;
                    server.isSelected = true;

                }

        }
        public static string ToListString(this IDictionary<string, object?> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
                return string.Empty;

            return string.Join(
                ItemSeparator,
                dictionary.Select(x =>
                    $"{x.Key}{KeyValueSeparator}{x.Value ?? string.Empty}{KeyValueSeparator}"));
        }


        public static IDictionary<string, object?> KeyToLowerCase(this IDictionary<string, object?> data)
        {
            var result = new Dictionary<string, object?>(data.Count, StringComparer.Ordinal);

            foreach (var kv in data)
            {
                result[kv.Key.ToLowerInvariant()] = kv.Value;
            }

            return result;
        }

        //public static string? ToListString(this IDictionary<string, object?> keyValues)
        //{
        //    if (keyValues == null || keyValues.Count == 0)
        //        return null;

        //    return string.Join(';',
        //        keyValues.Select(x => $"{x.Key}:{x.Value}"));
        //}
        public static bool TryGetValue<T>(
        this IDictionary<string, object?>? dictionary,
        string _key,
        out T? value)
        {
            value = default;


            if (dictionary == null || !dictionary.TryGetValue(_key, out object? rawValue))
            {
                return false;
            }


            if (rawValue == null)
            {

                if (default(T) == null)
                {
                    value = default;
                    return true;
                }
                return false;
            }


            if (rawValue is T typedValue)
            {
                value = typedValue;
                return true;
            }


            Type targetType = typeof(T);
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType == typeof(string))
            {
                value = (T)(object)rawValue.ToString()!;
                return true;
            }


            if (rawValue is string stringValue)
            {

                try
                {
                    if (underlyingType.IsEnum)
                    {
                        var enumValue = Enum.Parse(underlyingType, stringValue, ignoreCase: true);
                        value = (T)enumValue;
                        return true;
                    }

                    if (underlyingType == typeof(Guid))
                    {
                        var guidValue = Guid.Parse(stringValue);
                        value = (T)(object)guidValue;
                        return true;
                    }

                    if (underlyingType == typeof(DateTime))
                    {
                        var dateValue = DateTime.Parse(stringValue, CultureInfo.InvariantCulture);
                        value = (T)(object)dateValue;
                        return true;
                    }

                    if (underlyingType == typeof(TimeSpan))
                    {
                        var timeSpanValue = TimeSpan.Parse(stringValue, CultureInfo.InvariantCulture);
                        value = (T)(object)timeSpanValue;
                        return true;
                    }


                    var converter = TypeDescriptor.GetConverter(underlyingType);
                    if (converter != null && converter.CanConvertFrom(typeof(string)))
                    {
                        var convertedValue = converter.ConvertFromString(null, CultureInfo.InvariantCulture, stringValue);
                        if (convertedValue != null)
                        {
                            value = (T)convertedValue;
                            return true;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }


            try
            {

                if (rawValue is IConvertible convertible &&
                    underlyingType != typeof(object) &&
                    underlyingType != typeof(string))
                {
                    object convertedValue = Convert.ChangeType(convertible, underlyingType, CultureInfo.InvariantCulture);
                    value = (T)convertedValue;
                    return true;
                }

                // استفاده از TypeConverter
                var typeConverter = TypeDescriptor.GetConverter(rawValue.GetType());
                if (typeConverter.CanConvertTo(underlyingType))
                {
                    var convertedValue = typeConverter.ConvertTo(null, CultureInfo.InvariantCulture, rawValue, underlyingType);
                    if (convertedValue != null)
                    {
                        value = (T)convertedValue;
                        return true;
                    }
                }


                if (rawValue.GetType().Name == "JsonElement")
                {
                    var jsonString = rawValue.ToString();
                    if (!string.IsNullOrEmpty(jsonString))
                    {

                        var jsonElement = (System.Text.Json.JsonElement)rawValue;
                        var convertedValue = jsonElement.Deserialize<T>();
                        if (convertedValue != null)
                        {
                            value = convertedValue;
                            return true;
                        }
                    }
                }
            }
            catch
            {
                // خطا در تبدیل
            }

            return false;
        }
        public static bool TryGetValue<T>(
        this IDictionary<string, string> dictionary,
        string _key,
        out T? value)
        {
            value = default;


            if (dictionary == null || !dictionary.TryGetValue(_key, out object? rawValue))
            {
                return false;
            }


            if (rawValue == null)
            {

                if (default(T) == null)
                {
                    value = default;
                    return true;
                }
                return false;
            }


            if (rawValue is T typedValue)
            {
                value = typedValue;
                return true;
            }


            Type targetType = typeof(T);
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType == typeof(string))
            {
                value = (T)(object)rawValue.ToString()!;
                return true;
            }


            if (rawValue is string stringValue)
            {

                try
                {
                    if (underlyingType.IsEnum)
                    {
                        var enumValue = Enum.Parse(underlyingType, stringValue, ignoreCase: true);
                        value = (T)enumValue;
                        return true;
                    }

                    if (underlyingType == typeof(Guid))
                    {
                        var guidValue = Guid.Parse(stringValue);
                        value = (T)(object)guidValue;
                        return true;
                    }

                    if (underlyingType == typeof(DateTime))
                    {
                        var dateValue = DateTime.Parse(stringValue, CultureInfo.InvariantCulture);
                        value = (T)(object)dateValue;
                        return true;
                    }

                    if (underlyingType == typeof(TimeSpan))
                    {
                        var timeSpanValue = TimeSpan.Parse(stringValue, CultureInfo.InvariantCulture);
                        value = (T)(object)timeSpanValue;
                        return true;
                    }


                    var converter = TypeDescriptor.GetConverter(underlyingType);
                    if (converter != null && converter.CanConvertFrom(typeof(string)))
                    {
                        var convertedValue = converter.ConvertFromString(null, CultureInfo.InvariantCulture, stringValue);
                        if (convertedValue != null)
                        {
                            value = (T)convertedValue;
                            return true;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }


            try
            {

                if (rawValue is IConvertible convertible &&
                    underlyingType != typeof(object) &&
                    underlyingType != typeof(string))
                {
                    object convertedValue = Convert.ChangeType(convertible, underlyingType, CultureInfo.InvariantCulture);
                    value = (T)convertedValue;
                    return true;
                }

                // استفاده از TypeConverter
                var typeConverter = TypeDescriptor.GetConverter(rawValue.GetType());
                if (typeConverter.CanConvertTo(underlyingType))
                {
                    var convertedValue = typeConverter.ConvertTo(null, CultureInfo.InvariantCulture, rawValue, underlyingType);
                    if (convertedValue != null)
                    {
                        value = (T)convertedValue;
                        return true;
                    }
                }


                if (rawValue.GetType().Name == "JsonElement")
                {
                    var jsonString = rawValue.ToString();
                    if (!string.IsNullOrEmpty(jsonString))
                    {

                        var jsonElement = (System.Text.Json.JsonElement)rawValue;
                        var convertedValue = jsonElement.Deserialize<T>();
                        if (convertedValue != null)
                        {
                            value = convertedValue;
                            return true;
                        }
                    }
                }
            }
            catch
            {
                // خطا در تبدیل
            }

            return false;
        }
        public static IDictionary<string, object?> TooDictionary(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new Dictionary<string, object?>();

            return value
                .Split(new[] { ItemSeparator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(new[] { KeyValueSeparator }, 2, StringSplitOptions.None))
                .ToDictionary(
                    x => x[0],
                    x => (object?)(x.Length > 1
                        ? x[1].TrimEnd('%')
                        : string.Empty));
        }


        public static void AddOnList<T>(this IDictionary<string, List<T>> keyValues, string key, T value)
        {
            if (keyValues.TryGetValue(key, out List<T>? data))
            {
                data.Add(value);
            }
            else
            {
                data = [value];
                keyValues.Add(key, data);
            }

        }

    }
}
