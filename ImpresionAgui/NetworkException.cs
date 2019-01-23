using System;
using System.Net;

namespace ImpresionAgui
{
    class NetworkException : Exception
    {

        public readonly HttpStatusCode StatusCode;
        public readonly String body;

        public NetworkException(HttpStatusCode statusCode, String body)
        {
            this.StatusCode = statusCode;
            this.body = body;
        }
    }
}
