using System;
namespace Snaps.WMS.warehouse { 

    public class zoneprep_md { 
        public string orgcode               { get; set; }
        public string site                  { get; set; }
        public string depot                 { get; set; }
        public string spcarea               { get; set; }
        public string przone                { get; set; }
        public string przonename            { get; set; }
        public string przonedesc            { get; set; }
        public string tflow                 { get; set; }
        public DateTimeOffset? datecreate   { get; set; }
        public string accncreate            { get; set; }
        public DateTimeOffset? datemodify   { get; set; }
        public string accnmodify            { get; set; }
        public string procmodify            { get; set; }
        public string hutype                { get; set; }
        public string description           { get; set; }
        public decimal huvalweight          { get; set; }
        public decimal huvalvolume          { get; set; }
        public decimal hucapweight          { get; set; }
        public decimal hucapvolume          { get; set; }
    }

    public class zoneprln_md { 
        public string orgcode               { get; set; }
        public string site                  { get; set; }
        public string depot                 { get; set; }
        public string spcarea               { get; set; }
        public string fltype                { get; set; }
        public string przone                { get; set; }
        public string lszone                { get; set; }
        public string lsaisle               { get; set; }
        public string lsbay                 { get; set; }
        public string lslevel               { get; set; }
        public string lsloc                 { get; set; }
        public string lsstack               { get; set; }
        public string lscode                { get; set; }
        public string spcproduct            { get; set; }
        public string spcpv                 { get; set; }
        public string spclv                 { get; set; }
        public string spcunit               { get; set; }
        public string spcthcode             { get; set; }
        public string lsdirection           { get; set; }
        public Int32 lspath                 { get; set; }
        public string tflow                 { get; set; }
        public string lshash                { get; set; }
        public DateTimeOffset? datecreate   { get; set; }
        public string accncreate            { get; set; }
        public DateTimeOffset? datemodify   { get; set; }
        public string accnmodify            { get; set; }
        public string procmodify            { get; set; }

        //Additional for display on grid
        public string barcode               { get; set ;}
        public string description           { get; set ;}
        public Int32 rtoskuofpu             { get; set; }
    }
}