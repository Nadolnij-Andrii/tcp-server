using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Synchronization.Data;
using System.Data;

namespace tcp_server
{
    /// <summary>
    /// Contains methods for Creating Commands for Sync Adapter 
    /// </summary>
    public class SyncAdapterCommandHelper
    {

        #region Data Commands [cards]

        /// <summary>
        /// Returns command for Selecting Incremental Changes
        /// </summary>
        /// <returns></returns>
        public static SqlCommand GetSelectIncrementalChangesCommand()
        {
            SqlCommand cmdSelectIncrementalChanges = new SqlCommand();
            cmdSelectIncrementalChanges.CommandType = CommandType.StoredProcedure;
            cmdSelectIncrementalChanges.CommandText = "sp_Account_SelectChanges";
            cmdSelectIncrementalChanges.Parameters.Add("@" + DbSyncSession.SyncMetadataOnly, SqlDbType.Int);
            cmdSelectIncrementalChanges.Parameters.Add("@" + DbSyncSession.SyncMinTimestamp, SqlDbType.BigInt);
            cmdSelectIncrementalChanges.Parameters.Add("@" + DbSyncSession.SyncInitialize, SqlDbType.Int);
            return cmdSelectIncrementalChanges;
        }

        /// <summary>
        /// Returns command for Inserting data in table
        /// </summary>
        /// <returns></returns>
        public static SqlCommand GetInsertCommand()
        {
            SqlCommand cmdInsert = new SqlCommand();
            cmdInsert.CommandType = CommandType.StoredProcedure;
            cmdInsert.CommandText = "sp_cards_ApplyInsert";
            cmdInsert.Parameters.Add("@id", SqlDbType.UniqueIdentifier);
            cmdInsert.Parameters.Add("@Name", SqlDbType.NVarChar);
            cmdInsert.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;
            return cmdInsert;
        }

        /// <summary>
        /// Returns command for Updating data in table
        /// </summary>
        /// <returns></returns>
        public static SqlCommand GetUpdateCommand()
        {
            SqlCommand cmdUpdate = new SqlCommand();
            cmdUpdate.CommandType = CommandType.StoredProcedure;
            cmdUpdate.CommandText = "sp_cards_ApplyUpdate";
            cmdUpdate.Parameters.Add("@id", SqlDbType.UniqueIdentifier);
            cmdUpdate.Parameters.Add("@Name", SqlDbType.NVarChar);
            cmdUpdate.Parameters.Add("@" + DbSyncSession.SyncMinTimestamp, SqlDbType.BigInt);
            cmdUpdate.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdUpdate.Parameters.Add("@" + DbSyncSession.SyncForceWrite, SqlDbType.Int);
            return cmdUpdate;
        }

        /// <summary>
        ///  Returns command for Deleting data in table
        /// </summary>
        /// <returns></returns>
        public static SqlCommand GetDeleteCommand()
        {
            SqlCommand cmdDelete = new SqlCommand();
            cmdDelete.CommandType = CommandType.StoredProcedure;
            cmdDelete.CommandText = "sp_cards_ApplyDelete";
            cmdDelete.Parameters.Add("@id", SqlDbType.UniqueIdentifier);
            cmdDelete.Parameters.Add("@" + DbSyncSession.SyncMinTimestamp, SqlDbType.BigInt);
            cmdDelete.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;
            return cmdDelete;
        }

        /// <summary>
        ///  Returns command for Selecting Conflict data from table
        /// </summary>
        /// <returns></returns>
        public static SqlCommand GetSelectRowCommand()
        {
            SqlCommand cmdSelectRow = new SqlCommand();
            cmdSelectRow.CommandType = CommandType.StoredProcedure;
            cmdSelectRow.CommandText = "sp_cards_SelectRow";
            cmdSelectRow.Parameters.Add("@id", SqlDbType.UniqueIdentifier);
            return cmdSelectRow;
        }

        # endregion

        #region MetaData Commands [cards_Tacking]

