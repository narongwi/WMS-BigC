using Microsoft.Extensions.Options;
using Snaps.Helpers;
using Snaps.WMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Snaps.WMS.Services
{
    public class correctionService : IcorrectionService
    {
        private readonly IOptions<AppSettings> _appSettings;
        private correction_ops op;
        public correctionService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            op = new correction_ops(_appSettings.Value.Conxstr);
        }

        public async Task process(correction_md o)
        {
            try { await op.CorrectionStock(o); }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        public async Task<List<lov>> getLocation(stock_ls o)
        {
            try { return await op.getLocation(o); }
            catch (Exception ex) { throw ex; }
            finally { }
        }

    }
}