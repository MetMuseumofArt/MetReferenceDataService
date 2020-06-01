using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace MetReferenceDataService.Controllers
{

    
    public class PlainTextFormatter : MediaTypeFormatter
    {
        public PlainTextFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }
        public override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }
        public override bool CanWriteType(Type type)
        {
            return type == typeof(string);
        }
        public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var reader = new StreamReader(stream);
            string value = reader.ReadToEnd();
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(value); return tcs.Task;
        }
        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            var writer = new StreamWriter(stream);
            writer.Write((string)value);
            writer.Flush();
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }
    }


    public class BaseController : ApiController
    {
        private PlainTextFormatter _plainTextFormatter = new PlainTextFormatter();

        public PlainTextFormatter PlainTextFormatter { get { return _plainTextFormatter; } }

        public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes
                = System.Convert.FromBase64String(encodedData);

            string returnValue =
               System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);

            return returnValue;
        }

        public string GetClientConnectionInfo()
        {
            string address = string.Empty;
            string port = string.Empty;
            string name = string.Empty;

            try
            {
                GetConnectionInfo(out address, out port, out name);
            }
            catch (Exception ex)
            {
                address = "Uknown";
                port = "Uknown";
                name = "Uknown";
            }

            return string.Format("name: {0} address:{1} port: {2} ", name, address, port);
        }

        protected void GetConnectionInfo(out string address, out string port, out string name)
        {
            address = string.Empty;
            port = string.Empty;
            name = string.Empty;
            try
            {
                if (Request.Properties.ContainsKey("MS_HttpContext"))
                {
                    HttpContextWrapper ctx = (HttpContextWrapper)Request.Properties["MS_HttpContext"];
                    address = ctx.Request.UserHostAddress;
                    name = ctx.Request.UserHostName;
                }
                else if (Request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
                {
                    RemoteEndpointMessageProperty prop;
                    prop = (RemoteEndpointMessageProperty)this.Request.Properties[RemoteEndpointMessageProperty.Name];
                    address = prop.Address;
                    port = prop.Port.ToString();
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        protected HttpResponseMessage ConvertXmlToHTTPResponse(XDocument xDoc)
        {
            string responseStr = "";
            var mtype1 = new MediaTypeHeaderValue("text/json");
            var mtype2 = new MediaTypeHeaderValue("application/json");
            if (Request.Headers.Accept.Contains(mtype1) == true || Request.Headers.Accept.Contains(mtype2))
            {
                responseStr = JsonConvert.SerializeObject(xDoc, Newtonsoft.Json.Formatting.None);
            }
            else
            {
                responseStr = xDoc.ToString(SaveOptions.DisableFormatting);
            }

            HttpResponseMessage response = Request.CreateResponse(
                HttpStatusCode.OK, responseStr, PlainTextFormatter);
            return response;
        }


        public BaseController()
        {
        }
    }
}
