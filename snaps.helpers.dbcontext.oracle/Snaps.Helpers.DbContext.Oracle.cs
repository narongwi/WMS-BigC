using System;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;

namespace Snaps.Helpers.DbContext
{
    public static class DbOracle
    {
        public static async Task<DbDataReader> snapsReadAsync(this OracleCommand cm){
            try { 
                if (cm.Connection.State ==  ConnectionState.Closed) 
                    await cm.Connection.OpenAsync(); 
                return await cm.ExecuteReaderAsync();
            }catch (Exception ex) { 
                await cm.Connection.CloseAsync(); 
                throw ex;
            }finally { }
        }
        public static async Task<Object> snapsScalarAsync(this OracleCommand cm,Boolean isAutoClose = false){ 
            try { 
                if (cm.Connection.State ==  ConnectionState.Closed) 
                    await cm.Connection.OpenAsync();  
                return await cm.ExecuteScalarAsync();
            }catch (Exception ex) { 
                 cm.Connection.Close(); 
                throw ex;
            }finally { 
                if (isAutoClose == true ){ await cm.Connection.CloseAsync(); }
            }
        }
        public static async Task<String> snapsScalarStrAsync(this OracleCommand cm){ 
            String rn = "";
            try { 
                if (cm.Connection.State ==  ConnectionState.Closed) 
                    await cm.Connection.OpenAsync(); 
                rn = cm.ExecuteScalarAsync().GetAwaiter().GetResult().ToString();
                return rn;
            }catch (Exception ex) { 
                 cm.Connection.Close(); 
                throw ex;
            }finally { 
                await cm.Connection.CloseAsync(); 
            }
        }

        public static async Task<DataTable> snapsTableAsync(this OracleCommand cm){  
            DataTable rn = new DataTable();
            try { 
                using (OracleDataAdapter dp = new OracleDataAdapter())
                {
                    dp.SelectCommand = cm;
                    dp.Fill(rn);
                }
                await cm.Connection.CloseAsync(); 
                return rn;
            }catch (Exception ex) { 
                 await cm.Connection.CloseAsync(); 
                throw ex;
            }
        }
        public static async Task snapsExecuteAsync(this List<OracleCommand> ls,Boolean isAutoClose = false){           
            foreach(OracleCommand cm in ls) { 
                try { 
                    if (cm.Connection.State ==  ConnectionState.Closed) 
                        await cm.Connection.OpenAsync();                    
                    await cm.ExecuteNonQueryAsync();
                    await cm.Connection.CloseAsync(); 
                }catch (Exception ex) { 
                    await cm.Connection.CloseAsync();  throw ex;
                }
            }           
        }
        public static async Task snapsExecuteTransAsync(this List<OracleCommand> ls, OracleConnection cn){
            OracleTransaction tx = null;
            try { 
                if (cn.State == ConnectionState.Closed) { cn.Open(); }
                tx = cn.BeginTransaction();
                foreach(OracleCommand ln in ls) { 
                    ln.Connection = cn;
                    await ln.ExecuteNonQueryAsync();
                }
                await tx.CommitAsync();
                await cn.CloseAsync();
            }catch (Exception ex) {
                await tx.RollbackAsync();
                await cn.CloseAsync();
                throw ex;
            }finally  { 
                if (tx!=null) { await tx.DisposeAsync();}
                ls.ForEach(c => c.Dispose()); 
            }
        }
        public static async Task snapsExecuteAsync(this OracleCommand cm,Boolean isAutoClose = false){
            try { 
                if (cm.Connection.State ==  ConnectionState.Closed) 
                    await cm.Connection.OpenAsync();                    
                await cm.ExecuteNonQueryAsync();
               await cm.Connection.CloseAsync(); 
            }catch (Exception ex) { 
               await cm.Connection.CloseAsync(); throw ex;
            }
        }
    }

    public static class DbOracleExt {
        public static OracleCommand snapsCommand(this String vl, OracleConnection cn) { return new OracleCommand(){ CommandText = vl, Connection = cn, BindByName = true, CommandType = CommandType.Text }; }
        public static OracleCommand snapsProc(this String vl, OracleConnection cn) { return new OracleCommand() { CommandText = vl, Connection = cn, BindByName = true, CommandType = CommandType.StoredProcedure }; }
        public static async Task snapsSqlExc(this String vl, OracleConnection cn) {  
                OracleCommand cm = vl.snapsCommand(cn);
                try { await cm.snapsExecuteAsync(); }
                catch (Exception ex) { throw ex;} 
                finally { await cm.DisposeAsync();}
        }
        public static async Task<String> snapsSqlScl(this String vl, OracleConnection cn) {  
                OracleCommand cm = vl.snapsCommand(cn);
                try { return await cm.snapsScalarStrAsync(); }
                catch (Exception ex) { throw ex;} 
                finally { await cm.DisposeAsync();}
        }
        public static OracleParameter snapsPar(this String vl, string name) { return new OracleParameter(name,vl); }
        public static OracleParameter snapsPar(this Int32 vl, string name){ return new OracleParameter(name,OracleDbType.Int32,vl,ParameterDirection.Input); }
        public static OracleParameter snapsPar(this Double vl, string name){ return new OracleParameter(name,OracleDbType.Double,vl,ParameterDirection.Input); }
        public static OracleParameter snapsPar(this DateTime vl, string name) {  return new OracleParameter(name,OracleDbType.Date,vl,ParameterDirection.Input); }
    } 

}
