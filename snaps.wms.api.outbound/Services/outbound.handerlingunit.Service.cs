using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services
{
    public class ouhanderlingunitService : IouhanderlingunitService
    {
        private readonly IOptions<AppSettings> _appSettings;
        private handerlingunit_ops op;
        public ouhanderlingunitService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            op = new handerlingunit_ops(_appSettings.Value.Conxstr);
        }

        public async Task<List<handerlingunit>> listAsync(handerlingunit o)
        {
            try { return await op.find(o); }
            catch (Exception ex) { throw ex; }
            finally { }
        }
        public async Task<handerlingunit> getAsync(handerlingunit o)
        {
            try {
                return await op.get(o); 
                //return o;
            }
            catch (Exception ex) { throw ex; }
            finally { }
        }
        public async Task upsert(handerlingunit o)
        {
            try { await op.upsert(o); }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        public async Task generate(handerlingunit_gen o)
        {
            try { await op.generate(o); }
            catch (Exception ex) { throw ex; }
            finally {  }
        }

        public async Task<List<Snaps.WMS.lov>> getmaster(String orgcode, String site, String depot) {
            try { return await op.getmaster(orgcode, site, depot); }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        public async Task<List<handerlingunit_item>> lines(handerlingunit o)
        {
            try { return await op.lines(o); }
            catch (Exception ex) { throw ex; }
            finally { }
        }
        public async Task<List<handerlingunit_item>> linesnonsum(handerlingunit o)
        {
            try { return await op.lines(o,false); }
            catch (Exception ex) { throw ex; }
            finally { }
        }
        public async Task close(handerlingunit o){  
            try { 
                await op.close(o);
            }catch (Exception ex) { throw ex; }
            finally { }
        }

        
        //public async Task<List<lov>> getstaging(String orgcode, String site, String depot)
        //{
        //    try { return await op.getstaging(orgcode, site, depot); }
        //    catch (Exception ex) { throw ex; }
        //    finally { }
        //}

    }
}
