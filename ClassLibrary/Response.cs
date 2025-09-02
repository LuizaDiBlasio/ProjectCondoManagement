using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Response<T>
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }


        public T Results { get; set; }

        public string? Token { get; set; }

        public DateTime? Expiration { get; set; }

        public bool Requires2FA { get; set; } = false;

        public string? Role { get; set; }
    }
}

