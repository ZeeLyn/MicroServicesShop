using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Common
{
    public class ResponseResult<T>
    {
        public bool Success;
        public T Result;
        public string Error;
    }
}
