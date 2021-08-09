using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snaps.WMS
{
    public partial class sequence_ops : IDisposable {

        private string sqlseq_msql = "select next value for {0} rsl";
        //Goods receipt number sequence
        private string sqlseqgrno_msql = "seq_grno";

        //Goods receipt transaction sequence number 
        private string sqlseqgrtx_msql = "seq_intx";

        //Advance goods receive number sequence
        private string sqlseqagrn_msql = "seq_agrn";

        //Correction sequence number
        private string sqlseqcorr_msql = "seq_corr";

        //Delivery note sequence number
        private string sqlseqdnno_msql = "seq_dnno";

        //Handerling unit sequence number
        private string sqlseqhuno_msql = "seq_huno";

        //Task seqeunce number
        private string sqlseqtask_msql = "seq_task";

        //Transport Note sequence number 
        private string sqlseqtrno_msql = "seq_trtno";

        //Location sequence no
        private string sqlseqloc_msql  = "seq_locdw";

        //Preparation sequence no 
        private string sqlseqprep_msql = "seq_prep";

        //Stock sequence no
        private string sqlseqstock_msql = "seq_stockid";

        //Count task no 
        private string sqlseqcntk_msql = "seq_cntk";

        //Count task plan 
        private string sqlseqcnpn_msql = "seq_cnpn";

        //Route sequence no
        private string sqlseqroute_msql = "seq_route";

        //Transfer sequence no
        private string sqlseqtransfer_msql = "seq_transfer";

        //Preparatioin set 
        private string sqlseqpreset_msql = "seq_prepset";
    }
}
