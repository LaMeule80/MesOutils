using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Outils.SQL
{
    public class ParametresSql : List<Parametre>
    {
        public string Clause
        {
            get
            {
                if (Count == 0)
                    return string.Empty;

                var clauses = this.Select(x => x.Clause);
                return new StringBuilder(" WHERE ")
                    .Append(string.Join(" AND ", clauses))
                    .ToString();
            }
        }

        public void Add(FieldViewDescription champ, object value)
        {
            base.Add(new Parametre(champ, value));
        }

        public void Add(FieldViewDescription champ, object value, OrdreLogique ordreLogique)
        {
            base.Add(new Parametre(champ, value, ordreLogique));
        }

        public void IsNull(FieldViewDescription champ)
        {
            base.Add(new Parametre(champ, null, OrdreLogique.Null));
        }
    }
}