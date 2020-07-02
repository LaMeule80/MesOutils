using System.Globalization;

namespace Outils.SQL
{
    public class FieldViewDescription
    {
        private readonly View _view;

        public FieldViewDescription(string name, View view)
        {
            Name = name;
            _view = view;
            _view.Add(this);
        }

        public string ParameterName => $"@{_view.Name}_{Name}";

        public string SqlName => $"[dbo].[{_view.Name}].[{Name}]";

        public string Name { get; }
    }
}