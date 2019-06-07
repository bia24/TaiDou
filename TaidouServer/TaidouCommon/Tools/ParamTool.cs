using System;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.Text.RegularExpressions;


namespace TaidouCommon.ParamTools
{
    public class ParamTool
    {
        public static T GetParam<T>(ParamCode code,Dictionary<byte,object>param)
        {
            T res=default(T);
            if (res==null)
            {
                object json = null;
                if (param.TryGetValue((byte)code, out json))
                {
                    res = JsonMapper.ToObject<T>((string)json);
                }
            }
            else
            {
                object resobj = null;
                if (param.TryGetValue((byte)code, out resobj))
                {
                    res = (T)resobj;
                }
            }
            return res;
        }
        public static Dictionary<byte, object> ConstructParam<T>(ParamCode code,T t)
        {
            Dictionary<byte, object> param = new Dictionary<byte, object>();
            if (default(T)==null)
            {
                string json = Regex.Unescape(JsonMapper.ToJson(t));
                param.Add((byte)code, json);
            }
            else
            {
                param.Add((byte)code, t);
            }
            return param;
        }
        
    }
}
