using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace snaps.wms.api.report.Models
{
    public class DateValueModel
    {
        private string _day;
        public string Day
        {
            get { return _day; }
            set { _day = value.PadLeft(2, '0'); }
        }
        private string _month;

        public string Month
        {
            get { return _month; }
            set { _month = value.PadLeft(2, '0'); }
        }

        private string year;

        public string Year
        {
            get { return year; }
            set { year = value; }
        }

    }
}
