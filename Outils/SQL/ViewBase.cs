using System;

namespace Outils.SQL
{
    public abstract class ViewBase<TSelf> where TSelf : ViewBase<TSelf>, new()
    {
        static ViewBase()
        {
            Instance = Activator.CreateInstance<TSelf>();
            Bolt = new object();
        }

        public static object Bolt { get; }

        public static TSelf Instance { get; }

        public abstract string Name { get; }

        protected T GetViewInstance<T>(ref T table)
            where T : View, new()
        {
            if (table != null) 
                return table;
            lock (Bolt)
            {
                if (table == null)
                {
                    var t = Activator.CreateInstance<T>();
                    table = t;
                }
            }

            return table;
        }
    }
}