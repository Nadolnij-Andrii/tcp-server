using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;


namespace RemoteProvider
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IRemotePeerSyncContract" в коде и файле конфигурации.
    [ServiceContract(SessionMode = SessionMode.Required)]
    [ServiceKnownType(typeof(SyncIdFormatGroup))]
    [ServiceKnownType(typeof(DbSyncContext))]
    public interface IRemotePeerSyncContract
    {
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        void BeginSession();

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void GetKnowledge(out uint batchSize, out SyncKnowledge knowledge);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        ChangeBatch GetChanges(uint batchSize, SyncKnowledge destinationKnowledge, out object changeData);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void ApplyChanges(ConflictResolutionPolicy resolutionPolicy, ChangeBatch sourceChanges, object changeData,
            ref SyncSessionStatistics sessionStatistics);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void EndSession();
    }
}
