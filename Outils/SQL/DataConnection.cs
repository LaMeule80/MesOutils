using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Outils.ObjectResultData;

namespace Outils.SQL
{
    public abstract class DataConnection<TValue, Vue, Repo> : DataConnection<TValue, Vue>
        where TValue : class
        where Vue : View
        where Repo : ProcStoBase<TValue>
    {
        private readonly ProcStoBase<TValue> _repo;

        public DataConnection(IService afficher, Vue view, Func<AccessReaderResult, TValue> factory)
            : base(afficher, view, factory) 
        {
            _repo = (ProcStoBase<TValue>)Activator.CreateInstance(typeof(Repo), Afficher);
        }

        public ObjectResult Save(TValue value)
        {
            return _repo.ExecuteNonQuery(value, TypeEnregistrement.Ajout);
        }

        public ObjectResult Update(TValue value)
        {
            return _repo.ExecuteNonQuery(value, TypeEnregistrement.Modifie);
        }

        public ObjectResult Delete(TValue value)
        {
            return _repo.ExecuteNonQuery(value, TypeEnregistrement.Suppression);
        }
    }

    public abstract class DataConnection<TValue, Vue> : DataConnection
        where TValue : class
        where Vue : View
    {
        private View _view { get; }
        private Func<AccessReaderResult, TValue> _factory { get; }

        public DataConnection(IService afficher, View view, Func<AccessReaderResult, TValue> factory)
            : base(afficher)
        {
            _view = view;
            _factory = factory;
        }

        public List<TValue> LireValues(ParametresSql parametres = null)
        {
            string requete = CreeClauseSelect(_view);
            if (parametres != null)
                requete += parametres.Clause;
            return LireValues(_factory, requete, parametres);
        }

        public TValue LireValue(ParametresSql parametres = null)
        {
            string requete = CreeClauseSelect(_view);
            if (parametres != null)
                requete += parametres.Clause;
            return LireValue(_factory, requete, parametres);
        }
    }

    public class DataConnection<TValue> : DataConnection
    {
        public DataConnection(IService afficher) : base(afficher)
        {
        }

        public static List<TValue> LireValues(
            string commandText,
            string fieldKey)
        {
            return LireValues<TValue>(commandText, fieldKey);
        }

        public static List<TValue> LireValues(
            Func<AccessReaderResult, TValue> factory,
            string commandText,
            ParametresSql parameters = null)
        {
            return LireValues<TValue>(factory, commandText, parameters);
        }

        public static TValue LireValue(
            string commandText,
            string fieldKey)
        {
            return LireValue<TValue>(commandText, fieldKey);
        }
    }

    public class DataConnection
    {
        public IService Afficher { get; }

        public DataConnection(IService afficher)
        {
            Afficher = afficher;
        }

        internal static string CreeClauseSelect(View table)
        {
            return new StringBuilder("SELECT ")
                .Append(string.Join(",", table.Champs.Select(x => x.SqlName)))
                .Append(" FROM ")
                .Append(table.SqlName)
                .ToString();
        }

        public static List<TValue> LireValues<TValue>(
            string commandText,
            string fieldKey)
        {
            var result = new ObjectResult<TValue>().Creer();

            using (var cmd = new AccessCommand(result, commandText, CommandType.Text))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Value.Add(reader.GetValue<TValue>(fieldKey));
                    }
                }
            }

            if (result.IsSuccess)
                return result.Value.ToList();
            return null;
        }

        public static List<TValue> LireValues<TValue>(
            Func<AccessReaderResult, TValue> factory,
            View table,
            ParametresSql parametres = null)
        {
            string requete = CreeClauseSelect(table);
            if (parametres != null)
                requete += parametres.Clause;
            return LireValues(factory, requete, parametres);
        }

        public static List<TValue> LireValues<TValue>(
            Func<AccessReaderResult, TValue> factory,
            string commandText,
            ParametresSql parameters = null)
        {
            var result = new ObjectResult<TValue>().Creer();

            using (var cmd = new AccessCommand(result, commandText, CommandType.Text))
            {
                if (parameters != null && parameters.Count > 0)
                    parameters.ForEach(x => cmd.Add(x.Champ, x.Value));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Value.Add(factory(reader));
                    }
                }
            }

            return result.IsSuccess ? result.Value.ToList() : null;
        }

        public static TValue LireValue<TValue>(
            string commandText,
            string fieldKey)
        {
            var result = new ObjectResult<TValue>();

            using (var cmd = new AccessCommand(result, commandText, CommandType.Text))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result.Value = reader.GetValue<TValue>(fieldKey);
                    }
                }
            }

            if (result.IsSuccess)
                return result.Value;
            return default;
        }

        public static TValue LireValue<TValue>(
            Func<AccessReaderResult, TValue> factory,
            View table,
            ParametresSql parametres = null)
        {
            string requete = CreeClauseSelect(table);
            if (parametres != null)
                requete += parametres.Clause;
            return LireValue(factory, requete, parametres);
        }

        public static TValue LireValue<TValue>(
            Func<AccessReaderResult, TValue> factory,
            string commandText,
            ParametresSql parameters = null)
        {
            ObjectResult<TValue> result = new ObjectResult<TValue>();

            using (var cmd = new AccessCommand(result, commandText, CommandType.Text))
            {
                if (parameters != null && parameters.Count > 0)
                    parameters.ForEach(x => cmd.Add(x.Champ, x.Value));

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result.Value = factory(reader);
                    }
                }
            }

            if (result.IsSuccess)
                return result.Value;
            return default;
        }
    }
}