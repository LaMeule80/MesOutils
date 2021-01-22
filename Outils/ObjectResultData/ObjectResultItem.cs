namespace Outils.ObjectResultData
{
    public class ObjectResultItem
    {
        public ObjectResultItem()
        {
            
        }

        public ObjectResultItem(string message, Level niveau)
        {
            Message = message;
            Niveau = niveau;
        }

        public string Message { get; set; }

        public Level Niveau { get; set; }
    }
}