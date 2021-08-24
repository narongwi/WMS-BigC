using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces
{
    public interface ImergeService
    {
        Task<merge_md> Generate(merge_set os);
        Task<bool> Cancel(merge_set os);
        Task<List<mergehu_ln>> Find(merge_find of);
        Task<bool> Mergehu(merge_md os);
        Task<List<mergehu_md>> Mergelist(merge_find os);
        Task<List<mergehu_ln>> Mergeline(mergehu_md os);
        Task Label(merge_set os);
    }
}