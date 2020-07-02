using System;
using System.Collections.ObjectModel;
using System.Data;
using Outils.ObjectResultData;

namespace Outils.SQL
{
    public abstract class ProcStoBase
    {
        public IService Afficher { get; }

        public Collection<FieldProcStoDescription> Champs { get; } = new Collection<FieldProcStoDescription>();

        public abstract string Name { get; }

        internal void Add(FieldProcStoDescription field)
        {
            Champs.Add(field);
        }

        protected ProcStoBase(IService afficher = null)
        {
            Afficher = afficher;
        }

        public bool ExecuteNonQuery()
        {
            var result = new ObjectResult();
            using (var accessCommand = new AccessCommand(result, Name, CommandType.StoredProcedure))
            {
                foreach (var current in Champs)
                    accessCommand.Add(current.Name, current.Value);
                accessCommand.ExecuteNonQuery();
            }

            if (!result.IsSuccess)
                Afficher.Erreur(result.Message);

            return result.IsSuccess;
        }
    }
}