using System.Collections.ObjectModel;
using System.ComponentModel;
    
namespace Outils.SQL
{
    public abstract class ProsStoBase
    {
        public Collection<FieldProcStoDescription> Champs { get; } = new Collection<FieldProcStoDescription>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDataAccessDbDescriptionElementBase Schema { get; set; }

        public abstract string Name { get; }
        
        internal void Add(FieldProcStoDescription field)
        {
            Champs.Add(field);
        }
    }
}