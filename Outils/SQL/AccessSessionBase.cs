using Outils.ObjectResultData;
using System.Data;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Outils.SQL
{
    internal class AccessSessionBase : DisposableBase
    {
        private readonly TransactionScope _transactionScope;

        internal AccessSessionBase(ObjectResult result)
        {
            _transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            });
            ObjectResult = result;
        }
        
        private ObjectResult ObjectResult { get; }

        internal AccessCommand CreateSqlCommand(string requete)
        {
            return new AccessCommand(ObjectResult, requete, CommandType.Text);
        }

        internal void ExecuteNonQuery(string requete, CommandType commandType, ParametresSQL parametres)
        {
            using (var accessCommand = new AccessCommand(ObjectResult, requete, commandType))
            {
                if (parametres != null && parametres.Count > 0)
                    foreach (var current in parametres)
                        accessCommand.SetParam(current.Champ, current.Value);
                accessCommand.ExecuteNonQuery();
            }
        }

        protected override void DisposeManagedResources()
        {
            if (ObjectResult.IsSuccess)
                _transactionScope.Complete();
            _transactionScope.Dispose();
            base.DisposeManagedResources();
        }
    }
}