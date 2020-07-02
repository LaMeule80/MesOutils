using System;
using System.Globalization;
using System.Windows.Controls;

namespace Outils.SQL
{
    public abstract class DescriptionBase<TSelf> : IDataAccessDbDescriptionElementBase where TSelf : DescriptionBase<TSelf>, new()
    {
        public static TSelf Instance { get; }

        public abstract string Name { get; }

        public string SqlName => string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[]
        {
            Name
        });

        static DescriptionBase()
        {
            Instance = Activator.CreateInstance<TSelf>();
        }

        protected T GetViewInstanceInstance<T>(ref T table, IDataAccessDbDescriptionElementBase schema)
            where T : ViewBase, new()
        {
            if (table == null)
            {
                var t = Activator.CreateInstance<T>();
                table = t;
            }
            return table;
        }
    }
}