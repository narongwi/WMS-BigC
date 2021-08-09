using System.Collections.Generic;
using System.Threading.Tasks;
namespace Snaps.WMS.Interfaces
{
    public interface IcorrectionService
    {
        Task process(correction_md o);
        Task<List<lov>> getLocation(stock_ls o);
    }
}