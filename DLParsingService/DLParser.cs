using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using MMA.ADS.CSS.DLParsingService;

namespace MMA.ADS.CSS.DLParsingService
{
    public class DLParser
    {

        static public XDocument ParseDLBarcodeString(string licenseText, string licenseKey = "")
        {
            try
            {
                using (var client = new IDScanWebAPI.DriverLicenseParserClient())
                {
                    XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
                    doc.Add(new XElement("RESPONSE", null));

                    var authKey = string.Empty;

                    if (!string.IsNullOrEmpty(licenseKey))
                    {
                        if ( !licenseKey.Equals("00000000-0000-0000-0000-000000000000" ) )
                            authKey = licenseKey;
                    }
                    else
                    {
                        authKey = ConfigurationManager.AppSettings["authKey"].ToString();
                    }

                    if (string.IsNullOrWhiteSpace(licenseText) == true)
                    {
                        doc.Root.Add(new XElement("SUCCESS", "false"));
                        doc.Root.Add(new XElement("ERROR", "No data found to parse - terminated before calling service"));

                    }
                    else if (string.IsNullOrWhiteSpace(authKey) == true)
                    {
                        doc.Root.Add(new XElement("SUCCESS", "false"));
                        doc.Root.Add(new XElement("ERROR", "No license key found - terminated before calling service"));
                    }
                    else
                    {

                        var result = client.ParseString(authKey, licenseText, null);
                        if (result.Success)
                        {
                            doc.Root.Add(new XElement("SUCCESS", "true"));
                            doc.Root.Add(new XElement("FirstName", result.DriverLicense.FirstName));
                            doc.Root.Add(new XElement("MiddleName", result.DriverLicense.MiddleName));
                            doc.Root.Add(new XElement("LastName", result.DriverLicense.LastName));
                            doc.Root.Add(new XElement("FullName", result.DriverLicense.FullName));
                            doc.Root.Add(new XElement("Address1", result.DriverLicense.Address1));
                            doc.Root.Add(new XElement("Address2", result.DriverLicense.Address2));
                            doc.Root.Add(new XElement("City", result.DriverLicense.City));
                            doc.Root.Add(new XElement("State", result.DriverLicense.JurisdictionCode));
                            doc.Root.Add(new XElement("PostalCode", result.DriverLicense.PostalCode));
                            doc.Root.Add(new XElement("Birthdate", result.DriverLicense.Birthdate));
                            doc.Root.Add(new XElement("Country", result.DriverLicense.Country));
                            doc.Root.Add(new XElement("NameSuffix", result.DriverLicense.NameSuffix));
                            doc.Root.Add(new XElement("NamePrefix", result.DriverLicense.NamePrefix));
                        }
                        else
                        {
                            doc.Root.Add(new XElement("SUCCESS", "false"));
                            doc.Root.Add(new XElement("ERROR", result.ErrorMessage));
                        }
                    }

                    return doc;
                }
            }
            catch (FaultException ex)
            {
                // log this to console for now  - throw for another level to process
                Console.WriteLine(string.Format("Error Message: {0}", ex.Message));
                throw new Exception("DLParser.ParseDLBarcodeString - FaultException " + ex.Message, ex); 
            }

        }

    }
}
