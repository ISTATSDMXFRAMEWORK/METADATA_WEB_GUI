using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ISTATRegistry.Classes
{

    public class JsonMessage
    {
        // messaggi standard di ritorno
        public static string ErrorOccured { get { return "{\"ERRORS\" : true }"; } }
        public static string SessionExpired { get { return "{\"SESSIONEXPIRED\" : true }"; } }
        public static string EmptyJSON { get { return new JavaScriptSerializer().Serialize(string.Empty); } }
        public static string GetError(string error)
        {
            Dictionary<string, object> _return = new Dictionary<string, object>();
            _return.Add("ERRORS", error);
            return new JavaScriptSerializer().Serialize(_return);
        }
        public static string GetMessage(string message)
        {
            Dictionary<string, object> _return = new Dictionary<string, object>();
            _return.Add("message", message);
            return new JavaScriptSerializer().Serialize(_return);
        }
        public static string GetData(object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }
    }


}