using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Common.ReturnMessage
{
    public class Response
    {
        public Response(bool status, string message)
        {
            Status = status;
            Message = message;
        }

        public bool Status { get; set; }

        public string Message { get; set; }
    }
}
