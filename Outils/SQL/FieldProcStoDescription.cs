namespace Outils.SQL
{
    public class FieldProcStoDescription
    {
        public FieldProcStoDescription(string name, ProcStoBase proSto)
        {
            Name = name;
            proSto.Add(this);
        }

        public string Name { get; }

        public object Value { get; set; }
    }
}