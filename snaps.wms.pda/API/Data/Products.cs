using Newtonsoft.Json;
using snaps.wms.api.pda.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace snaps.wms.api.pda.Data
{
    public class Products
    {
        public string barcode { get; set; }
        public string article { get; set; }
        public int pv { get; set; }
        public int lv { get; set; }
        public string descalt { get; set; }
        public int rtoskuofpu { get; set; }
        public int rtopckoflayer { get; set; }
        public int rtolayerofhu { get; set; }
        public int rtopckofpallet { get; set; }
        public int rtoskuofipck { get; set; }
        public int rtoskuofpck { get; set; }
        public int rtoskuoflayer { get; set; }
        public int rtoskuofhu { get; set; }
        public string unitprep { get; set; }
        public string unitreceipt { get; set; }
        public string unitmanage { get; set; }
        public string unitdesc { get; set; }


        private readonly IConnector connector;

        public Products() { }
        public Products(string _connectionString)
        {
            connector = new Connector(_connectionString);
        }

        public Products GetActive(string orgcode, string site,string depot, string product)
        {
            var param = new Params();
            string commandText =
               @"select top 1 dbo.get_barcode(p.orgcode,p.site,p.depot,p.article,p.pv,p.lv) barcode,p.article,p.pv,p.lv,p.descalt,p.rtoskuofpu,p.rtopckoflayer,p.rtolayerofhu,
	                (p.rtopckoflayer * p.rtolayerofhu) rtopckofpallet,p.rtoskuofipck,p.rtoskuofpck,p.rtoskuoflayer,p.rtoskuofhu,p.unitprep,p.unitreceipt,p.unitmanage, p.unitdesc
                from wm_product p where   p.orgcode = @orgcode and p.site = @site  and p.depot = @depot
	            and p.article = (select top 1 t.article from wm_barcode t where t.orgcode = @orgcode and t.site = @site  and t.depot = @depot and (t.article = @productCode or  t.barcode = @productCode))";
            param.add("orgcode", orgcode);
            param.add("site", site);
            param.add("depot", depot);
            param.add("productCode", product);
            var datatable = connector.GetDtb(commandText, param.Values);
            return datatable.toObj<Products>();
        }
        public Products GetActive(string orgcode, string site, string depot, string article,string lv)
        {
            var param = new Params();
            //string commandText =
            //    @"select top 1 b.barcode,b.article,b.pv,b.lv,p.descalt,p.rtoskuofpu,p.rtopckoflayer,p.rtolayerofhu,
            //     (p.rtopckoflayer * p.rtolayerofhu) rtopckofpallet,p.rtoskuofipck,p.rtoskuofpck,p.rtoskuoflayer,p.rtoskuofhu,
            //        p.unitprep,p.unitreceipt,p.unitmanage, p.unitdesc
            //    from wm_barcode b , wm_product p
            //    where b.orgcode = p.orgcode 
            //     and b.site = p.site 
            //     and b.depot = p.depot 
            //     and b.article = p.article 
            //     and b.pv = p.pv 
            //     and b.lv = p.lv
            //     and b.orgcode = @orgcode
            //     and b.site = @site 
            //     and b.depot = @depot
            //     and b.isprimary = 1 
            //     and b.article = @article
            //        and b.lv = @lv";

            string commandText = @"select top 1 dbo.get_barcode(p.orgcode,p.site,p.depot,p.article,p.pv,p.lv) barcode ,p.article,p.pv,p.lv,p.descalt,p.rtoskuofpu,p.rtopckoflayer,p.rtolayerofhu,
                (p.rtopckoflayer * p.rtolayerofhu) rtopckofpallet,p.rtoskuofipck,p.rtoskuofpck,p.rtoskuoflayer,p.rtoskuofhu,
                p.unitprep,p.unitreceipt,p.unitmanage, p.unitdesc
                from  wm_product p
                where P.orgcode = @orgcode
                    and P.site = @site 
                    and P.depot = @depot
                    and P.article = @article
                    and P.lv = @lv";

            param.add("orgcode", orgcode);
            param.add("site", site);
            param.add("depot", depot);
            param.add("article", article);
            param.add("lv", lv);

            var datatable = connector.GetDtb(commandText, param.Values);
            return datatable.toObj<Products>();
        }
        public object GetExp(string orgcode, string site, string depot, string article, string lv, DateTime mfgDate)
        {
            Params param = new Params();
            string sql = @" SELECT format(dateadd(day,coalesce(p.dlcall,0),convert(date,@mfgdate,103)),'dd/MM/yyyy') as expdate,
            case when ((datediff(day,getdate(),dateadd(day,coalesce(p.dlcall,0),convert(date,'23/02/2021',103))) * 100 )/coalesce(p.dlcall,0)) < (coalesce(p.dlcshop,0) 
            + coalesce(p.dlconsumer,0)) then '0' else '1' end IsValid  from wm_product p  where p.orgcode=@orgcode 
             and p.site=@site and depot = @depot and article=@article and lv = @lv";
            param.add("orgcode", orgcode);
            param.add("site", site);
            param.add("depot", depot);
            param.add("mfgdate", mfgDate.ToString("dd/MM/yyyy"));
            param.add("article", article);
            param.add("lv", lv);

            // Data Adapter
            var dtexp = connector.GetDtb(sql, param.Values);
            if (dtexp.Rows.Count == 0)  throw new Exception("Product Not Found.");

            // JSON Serialize
            return JsonConvert.SerializeObject(dtexp.AsEnumerable()
                .Select(x => new { expdate = x["expdate"], isvalid = (x["isvalid"].ToString() == "1" ? true : false) })
                .SingleOrDefault());
        }

        public object GetMfg(string orgcode, string site, string depot, string article, string lv, DateTime expDate)
        {
            Params param = new Params();
            string sql = @"SELECT format(dateadd(day,(coalesce(p.dlcall,0)*-1),convert(date,@expdate,103)),'dd/MM/yyyy') as mfgdate,
            case when ((datediff(day,getdate(),convert(date,@expdate,103)) * 100 ) / coalesce(p.dlcall,0)) < (coalesce(p.dlcshop,0)
            + coalesce(p.dlconsumer,0)) then '0' else '1' end IsValid from wm_product p where p.orgcode=@orgcode
            and p.site=@site and depot = @depot and article = @article and lv = @lv";
            param.add("orgcode", orgcode);
            param.add("site", site);
            param.add("depot", depot);
            param.add("expdate", expDate.ToString("dd/MM/yyyy"));
            param.add("article", article);
            param.add("lv", lv);

            // Data Adapter
            var dtmfg = connector.GetDtb(sql, param.Values);

            if (dtmfg.Rows.Count == 0) throw new Exception("Product Not Found.");

            // JSON Serialize
            return JsonConvert.SerializeObject(dtmfg.AsEnumerable()
                .Select(x => new { mfgdate = x["mfgdate"], isvalid = (x["isvalid"].ToString() == "1" ? true : false) })
                .SingleOrDefault());
        }
    }
}
