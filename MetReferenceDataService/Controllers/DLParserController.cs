using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MMA.ADS.CSS.DLParsingService;


namespace MetReferenceDataService.Controllers
{
    public class DLParserController : BaseController
    {
        public HttpResponseMessage Get()
        {
            try
            {
                throw new NotImplementedException("Must supply an Drivers License");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, ex);
            }
        }

        public HttpResponseMessage GetBylicenseText(string licenseText)
        {
            try
            {
                // Initialize the provider
                ParsingServiceProvider provider = (ParsingServiceProvider)ParsingServiceProvider.DefaultProvider;
                provider.LogInfo(string.Format("{0}", GetClientConnectionInfo()));
                provider.LogDebug(string.Format("encoded <{0}>", Obfuscator.Encode(licenseText, Obfuscator.getKey())));

                // Decode the input string from base64
                object[] objArray = new object[] { (object)DecodeFrom64(licenseText) };
                provider.LogDebug(string.Format("decoded <{0}>", Obfuscator.Encode(DecodeFrom64(licenseText), Obfuscator.getKey())));

                // Call the provider to parse
                var xDoc = XDocument.Parse(provider.GetItem("GetParsedDLFromBarcode", objArray));
                provider.LogDebug(string.Format("parsed xml <{0}>", Obfuscator.Encode(xDoc.ToString(), Obfuscator.getKey())));

                // return to client
                return ConvertXmlToHTTPResponse(xDoc);
            }
            catch (Exception ex)
            {
                // Simply return
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
