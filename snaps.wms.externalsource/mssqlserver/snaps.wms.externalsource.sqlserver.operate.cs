using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Snaps.Helpers.DbContext.SQLServer;
using System.Threading.Tasks;
namespace Snaps.WMS {
    public partial class exSource_ops : IDisposable {
        
        public async Task<List<exsFile>> findAsync(exsFile o) { 
            SqlCommand cm =  new SqlCommand(sqlexssource_find,cn);
            List<exsFile> rn = new List<exsFile>();
            SqlDataReader r = null;
            try { 
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.imptype,"imptype");
                cm.CommandText += " order by datestart desc";
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillexsfile(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }catch (Exception ex) { throw ex; 
            }finally { cm.Dispose(); }
        }
        public async Task<List<exsBarcode>> lineBarcodeAsync(exsFile o) { 
            SqlCommand cm =  new SqlCommand(sqlexssource_barcode,cn);
            List<exsBarcode> rn = new List<exsBarcode>();
            SqlDataReader r = null;
            try { 
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.fileid.ToString(),"fileid");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new exsBarcode(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }catch (Exception ex) { throw ex; 
            }finally { cm.Dispose(); if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<string> ImpexsBarcodeAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsBarcode> o) { 
            SqlCommand cm = new SqlCommand(sqlexssource_getid,cn);
            Int32 cnterr = 0;
            string fileId = "";
            DateTimeOffset impstart = DateTimeOffset.Now;
            try { 
                fileId = cm.snapsScalarStrAsync().Result;

                cm = getCommand(o[0],sqlexsbarcode_insert_one);
                cm.Connection = cn;
                cm.snapsPar(filename,"filename");
                cm.snapsPar(filetype,"filetype");
                cm.Parameters.AddWithValue("filelength",filelength);
                cm.Parameters["filelength"].Value = filelength;
                cm.snapsPar(accncode,"accncreate");
                cm.snapsPar(decstart,"opsdecstart");
                cm.snapsPar(impstart,"opsimpstart");
                cm.snapsPar("barcode","imptype");
                cm.Parameters["fileid"].Value = fileId;
                
                cm.CommandText = sqlexssource_start;            //Create task upload
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsbarcode_insert_one;
                foreach(exsBarcode ln in o){                    //Insert line by line
                    fillCommand(ref cm,ln,fileId);
                    try { 
                        await cm.snapsExecuteAsync();
                    }catch(Exception ex){ 
                        cnterr++;
                        cm.CommandText = sqlexsbarcode_insert_err;
                        cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                        cm.Parameters["tflow"].Value = "ER";
                        await cm.snapsExecuteAsync();
                    }
                }

                cm.CommandText = sqlexsbarcode_import_step1;    //validate;
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsbarcode_import_step2;    //Insert
                await cm.snapsExecuteAsync();
                
                cm.CommandText = sqlexsbarcode_import_step3;    //Update
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsbarcode_import_step4;    //Remove
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsbarcode_import_step5;    //set to finish 
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsbarcode_end;             //Set to finish upload
                await cm.snapsExecuteAsync();

                return fileId;
            }catch (Exception ex) { 
                cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                cm.Parameters["tflow"].Value = "ER";
                cm.CommandText = sqlexsource_error;
                await cm.snapsExecuteAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync(); 
            }            
        }

        public async Task<List<exsThirdparty>> lineTHPartyAsync(exsFile o){
            SqlCommand cm =  new SqlCommand(sqlexssource_thparty,cn);
            List<exsThirdparty> rn = new List<exsThirdparty>();
            SqlDataReader r = null;
            try { 
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.fileid.ToString(),"fileid");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new exsThirdparty(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }catch (Exception ex) { 
                throw ex; 
            }finally { cm.Dispose(); if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<string> ImpexsTHPartyAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsThirdparty> o) { 
            SqlCommand cm = new SqlCommand(sqlexssource_getid,cn);
            Int32 cnterr = 0;
            string fileId = "";
            DateTimeOffset impstart = DateTimeOffset.Now;
            try { 
                fileId = cm.snapsScalarStrAsync().Result;

                cm = getCommand(o[0],sqlexsthparty_insert);
                cm.Connection = cn;
                cm.snapsPar(filename,"filename");
                cm.snapsPar(filetype,"filetype");
                cm.Parameters.AddWithValue("filelength",filelength);
                cm.Parameters["filelength"].Value = filelength;
                cm.snapsPar(accncode,"accncreate");
                cm.snapsPar(decstart,"opsdecstart");
                cm.snapsPar(impstart,"opsimpstart");
                cm.snapsPar("thparty","imptype");
                cm.Parameters["fileid"].Value = fileId;
                
                cm.CommandText = sqlexssource_start;            //Create task upload
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsthparty_insert;
                foreach(exsThirdparty ln in o){                    //Insert line by line
                    fillCommand(ref cm,ln,fileId);
                    try { 
                        await cm.snapsExecuteAsync();
                    }catch(Exception ex){ 
                        cnterr++;
                        cm.CommandText = sqlexsthparty_insert_err;
                        cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                        cm.Parameters["tflow"].Value = "ER";
                        await cm.snapsExecuteAsync();
                    }
                }

                cm.CommandText = sqlexsthparty_import_step1;    //validate;
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsthparty_import_step2;    //Backup
                await cm.snapsExecuteAsync();
                
                cm.CommandText = sqlexsthparty_import_step3;    //Insert
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsthparty_import_step4;    //Update
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsthparty_import_step5;    //Remove
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsthparty_import_step6;    //set to finish 
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsthparty_end;             //Set to finish upload
                await cm.snapsExecuteAsync();

                return fileId;
            }catch (Exception ex) { 
                cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                cm.Parameters["tflow"].Value = "ER";
                cm.CommandText = sqlexsource_error;
                await cm.snapsExecuteAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync(); 
            }            
        }
        
        public async Task<List<exsProduct>> lineProductAsync(exsFile o){
            SqlCommand cm =  new SqlCommand(sqlexssource_product,cn);
            List<exsProduct> rn = new List<exsProduct>();
            SqlDataReader r = null;
            try { 
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.fileid.ToString(),"fileid");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new exsProduct(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }catch (Exception ex) { 
                throw ex; 
            }finally { cm.Dispose(); if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<string> ImpexsProductAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsProduct> o) {
            SqlCommand cm = new SqlCommand(sqlexssource_getid,cn);
            Int32 cnterr = 0;
            string fileId = "";
            DateTimeOffset impstart = DateTimeOffset.Now;
            try { 
                fileId = cm.snapsScalarStrAsync().Result;

                cm = getCommand(o[0],sqlexsproduct_insert);
                cm.Connection = cn;
                cm.snapsPar(filename,"filename");
                cm.snapsPar(filetype,"filetype");
                cm.Parameters.AddWithValue("filelength",filelength);
                cm.Parameters["filelength"].Value = filelength;
                cm.snapsPar(accncode,"accncreate");
                cm.snapsPar(decstart,"opsdecstart");
                cm.snapsPar(impstart,"opsimpstart");
                cm.snapsPar("product","imptype");
                cm.Parameters["fileid"].Value = fileId;
                
                cm.CommandText = sqlexssource_start;            //Create task upload
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsproduct_insert;
                foreach(exsProduct ln in o){                    //Insert line by line
                    fillCommand(ref cm,ln,fileId);
                    try { 
                        await cm.snapsExecuteAsync();
                    }catch(Exception ex){ 
                        cnterr++;
                        cm.CommandText = sqlexsproduct_insert_err;
                        cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                        cm.Parameters["tflow"].Value = "ER";
                        await cm.snapsExecuteAsync();
                    }
                }

                cm.CommandText = sqlexsproduct_import_step1;    //validate;
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsproduct_import_step2;    //Insert
                await cm.snapsExecuteAsync();
                
                cm.CommandText = sqlexsproduct_import_step3;    //backup
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsproduct_import_step4;    //update
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsproduct_import_step5;    //remove
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsproduct_import_step6;    //set to finish 
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsproduct_end;             //Set to finish upload
                await cm.snapsExecuteAsync();

                return fileId;
            }catch (Exception ex) { 
                cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                cm.Parameters["tflow"].Value = "ER";
                cm.CommandText = sqlexsource_error;
                await cm.snapsExecuteAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync(); 
            }  
        }

        public async Task<List<exsInbound>> lineInboundAsync(exsFile o){
            SqlCommand cm =  new SqlCommand(sqlexssource_inbound,cn);
            List<exsInbound> rn = new List<exsInbound>();
            SqlDataReader r = null;
            try { 
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.fileid.ToString(),"fileid");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new exsInbound(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }catch (Exception ex) { 
                throw ex; 
            }finally { cm.Dispose(); if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<string> ImpexsInboundAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsInbound> o){
            SqlCommand cm = new SqlCommand(sqlexssource_getid,cn);
            Int32 cnterr = 0;
            string fileId = "";
            DateTimeOffset impstart = DateTimeOffset.Now;
            try { 
                fileId = cm.snapsScalarStrAsync().Result;

                cm = getCommand(o[0],sqlexsinbound_insert);
                cm.Connection = cn;
                cm.snapsPar(filename,"filename");
                cm.snapsPar(filetype,"filetype");
                cm.Parameters.AddWithValue("filelength",filelength);
                cm.Parameters["filelength"].Value = filelength;
                cm.snapsPar(accncode,"accncreate");
                cm.snapsPar(decstart,"opsdecstart");
                cm.snapsPar(impstart,"opsimpstart");
                cm.snapsPar("inbound","imptype"); 
                cm.Parameters["fileid"].Value = fileId;
                
                cm.CommandText = sqlexssource_start;            //Create task upload
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsinbound_insert;
                foreach(exsInbound ln in o){                    //Insert line by line
                    fillCommand(ref cm,ln,fileId);
                    try { 
                        await cm.snapsExecuteAsync();
                    }catch(Exception ex){ 
                        cnterr++;
                        cm.CommandText = sqlexsinbound_insert_err;
                        cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                        cm.Parameters["tflow"].Value = "ER";
                        await cm.snapsExecuteAsync();
                    }
                }

                cm.CommandText = sqlexsinbound_import_step1;    //validate;
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsinbound_import_step2;    //Insert
                await cm.snapsExecuteAsync();
                
                cm.CommandText = sqlexsinbound_import_step3;    //Update
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsinbound_import_step4;    //Remove
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsinbound_import_step5;    //set to finish 
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsinbound_end;             //Set to finish upload
                await cm.snapsExecuteAsync();

                return fileId;
            }catch (Exception ex) { 
                cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                cm.Parameters["tflow"].Value = "ER";
                cm.CommandText = sqlexsource_error;
                await cm.snapsExecuteAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync(); 
            }  
         }

        public async Task<List<exsInbouln>> lineInboulnAsync(exsFile o){
            SqlCommand cm =  new SqlCommand(sqlexssource_inbouln,cn);
            List<exsInbouln> rn = new List<exsInbouln>();
            SqlDataReader r = null;
            try { 
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.fileid.ToString(),"fileid");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new exsInbouln(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }catch (Exception ex) { 
                throw ex; 
            }finally { cm.Dispose(); if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<string> ImpexsInboulnAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsInbouln> o) {
            SqlCommand cm = new SqlCommand(sqlexssource_getid,cn);
            Int32 cnterr = 0;
            string fileId = "";
            DateTimeOffset impstart = DateTimeOffset.Now;
            try { 
                fileId = cm.snapsScalarStrAsync().Result;

                cm = getCommand(o[0],sqlexsinbouln_insert);
                cm.Connection = cn;
                cm.snapsPar(filename,"filename");
                cm.snapsPar(filetype,"filetype");
                cm.Parameters.AddWithValue("filelength",filelength);
                cm.Parameters["filelength"].Value = filelength;
                cm.snapsPar(accncode,"accncreate");
                cm.snapsPar(decstart,"opsdecstart");
                cm.snapsPar(impstart,"opsimpstart");
                cm.snapsPar("inbouln","imptype");
                cm.Parameters["fileid"].Value = fileId;
                
                cm.CommandText = sqlexssource_start;            //Create task upload
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsinbouln_insert;
                foreach(exsInbouln ln in o){                    //Insert line by line
                    fillCommand(ref cm,ln,fileId);
                    try { 
                        await cm.snapsExecuteAsync();
                    }catch(Exception ex){ 
                        cnterr++;
                        cm.CommandText = sqlexsinbouln_insert_err;
                        cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                        cm.Parameters["tflow"].Value = "ER";
                        await cm.snapsExecuteAsync();
                    }
                }

                cm.CommandText = sqlexsinbouln_import_step1;    //validate;
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsinbouln_import_step2;    //Insert
                await cm.snapsExecuteAsync();
                
                cm.CommandText = sqlexsinbouln_import_step3;    //Update
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsinbouln_import_step4;    //Remove
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsinbouln_import_step5;    //set to finish 
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsinbouln_end;             //Set to finish upload
                await cm.snapsExecuteAsync();

                return fileId;
            }catch (Exception ex) { 
                cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                cm.Parameters["tflow"].Value = "ER";
                cm.CommandText = sqlexsource_error;
                await cm.snapsExecuteAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync(); 
            }  
        }
        
        public async Task<List<exsOutbound>> lineOutboundAsync(exsFile o){
            SqlCommand cm =  new SqlCommand(sqlexssource_outbound,cn);
            List<exsOutbound> rn = new List<exsOutbound>();
            SqlDataReader r = null;
            try { 
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.fileid.ToString(),"fileid");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new exsOutbound(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }catch (Exception ex) { 
                throw ex; 
            }finally { cm.Dispose(); if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<string> ImpexsOutboundAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsOutbound> o) {
            SqlCommand cm = new SqlCommand(sqlexssource_getid,cn);
            Int32 cnterr = 0;
            string fileId = "";
            DateTimeOffset impstart = DateTimeOffset.Now;
            try { 
                fileId = cm.snapsScalarStrAsync().Result;

                cm = getCommand(o[0],sqlexsoutbound_insert);
                cm.Connection = cn;
                cm.snapsPar(filename,"filename");
                cm.snapsPar(filetype,"filetype");
                cm.Parameters.AddWithValue("filelength",filelength);
                cm.Parameters["filelength"].Value = filelength;
                cm.snapsPar(accncode,"accncreate");
                cm.snapsPar(decstart,"opsdecstart");
                cm.snapsPar(impstart,"opsimpstart");
                cm.snapsPar("outbound","imptype");
                cm.Parameters["fileid"].Value = fileId;
                
                cm.CommandText = sqlexssource_start;            //Create task upload
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsoutbound_insert;
                foreach(exsOutbound ln in o){                    //Insert line by line
                    fillCommand(ref cm,ln,fileId);
                    try { 
                        await cm.snapsExecuteAsync();
                    }catch(Exception ex){ 
                        cnterr++;
                        cm.CommandText = sqlexsoutbound_insert_err;
                        cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                        cm.Parameters["tflow"].Value = "ER";
                        await cm.snapsExecuteAsync();
                    }
                }

                cm.CommandText = sqlexsoutbound_import_step1;    //validate;
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsoutbound_import_step2;    //Insert
                await cm.snapsExecuteAsync();
                
                cm.CommandText = sqlexsoutbound_import_step3;    //Update
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsoutbound_import_step4;    //Remove
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsoutbound_import_step5;    //set to finish 
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsoutbound_end;             //Set to finish upload
                await cm.snapsExecuteAsync();

                return fileId;
            }catch (Exception ex) { 
                cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                cm.Parameters["tflow"].Value = "ER";
                cm.CommandText = sqlexsource_error;
                await cm.snapsExecuteAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync(); 
            }  
        }
        
        public async Task<List<exsOutbouln>> lineOutboulnAsync(exsFile o){
            SqlCommand cm =  new SqlCommand(sqlexssource_outbouln,cn);
            List<exsOutbouln> rn = new List<exsOutbouln>();
            SqlDataReader r = null;
            try { 
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.fileid.ToString(),"fileid");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new exsOutbouln(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }catch (Exception ex) { 
                throw ex; 
            }finally { cm.Dispose(); if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<string> ImpexsOutboulnAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsOutbouln> o) {
            SqlCommand cm = new SqlCommand(sqlexssource_getid,cn);
            Int32 cnterr = 0;
            string fileId = "";
            DateTimeOffset impstart = DateTimeOffset.Now;
            try { 
                fileId = cm.snapsScalarStrAsync().Result;

                cm = getCommand(o[0],sqlexsoutbouln_insert);
                cm.Connection = cn;
                cm.snapsPar(filename,"filename");
                cm.snapsPar(filetype,"filetype");
                cm.Parameters.AddWithValue("filelength",filelength);
                cm.Parameters["filelength"].Value = filelength;
                cm.snapsPar(accncode,"accncreate");
                cm.snapsPar(decstart,"opsdecstart");
                cm.snapsPar(impstart,"opsimpstart");
                cm.snapsPar("outbouln","imptype");
                cm.Parameters["fileid"].Value = fileId;
                
                cm.CommandText = sqlexssource_start;            //Create task upload
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsoutbouln_insert;
                foreach(exsOutbouln ln in o){                    //Insert line by line
                    fillCommand(ref cm,ln,fileId);
                    try { 
                        await cm.snapsExecuteAsync();
                    }catch(Exception ex){ 
                        cnterr++;
                        cm.CommandText = sqlexsoutbouln_insert_err;
                        cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                        cm.Parameters["tflow"].Value = "ER";
                        await cm.snapsExecuteAsync();
                    }
                }

                cm.CommandText = sqlexsoutbouln_import_step1;    //validate;
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsoutbouln_import_step2;    //Insert
                await cm.snapsExecuteAsync();
                
                cm.CommandText = sqlexsoutbouln_import_step3;    //Update
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsoutbouln_import_step4;    //Remove
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsoutbouln_import_step5;    //set to finish 
                await cm.snapsExecuteAsync();

                cm.CommandText = sqlexsoutbouln_end;             //Set to finish upload
                await cm.snapsExecuteAsync();

                return fileId;
            }catch (Exception ex) { 
                cm.Parameters["ermsg"].Value = (ex.Message.Length > 100) ? ex.Message.Substring(0,98) : ex.Message;
                cm.Parameters["tflow"].Value = "ER";
                cm.CommandText = sqlexsource_error;
                await cm.snapsExecuteAsync();
                throw ex;
            }finally {
                await cm.DisposeAsync(); 
            }
        }  
    }
}