        /// <summary>
        /// Returns command for Inserting metadata in Change tracking table
        /// </summary>
        /// <returns></returns>
        public static SqlCommand GetInsertMetadataCommand()
        {
            SqlCommand cmdInsertMetadata = new SqlCommand();
            cmdInsertMetadata.CommandType = CommandType.StoredProcedure;
            cmdInsertMetadata.CommandText = "sp_cards_InsertMetadata";
            cmdInsertMetadata.Parameters.Add("@id", SqlDbType.UniqueIdentifier);
            cmdInsertMetadata.Parameters.Add("@" + DbSyncSession.SyncCreatePeerKey, SqlDbType.Int);
            cmdInsertMetadata.Parameters.Add("@" + DbSyncSession.SyncCreatePeerTimestamp, SqlDbType.BigInt);
            cmdInsertMetadata.Parameters.Add("@" + DbSyncSession.SyncUpdatePeerKey, SqlDbType.Int);
            cmdInsertMetadata.Parameters.Add("@" + DbSyncSession.SyncUpdatePeerTimestamp, SqlDbType.BigInt);
            cmdInsertMetadata.Parameters.Add("@" + DbSyncSession.SyncRowIsTombstone, SqlDbType.Int);
            cmdInsertMetadata.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;
            return cmdInsertMetadata;
        }

        /// <summary>
        /// Returns Command for Updating metadata in Change tracking table
        /// </summary>
        /// <returns></returns>
        public static SqlCommand GetUpdateMetadataCommand()
        {
            SqlCommand cmdUpdateMetadata = new SqlCommand();
            cmdUpdateMetadata.CommandType = CommandType.StoredProcedure;
            cmdUpdateMetadata.CommandText = "sp_cards_UpdateMetadata";
            cmdUpdateMetadata.Parameters.Add("@id", SqlDbType.UniqueIdentifier);
            cmdUpdateMetadata.Parameters.Add("@" + DbSyncSession.SyncCreatePeerKey, SqlDbType.Int);
            cmdUpdateMetadata.Parameters.Add("@" + DbSyncSession.SyncCreatePeerTimestamp, SqlDbType.BigInt);
            cmdUpdateMetadata.Parameters.Add("@" + DbSyncSession.SyncUpdatePeerKey, SqlDbType.Int);
            cmdUpdateMetadata.Parameters.Add("@" + DbSyncSession.SyncUpdatePeerTimestamp, SqlDbType.BigInt);
            cmdUpdateMetadata.Parameters.Add("@" + DbSyncSession.SyncCheckConcurrency, SqlDbType.Int);
            cmdUpdateMetadata.Parameters.Add("@" + DbSyncSession.SyncRowTimestamp, SqlDbType.BigInt);
            cmdUpdateMetadata.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;
            return cmdUpdateMetadata;
        }

        /// <summary>
        /// Returns Command for Deleting metadata in Change tracking table
        /// </summary>
        /// <returns></returns>
        public static SqlCommand GetDeleteMetadataCommand()
        {
            SqlCommand cmdDeleteMetadata = new SqlCommand();
            cmdDeleteMetadata.CommandType = CommandType.StoredProcedure;
            cmdDeleteMetadata.CommandText = "card_id";
            cmdDeleteMetadata.Parameters.Add("@id", SqlDbType.UniqueIdentifier);
            cmdDeleteMetadata.Parameters.Add("@" + DbSyncSession.SyncCheckConcurrency, SqlDbType.Int);
            cmdDeleteMetadata.Parameters.Add("@" + DbSyncSession.SyncRowTimestamp, SqlDbType.BigInt);
            cmdDeleteMetadata.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;
            return cmdDeleteMetadata;
        }

        /// <summary>
        /// Returns Command for Slecting metadata in Change tracking table than can be deleted
        /// </summary>
        /// <param name="MetadataAgingInHours">Integer specifying the amount of time to retain metadata</param>
        /// <returns></returns>
        public static SqlCommand GetSelectMetadataForCleanupCommand(int MetadataAgingInHours)
        {
            SqlCommand cmdSelectMetadataForCleanup = new SqlCommand();
            cmdSelectMetadataForCleanup.CommandType = CommandType.StoredProcedure;
            cmdSelectMetadataForCleanup.CommandText = "sp_cards_SelectMetadata";
            cmdSelectMetadataForCleanup.Parameters.Add("@metadata_aging_in_hours", SqlDbType.Int).Value = MetadataAgingInHours;
            return cmdSelectMetadataForCleanup;
        }

        # endregion
    }
}
