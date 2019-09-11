using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Web
{
    public class PostResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public dynamic Data { get; set; }
        public dynamic ExtendData { get; set; }

        public PostResult()
        {
            this.Success = false;
            this.Message = "";
            this.Data = "";
        }
    }
}
