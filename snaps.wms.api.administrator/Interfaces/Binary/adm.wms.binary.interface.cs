using System.Threading.Tasks;
using System.Collections.Generic;
using Snaps.WMS.parameter;
namespace Snaps.WMS.Interfaces { 
     public interface IbinaryService { 
        Task<List<binary_md>> desc(binary_md o);
        Task<List<binary_md>> list(binary_md o);
        Task upsert(binary_md o);
        Task remove(binary_md o);
     }
}