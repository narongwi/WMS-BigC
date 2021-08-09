/*  */using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;
namespace Snaps.Helpers.DbContext.SQLServer {
    public enum dbOperator {
        equal,
        morethan,
        morethanequal,
        lessthan,
        lessthanequal,
        like,
        likeStart,
        likeEnd,
        findin
    }
    public static class DbSqlServer {
        public static int numAffectedRows { get; set; }

        public static async Task<DbDataReader> snapsReadDbAsync(this SqlCommand cm) {
            try {
                if(cm.Connection.State == ConnectionState.Closed)
                    await cm.Connection.OpenAsync();
                return await cm.ExecuteReaderAsync();
            } catch(Exception ex) {
                await cm.Connection.CloseAsync();
                throw ex;
            } finally { }
        }
        public static async Task<SqlDataReader> snapsReadAsync(this SqlCommand cm) {
            try {
                SqlDataReader rn;
                if(cm.Connection.State == ConnectionState.Closed)
                    await cm.Connection.OpenAsync();
                rn = await cm.ExecuteReaderAsync();
                return rn;
            } catch(Exception ex) {
                await cm.Connection.CloseAsync();
                throw ex;
            } finally { }
        }
        public static async Task<Object> snapsScalarAsync(this SqlCommand cm,Boolean isAutoClose = false) {
            try {
                if(cm.Connection.State == ConnectionState.Closed)
                    await cm.Connection.OpenAsync();
                return await cm.ExecuteScalarAsync();
            } catch(Exception ex) {
                cm.Connection.Close();
                throw ex;
            } finally {
                if(isAutoClose == true) { await cm.Connection.CloseAsync(); }
            }
        }
        public static async Task<String> snapsScalarStrAsync(this SqlCommand cm) {
            String rn = "";
            try {
                if(cm.Connection.State == ConnectionState.Closed)
                    await cm.Connection.OpenAsync();
                rn = cm.ExecuteScalarAsync().GetAwaiter().GetResult().ToString();
                return rn;
            } catch(Exception ex) {
                cm.Connection.Close();
                throw ex;
            } finally {
                await cm.Connection.CloseAsync();
            }
        }
        public static async Task<DataTable> snapsTableAsync(this SqlCommand cm) {
            DataTable rn = new DataTable();
            try {
                using(SqlDataAdapter dp = new SqlDataAdapter()) {
                    dp.SelectCommand = cm;
                    dp.Fill(rn);
                }
                await cm.Connection.CloseAsync();
                return rn;
            } catch(Exception ex) {
                await cm.Connection.CloseAsync();
                throw ex;
            }
        }
        public static async Task snapsExecuteAsync(this List<SqlCommand> ls,Boolean isAutoClose = false) {
            foreach(SqlCommand cm in ls) {
                try {
                    if(cm.Connection.State == ConnectionState.Closed)
                        await cm.Connection.OpenAsync();
                    await cm.ExecuteNonQueryAsync();
                    await cm.Connection.CloseAsync();
                } catch(Exception ex) {
                    await cm.Connection.CloseAsync(); throw ex;
                }
            }
        }
        public static void snapsExecute(this List<SqlCommand> ls,Boolean isAutoClose = false) {
            foreach(SqlCommand cm in ls) {
                try {
                    if(cm.Connection.State == ConnectionState.Closed) { cm.Connection.Open(); }
                    cm.ExecuteNonQuery();
                    cm.Connection.Close();
                } catch(Exception ex) {
                    cm.Connection.Close(); throw ex;
                }
            }
        }

        public static void snapsExecuteTrans(this List<SqlCommand> ls,SqlConnection cn,ref String errorsql,Boolean isTrace = false) {
            SqlTransaction tx = null;
            Int32 rx = 0;
            try {
                if(cn.State == ConnectionState.Closed) { cn.Open(); }
                tx = cn.BeginTransaction();
                foreach(SqlCommand ln in ls) {
                    if(isTrace == true) { ln.Parameters.Add(DateTimeOffset.Now.snapsPar("startdate")); errorsql = ln.CommandText; }
                    ln.Connection = cn;
                    ln.Transaction = tx;
                    ln.ExecuteNonQuery();
                    if(isTrace == true) { ln.Parameters.Add(DateTimeOffset.Now.snapsPar("enddate")); }
                    rx++;
                }
                tx.Commit();
                cn.Close();
            } catch(Exception ex) {

                tx.Rollback();
                cn.Close();
                throw ex;
            } finally {
                if(tx != null) { tx.Dispose(); }
                ls.ForEach(c => c.Dispose());
            }
        }
        public static async Task snapsExecuteTransAsync(this List<SqlCommand> ls,SqlConnection cn,Boolean isTrace = false,String errorsql = "") {
            SqlTransaction tx = null;
            Int32 rx = 0;
            try {
                if(cn.State == ConnectionState.Closed) {

                    await cn.OpenAsync();

                }
                tx = cn.BeginTransaction();
                foreach(SqlCommand ln in ls) {
                    if(isTrace == true) { ln.Parameters.Add(DateTimeOffset.Now.snapsPar("startdate")); errorsql = ln.CommandText; }
                    ln.Connection = cn;
                    ln.Transaction = tx;
                    try {
                        await ln.ExecuteNonQueryAsync();
                    } catch(Exception exn) {
                        throw exn;
                    }
                    if(isTrace == true) { ln.Parameters.Add(DateTimeOffset.Now.snapsPar("enddate")); }
                    rx++;
                }
                await tx.CommitAsync();
                await cn.CloseAsync();
            } catch(Exception ex) {

                await tx.RollbackAsync();
                await cn.CloseAsync();
                throw ex;
            } finally {
                if(tx != null) { await tx.DisposeAsync(); }
                ls.ForEach(c => c.Dispose());
            }
        }

