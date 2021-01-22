using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Outils.ObjectResultData;
using Outils.Helper;

namespace Outils.SQL
{
    public interface IProcStoBase
    {
        void Add(FieldProcStoDescription field);
    }

    public enum TypeEnregistrement
    {
        Defaut,
        Ajout,
        Modifie,
        Suppression
    }

    public abstract class ProcStoBase<T> : IProcStoBase
        where T : class
    {
        public IService Afficher { get; }

        public ObjectResult ObjectResult { get; }

        public List<FieldProcStoDescription> Champs { get; } = new List<FieldProcStoDescription>();

        public virtual string Name { get; }

        public virtual string NameProcStoAdded { get; }

        public virtual string NameProcStoModified { get; }

        public virtual string NameProcStoDeleted { get; }

        public void Add(FieldProcStoDescription field)
        {
            Champs.Add(field);
        }

        protected ProcStoBase(IService afficher)
        {
            Afficher = afficher;
            ObjectResult = new ObjectResult();
        }

        public ObjectResult ExecuteNonQuery(T item, TypeEnregistrement typeEnregistrement = TypeEnregistrement.Defaut)
        {
            string name = null;
            IEnumerable<FieldProcStoDescription> fields = null;
            switch (typeEnregistrement)
            {
                case TypeEnregistrement.Defaut:
                    name = Name;
                    fields = Champs;
                    break;
                case TypeEnregistrement.Ajout:
                    name = NameProcStoAdded;
                    fields = Champs.Where(x => x.TypeField.HasFlag(TypeField.Ajout));
                    break;
                case TypeEnregistrement.Modifie:
                    name = NameProcStoModified;
                    fields = Champs.Where(x => x.TypeField.HasFlag(TypeField.Modification));
                    break;
                case TypeEnregistrement.Suppression:
                    name = NameProcStoDeleted;
                    fields = Champs.Where(x => x.TypeField.HasFlag(TypeField.Suppression));
                    break;
                default:
                    break;
            }

            if (name.IsNullOrEmpty())
                throw new InvalidOperationException($"La commande {typeEnregistrement} n'a pas été initialisé pr la repo {Name}");

            using (var accessCommand = new AccessCommand(ObjectResult, name, CommandType.StoredProcedure))
            {
                foreach (var current in fields)
                {
                    var pi = item.GetType().GetProperty(current.Field);
                    var value = pi.GetValue(item);
                    accessCommand.Add(current.Name, value);
                }
                accessCommand.ExecuteNonQuery();
            }

            if (!ObjectResult.IsSuccess)
                Afficher.Erreur(ObjectResult.Message);

            return ObjectResult;
        }
    }

    public abstract class ProcStoBase : IProcStoBase
    {
        public IService Afficher { get; }

        public ObjectResult ObjectResult { get; }

        public Collection<FieldProcStoDescription> Champs { get; } = new Collection<FieldProcStoDescription>();

        public abstract string Name { get; }

        public void Add(FieldProcStoDescription field)
        {
            Champs.Add(field);
        }

        protected ProcStoBase(IService afficher)
        {
            Afficher = afficher;
            ObjectResult = new ObjectResult();
        }

        public ObjectResult ExecuteNonQuery()
        {
            using (var accessCommand = new AccessCommand(ObjectResult, Name, CommandType.StoredProcedure))
            {
                foreach (var current in Champs)
                    accessCommand.Add(current.Name, current.Value);
                accessCommand.ExecuteNonQuery();
            }

            if (!ObjectResult.IsSuccess)
                Afficher.Erreur(ObjectResult.Message);

            return ObjectResult;
        }
    }
}