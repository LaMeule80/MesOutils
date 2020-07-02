namespace Outils.SQL
{
    public class RapportEntity
    {
        public string Champ1 { get; set; }
        public string Champ2 { get; set; }
        public string Champ3 { get; set; }

        public RapportEntity(AccessReaderResult reader, FieldViewDescription champ1, FieldViewDescription champ2)
        {
            Champ1 = reader.GetStringValue(champ1);
            Champ2 = reader.GetStringValue(champ2);
        }

        public RapportEntity(AccessReaderResult reader, FieldViewDescription champ1, FieldViewDescription champ2, FieldViewDescription champ3)
        {
            Champ1 = reader.GetStringValue(champ1);
            Champ2 = reader.GetStringValue(champ2);
            Champ3 = reader.GetStringValue(champ3);
        }
    }
}