        public static KeyValuePair<SqlCommand,string> snapsKeyValue(this SqlCommand sqlCommand,string message) {
            return new KeyValuePair<SqlCommand,string>(sqlCommand,message);
        }
        public static async Task<List<KeyValuePair<string,int>>> snapsExecuteTransAsync(List<KeyValuePair<SqlCommand,string>> ls,SqlConnection cn) {
            SqlTransaction tx = null;
            var trace = new List<KeyValuePair<string,int>>();
            Int32 rx = 0;
            try {
                if(cn.State == ConnectionState.Closed) {
                    await cn.OpenAsync();
                }
                tx = cn.BeginTransaction();
                foreach(KeyValuePair<SqlCommand,string> ln in ls) {
                    ln.Key.Connection = cn;
                    ln.Key.Transaction = tx;
                    try {
                        int affectedrow = await ln.Key.ExecuteNonQueryAsync();
                        trace.Add(new KeyValuePair<string,int>(ln.Value,affectedrow));
                    } catch(Exception exn) {
                        throw new Exception("Error " + ln.Value + "index:" + rx + " => " + exn.Message);
                    }
                    rx++;
                }
                await tx.CommitAsync();
                await cn.CloseAsync();
                return trace;
            } catch(Exception ex) {

                await tx.RollbackAsync();
                await cn.CloseAsync();
                throw ex;
            } finally {
                if(tx != null) { await tx.DisposeAsync(); }
                ls.ForEach(c => c.Key.Dispose());
            }
        }
        public static async Task<snapsCmdTag> snapsExecuteTagsAsync(this snapsCmdTag tags,SqlConnection cn) {
            SqlTransaction tx = null;
            snapsCmdTag cmdtags = tags;
            try {
                if(cn.State == ConnectionState.Closed) {
                    await cn.OpenAsync();
                }
                tx = cn.BeginTransaction();
                for(int i = 0 ; i < cmdtags.cmdTags.Count ; i++) {
                    if(cmdtags.cmdTags[i].isCommand) {
                        cmdtags.cmdTags[i].sqlCommand.Connection = cn;
                        cmdtags.cmdTags[i].sqlCommand.Transaction = tx;
                        try {
                            cmdtags.cmdTags[i].affectedRows = await cmdtags.cmdTags[i].sqlCommand.ExecuteNonQueryAsync();
                        } catch(Exception exn) {
                            throw new Exception("Error " + cmdtags.Gettag(i) + exn.Message);
                        }
                    }
                }
              
                await tx.CommitAsync();
                await cn.CloseAsync();
                return cmdtags;
            } catch(Exception ex) {
                await tx.RollbackAsync();
                await cn.CloseAsync();
                throw ex;
            } finally {
                if(tx != null) { await tx.DisposeAsync(); }
                foreach(snapsCmdTags tg in cmdtags.cmdTags) { if(tg.isCommand) { tg.sqlCommand.Dispose(); } }
            }
        }

        public static void snapsExecuteTrans(this List<SqlCommand> ls,SqlConnection cn,Boolean isTrace = false,String errorsql = "") {
            SqlTransaction tx = null;
            Int32 rx = 0;
            try {
                if(cn.State == ConnectionState.Closed) {
                    cn.Open();
                }
                tx = cn.BeginTransaction();
                foreach(SqlCommand ln in ls) {
                    if(isTrace == true) { ln.Parameters.Add(DateTimeOffset.Now.snapsPar("startdate")); errorsql = ln.CommandText; }
                    ln.Connection = cn;
                    ln.Transaction = tx;
                    ln.ExecuteNonQuery();
                    if(isTrace == true) { ln.Parameters.Add(DateTimeOffset.Now.snapsPar("enddate")); }
                    rx++;
                }
                tx.Commit();
                cn.Close();
            } catch(Exception ex) {

                tx.Rollback();
                cn.Close();
                throw ex;
            } finally {
                if(tx != null) { tx.Dispose(); }
                ls.ForEach(c => c.Dispose());
            }
        }
        public static async Task snapsExecuteAsync(this SqlCommand cm,Boolean isAutoClose = false) {
            try {
                if(cm.Connection.State == ConnectionState.Closed)
                    await cm.Connection.OpenAsync();
                numAffectedRows = 0;
                numAffectedRows = await cm.ExecuteNonQueryAsync();
                await cm.Connection.CloseAsync();
            } catch(Exception ex) {
                await cm.Connection.CloseAsync(); throw ex;
            }
        }

