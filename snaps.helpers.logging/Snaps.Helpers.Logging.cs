using System;
using Snaps.Helpers.Json;
using System.Diagnostics;

namespace Snaps.Helpers.Logging
{
    public class SnapsLogInfo { 
        public DateTime logdate { get; set; }
        public String logtype { get; set; } 
        public String instance { get; set; }
        public String pools { get; set;}
        public String apps { get; set; }
        public String method { get; set; }
        public String accncode { get; set; }
        public String client { get; set; }
        public String reqid { get; set; }
        public String message { get; set;}

    }
    public class SnapsLogExc : SnapsLogInfo
    {    
        public String stacktrace { get; set; }        
        public String objectops { get; set; }
        public String hresult { get; set; }
        public String source { get; set; }        
    }

    public class SnapsBadReq { 
        public string errid { get; set;}
        public string message { get; set;}
        public SnapsBadReq(){}
        
        public SnapsBadReq(Exception ex){ 
            this.errid = ex.HResult.ToString();
            this.message = ex.Message.ToString();
        }
    }

    public class SnapsLogDbg : SnapsLogInfo, IDisposable { 
        public String objectID { get; set; }
        public Int32 sessionID { get; set;}
        public String processName { get; set;}
        public String methodName { get; set;}
        public String instaceName { get; set;}
        
        public TimeSpan startCpuUsage { get; set;}
        public DateTime startTime { get; set;}
        public DateTime endTime { get; set; }
        public TimeSpan endCpuUsage { get; set; }

        public Double startMem { get; set;}
        public Double endMem { get; set;}
        public Double cpuUsedMs { get; set;}
  
        public Double totalMsPassed  { get; set;}
        public Double cpuUsage { get; set; }
        public Double memUsage { get; set; }
        public Double memPeak { get; set; }
        public Double memActive { get; set;}

        public SnapsLogDbg(Process proID,String medth, String objID,String ip, String app,String accn ) {
            logtype = "PERF";
            startTime = DateTime.UtcNow;
            processName = proID.ProcessName;
            instaceName = System.Environment.MachineName.ToString();
            sessionID = proID.SessionId;
            methodName = medth;
            objectID= objID;
            reqid = objID;
            client = ip;
            accncode = accn;
            apps = app;
            startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            startMem = Process.GetCurrentProcess().PrivateMemorySize64;
            memActive = Process.GetCurrentProcess().PrivateMemorySize64 / (1024*1024);
        }
        public void snap(){
            endTime = DateTime.UtcNow;
            endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            memPeak = Process.GetCurrentProcess().PeakPagedMemorySize64 / (1024*1024);
            endMem = Process.GetCurrentProcess().PrivateMemorySize64  ;
            cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            totalMsPassed = (endTime - startTime).TotalMilliseconds;
            cpuUsage = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            cpuUsage = Math.Round(cpuUsage * 100,2);
            memUsage = (endMem - startMem) / (1024*1024);
        }
        //public String toJson() { return this.toJson(); }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                    objectID = null;
                    sessionID = 0;
                    processName = null;
                    instaceName = null;
                    startCpuUsage = TimeSpan.Zero;
                    endCpuUsage = TimeSpan.Zero;
                    startMem = 0;
                    endMem = 0;
                    cpuUsedMs = 0;
                    totalMsPassed = 0;
                    cpuUsage = 0;
                    memUsage =0;
                    memPeak = 0;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~test()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
            GC.Collect();
        }
        #endregion
    
    }

    public static class LogExtention  { 
        public static String SnapsLogInfo(this String vl,String client, String accn,String app, String meth, String rqid) {
            return new SnapsLogInfo()  {
                logdate = DateTime.Now, 
                logtype = "Info",
                instance = System.Environment.MachineName.ToString(),
                pools = System.Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process),
                apps = app,
                method = meth,
                accncode = accn,
                reqid = rqid,
                message = vl
            }.toJson();
        }
        public static String SnapsLogExc(this System.Exception ex,String ip, String accn,String app, String meth, String rqid, object ob) {
            return new SnapsLogExc() {
                logdate = DateTime.Now, 
                logtype = "Error",
                instance = System.Environment.MachineName.ToString(),
                pools = System.Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process),
                apps = app,
                method = meth,
                accncode = accn,
                hresult = ex.HResult.ToString(),
                message = (ex.InnerException != null) ? ex.InnerException.Message.ToString() : ex.Message.ToString(),
                stacktrace = ex.StackTrace,
                source = ex.Source,
                objectops = ob.toJson(),
                client = ip
            }.toJson();
        }
        public static String SnapsBadRequest(this System.Exception ex) {    
            if (ex.InnerException != null) { 
                return new SnapsBadReq() { message = ex.InnerException.Message.ToString(), errid = ex.HResult.ToString()}.toJson();
            }else { 
                return new SnapsBadReq() { message = ex.Message.ToString(), errid = ex.HResult.ToString()}.toJson();
            }  
        }

        public static String SnapsBadRequest(this String vl, String id){ 
            return new SnapsBadReq() { message = vl, errid = id}.toJson();
        }
    }

}
