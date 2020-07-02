using System.Collections.Generic;
using System.Linq;
using Outils.ObjectResultData;

namespace Outils.SQL
{
    public class RapportBase : DataConnection<RapportEntity>
    {
        public RapportBase(IService service) : base(service)
        {
            
        }

        private Dictionary<string, List<string>> DonneDictionnaire(List<RapportEntity> source)
        {
            var result = new Dictionary<string, List<string>>();
            if (source.Count > 0)
            {
                var actrices = source
                    .GroupBy(x => x.Champ1)
                    .OrderBy(x => x.Key);

                foreach (var item in actrices)
                {
                    var actrice = item.Key;
                    var items = new List<string>();
                    foreach (var rapportEntity in item)
                        if (string.IsNullOrEmpty(rapportEntity.Champ3))
                            items.Add(rapportEntity.Champ2);
                        else
                            items.Add(rapportEntity.Champ2 + " - " + rapportEntity.Champ3);
                    result.Add(actrice, items.OrderBy(x => x).Select(x => x).ToList());
                }
            }

            return result;
        }

        public ObjectResult<Dictionary<string, List<string>>> CreeRapport(View table, FieldViewDescription actrice,
            FieldViewDescription champ2)
        {
            var values = LireValues(rec => new RapportEntity(rec, actrice, champ2), CreeClauseSelect(table));
            return new ObjectResult<Dictionary<string, List<string>>>
            {
                Value = DonneDictionnaire(values)
            };
        }

        public ObjectResult<Dictionary<string, List<string>>> CreeRapport(View table, FieldViewDescription actrice,
            FieldViewDescription champ2, FieldViewDescription champ3)
        {
            var values = LireValues(rec => new RapportEntity(rec, actrice, champ2, champ3), CreeClauseSelect(table));
            return new ObjectResult<Dictionary<string, List<string>>>
            {
                Value = DonneDictionnaire(values)
            };
        }
    }
}