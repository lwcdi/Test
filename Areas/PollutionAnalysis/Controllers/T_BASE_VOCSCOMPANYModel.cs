using System;

namespace UI.Web.Areas.PollutionAnalysis.Controllers
{
    internal class T_BASE_VOCSCOMPANYModel
    {
        public string ISDEL { get; internal set; }
        public string NAME { get; internal set; }
        public DateTime CREATETIME { get; internal set; }
        public DateTime UPDATETIME { get; internal set; }

        internal bool Update(bool v)
        {
            throw new NotImplementedException();
        }

        internal bool Insert()
        {
            throw new NotImplementedException();
        }
    }
}