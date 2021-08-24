using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services { 
    public class mergeService : ImergeService { 
        private readonly string cnx = "";
        public mergeService(IOptions<AppSettings> appSettings) { 
            cnx = appSettings.Value.Conxstr;  
        }
        public async Task<merge_md> Generate(merge_set os){
            mergehu_ops ops = new mergehu_ops(cnx);
            try     { return await ops.Generate(os);  } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        public async Task<bool> Cancel(merge_set os) {
            mergehu_ops ops = new mergehu_ops(cnx);
            try { return await ops.Cancel(os); } 
            catch(Exception ex) { throw ex; }
            finally { ops.Dispose(); }
        }
        public async Task<List<mergehu_ln>> Find(merge_find of) {
            mergehu_ops ops = new mergehu_ops(cnx);
            try { return await ops.Find(of); } 
            catch(Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        public async Task<bool> Mergehu(merge_md os) {
            mergehu_ops ops = new mergehu_ops(cnx);
            try { return await ops.Mergehu(os); } 
            catch(Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        public async Task<List<mergehu_md>> Mergelist(merge_find of) {
            mergehu_ops ops = new mergehu_ops(cnx);
            try { return await ops.MergeList(of); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        public async Task<List<mergehu_ln>> Mergeline(mergehu_md of) {
            mergehu_ops ops = new mergehu_ops(cnx);
            try { return await ops.Mergeline(of); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        //Plan 
        public async Task Label(merge_set os) {
            mergehu_ops ops = new mergehu_ops(cnx);
            try { await ops.Label(os); } 
            catch(Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
    }
}