using System.Collections.Generic;
using System.Reflection;

namespace Outils.Helper.EnumTools
{
    public class EnumValueManager<TEnum>
    {
        private readonly Dictionary<int, TEnum> _values;

        public EnumValueManager()
        {
            _values = new Dictionary<int, TEnum>();

            foreach (var field in typeof(TEnum).GetFields(
                BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                var attributs = field.GetCustomAttributes(typeof(EnumValueAttribute), true);
                var description = (EnumValueAttribute) attributs[0];
                var value = (TEnum)field.GetValue(null);
                _values.Add(description.Id, value);
            }
        }
        
        public bool GetId(int value, out TEnum result)
        {
            return _values.TryGetValue(value, out result);
        }
    }
}