using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net;

namespace nlp.data
{
    public class NlpException : ApplicationException
    {
        public int StatusCode { get; set; }
        public string ContentType { get; set; }

        public NlpException(int StatusCode)
        {
            this.StatusCode = StatusCode;
        }

        public NlpException(string Message) : base(Message)
        {
            this.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        public NlpException(int StatusCode, string Message) : base(Message)
        {
            this.StatusCode = StatusCode;
        }

        public NlpException(HttpStatusCode StatusCode, string Message) : base(Message)
        {
            this.StatusCode = (int)StatusCode;
        }

        public NlpException(int StatusCode, Exception Inner) : this(StatusCode, Inner.ToString()) { }
        public NlpException(HttpStatusCode StatusCode, Exception Inner) : this(StatusCode, Inner.ToString()) { }
        public NlpException(int StatusCode, JsonElement ErrorObject) : this(StatusCode, ErrorObject.ToString()) { this.ContentType = @"application/problem+json"; }
    }
}
