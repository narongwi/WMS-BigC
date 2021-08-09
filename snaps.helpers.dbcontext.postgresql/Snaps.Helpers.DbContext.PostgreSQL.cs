using System;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;
namespace Snaps.Helpers.DbContext
{
    public static class DbNpgsql
    {
        public static async Task<DbDataReader> snapsReadAsync(this NpgsqlCommand cm){
            try { 
                if (cm.Connection.State ==  ConnectionState.Closed) 
                    await cm.Connection.OpenAsync(); 
                return await cm.ExecuteReaderAsync();
            }catch (Exception ex) { 
                await cm.Connection.CloseAsync(); 
                throw ex;
            }finally { }
        }
        public static async Task<Object> snapsScalarAsync(this NpgsqlCommand cm,Boolean isAutoClose = false){ 
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
        public static async Task<String> snapsScalarStrAsync(this NpgsqlCommand cm){ 
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

        public static async Task<DataTable> snapsTableAsync(this NpgsqlCommand cm){  
            DataTable rn = new DataTable();
            try { 
                using (NpgsqlDataAdapter dp = new NpgsqlDataAdapter())
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
        public static async Task snapsExecuteAsync(this List<NpgsqlCommand> ls,Boolean isAutoClose = false){           
            foreach(NpgsqlCommand cm in ls) { 
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
        public static async Task snapsExecuteTransAsync(this List<NpgsqlCommand> ls, NpgsqlConnection cn){
            NpgsqlTransaction tx = null;
            try { 
                if (cn.State == ConnectionState.Closed) { cn.Open(); }
                tx = cn.BeginTransaction();
                foreach(NpgsqlCommand ln in ls) { 
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
        public static async Task snapsExecuteAsync(this NpgsqlCommand cm,Boolean isAutoClose = false){
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

    public static class DbNpgsqlExt {
        public static NpgsqlDbType decvalMSSQL(String vl,String vt = "Varchar") { 
            if (vt == "NVarchar") { return NpgsqlDbType.Varchar; }
            else if (vl == "System.String") { return NpgsqlDbType.Varchar; }
            else if (vl == "System.DateTime") { return NpgsqlDbType.TimestampTz; }
            else if (vl == "System.Int32") { return NpgsqlDbType.Numeric; }
            else if (vl == "System.Double") { return NpgsqlDbType.Double; }
            else if (vl == "System.Single") { return NpgsqlDbType.Numeric; }
            else { return NpgsqlDbType.Varchar; }
        }
        public static NpgsqlCommand snapsCommand(this String vl, NpgsqlConnection cn) { return new NpgsqlCommand(){ CommandText = vl, Connection = cn, CommandType = CommandType.Text }; ; }
        public static NpgsqlCommand snapsProc(this String vl, NpgsqlConnection cn) { return new NpgsqlCommand() { CommandText = vl, Connection = cn, CommandType = CommandType.StoredProcedure }; }
        public static async Task snapsSqlExc(this String vl, NpgsqlConnection cn) {  
                NpgsqlCommand cm = vl.snapsCommand(cn);
                try { await cm.snapsExecuteAsync(); }
                catch (Exception ex) { throw ex;} 
                finally { cm.Dispose(); }
        }
        public static async Task<String> snapsSqlScl(this String vl, NpgsqlConnection cn) {  
                NpgsqlCommand cm = vl.snapsCommand(cn);
                try { return await cm.snapsScalarStrAsync(); }
                catch (Exception ex) { throw ex;} 
               finally { cm.Dispose(); }
        }
        public static NpgsqlParameter snapsPar(this String vl) { return new NpgsqlParameter() { ParameterName = nameof(vl), NpgsqlDbType = NpgsqlDbType.Varchar, Direction = ParameterDirection.Input, Value = vl }; }
        public static NpgsqlParameter snapsPar(this String vl, string name) { return new NpgsqlParameter() { ParameterName = name, NpgsqlDbType = NpgsqlDbType.Varchar, Direction = ParameterDirection.Input, Value = vl }; }

        public static NpgsqlParameter snapsPar(this Int32 vl){ return new NpgsqlParameter() { ParameterName = nameof(vl), NpgsqlDbType = NpgsqlDbType.Numeric, Direction = ParameterDirection.Input, Value = vl }; }
        public static NpgsqlParameter snapsPar(this Int32 vl, string name){ return new NpgsqlParameter() { ParameterName = name, NpgsqlDbType = NpgsqlDbType.Numeric, Direction = ParameterDirection.Input, Value = vl }; }

        public static NpgsqlParameter snapsPar(this Decimal vl){ return new NpgsqlParameter() { ParameterName = nameof(vl), NpgsqlDbType = NpgsqlDbType.Numeric, Direction = ParameterDirection.Input, Value = vl }; }
        public static NpgsqlParameter snapsPar(this Decimal vl, string name){ return new NpgsqlParameter() { ParameterName = name, NpgsqlDbType = NpgsqlDbType.Numeric, Direction = ParameterDirection.Input, Value = vl }; }

        public static NpgsqlParameter snapsPar(this Double vl){ return new NpgsqlParameter() { ParameterName = nameof(vl), NpgsqlDbType = NpgsqlDbType.Numeric, Direction = ParameterDirection.Input, Value = vl }; }
        public static NpgsqlParameter snapsPar(this Double vl, string name){ return new NpgsqlParameter() { ParameterName = name, NpgsqlDbType = NpgsqlDbType.Numeric, Direction = ParameterDirection.Input, Value = vl }; }

        public static NpgsqlParameter snapsPar(this DateTime vl) { return new NpgsqlParameter() { ParameterName = nameof(vl), NpgsqlDbType = NpgsqlDbType.TimestampTz, Direction = ParameterDirection.Input, Value = vl }; }
        public static NpgsqlParameter snapsPar(this DateTime vl, string name) { return new NpgsqlParameter() { ParameterName = name, NpgsqlDbType = NpgsqlDbType.TimestampTz, Direction = ParameterDirection.Input, Value = vl }; }

        public static NpgsqlParameter snapsPar(this DateTime? vl) { return new NpgsqlParameter() { ParameterName = nameof(vl), NpgsqlDbType = NpgsqlDbType.TimestampTz, Direction = ParameterDirection.Input, Value = vl, IsNullable = true }; }
        public static NpgsqlParameter snapsPar(this DateTime? vl, string name) { return new NpgsqlParameter() { ParameterName = name, NpgsqlDbType = NpgsqlDbType.TimestampTz, Direction = ParameterDirection.Input, Value = vl, IsNullable = true  }; }
    } 
    

}
