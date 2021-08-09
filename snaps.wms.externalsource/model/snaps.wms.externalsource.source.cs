using System;

namespace Snaps.WMS {
    public class exsFile { 
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public decimal fileid    { get; set; }
        public string filename    { get; set; }
        public string filetype    { get; set; }
        public decimal filelength    { get; set; }
        public string imptype    { get; set; }
        public DateTimeOffset? datestart    { get; set; }
        public DateTimeOffset? dateend    { get; set; }
        public string opsperform    { get; set; }
        public decimal opsins    { get; set; }
        public decimal opsupd    { get; set; }
        public decimal opsrem    { get; set; }
        public decimal opserr    { get; set; }
        public DateTimeOffset? opsdecstart    { get; set; }
        public DateTimeOffset? opsimpstart    { get; set; }
        public string tflow    { get; set; }
        public DateTimeOffset? datecreate    { get; set; }
        public string accncreate    { get; set; }
    }

}