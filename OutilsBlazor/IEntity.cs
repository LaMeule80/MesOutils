namespace OutilsBlazor
{
    public interface IEntity
    {
        /// <summary>
        /// Nom de l'entité
        /// </summary>
        string Nom { get; }

        /// <summary>
        /// Insetion de l'entité
        /// </summary>
        void Insert();

        /// <summary>
        /// Maj de l'entité
        /// </summary>
        void Update();

        /// <summary>
        /// Suppression de l'entité
        /// </summary>
        void Delete();
    }
}