        /// <summary>
        /// return number Affected Rows insert update delete
        /// </summary>
        /// <returns>numAffectedRows</returns>
        public static async Task<int> snapsExecuteAffectedRowsAsync(this SqlCommand cm) {
            try {
                if(cm.Connection.State == ConnectionState.Closed)
                    await cm.Connection.OpenAsync();
                int numAffectedRows = await cm.ExecuteNonQueryAsync();
                await cm.Connection.CloseAsync();
                return numAffectedRows;
            } catch(Exception ex) {
                await cm.Connection.CloseAsync(); throw ex;
            }
        }

        /// <summary>
        /// return number Affected Rows insert update delete
        /// </summary>
        /// <returns>numAffectedRows</returns>
        public static int snapsExecuteAffectedRows(this SqlCommand cm) {
            try {
                if(cm.Connection.State == ConnectionState.Closed)
                    cm.Connection.OpenAsync();
                int numAffectedRows = cm.ExecuteNonQuery();
                cm.Connection.CloseAsync();
                return numAffectedRows;
            } catch(Exception ex) {
                cm.Connection.CloseAsync();
                throw ex;
            }
        }
        public static void snapsExecute(this SqlCommand cm,Boolean isAutoClose = false) {
            try {
                if(cm.Connection.State == ConnectionState.Closed) { cm.Connection.Open(); }
                cm.ExecuteNonQuery();
                cm.Connection.Close();
            } catch(Exception ex) {
                cm.Connection.Close(); throw ex;
            }
        }

        public static void snapsExecuteTransRef(this List<SqlCommand> ls,ref SqlConnection cn,ref SqlTransaction tx,ref String errorsql,Boolean isTrace = false) {
            Int32 rx = 0;
            try {
                if(cn.State == ConnectionState.Closed) { cn.Open(); }
                tx = cn.BeginTransaction();
                foreach(SqlCommand ln in ls) {
                    if(isTrace == true) { ln.Parameters.Add(DateTimeOffset.Now.snapsPar("startdate")); errorsql = ln.CommandText; }
                    ln.Connection = cn;
                    ln.Transaction = tx;
                    ln.ExecuteNonQuery();
                    if(isTrace == true) { ln.Parameters.Add(DateTimeOffset.Now.snapsPar("enddate")); }
                    rx++;
                }
            } catch(Exception ex) {
                tx.Rollback();
                cn.Close();
                throw ex;
            } finally {
                ls.ForEach(c => c.Dispose());
            }
        }
    }

    public static class DbOracleExt {
        public static SqlDbType decvalMSSQL(String vl,String vt = "Varchar") {
            if(vt == "NVarchar") { return SqlDbType.NVarChar; } else if(vl == "System.String") { return SqlDbType.VarChar; } else if(vl == "System.DateTime") { return SqlDbType.DateTimeOffset; } else if(vl == "System.Int32") { return SqlDbType.Int; } else if(vl == "System.Double") { return SqlDbType.Float; } else if(vl == "System.Single") { return SqlDbType.Float; } else { return SqlDbType.VarChar; }
        }
        public static SqlCommand snapsCommand(this String vl,SqlConnection cn) { return new SqlCommand() { CommandText = vl,Connection = cn,CommandType = CommandType.Text }; }
        public static SqlCommand snapsCommand(this String vl) { return new SqlCommand() { CommandText = vl,CommandType = CommandType.Text }; }
        public static SqlCommand snapsProc(this String vl,SqlConnection cn) { return new SqlCommand() { CommandText = vl,Connection = cn,CommandType = CommandType.StoredProcedure }; }
        public static async Task snapsSqlExc(this String vl,SqlConnection cn) {
            SqlCommand cm = vl.snapsCommand(cn);
            try { await cm.snapsExecuteAsync(); } catch(Exception ex) { throw ex; } finally { await cm.DisposeAsync(); }
        }
        public static async Task<String> snapsSqlScl(this String vl,SqlConnection cn) {
            SqlCommand cm = vl.snapsCommand(cn);
            try { return await cm.snapsScalarStrAsync(); } catch(Exception ex) { throw ex; } finally { await cm.DisposeAsync(); }
        }
        public static SqlParameter snapsPar(this String vl,string name) {
            return new SqlParameter() {
                ParameterName = name,SqlDbType = SqlDbType.VarChar,Direction = ParameterDirection.Input,Value = vl ?? (object)DBNull.Value,IsNullable = true
            };
        }
        public static SqlParameter snapsParNvar(this String vl,string name) {
            return new SqlParameter() {
                ParameterName = name,SqlDbType = SqlDbType.NVarChar,Direction = ParameterDirection.Input,Value = vl ?? (object)DBNull.Value,IsNullable = true
            };
        }
        public static SqlParameter snapsPar(this Int32? vl,string name) {
            if(vl == null) {
                return snapsParNull(name);
            } else {
                return new SqlParameter() {
                    ParameterName = name,SqlDbType = SqlDbType.Int,Direction = ParameterDirection.Input,Value = vl,IsNullable = true
                };
            }
        }
        public static SqlParameter snapsPar(this Int32 vl,string name) {
            return new SqlParameter() {
                ParameterName = name,SqlDbType = SqlDbType.Int,Direction = ParameterDirection.Input,Value = vl,IsNullable = true
            };
        }

