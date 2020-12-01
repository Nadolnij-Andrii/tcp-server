using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace tcp_server
{
    /// <summary>
    /// Contains Static method to Populate controls and Handle Data operations for base table[Account]
    /// </summary>
    public class DBHelper
    {
        /// <summary>
        /// Gets the connection string for peer
        /// </summary>
        /// <param name="peerID"></param>
        /// <returns></returns>
        public static string GetPeerConncetionStringByPeerID(int peerID)
        {
            switch (peerID)
            {
                case 1:
                    return "Data Source=DESKTOP-JS52HPM\\SQLEXPRESS;Initial Catalog=dbo;User ID=sa;Password=123456;" ;                   
                case 2:
                    return "Data Source=DESKTOP-JS52HPM\\SQLEXPRESS;Initial Catalog=dbo1;User ID=sa;Password=123456;";                 
                case 3:
                    return "Data Source=DESKTOP-JS52HPM\\SQLEXPRESS;Initial Catalog=dbo2;User ID=sa;Password=123456;";                    
                default:
                    return string.Empty;                  
            }
        }

        /// <summary>
        /// Returns a datatable containing Peers
        /// </summary>
        /// <returns></returns>
        public static DataTable GetPeers()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PeerID", typeof(int));
            dt.Columns.Add("PeerName", typeof(string));
            DataRow dr = dt.NewRow();
            dr["PeerID"] = 1;
            dr["PeerName"] = "Peer1";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["PeerID"] = 2;
            dr["PeerName"] = "Peer2";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["PeerID"] = 3;
            dr["PeerName"] = "Peer3";
            dt.Rows.Add(dr);
            return dt;
        }

       
    }
}
