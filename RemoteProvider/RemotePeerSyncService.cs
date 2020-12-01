using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using tcp_server;

namespace RemoteProvider
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "RemotePeerSyncContract" в коде и файле конфигурации.

    //[ServiceKnownType(typeof(SyncIdFormatGroup))]
    //[ServiceKnownType(typeof(DbSyncContext))]
    //[ServiceContract(SessionMode = SessionMode.Required)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RemotePeerSyncService : IRemotePeerSyncContract
    {
        private DbSyncProvider remoteSyncProvider = null;

        //[OperationContract(IsInitiating = true, IsTerminating = false)]
        public void BeginSession()
        {
            remoteSyncProvider = SyncProviderHelper.CreateSyncProvider(3);//@:Peer2
        }
        //[OperationContract(IsInitiating = false, IsTerminating = false)]
        public void GetKnowledge(
            out uint batchSize,
            out SyncKnowledge knowledge)
        {
            remoteSyncProvider.GetSyncBatchParameters(out batchSize, out knowledge);
        }
        //[OperationContract(IsInitiating = false, IsTerminating = false)]
        public ChangeBatch GetChanges(
            uint batchSize,
            SyncKnowledge destinationKnowledge,
            out object changeData)
        {
            return remoteSyncProvider.GetChangeBatch(batchSize, destinationKnowledge, out changeData);
        }
        //[OperationContract(IsInitiating = false, IsTerminating = false)]
        public void ApplyChanges(
            ConflictResolutionPolicy resolutionPolicy,
            ChangeBatch sourceChanges,
            object changeData,
            ref SyncSessionStatistics sessionStatistics)
        {
            SyncCallbacks syncCallback = new SyncCallbacks();
            remoteSyncProvider.ProcessChangeBatch(resolutionPolicy, sourceChanges, changeData, syncCallback,
                sessionStatistics);
        }
        //[OperationContract(IsInitiating = false, IsTerminating = false)]
        public void EndSession()
        {
            remoteSyncProvider = null;
        }
    }


}
