using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Infrastructure.OwsServiceClass.OwsHelper
{
    public class Helper
    {
        public static string Get8Digits()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
            return String.Format("{0:D8}", random);
        }
        public CustomHeader GetOGHeader(string HotelDoman, string Username, string Password, string DestinationEntityID, string DestinationSystemType, string SourceEntityID, string SourceSystemType)
        {
            CustomHeader OgHeader = new CustomHeader(getOGHeaderXML(HotelDoman, Username, Password, DestinationEntityID, DestinationSystemType, SourceEntityID,
                   SourceSystemType), "OGHeader", "http://webservices.micros.com/og/4.3/Core/");
            List<CusttomHeaderAttributes> attributesList = new List<CusttomHeaderAttributes>();
            CusttomHeaderAttributes CustomAttributes = new CusttomHeaderAttributes();
            CustomAttributes.AttributPrefix = null;
            CustomAttributes.AttributeLocalName = "transactionID";
            CustomAttributes.Attributens = null;
            CustomAttributes.Value = Get8Digits();//"3297907325" // RandomNumber
            attributesList.Add(CustomAttributes);
            CustomAttributes = new CusttomHeaderAttributes();
            CustomAttributes.AttributPrefix = null;
            CustomAttributes.AttributeLocalName = "timeStamp";
            CustomAttributes.Attributens = null;
            CustomAttributes.Value = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            attributesList.Add(CustomAttributes);
            OgHeader.Attributes = attributesList;
            return OgHeader;
        }

        public CustomHeader getSecurityHeader(string UserName, string Password)
        {
            CustomHeader securityHeader = new CustomHeader(getSecurityHeaderXML(UserName, Password), "wsse:Security", "");
            List<CusttomHeaderAttributes> attributesList = new List<CusttomHeaderAttributes>();
            CusttomHeaderAttributes CustomAttributes = new CusttomHeaderAttributes();
            CustomAttributes.AttributPrefix = "xmlns";
            CustomAttributes.AttributeLocalName = "wsse";
            CustomAttributes.Attributens = null;
            CustomAttributes.Value = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
            attributesList.Add(CustomAttributes);
            CustomAttributes = new CusttomHeaderAttributes();
            CustomAttributes.AttributPrefix = "xmlns";
            CustomAttributes.AttributeLocalName = "wsu";
            CustomAttributes.Attributens = null;
            CustomAttributes.Value = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
            attributesList.Add(CustomAttributes);
            securityHeader.Attributes = attributesList;
            return securityHeader;
        }
        private XmlDocument getSecurityHeaderXML(string UserName, string Password)
        {
            XmlDocument Xdoc = new XmlDocument();
            Models.Header.Security SecurityXMl = new Models.Header.Security();
            Models.Header.UsernameToken UNToken = new Models.Header.UsernameToken();
            Models.Header.Password Pass = new Models.Header.Password();

            Pass.Text = Password;
            Pass.Type = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText";
            UNToken.Password = Pass;

            UNToken.Username = UserName;
            SecurityXMl.UsernameToken = UNToken;
            SecurityXMl.Wsse = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
            SecurityXMl.Wsu = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
            Models.Header.Timestamp TStamp = new Models.Header.Timestamp();
            TStamp.Created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "Z";
            TStamp.Expires = DateTime.UtcNow.AddMinutes(5).ToString("yyyy-MM-ddTHH:mm:ss") + "Z";
            SecurityXMl.Timestamp = TStamp;
            XmlSerializer ser = new XmlSerializer(SecurityXMl.GetType());
            using (MemoryStream memStm = new MemoryStream())
            {
                ser.Serialize(memStm, SecurityXMl);

                memStm.Position = 0;

                XmlReaderSettings Readersettings = new XmlReaderSettings();
                Readersettings.IgnoreWhitespace = true;

                using (var xtr = XmlReader.Create(memStm, Readersettings))
                {
                    Xdoc = new XmlDocument();
                    Xdoc.Load(xtr);
                }
            }
            Xdoc.RemoveChild(Xdoc.FirstChild);
            return Xdoc;
        }

        private XmlDocument getOGHeaderXML(string HotelDoman, string Username, string Password, string DestinationEntityID, string DestinationSystemType, string SourceEntityID, string SourceSystemType)
        {
            XmlDocument Xdoc = new XmlDocument();
            Models.Header.OGHeader OGHeader = new Models.Header.OGHeader();
            Models.Header.Authentication Authentication = new Models.Header.Authentication();
            Authentication.Xmlns = "http://webservices.micros.com/og/4.3/Core/";
            Models.Header.UserCredentials UserCredentials = new Models.Header.UserCredentials();
            //UserCredentials.Domain = "OWS";
            UserCredentials.Domain = HotelDoman;//"CLA";
            UserCredentials.UserName = Username;//"CLA_SAMSOTECH@OHC01-OHO-PROD-AD.OHRC";
            UserCredentials.UserPassword = Password;// "c9eS2iFkAP!";
            Authentication.UserCredentials = UserCredentials;
            Authentication.Xmlns = "http://webservices.micros.com/og/4.3/Core/";
            OGHeader.Authentication = Authentication;
            Models.Header.Destination Destination = new Models.Header.Destination();
            Destination.EntityID = DestinationEntityID;//"KIOSK";
            Destination.SystemType = DestinationSystemType;//"PMS";
            OGHeader.Destination = Destination;
            Models.Header.Origin Orgin = new Models.Header.Origin();
            Orgin.EntityID = SourceEntityID;//"KIOSK";
            Orgin.SystemType = SourceSystemType;// "KIOSK";
            OGHeader.Origin = Orgin;
            XmlSerializer ser = new XmlSerializer(OGHeader.GetType());
            //XmlDocument xd = null;

            using (MemoryStream memStm = new MemoryStream())
            {
                ser.Serialize(memStm, OGHeader);

                memStm.Position = 0;

                XmlReaderSettings Readersettings = new XmlReaderSettings();
                Readersettings.IgnoreWhitespace = true;

                using (var xtr = XmlReader.Create(memStm, Readersettings))
                {
                    Xdoc = new XmlDocument();
                    Xdoc.Load(xtr);
                }
            }
            Xdoc.RemoveChild(Xdoc.FirstChild);
            return Xdoc;
        }

    }
}
