using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web
{
    /// <summary>
    /// 因子信息
    /// </summary>
    public class ItemInfo
    {
        public string CODE { get; set; }
        public string TITLE { get; set; }
        public string REMARK { get; set; }
        public string CHILD_CODE { get; set; }
        public string CHILD_TITLE { get; set; }
        public string CHILD_REMARK { get; set; }
        //        "d1".Field(BASDIC.CODE)
        //, "d1".Field(BASDIC.TITLE)
        //, "d2".Field(BASDIC.CODE).As("CHILD_CODE")
        //, "d2".Field(BASDIC.TITLE).As("CHILD_ITEMCODE")
    }
}