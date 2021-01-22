using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Outils.Helper.EnumTools;

namespace Outils.ObjectResultData
{
    public class ObjectResult<T> : ObjectResult
    {
        public T Value { get; set; }

        public ObjectResult<List<T>> Creer()
        {
            return new ObjectResult<List<T>>
            {
                Value = new List<T>()
            };
        }
    }

    public class ObjectResult : List<ObjectResultItem>
    {
        public bool IsSuccess => !this.Any();

        public string Message
        {
            get
            {
                if (this.Count == 0)
                    return string.Empty;

                List<string> result = new List<string>();

                var erreurs = CreerMessage(Level.Erreur);
                if (!string.IsNullOrEmpty(erreurs))
                    result.Add(erreurs);

                var warnings = CreerMessage(Level.Attention);
                if (!string.IsNullOrEmpty(warnings))
                    result.Add(warnings);

                var infos = CreerMessage(Level.Information);
                if (!string.IsNullOrEmpty(infos))
                    result.Add(infos);

                return string.Join(Environment.NewLine, result);
            }
        }

        private string CreerMessage(Level level)
        {
            var items = this.Where(x => x.Niveau == level).ToList();
            if (!items.Any())
                return string.Empty;
            List<string> result = new List<string>
            {
                $"Une ou plusieurs {EnumHelper.GetValue<string>(level)} ont été levées : "
            };
            foreach (var item in items)
                result.Add($"- {item.Message}");
            return string.Join(Environment.NewLine, result);
        }

        private void Add(string message, Level level)
        {
            Add(new ObjectResultItem() { Message = message, Niveau = level});
        }

        public void AddError(string message)
        {
            Add(message, Level.Erreur);
        }

        public void AddWarning(string message)
        {
            Add(message, Level.Attention);
        }

        public void AddInformation(string message)
        {
            Add(message, Level.Information);
        }

        public void AddSqlException(SqlException sqlException)
        {
            if (sqlException.Class <= 16)
                AddInformation(sqlException.Message);
            else
            {
                var stackTrace = new StackTrace();
                var method = stackTrace.GetFrame(2).GetMethod();
                var stringBuilder = new StringBuilder($"{method.ReflectedType?.Name}.{method.Name} : Une erreur SQL a été levée.");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append("Informations complémentaires :");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append(sqlException.Message);
                AddError(stringBuilder.ToString());
            }
        }

        public void Merge(ObjectResult objectResult)
        {
            AddRange(objectResult);
        }
    }
}