        public static SqlParameter snapsPar(this Decimal vl,string name) {
            return new SqlParameter() {
                ParameterName = name,SqlDbType = SqlDbType.Decimal,Direction = ParameterDirection.Input,Value = vl,IsNullable = true
            };
        }

        public static SqlParameter snapsPar(this Decimal? vl,string name) {
            if(vl == null) {
                return snapsParNull(name);
            } else {
                return new SqlParameter() {
                    ParameterName = name,SqlDbType = SqlDbType.Decimal,Direction = ParameterDirection.Input,Value = vl,IsNullable = true
                };
            }
        }

        public static SqlParameter snapsPar(this Double vl,string name) {
            return new SqlParameter() {
                ParameterName = name,SqlDbType = SqlDbType.Float,Direction = ParameterDirection.Input,Value = vl,IsNullable = true
            };
        }

        public static SqlParameter snapsPar(this Double? vl,string name) {
            if(vl == null) {
                return snapsParNull(name);
            } else {
                return new SqlParameter() {
                    ParameterName = name,SqlDbType = SqlDbType.Float,Direction = ParameterDirection.Input,Value = vl,IsNullable = true
                };
            }
        }

        public static SqlParameter snapsPar(this DateTime vl,string name) {
            if(vl == null) {
                return snapsParNull(name);
            } else {
                return new SqlParameter() {
                    ParameterName = name,SqlDbType = SqlDbType.Date,Direction = ParameterDirection.Input,Value = vl.Date,IsNullable = true
                };
            }
        }
        public static SqlParameter snapsPar(this DateTime? vl,string name) {
            if(vl == null) {
                return snapsParNullDateTime(name);
            } else {
                return new SqlParameter() {
                    ParameterName = name,SqlDbType = SqlDbType.Date,Direction = ParameterDirection.Input,Value = vl ?? (object)DBNull.Value,IsNullable = true
                };
            }
        }
        public static SqlParameter snapsPar(this DateTimeOffset vl,string name) {
            return new SqlParameter() {
                ParameterName = name,SqlDbType = SqlDbType.DateTimeOffset,Direction = ParameterDirection.Input,Value = vl,IsNullable = true
            };
        }
        public static SqlParameter snapsPar(this DateTimeOffset? vl,string name) {
            if(vl == null) {
                return snapsParNullDateTimeOffset(name);
            } else {
                return new SqlParameter() {
                    ParameterName = name,SqlDbType = SqlDbType.DateTimeOffset,Direction = ParameterDirection.Input,Value = vl ?? (object)DBNull.Value,IsNullable = true
                };
            }
        }
        public static SqlParameter snapsPar(this TimeSpan vl,string name) {
            return new SqlParameter() {
                ParameterName = name,SqlDbType = SqlDbType.Time,Direction = ParameterDirection.Input,Value = vl,IsNullable = true
            };
        }
        public static SqlParameter snapsPar(this TimeSpan? vl,string name) {
            if(vl == null) {
                return snapsParNullDateTimeOffset(name);
            } else {
                return new SqlParameter() {
                    ParameterName = name,SqlDbType = SqlDbType.Time,Direction = ParameterDirection.Input,Value = vl ?? (object)DBNull.Value,IsNullable = true
                };
            }
        }


        public static SqlParameter snapsPar(this bool vl,string name) {
            return new SqlParameter() {
                ParameterName = name,SqlDbType = SqlDbType.TinyInt,Direction = ParameterDirection.Input,Value = (vl == true) ? 1 : 0,IsNullable = true
            };
        }
        public static SqlParameter snapsParNull(string name) {
            return new SqlParameter() {
                ParameterName = name,SqlDbType = SqlDbType.TinyInt,Direction = ParameterDirection.Input,Value = DBNull.Value,IsNullable = true
            };
        }
        public static SqlParameter snapsParNullDateTime(string name) {
            return new SqlParameter() {
                ParameterName = name,SqlDbType = SqlDbType.DateTime,Direction = ParameterDirection.Input,Value = DBNull.Value,IsNullable = true
            };
        }
        public static SqlParameter snapsParNullDateTimeOffset(string name) {
            return new SqlParameter() {
                ParameterName = name,SqlDbType = SqlDbType.DateTimeOffset,Direction = ParameterDirection.Input,Value = DBNull.Value,IsNullable = true
            };
        }

        // get error message 
        public static async Task<String> snapsgetMessage(this String value,String apps,String lang,SqlCommand command) {
            command.CommandText = string.Format("select ISNULL((select descmsg from wm_message where apps = '{2}' " +
            " and langmsg = '{0}' and codemsg = '{1}'),'{1}')",lang,value,apps);
            return await command.snapsScalarStrAsync();
        }

