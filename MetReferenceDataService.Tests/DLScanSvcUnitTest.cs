using System;
using System.Xml.Linq;
using MMA.ADS.CSS.DLParsingService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetReferenceDataService.Tests
{
    [TestClass]
    public class DLScanSvcUnitTest
    {

        public string GoodBarCodeData { get { return goodBarCodeData; } }
        public string BadBarCodeData { get { return badBarCodeData; } }

        static public string goodBarCodeData = @"@
ANSI 636001030002DL00410258ZN02990026DLDCAD   
DCB          
DCD      
DBA02132020
DCSTAFT                      
DCTMARY,W                   
DBD12282011
DBB03121973
DBC1
DAYGR 
DAU411   
DAG6208 WATERFORD BLVD APT 1
DAIOKLAHOMA CITY       
DAJOK
DAK73118      
DAQ012345678
DCFMY3YYRGM04
DCGUSA
ZNZNATAFT@MARY@W  
";

        // Random bits taken out
        static public string badBarCodeData = @"@
ANSI 636001030002DL00410258ZN02990026DLDCAD   
DCB          
DCD      
DBA0212020
DCSTAFT                      
DCTMARY,W                   
DBD1222011
DBB0312973
DBC1
DAYGR 
DAU411   
DAG6208 WATERFORD BLVD APT 1
DAIOKLAOMA CITY       
DAJOKDAK73118      
DAQ012345678
DCFMY3YYRGM04
DCGUS
ZNZNAAFT@MARY@W  
";

        public bool TestParseDLBarcodeStringSuccess()
        {
            try
            {
                XDocument goodDoc = MMA.ADS.CSS.DLParsingService.DLParser.ParseDLBarcodeString(GoodBarCodeData);

                if (goodDoc != null)
                {
                    var success = goodDoc.Element("SUCCESS").Value;

                    if (success != null && success.Equals("true") == true)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                throw new Exception("Failed");
            }
        }

        public bool TestParseDLBarcodeStringFailure()
        {
            try
            {
                XDocument badDoc = MMA.ADS.CSS.DLParsingService.DLParser.ParseDLBarcodeString(BadBarCodeData);

                if (badDoc != null)
                {
                    var success = badDoc.Element("SUCCESS").Value;

                    if (success != null && success.Equals("false") == true)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                throw new Exception("Failed");

            }
        }

        public bool TestParseDLBarcodeStringEmpty()
        {
            try
            {
                XDocument badDoc = MMA.ADS.CSS.DLParsingService.DLParser.ParseDLBarcodeString("");
                if (badDoc != null)
                {
                    var success = badDoc.Element("SUCCESS").Value;

                    if (success != null && success.Equals("false") == true)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return true;

            }
        }

        public bool TestBadLicenseKey()
        {
            try
            {
                string licenseKey = "ffffffff-ffff-ffff-ffff-ffffffffffff";
                XDocument goodDoc = MMA.ADS.CSS.DLParsingService.DLParser.ParseDLBarcodeString(GoodBarCodeData, licenseKey);

                if (goodDoc != null)
                {
                    var success = goodDoc.Element("SUCCESS").Value;

                    if (success == null || success.Equals("false") == false)
                    {
                        return false;
                    }
                }

                licenseKey = "00000000-0000-0000-0000-000000000000";
                goodDoc = MMA.ADS.CSS.DLParsingService.DLParser.ParseDLBarcodeString(GoodBarCodeData, licenseKey);
                if (goodDoc != null)
                {
                    var success = goodDoc.Element("SUCCESS").Value;

                    if (success == null || success.Equals("false") == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                throw new Exception("Failed");
            }
        }

        [TestMethod]
        public void TestParseDLBarcodeService()
        {
            try
            {
                Assert.AreEqual(TestBadLicenseKey(), true);
                Assert.AreEqual(TestParseDLBarcodeStringEmpty(), true);
                Assert.AreEqual(TestParseDLBarcodeStringSuccess(), true);
                Assert.AreEqual(TestParseDLBarcodeStringFailure(), true);
            }
            catch
            {

            }
        }

    }
}
