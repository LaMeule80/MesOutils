using Outils.ObjectResultData;

namespace Outils.SQL
{
    public class AccessResultBase : DisposableBase
    {
        private readonly ObjectResult _result;

        public AccessResultBase(ObjectResult result)
        {
            _result = result;
        }

        internal bool IsSuccess => _result.IsSuccess;
    }
}