        public static void snapsPar(this SqlCommand command,String value,string name) { command.Parameters.Add(snapsPar(value,name)); }
        public static void snapsParnv(this SqlCommand command,String value,string name) { command.Parameters.Add(snapsParNvar(value,name)); }
        public static void snapsPar(this SqlCommand command,DateTime value,string name) { command.Parameters.Add(snapsPar(value,name)); }
        public static void snapsPar(this SqlCommand command,DateTime? value,string name) {
            if(value == null) { command.Parameters.Add(snapsParNullDateTime(name)); } else { command.Parameters.Add(snapsPar(value,name)); }
        }
        public static void snapsPar(this SqlCommand command,DateTimeOffset value,string name) { command.Parameters.Add(snapsPar(value,name)); }
        public static void snapsPar(this SqlCommand command,DateTimeOffset? value,string name) {
            if(value == null) { command.Parameters.Add(snapsParNullDateTimeOffset(name)); } else { command.Parameters.Add(snapsPar(value,name)); }
        }
        public static void snapsPar(this SqlCommand command,Int32 value,string name) { command.Parameters.Add(snapsPar(value,name)); }
        public static void snapsPar(this SqlCommand command,Decimal value,string name) { command.Parameters.Add(snapsPar(value,name)); }
        public static void snapsPar(this SqlCommand command,Double value,string name) { command.Parameters.Add(snapsPar(value,name)); }
        public static void snapsPar(this SqlCommand command,Boolean value,String name) { command.Parameters.Add(snapsPar(value,name)); }

        //Auto assign sysdate
        public static void snapsParsysdateoffset(this SqlCommand command) { command.Parameters.Add(snapsPar(DateTimeOffset.Now,"sysdate")); }
        public static void snapsParsysdate(this SqlCommand command) { command.Parameters.Add(snapsPar(DateTime.Now,"sysdate")); }
        private static String decodeOperator(dbOperator o,String nm) {
            if(o == dbOperator.equal) {
                return " and " + nm + " = @" + nm;
            } else if(o == dbOperator.findin) {
                return " and " + nm + " in ( @" + nm + " ) ";
            } else if(o == dbOperator.lessthan) {
                return " and " + nm + " < @" + nm;
            } else if(o == dbOperator.lessthanequal) {
                return " and " + nm + " <= @" + nm;
            } else if(o == dbOperator.like) {
                return " and " + nm + " like '%'+@" + nm + "+'%' ";
            } else if(o == dbOperator.likeEnd) {
                return " and " + nm + " like @" + nm + "+'%' ";
            } else if(o == dbOperator.likeStart) {
                return " and " + nm + " like '%'+@" + nm;
            } else if(o == dbOperator.morethan) {
                return " and " + nm + " > @" + nm;
            } else if(o == dbOperator.morethanequal) {
                return " and " + nm + " >= @" + nm;
            } else { return " and " + nm + " = @" + nm; }
        }

        //Condition for date
        public static void snapsCdn(this SqlCommand cm,DateTime value,String parametername) {
            if(value != DateTime.MinValue) {
                cm.CommandText = cm.CommandText + decodeOperator(dbOperator.equal,parametername);
                cm.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Date,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand cm,DateTime value,String parametername,dbOperator dboperator) {
            if(value != DateTime.MinValue) {
                cm.CommandText = cm.CommandText + decodeOperator(dboperator,parametername);
                cm.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Date,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand cm,DateTime value,String parametername,String condition) {
            if(value != DateTime.MinValue) {
                cm.CommandText = cm.CommandText + " " + condition;
                cm.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Date,
                    IsNullable = true,
                    Value = value
                });
            }
        }

