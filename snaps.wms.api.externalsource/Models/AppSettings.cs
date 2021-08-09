using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Snaps.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string Conxstr { get; set;}
    }
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string FromAddress { get; set; }
        public String Account { get; set;} 
        public String Password { get; set;}
    }
    public class BadResult{
        public String codemsg { get; set;} 
        public String descmsg { get; set;}
    }

    public static class HTTPRequestExt { 
        public static String getIP(this Microsoft.AspNetCore.Http.HttpRequest vl) { return vl.HttpContext.Connection.RemoteIpAddress.ToString(); }
        public static resultRequest getResultOk(this String vl){ 
            return new resultRequest(resultState.Ok,vl);
        }
        public static resultRequest getResultEr(this String vl) {
            return new resultRequest(resultState.Error,vl);
        }
    }
    public enum resultState { Ok, Error }
    
    public class resultRequest { 
        public String reqid {get; set;} 
        public resultState state { get; set;}
        public String message { get; set;} 
        public resultRequest(resultState o) { this.state = o; message = ""; reqid = ""; }
        public resultRequest(resultState o,String desc) { this.state = o; message = desc; reqid = ""; }
        public resultRequest(resultState o,String desc, string rqid){ this.state = o; message = desc; reqid = rqid; }
    }

    // public class lov { 
    //     public String desc { get; set; }
    //     public String value { get; set; } 
    //     public String valopnfirst { get; set; }
    //     public String valopnsecond { get; set;}
    //     public String icon { get; set;}
    //     public lov(String val, String dsv) { this.desc = dsv; this.value = val; }
    //     public lov(String val, String dsv, String opnfn) { 
    //         this.desc = dsv; this.value = val; this.valopnfirst = opnfn; 
    //     }
    //     public lov(String val, String dsv, String opnfn, String opnsc) { 
    //         this.desc = dsv; this.value = val; this.valopnfirst = opnfn; this.valopnsecond = opnsc;
    //     }
    //     public lov(String val, String dsv, String opnfn, String opnsc,String icn) { 
    //         this.desc = dsv; this.value = val; this.valopnfirst = opnfn; this.valopnsecond = opnsc; this.icon = icn;
    //     }
    // }
}