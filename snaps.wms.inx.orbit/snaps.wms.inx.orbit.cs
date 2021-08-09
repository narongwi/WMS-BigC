using System;
using System.Threading.Tasks;
using Snaps.WMS;
namespace snaps.wms.inx.orbit
{
    partial class Program
    {
        static void retrive_thirdparty() {
            orbit_ops oo = new orbit_ops(cnx_snapswms, cnx_legacysource);
            try { 
                oo.retrive_thirdparty();
            }catch (Exception ex) { 
                throw ex;
            }finally { oo.Dispose(); }
        }
        static void retrive_product(){ 
            orbit_ops oo = new orbit_ops(cnx_snapswms, cnx_legacysource);
            try { 
                oo.retrive_product();
            }catch (Exception ex) { 
                throw ex;
            }finally { oo.Dispose(); }
        }
        static void retrive_inbound(){ 
            orbit_ops oo = new orbit_ops(cnx_snapswms, cnx_legacysource);
            try { 
                oo.retrive_inbound();
            }catch (Exception ex) { 
                throw ex;
            }finally { oo.Dispose(); }
        }
        static void retrive_outbound(string notifyToken = "",string notifyProxy = "",bool errorOnly = true) {
            orbit_ops oo = new orbit_ops(cnx_snapswms, cnx_legacysource);
            try { 
                oo.retrive_outbound(notifyToken,notifyProxy,errorOnly);
            }catch (Exception ex) { 
                throw ex;
            }finally { oo.Dispose(); }
        }
        static void retrive_barcode() { 
            orbit_ops oo = new orbit_ops(cnx_snapswms, cnx_legacysource);
            try { 
                oo.retrive_barcode();
            } catch (Exception ex){ 
                throw ex;
            } finally { oo.Dispose(); }
        }
        static void crunching_barcode() { 
            orbit_ops oo = new orbit_ops(cnx_snapswms,cnx_legacysource);
            try { 
                oo.crunch_barcode();
            }catch (Exception ex) { 
                throw ex;
            }finally { oo.Dispose(); }
        }
        static void crunching_inbound() { 
            orbit_ops oo = new orbit_ops(cnx_snapswms,cnx_legacysource);
            try { 
                oo.crunch_inbound();
            }catch (Exception ex) { 
                throw ex;
            }finally { oo.Dispose(); }
        }
        static void crunching_outbound() { 
            orbit_ops oo = new orbit_ops(cnx_snapswms,cnx_legacysource);
            try { 
                oo.crunch_outbound();
            }catch (Exception ex) { 
                throw ex;
            }finally { oo.Dispose(); }
        }
        static void crunching_thirdparty() {             
            orbit_ops oo = new orbit_ops(cnx_snapswms, cnx_legacysource);
            try { 
                oo.crunch_thirdparty();
            }catch (Exception ex) { 
                throw ex;
            }finally { oo.Dispose(); }
        }
        static void crunching_product() { 
            orbit_ops oo = new orbit_ops(cnx_snapswms, cnx_legacysource);
            try { 
                oo.crunch_product();
            }catch (Exception ex) { 
                throw ex;
            }finally { oo.Dispose(); }            
        }
        static void launching_receipt() { 
            orbit_ops oo = new orbit_ops(cnx_snapswms, cnx_legacysource);
            try { 
                oo.luanch_receipt();
            }catch (Exception ex) { 
                throw ex;
            }finally { oo.Dispose(); }
        }
        static void launching_correction(){ 
            orbit_ops oo = new orbit_ops(cnx_snapswms, cnx_legacysource);
            try { 
                oo.luanch_correction();
            }catch (Exception ex) { 
                throw ex;
            }finally { oo.Dispose(); }
        }
        static void launching_delivery() {
            orbit_ops oo = new orbit_ops(cnx_snapswms, cnx_legacysource);
            try { 
                oo.luanch_delivery();
            }catch (Exception ex) { 
                throw ex;
            }finally { oo.Dispose(); }
        }
        static void launching_block() {
            orbit_ops oo = new orbit_ops(cnx_snapswms, cnx_legacysource);
            try { 
                oo.launch_block();
            }catch (Exception ex) { 
                throw ex;
            }finally { oo.Dispose(); }
        }
        static void launching_imstock() {
            orbit_ops oo = new orbit_ops(cnx_snapswms,cnx_legacysource);
            try {
                oo.launch_imstock();
            } catch(Exception ex) {
                throw ex;
            } finally { oo.Dispose(); }
        }
        static void hourly_stock() { 
            statisic_ops op = new statisic_ops();
            try { 
                op.Hourly(cnx_snapswms);
            }catch (Exception ex) { 
                throw ex;
            }finally { op.Dispose(); }
        }
        static void purge_current() {
            statisic_ops op = new statisic_ops();
            try {
                op.Purgecurrent(cnx_snapswms);
                Console.WriteLine("purge_current Success");
            } catch(Exception ex) {
                throw ex;
            } finally { op.Dispose(); }
        }
        static void purge_history() {
            statisic_ops op = new statisic_ops();
            try {
                op.Purgehistory(cnx_snapswms);
                Console.WriteLine("purge_history Success");
            } catch(Exception ex) {
                throw ex;
            } finally { op.Dispose(); }
        }
        static void generate_preproute() {
            statisic_ops op = new statisic_ops();
            try {
                op.GeneratePrepRoute(cnx_snapswms);
                Console.WriteLine("generate_preproute Success");
            } catch(Exception ex) {
                throw ex;
            } finally { op.Dispose(); }
        }
    }
}