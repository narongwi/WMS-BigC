using System;
using System.Collections.Generic;
using System.Text;

namespace snaps.wms.labels {
	public class label_mst {
		public label_mst() {}

		public label_mst(string orgcode,string site,string depot,string spcarea,int lblid,string lblcode,string lbldesr,string lblcfg,string tflow,DateTimeOffset datecreate,string accncreate,DateTimeOffset datemodify,string accnmodify) {
			this.orgcode = orgcode;
			this.site = site;
			this.depot = depot;
			this.spcarea = spcarea;
			this.lblid = lblid;
			this.lblcode = lblcode;
			this.lbldesr = lbldesr;
			this.lblcfg = lblcfg;
			this.tflow = tflow;
			this.datecreate = datecreate;
			this.accncreate = accncreate;
			this.datemodify = datemodify;
			this.accnmodify = accnmodify;
		}

		public string orgcode { get; set; }
		public string site { get; set; }
		public string depot { get; set; }
		public string spcarea { get; set; }
		public int lblid { get; set; }
		public string lblcode { get; set; }
		public string lbldesr { get; set; }
		public string lblcfg { get; set; }
		public string tflow { get; set; }
		public DateTimeOffset datecreate { get; set; }
		public string accncreate { get; set; }
		public DateTimeOffset datemodify { get; set; }
		public string accnmodify { get; set; }

	}

	public class label_cfg {
		public int cfgid { get; set; }
		public string orgcode { get; set; }
		public string site { get; set; }
		public string depot { get; set; }
		public string spcarea { get; set; }
		public int lblid { get; set; }
		public int devid { get; set; }
		public int accncode { get; set; }
		public string tflow { get; set; }
		public DateTimeOffset datecreate { get; set; }
		public string accncreate { get; set; }
		public DateTimeOffset datemodify { get; set; }
		public string accnmodify { get; set; }
		public label_cfg() { }
		public label_cfg(int cfgid_,string orgcode_,string site_,string depot_,string spcarea_,int lblid_,int devid_,int accncode_,string tflow_,DateTimeOffset datecreate_,string accncreate_,DateTimeOffset datemodify_,string accnmodify_) {
			this.cfgid = cfgid_;
			this.orgcode = orgcode_;
			this.site = site_;
			this.depot = depot_;
			this.spcarea = spcarea_;
			this.lblid = lblid_;
			this.devid = devid_;
			this.accncode = accncode_;
			this.tflow = tflow_;
			this.datecreate = datecreate_;
			this.accncreate = accncreate_;
			this.datemodify = datemodify_;
			this.accnmodify = accnmodify_;
		}
	}

	public class label_job {
		public string orgcode { get; set; }
		public string site { get; set; }
		public string depot { get; set; }
		public string spcarea { get; set; }
		public int jobid { get; set; }
		public string jobval { get; set; }
		public int lblid { get; set; }
		public int devid { get; set; }
		public int accncode { get; set; }
		public string tflow { get; set; }
		public DateTimeOffset datecreate { get; set; }
		public string accncreate { get; set; }
		public DateTimeOffset datemodify { get; set; }
		public string accnmodify { get; set; }
		public label_job() { }
		public label_job(string orgcode_,string site_,string depot_,string spcarea_,int jobid_,string jobval_,int lblid_,int devid_,int accncode_,string tflow_,DateTimeOffset datecreate_,string accncreate_,DateTimeOffset datemodify_,string accnmodify_) {
			this.orgcode = orgcode_;
			this.site = site_;
			this.depot = depot_;
			this.spcarea = spcarea_;
			this.jobid = jobid_;
			this.jobval = jobval_;
			this.lblid = lblid_;
			this.devid = devid_;
			this.accncode = accncode_;
			this.tflow = tflow_;
			this.datecreate = datecreate_;
			this.accncreate = accncreate_;
			this.datemodify = datemodify_;
			this.accnmodify = accnmodify_;
		}
	}
}
