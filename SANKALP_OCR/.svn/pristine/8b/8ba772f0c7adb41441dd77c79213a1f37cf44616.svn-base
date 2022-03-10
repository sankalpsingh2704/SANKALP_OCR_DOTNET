using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace InvoiceNew.DataAccess
{
    public class ReadTallyData
    {
        public string  ReadXMLFile(string fPath)
        {

            XmlDocument d = new XmlDocument();
            fPath = @"D:\InvoiceNew\InvoiceNew\Tally\Accounting Voucher.xml";
            System.IO.File.ReadAllText(fPath);
            d.LoadXml(fPath);
            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            d.WriteTo(tx);
          return sw.ToString();
        }
    }
}