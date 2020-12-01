using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;

namespace tcp_server
{
    /// <summary>
    /// Contains Static Method to Create Sync Adapter
    /// </summary>
    public class SyncAdapterHelper
    {
        public static DbSyncAdapter CreateSyncAdpters()
        {

            DbSyncAdapter syncAdapter = new DbSyncAdapter("cards");
            syncAdapter.RowIdColumns.Add("card_id");


            syncAdapter.SelectIncrementalChangesCommand = SyncAdapterCommandHelper.GetSelectIncrementalChangesCommand();
            syncAdapter.InsertCommand = SyncAdapterCommandHelper.GetInsertCommand();
            syncAdapter.UpdateCommand = SyncAdapterCommandHelper.GetUpdateCommand();
            syncAdapter.DeleteCommand = SyncAdapterCommandHelper.GetDeleteCommand();
            syncAdapter.SelectRowCommand = SyncAdapterCommandHelper.GetSelectRowCommand();

            syncAdapter.InsertMetadataCommand = SyncAdapterCommandHelper.GetInsertMetadataCommand();
            syncAdapter.UpdateMetadataCommand = SyncAdapterCommandHelper.GetUpdateMetadataCommand();
            syncAdapter.DeleteMetadataCommand = SyncAdapterCommandHelper.GetDeleteMetadataCommand();
            syncAdapter.SelectMetadataForCleanupCommand = SyncAdapterCommandHelper.GetSelectMetadataForCleanupCommand(100);

            return syncAdapter;
        }

    }
}
