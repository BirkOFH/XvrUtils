using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Xvr.Utils.Network
{
    public class HttpServiceResponse<T>
    {
        public HttpServiceResponse(HttpStatusCode status, string? message=default, T? value = default)
        {
            Status = status;
            Message = message;
            Value = value;
        }

        public HttpStatusCode Status { get; set; }
        public string? Message { get; set; }
        public T? Value { get; set; }

    }
}
