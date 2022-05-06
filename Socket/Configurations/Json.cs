using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Socket
{
    public static class Json
    {
        public static string ToJson<TIn>(this TIn obj) where TIn : class
        {
            return JsonSerializer.Serialize(obj);
        }

        public static TOut? FromJson<TOut>(this string obj) where TOut : class
        {
            return JsonSerializer.Deserialize<TOut>(obj);
        }
    }
}
