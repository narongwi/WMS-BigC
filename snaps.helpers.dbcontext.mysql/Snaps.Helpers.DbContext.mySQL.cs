using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;

namespace Snaps.Helpers.DbContext
{

    public static class DbMySQL
    {
        public static async Task<DbDataReader> snapsReadAsync(this MySqlCommand cm){
            try { 
                if (cm.Connection.State ==  ConnectionState.Closed) 
                    await cm.Connection.OpenAsync(); 
                return await cm.ExecuteReaderAsync();
            }catch (Exception ex) { 
                await cm.Connection.CloseAsync(); 
                throw ex;
            }finally { }
        }
        public static async Task<Object> snapsScalarAsync(this MySqlCommand cm,Boolean isAutoClose = false){ 
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
        public static async Task<String> snapsScalarStrAsync(this MySqlCommand cm){ 
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

        public static async Task<DataTable> snapsTableAsync(this MySqlCommand cm){  
            DataTable rn = new DataTable();
            try { 
                using (MySqlDataAdapter dp = new MySqlDataAdapter())
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
        public static async Task snapsExecuteAsync(this List<MySqlCommand> ls,Boolean isAutoClose = false){           
            foreach(MySqlCommand cm in ls) { 
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
        public static async Task snapsExecuteTransAsync(this List<MySqlCommand> ls, MySqlConnection cn){
            MySqlTransaction tx = null;
            try { 
                if (cn.State == ConnectionState.Closed) { cn.Open(); }
                tx = cn.BeginTransaction();
                foreach(MySqlCommand ln in ls) { 
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
        public static async Task snapsExecuteAsync(this MySqlCommand cm,Boolean isAutoClose = false){
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

    public static class DbMySQLExt {
        public static MySqlDbType decvalMSSQL(String vl,String vt = "Varchar") { 
            if (vt == "NVarchar") { return MySqlDbType.VarChar; }
            else if (vl == "System.String") { return MySqlDbType.VarChar; }
            else if (vl == "System.DateTime") { return MySqlDbType.Timestamp; }
            else if (vl == "System.Int32") { return MySqlDbType.Int32; }
            else if (vl == "System.Double") { return MySqlDbType.Float; }
            else if (vl == "System.Single") { return MySqlDbType.Float; }
            else { return MySqlDbType.VarChar; }
        }
        public static MySqlCommand snapsCommand(this String vl, MySqlConnection cn) { return new MySqlCommand(){ CommandText = vl, Connection = cn, CommandType = CommandType.Text }; ; }
        public static MySqlCommand snapsProc(this String vl, MySqlConnection cn) { return new MySqlCommand() { CommandText = vl, Connection = cn, CommandType = CommandType.StoredProcedure }; }
        public static async Task snapsSqlExc(this String vl, MySqlConnection cn) {  
                MySqlCommand cm = vl.snapsCommand(cn);
                try { await cm.snapsExecuteAsync(); }
                catch (Exception ex) { throw ex;} 
                finally { await cm.DisposeAsync();}
        }
        public static async Task<String> snapsSqlScl(this String vl, MySqlConnection cn) {  
                MySqlCommand cm = vl.snapsCommand(cn);
                try { return await cm.snapsScalarStrAsync(); }
                catch (Exception ex) { throw ex;} 
                finally { await cm.DisposeAsync();}
        }
        public static MySqlParameter snapsPar(this String vl) { return new MySqlParameter() { ParameterName = nameof(vl), MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Input, Value = vl }; }
        public static MySqlParameter snapsPar(this String vl, string name) { return new MySqlParameter() { ParameterName = name, MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Input, Value = vl }; }

        public static MySqlParameter snapsPar(this Int32 vl){ return new MySqlParameter() { ParameterName = nameof(vl), MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Input, Value = vl }; }
        public static MySqlParameter snapsPar(this Int32 vl, string name){ return new MySqlParameter() { ParameterName = name, MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Input, Value = vl }; }

        public static MySqlParameter snapsPar(this Decimal vl){ return new MySqlParameter() { ParameterName = nameof(vl), MySqlDbType = MySqlDbType.Decimal, Direction = ParameterDirection.Input, Value = vl }; }
        public static MySqlParameter snapsPar(this Decimal vl, string name){ return new MySqlParameter() { ParameterName = name, MySqlDbType = MySqlDbType.Decimal, Direction = ParameterDirection.Input, Value = vl }; }

        public static MySqlParameter snapsPar(this Double vl){ return new MySqlParameter() { ParameterName = nameof(vl), MySqlDbType = MySqlDbType.Float, Direction = ParameterDirection.Input, Value = vl }; }
        public static MySqlParameter snapsPar(this Double vl, string name){ return new MySqlParameter() { ParameterName = name, MySqlDbType = MySqlDbType.Float, Direction = ParameterDirection.Input, Value = vl }; }

        public static MySqlParameter snapsPar(this DateTime vl) { return new MySqlParameter() { ParameterName = nameof(vl), MySqlDbType = MySqlDbType.DateTime, Direction = ParameterDirection.Input, Value = vl }; }
        public static MySqlParameter snapsPar(this DateTime vl, string name) { return new MySqlParameter() { ParameterName = name, MySqlDbType = MySqlDbType.DateTime, Direction = ParameterDirection.Input, Value = vl }; }

        public static MySqlParameter snapsPar(this DateTime? vl) { return new MySqlParameter() { ParameterName = nameof(vl), MySqlDbType = MySqlDbType.DateTime, Direction = ParameterDirection.Input, Value = vl, IsNullable = true }; }
        public static MySqlParameter snapsPar(this DateTime? vl, string name) { return new MySqlParameter() { ParameterName = name, MySqlDbType = MySqlDbType.DateTime, Direction = ParameterDirection.Input, Value = vl, IsNullable = true  }; }
    } 
    

}
