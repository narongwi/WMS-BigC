using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace snaps.wms.inx.orbit
{
    partial class Program
    {
        public static string cnx_snapswms;
        public static string cnx_legacysource;
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("SnapsConfiguration.json");


            var configuration = builder.Build();
            cnx_snapswms = configuration["cnx_snapswms"];
            cnx_legacysource = configuration["cnx_legacysource"];
            string proxyServer = configuration["notify:proxy"];
            string notifyToken = configuration["notify:token"];
            bool errorOnly = Convert.ToBoolean(configuration["notify:errorOnly"]);

            if (args.Length > 0) { 
                switch(args[0].ToString()) {
                    case "retrive_barcode" : retrive_barcode(); break;
                    case "retrive_thirdparty" : retrive_thirdparty(); break;
                    case "retrive_product" : retrive_product(); break;
                    case "retrive_outbound" : retrive_outbound(notifyToken,proxyServer,errorOnly); break;
                    case "retrive_inbound" : retrive_inbound(); break;

                    case "crunching_thirdparty" : crunching_thirdparty(); break;
                    case "crunching_product" : crunching_product(); break;
                    case "crunching_outbound" : crunching_outbound(); break;
                    case "crunching_inbound" : crunching_inbound(); break;
                    case "crunching_barcode" : crunching_barcode(); break;

                    case "launching_receipt" : launching_receipt(); break;
                    case "launching_delivery" : launching_delivery(); break;
                    case "launching_correction" : launching_correction(); break;
                    case "launching_block" : launching_block(); break;
                    case "launching_imstock": launching_imstock(); break;
                    case "houly" : hourly_stock(); break;
                    case "purge_current": purge_current(); break;
                    case "purge_history": purge_history(); break;
                    case "gen_preproute": generate_preproute(); break;

                    // Merege for Product Master 
                    case "product":
                        retrive_product();
                        Console.WriteLine("Retrive Product Success");
                        crunching_product();
                        Console.WriteLine("Crunching Product Success");
                        retrive_barcode();
                        Console.WriteLine("Retrive Barcode Success");
                        crunching_barcode();
                        Console.WriteLine("Crunching Barcode Success");
                        break;
                    // Merege for Inbound 
                    case "inbound":
                        retrive_inbound();
                        Console.WriteLine("Retrive Inbound Success");
                        crunching_inbound();
                        Console.WriteLine("Crunching Inbound Success");
                        break;
                    // Merege for Outbound 
                    case "outbound":
                        retrive_outbound();
                        Console.WriteLine("Retrive Outbound Success");
                        crunching_outbound();
                        Console.WriteLine("Crunching Outbound Success");
                        break;
                    // Merege for Thirdparty 
                    case "thirdparty":
                        retrive_thirdparty();
                        Console.WriteLine("Retrive Thirdparty Success");
                        crunching_thirdparty();
                        Console.WriteLine("Crunching Thirdparty Success");
                        break;
                }
                
                Console.WriteLine("corrent select args " + args[0]);
            }
        }
    }
}
