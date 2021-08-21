using Snaps.Helpers.DbContext.SQLServer;
using Snaps.Helpers.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Snaps.WMS{
    public partial class mergehu_ops : IDisposable {
        private readonly ISnapsLogger logger;
        private SqlConnection cn = null;
        private readonly string cs;
        private bool disposedValue;

        public mergehu_ops() { }
        public mergehu_ops(String cx) {
            cn = new SqlConnection(cx);
            // logger
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            logger = snapsLogFactory.Create<correction_ops>();
        }

        public async Task<merge_md> Setup(merge_set os) {
            merge_set ms = new merge_set();
            merge_md md = new merge_md();
            try {
                // generate hu
                using(var con = new SqlConnection(cs)) {
                    con.Open();
                    using(var cm = new SqlCommand("[dbo].[snaps_stocktake_genhuno]",con)) {
                        cm.CommandType = CommandType.Text;
                        cm.Parameters.AddWithValue("@orgcode",os.orgcode);
                        cm.Parameters.AddWithValue("@site",os.site);
                        cm.Parameters.AddWithValue("@depot",os.depot);
                        cm.Parameters.AddWithValue("@loccode",os.loccode);
                        cm.Parameters.Add(new SqlParameter("@cnhuno",SqlDbType.VarChar) { Direction = ParameterDirection.Output });
                        cm.Parameters.Add(new SqlParameter("@hutype",SqlDbType.VarChar) { Direction = ParameterDirection.Output });
                        cm.Parameters.Add(new SqlParameter("@spcarea",SqlDbType.VarChar) { Direction = ParameterDirection.Output });
                        await cm.ExecuteNonQueryAsync();
                        os.spcarea = (string)cm.Parameters["@spcarea"].Value;
                        os.hutype = (string)cm.Parameters["@hutype"].Value;
                        os.huno = (string)cm.Parameters["@cnhuno"].Value;
                        md.orgcode = os.orgcode;
                        md.site = os.site;
                        md.depot = os.depot;
                        md.spcarea = os.spcarea;
                        md.hutype = os.hutype;
                        md.hutarget =  os.huno;
                        md.loccode = os.huno;
                        md.tflow = "IO";
                        md.datecreate = DateTime.Now;
                        md.accncreate = os.accncode;
                        md.datemodify = DateTime.Now;
                        md.accnmodify = os.accncode;
                        md.procmodify = "wms.mergesetup";
                        md.remarks = "wms.mergesetup";
                        md.lines = new List<mergehu_ln>();
                        cm.Parameters.Clear();
                        cm.CommandText = sqlmergehu_insert;
                        cm.CommandType = CommandType.Text;
                        cm.Parameters.AddWithValue("@orgcode",os.orgcode);
                        cm.Parameters.AddWithValue("@site",os.site);
                        cm.Parameters.AddWithValue("@depot",os.depot);
                        cm.Parameters.AddWithValue("@spcarea",os.spcarea);
                        cm.Parameters.AddWithValue("@mergeno",os.mergeno);
                        cm.Parameters.AddWithValue("@hutype",os.hutype);
                        cm.Parameters.AddWithValue("@hutarget",os.huno);
                        cm.Parameters.AddWithValue("@loccode",os.loccode);
                        cm.Parameters.AddWithValue("@tflow","IO");
                        cm.Parameters.AddWithValue("@datecreate",DateTime.Now);
                        cm.Parameters.AddWithValue("@accncreate",os.accncode);
                        cm.Parameters.AddWithValue("@datemodify",DateTime.Now);
                        cm.Parameters.AddWithValue("@accnmodify",os.accncode);
                        cm.Parameters.AddWithValue("@procmodify","wms.mergesetup");
                        cm.Parameters.AddWithValue("@remarks","wms.mergesetup");
                        md.mergeno = (int)await cm.ExecuteScalarAsync();
                    }
                }

                return md;
            } catch(Exception ex) {
                logger.Error(os.orgcode,os.site,os.accncode,ex,ex.Message);
                throw ex;
            } 
         
        }
        public merge_item Item(merge_item model) {
            return new merge_item();
        }


        public mergehu_md List(merge_set model) {
            return new mergehu_md();
        }

        public mergehu_md Merge(merge_md model) {
            return new mergehu_md();
        }
        public mergehu_md Remove(mergehu_ln model) {
            return new mergehu_md();
        }

        protected virtual void Dispose(bool disposing) {
            if(!disposedValue) {
                if(disposing) {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~mergehu_ops()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
