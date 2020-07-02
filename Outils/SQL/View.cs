using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace Outils.SQL
{
    public abstract class View
    {
        public Collection<FieldViewDescription> Champs { get; } = new Collection<FieldViewDescription>();

        public abstract string Name { get; }

        public string SqlName => $"dbo.{Name}";

        internal void Add(FieldViewDescription field)
        {
            Champs.Add(field);
        }
    }
}