using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using MMA.ADS.CSS.DLParsingService;

namespace MMA.ADS.CSS.DLParsingService
{
    public class ParsingServiceProvider : ParsingServiceProviderBase
    {
        Dictionary<string, object> handledMessages;

        public override void Initialize(string name, NameValueCollection config)
        {
            log.Debug("Initialize() - Entry");
            base.Initialize(name, config);

            connectionString = string.Empty;
            connectionStringName = string.Empty;
            providerInvariantName = string.Empty;

            handledMessages = new Dictionary<string, object>();
            log.Debug("Initialize() - Exit");
        }

        public override bool CanHandle(string request)
        {
            return handledMessages.ContainsKey(request);
        }

        // Retrieves a list of items
        public override string GetItem(string request, params object[] p)
        {
            try
            {
                string p1 = string.Empty;

                if (p != null)
                {
                    if (p.Length > 0 && p[0] != null)
                        p1 = p[0].ToString();
                }

                log.Debug(string.Format("Request {0} with Parameters received ", request ));

                switch (request)
                {
                    case "GetParsedDLFromBarcode":
                        return DLParser.ParseDLBarcodeString(p1).ToString();
                        break;
                    default:
                        break;
                }

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                log.Error("Exception caught :" + ex.Message, ex);
                return string.Format(@"<RESPONSE><RECORD><SuccessFlag>F</SuccessFlag><ERROR_CODE>{0}</ERROR_CODE><ERROR_MESSAGE>{1}</ERROR_MESSAGE></RECORD></RESPONSE>", ex.HResult, ex.Message);
            }
        }

        // creates/updates an item
        public override string PutItem(string request, params object[] p)
        {
            throw new NotImplementedException();
        }

        public override int GetStatus()
        {
            return 0;
        }
    }
}
