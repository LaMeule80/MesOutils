using System;
using System.Collections.Generic;
using System.Linq;
using Outils.SQL;

namespace Outils.Helper.EnumTools
{
    public static class EnumHelper
    {
        public static List<T> EnumToList<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static T ParseToEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value);
        }

        public static T GetValue<T>(Enum value)
        {
            var attribute = GetAttribute<EnumValueAttribute>(value);
            if (attribute == null)
                throw new InvalidOperationException();
            var obj = attribute.Value;
            if (Guid.TryParse(obj.ToString(), out var guid))
                return (T) (object) guid;
            return (T) obj;
        }

        public static int GetId(Enum value)
        {
            var attribute = GetAttribute<EnumValueAttribute>(value);
            if (attribute == null)
                throw new InvalidOperationException();
            return attribute.Id;
        }

        public static bool IsVisible(Enum value)
        {
            var attribute = GetAttribute<EnumValueAttribute>(value);
            if (attribute == null)
                throw new InvalidOperationException();
            return attribute.Visible;
        }

        public static TEnum GetValueOfEnum<TEnum>(AccessReaderResult reader, FieldViewDescription champ)
        {
            var numericValue = reader.GetValue<int>(champ);
            var manager = new EnumValueManager<TEnum>();
            var result = manager.GetId(numericValue, out var enumValue);
            if (result)
                return enumValue;
            return default;
        }

        public static List<T> EnumToList<T>(T item)
        {
            var items = EnumToList<T>();
            var filter = item.ToString().Split(',');

            var result = new List<T>();
            foreach (var i in filter)
            {
                var p = ParseToEnum<T>(i.Trim());
                if (items.Contains(p))
                    result.Add(p);
            }

            return result;
        }

        private static T GetAttribute<T>(Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T) attributes[0];
        }
    }
}