        public static void snapsCdnlessthan(this SqlCommand cm,DateTime value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.lessthan); }
        public static void snapsCdnlessthanEqual(this SqlCommand cm,DateTime value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.lessthanequal); }
        public static void snapsCdnmorethan(this SqlCommand cm,DateTime value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.morethan); }
        public static void snapsCdnmorethanequal(this SqlCommand cm,DateTime value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.morethanequal); }
        public static void snapsCdnequal(this SqlCommand cm,DateTime value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.equal); }


        //Condition for date with nullable
        public static void snapsCdn(this SqlCommand cm,DateTime? value,String parametername) {
            if(value.HasValue) {
                cm.CommandText = cm.CommandText + decodeOperator(dbOperator.equal,parametername);
                cm.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Date,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand cm,DateTime? value,String parametername,dbOperator dboperator) {
            if(value.HasValue) {
                cm.CommandText = cm.CommandText + decodeOperator(dboperator,parametername);
                cm.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Date,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand cm,DateTime? value,String parametername,String condition) {
            if(value.HasValue) {
                cm.CommandText = cm.CommandText + " " + condition;
                cm.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Date,
                    IsNullable = true,
                    Value = value
                });
            }
        }

        public static void snapsCdnlessthan(this SqlCommand cm,DateTime? value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.lessthan); }
        public static void snapsCdnlessthanEqual(this SqlCommand cm,DateTime? value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.lessthanequal); }
        public static void snapsCdnmorethan(this SqlCommand cm,DateTime? value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.morethan); }
        public static void snapsCdnmorethanequal(this SqlCommand cm,DateTime? value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.morethanequal); }
        public static void snapsCdnequal(this SqlCommand cm,DateTime? value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.equal); }


        //Condition for datetimeoffset 
        public static void snapsCdn(this SqlCommand cm,DateTimeOffset value,String parametername) {
            if(value != DateTime.MinValue) {
                cm.CommandText = cm.CommandText + decodeOperator(dbOperator.equal,parametername);
                cm.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.DateTimeOffset,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand cm,DateTimeOffset value,String parametername,dbOperator dboperator) {
            if(value != DateTime.MinValue) {
                cm.CommandText = cm.CommandText + decodeOperator(dboperator,parametername);
                cm.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.DateTimeOffset,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand cm,DateTimeOffset value,String parametername,String condition) {
            if(value != DateTime.MinValue) {
                cm.CommandText = cm.CommandText + " " + condition;
                cm.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.DateTimeOffset,
                    IsNullable = true,
                    Value = value
                });
            }
        }

        public static void snapsCdnlessthan(this SqlCommand cm,DateTimeOffset value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.lessthan); }
        public static void snapsCdnlessthanEqual(this SqlCommand cm,DateTimeOffset value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.lessthanequal); }
        public static void snapsCdnmorethan(this SqlCommand cm,DateTimeOffset value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.morethan); }
        public static void snapsCdnmorethanequal(this SqlCommand cm,DateTimeOffset value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.morethanequal); }
        public static void snapsCdnequal(this SqlCommand cm,DateTimeOffset value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.equal); }


        //Condition for datetimeoffset nullable 
        public static void snapsCdn(this SqlCommand cm,DateTimeOffset? value,String parametername) {
            if(value.HasValue) {
                cm.CommandText = cm.CommandText + decodeOperator(dbOperator.equal,parametername);
                cm.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.DateTimeOffset,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand cm,DateTimeOffset? value,String parametername,dbOperator dboperator) {
            if(value.HasValue) {
                cm.CommandText = cm.CommandText + decodeOperator(dboperator,parametername);
                cm.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.DateTimeOffset,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand cm,DateTimeOffset? value,String parametername,String condition) {
            if(value.HasValue) {
                cm.CommandText = cm.CommandText + " " + condition;
                cm.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.DateTimeOffset,
                    IsNullable = true,
                    Value = value
                });
            }
        }

        public static void snapsCdnlessthan(this SqlCommand cm,DateTimeOffset? value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.lessthan); }
        public static void snapsCdnlessthanEqual(this SqlCommand cm,DateTimeOffset? value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.lessthanequal); }
        public static void snapsCdnmorethan(this SqlCommand cm,DateTimeOffset? value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.morethan); }
        public static void snapsCdnmorethanequal(this SqlCommand cm,DateTimeOffset? value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.morethanequal); }
        public static void snapsCdnequal(this SqlCommand cm,DateTimeOffset? value,String parametername) { snapsCdn(cm,value,parametername,dbOperator.equal); }

        //Condition for String 


        public static void snapsCdn(this SqlCommand command,String value,String parametername) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + decodeOperator(dbOperator.equal,parametername);
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.VarChar,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand command,String value,String parametername,dbOperator dboperator) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + decodeOperator(dboperator,parametername);
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.VarChar,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand command,String value,String parametername,String condition) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + " " + condition;
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.VarChar,
                    IsNullable = true,
                    Value = value
                });
            }
        }

        /// <summary>
        /// include order by syntax 
        /// </summary>
        /// <param name="orderby">column asc</param>
        public static void snapsCdnOrderby(this SqlCommand command,string orderby) {
            command.CommandText = command.CommandText + " order by " + orderby;
        }

        public static void snapsCdnlessthan(this SqlCommand command,String value,String parametername) { snapsCdn(command,value,parametername,dbOperator.lessthan); }
        public static void snapsCdnlessthanEqual(this SqlCommand command,String value,String parametername) { snapsCdn(command,value,parametername,dbOperator.lessthanequal); }
        public static void snapsCdnmorethan(this SqlCommand command,String value,String parametername) { snapsCdn(command,value,parametername,dbOperator.morethan); }
        public static void snapsCdnmorethanequal(this SqlCommand command,String value,String parametername) { snapsCdn(command,value,parametername,dbOperator.morethanequal); }
        public static void snapsCdnlike(this SqlCommand command,String value,String parametername) { snapsCdn(command,value,parametername,dbOperator.like); }
        public static void snapsCdnlikestart(this SqlCommand command,String value,String parametername) { snapsCdn(command,value,parametername,dbOperator.likeStart); }
        public static void snapsCdnlikeend(this SqlCommand command,String value,String parametername) { snapsCdn(command,value,parametername,dbOperator.likeEnd); }
        public static void snapsCdnfindin(this SqlCommand command,String value,String parametername) { snapsCdn(command,value,parametername,dbOperator.findin); }
        public static void snapsCdnequal(this SqlCommand command,String value,String parametername) { snapsCdn(command,value,parametername,dbOperator.equal); }

        //set command
        public static void snapsSetcmd(this SqlCommand command,String sqlcmd) { command.CommandText = sqlcmd; }

        //Condition for String Nvarchar
        public static void snapsCdnv(this SqlCommand command,String value,String parametername) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + decodeOperator(dbOperator.equal,parametername);
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.NVarChar,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdnv(this SqlCommand command,String value,String parametername,dbOperator dboperator) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + decodeOperator(dboperator,parametername);
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.NVarChar,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdnv(this SqlCommand command,String value,String parametername,String condition) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + " " + condition;
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.NVarChar,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdnvlessthan(this SqlCommand command,String value,String parametername) { snapsCdnv(command,value,parametername,dbOperator.lessthan); }
        public static void snapsCdnvlessthanEqual(this SqlCommand command,String value,String parametername) { snapsCdnv(command,value,parametername,dbOperator.lessthanequal); }
        public static void snapsCdnvmorethan(this SqlCommand command,String value,String parametername) { snapsCdnv(command,value,parametername,dbOperator.morethan); }
        public static void snapsCdnvmorethanequal(this SqlCommand command,String value,String parametername) { snapsCdnv(command,value,parametername,dbOperator.morethanequal); }
        public static void snapsCdnvlike(this SqlCommand command,String value,String parametername) { snapsCdnv(command,value,parametername,dbOperator.like); }
        public static void snapsCdnvlikestart(this SqlCommand command,String value,String parametername) { snapsCdnv(command,value,parametername,dbOperator.likeStart); }
        public static void snapsCdnvlikesnd(this SqlCommand command,String value,String parametername) { snapsCdnv(command,value,parametername,dbOperator.likeEnd); }
        public static void snapsCdnvfindin(this SqlCommand command,String value,String parametername) { snapsCdnv(command,value,parametername,dbOperator.findin); }
        public static void snapsCdnvequal(this SqlCommand command,String value,String parametername) { snapsCdnv(command,value,parametername,dbOperator.equal); }

        //Condition for Int32
        public static void snapsCdn(this SqlCommand command,Int32 value,String parametername) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + decodeOperator(dbOperator.equal,parametername);
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Int,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand command,Int32 value,String parametername,dbOperator dboperator) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + decodeOperator(dboperator,parametername);
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Int,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand command,Int32 value,String parametername,String condition) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + " " + condition;
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Int,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdnlessthan(this SqlCommand command,Int32 value,String parametername) { snapsCdn(command,value,parametername,dbOperator.lessthan); }
        public static void snapsCdnlessthanEqual(this SqlCommand command,Int32 value,String parametername) { snapsCdn(command,value,parametername,dbOperator.lessthanequal); }
        public static void snapsCdnmorethan(this SqlCommand command,Int32 value,String parametername) { snapsCdn(command,value,parametername,dbOperator.morethan); }
        public static void snapsCdnmorethanequal(this SqlCommand command,Int32 value,String parametername) { snapsCdn(command,value,parametername,dbOperator.morethanequal); }
        public static void snapsCdnlike(this SqlCommand command,Int32 value,String parametername) { snapsCdn(command,value,parametername,dbOperator.like); }
        public static void snapsCdnlikestart(this SqlCommand command,Int32 value,String parametername) { snapsCdn(command,value,parametername,dbOperator.likeStart); }
        public static void snapsCdnlikesnd(this SqlCommand command,Int32 value,String parametername) { snapsCdn(command,value,parametername,dbOperator.likeEnd); }
        public static void snapsCdnfindin(this SqlCommand command,Int32 value,String parametername) { snapsCdn(command,value,parametername,dbOperator.findin); }
        public static void snapsCdnequal(this SqlCommand command,Int32 value,String parametername) { snapsCdn(command,value,parametername,dbOperator.equal); }

        //Condition for decimal
        public static void snapsCdn(this SqlCommand command,Decimal value,String parametername) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + decodeOperator(dbOperator.equal,parametername);
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Decimal,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand command,Decimal value,String parametername,dbOperator dboperator) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + decodeOperator(dboperator,parametername);
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Decimal,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand command,Decimal value,String parametername,String condition) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + " " + condition;
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Decimal,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdnlessthan(this SqlCommand command,Decimal value,String parametername) { snapsCdn(command,value,parametername,dbOperator.lessthan); }
        public static void snapsCdnlessthanEqual(this SqlCommand command,Decimal value,String parametername) { snapsCdn(command,value,parametername,dbOperator.lessthanequal); }
        public static void snapsCdnmorethan(this SqlCommand command,Decimal value,String parametername) { snapsCdn(command,value,parametername,dbOperator.morethan); }
        public static void snapsCdnmorethanequal(this SqlCommand command,Decimal value,String parametername) { snapsCdn(command,value,parametername,dbOperator.morethanequal); }
        public static void snapsCdnlike(this SqlCommand command,Decimal value,String parametername) { snapsCdn(command,value,parametername,dbOperator.like); }
        public static void snapsCdnlikestart(this SqlCommand command,Decimal value,String parametername) { snapsCdn(command,value,parametername,dbOperator.likeStart); }
        public static void snapsCdnlikesnd(this SqlCommand command,Decimal value,String parametername) { snapsCdn(command,value,parametername,dbOperator.likeEnd); }
        public static void snapsCdnfindin(this SqlCommand command,Decimal value,String parametername) { snapsCdn(command,value,parametername,dbOperator.findin); }
        public static void snapsCdnequal(this SqlCommand command,Decimal value,String parametername) { snapsCdn(command,value,parametername,dbOperator.equal); }

        //Condition for double
        public static void snapsCdn(this SqlCommand command,Double value,String parametername) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + decodeOperator(dbOperator.equal,parametername);
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Float,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand command,Double value,String parametername,dbOperator dboperator) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + decodeOperator(dboperator,parametername);
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Float,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdn(this SqlCommand command,Double value,String parametername,String condition) {
            if(value.notNull()) {
                command.CommandText = command.CommandText + " " + condition;
                command.Parameters.Add(new SqlParameter() {
                    ParameterName = parametername,
                    SqlDbType = SqlDbType.Float,
                    IsNullable = true,
                    Value = value
                });
            }
        }
        public static void snapsCdnlessthan(this SqlCommand command,Double value,String parametername) { snapsCdn(command,value,parametername,dbOperator.lessthan); }
        public static void snapsCdnlessthanEqual(this SqlCommand command,Double value,String parametername) { snapsCdn(command,value,parametername,dbOperator.lessthanequal); }
        public static void snapsCdnmorethan(this SqlCommand command,Double value,String parametername) { snapsCdn(command,value,parametername,dbOperator.morethan); }
        public static void snapsCdnmorethanequal(this SqlCommand command,Double value,String parametername) { snapsCdn(command,value,parametername,dbOperator.morethanequal); }
        public static void snapsCdnlike(this SqlCommand command,Double value,String parametername) { snapsCdn(command,value,parametername,dbOperator.like); }
        public static void snapsCdnlikestart(this SqlCommand command,Double value,String parametername) { snapsCdn(command,value,parametername,dbOperator.likeStart); }
        public static void snapsCdnlikesnd(this SqlCommand command,Double value,String parametername) { snapsCdn(command,value,parametername,dbOperator.likeEnd); }
        public static void snapsCdnfindin(this SqlCommand command,Double value,String parametername) { snapsCdn(command,value,parametername,dbOperator.findin); }
        public static void snapsCdnequal(this SqlCommand command,Double value,String parametername) { snapsCdn(command,value,parametername,dbOperator.equal); }
    }

    public class snapsCmdTag {
        public string tagtitle { get; set; }
        public List<snapsCmdTags> cmdTags { get; set; }
        public snapsCmdTag() {
            tagtitle = "default";
            cmdTags = new List<snapsCmdTags>();
        }
        public snapsCmdTag(string title) {
            tagtitle = title;
            cmdTags = new List<snapsCmdTags>();
        }
        public void Addtags(string message) {
            this.cmdTags.Add(new snapsCmdTags(this.tagtitle,message));
        }
        public void Addtags(string message,SqlCommand sqlcommand) {
            this.cmdTags.Add(new snapsCmdTags(tagtitle,message,sqlcommand));
        }
        public void Addtags(string title,string message,SqlCommand sqlcommand) {
            this.tagtitle = title;
            this.cmdTags.Add(new snapsCmdTags(tagtitle,message,sqlcommand));
        }
        public void Addtags(string title,string message) {
            this.tagtitle = title;
            this.cmdTags.Add(new snapsCmdTags(tagtitle,message));
        }

        public void Cleartags(string title,string message,SqlCommand sqlcommand) {
            if(cmdTags.Count > 0) {
                cmdTags.ForEach(x => x.sqlCommand.Dispose());
                cmdTags.Clear();
                cmdTags = new List<snapsCmdTags>();
            }
        }
        public string Gettag(snapsCmdTags tag) {
            string serializeTag = string.Concat(tag.title," ",tag.message);
            return serializeTag;
        }
        public string Gettag(int index) {
            int total = cmdTags.Count, ln = 1;
            string numOftotal = ln.ToString().PadLeft(2,'0') + " of " + total;
            string serializeTag = string.Concat(cmdTags[index].title," ",numOftotal," => ",cmdTags[index].message," ",cmdTags[index].isCommand ? $" {cmdTags[index].affectedRows} row" : "");
            return serializeTag;
        }
        public List<string> SerializeTag() {
            int total = cmdTags.Count, ln = 1;
            var lstags = new List<string>();
            foreach(var tg in cmdTags) {
                string numOftotal = ln.ToString().PadLeft(2,'0') + " of " + total;
                string serializeTag = string.Concat(tg.title," ",numOftotal," => ",tg.message," ",tg.isCommand ? $" {tg.affectedRows} row " : "");
                lstags.Add(serializeTag);
                ln++;
            }
            return lstags;
        }
    }
    public class snapsCmdTags  {
        public string title { get; set; }
        public string message { get; set; }
        public SqlCommand sqlCommand { get; set; }
        public bool isCommand { get; set; }
        public string tag { get; set; }
        public int affectedRows { get; set; }
        public snapsCmdTags() { }
        public snapsCmdTags(string title,string message) {
            this.title = title;
            this.message = message;
            this.isCommand = false;
            this.affectedRows = 0;
        }

        public snapsCmdTags(string title,string message,SqlCommand sqlcommand) {
            this.title = title;
            this.message = message;
            this.sqlCommand = sqlcommand;
            this.isCommand = true;
            this.affectedRows = 0;
        }
    }
}
