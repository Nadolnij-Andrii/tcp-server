using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;

namespace WebSyncContract
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IService1" в коде и файле конфигурации.
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ISqlSyncContract : IRelationalSyncContract
    {
        [OperationContract]
        [FaultContract(typeof(WebSyncFaultException))]
        void CreateScopeDescription(DbSyncScopeDescription scopeDescription);

        [OperationContract]
        [FaultContract(typeof(WebSyncFaultException))]
        DbSyncScopeDescription GetScopeDescription();

        [OperationContract]
        [FaultContract(typeof(WebSyncFaultException))]
        bool NeedsScope();
    }
}
