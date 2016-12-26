using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using MyCouch;

namespace ConvertDataToNewFormat
{
    public class MemberProc
    {
        public MemberProc()
        {
            ml = new List<Member>();
        }
        public List<Member> ml;
        public List<string> msl;
        XmlNodeList maEls, mEls, pEls, payEls, cEls;
        public void begin()
        {
            
            
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.LoadXml(File.ReadAllText(@"g:\c2016\hubData.xml"));

                maEls = xml.GetElementsByTagName("membership_assignment");
                foreach (XmlNode node in maEls)
                {
                    
                    start(node.ChildNodes);
                }

                mEls = xml.GetElementsByTagName("membership");
                pEls = xml.GetElementsByTagName("person");
                payEls = xml.GetElementsByTagName("payment");
                cEls = xml.GetElementsByTagName("contact");
                foreach (var m in ml)
                {
                    
                    doMembers(m);
                    doPeople(m);
                    addPayments(m);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           




        }

        private void addPayments(Member m)
        {
            
            List<XmlNode> pays = new List<XmlNode>();
            for(int i = 0; i < payEls.Count; i++)
            {
                foreach (XmlNode xm in payEls[i].ChildNodes)
                {
                    if(xm.Name == "personID" && m.personID == xm.InnerText)
                        pays.Add(payEls[i]);
                }
            }
            if (pays.Count > 0)
            {
                m.payments = new List<IPayment>();
                foreach (XmlNode n in pays)
                {
                    var mm = new IPayment();
                    double pay;
                    DateTime outdt;
                    foreach (XmlNode nn in n.ChildNodes)
                    {
                        if (nn.Name == "paymentAmount")
                        {
                            if (double.TryParse(nn.InnerText, out pay))
                                mm.amount = pay;
                        }
                        if (nn.Name == "paymentDate")
                        {
                            if (DateTime.TryParse(nn.InnerText, out outdt))
                                mm.receivedDate = outdt;
                        }
                    }
                    m.payments.Add(mm);
                }
            }
        }

        private void doPeople(Member m)
        {
            foreach (XmlNode xm in pEls)
            {
                for (int i = 0; i < xm.ChildNodes.Count; i++)
                {
                    if (xm.ChildNodes[i].Name == "id" && xm.ChildNodes[i].InnerText == m.personID)
                    {
                        foreach (XmlNode xn in xm.ChildNodes)
                        {

                            if (xn.Name == "firstName")
                                m.firstName = xn.InnerText;
                            if (xn.Name == "lastName")
                                m.lastName = xn.InnerText;
                        }
                        foreach (XmlNode xxn in cEls)
                        {
                            for (int i1 = 0; i1 < xxn.ChildNodes.Count; i1++)
                            {
                                if (xm.ChildNodes[i].Name == "personID" && xm.ChildNodes[i].InnerText == m.personID)
                                {
                                    foreach (XmlNode xnn in xm.ChildNodes)
                                    {

                                        if (xnn.Name == "addressLine1")
                                            m.address = xnn.InnerText;
                                        if (xnn.Name == "addressLine2")
                                            m.address += " " + xnn.InnerText;
                                        if (xnn.Name == "city")
                                            m.city = xnn.InnerText;
                                        if (xnn.Name == "state")
                                            m.state = xnn.InnerText;

                                        if (xnn.Name == "zip")
                                            m.zip = xnn.InnerText;
                                        if (xnn.Name == "phone")
                                            m.phone = xnn.InnerText;
                                        if (xnn.Name == "emailAddress")
                                            m.email = xnn.InnerText;
                                    }
                                }
                            }

                        }

                    }

                }
            }
        }

        public void start(XmlNodeList xml)
        {
            var m = new Member();
            foreach (XmlNode x in xml)
            {
                if (x.Name == "membershipID")
                    m.memberID = x.InnerText;
                if (x.Name == "personID")
                    m.personID = x.InnerText;

            }
            
            ml.Add(m);
        }
        DateTime dt;
        internal void doMembers(Member m)
        {
            foreach (XmlNode xm in mEls)
            {
                for (int i = 0; i < xm.ChildNodes.Count; i++)
                {
                    if (xm.ChildNodes[i].Name == "membershipID" && xm.ChildNodes[i].InnerText == m.memberID)
                    {
                        foreach (XmlNode xn in xm.ChildNodes)
                        {
                            
                            if (xn.Name == "startDate")
                                if(DateTime.TryParse((string)xn.Value, out dt))
                                {
                                    m.joinedDate = dt;
                                }
                        }
                    }

                }
                
            }
        }
        internal void PushToDatabase()
        {
            var db = new MyCouchClient("http://foxjazz:greeper2@localhost:5984/", "members");

            foreach (var m in ml)
            {
                var json = JsonConvert.SerializeObject(m);

                db.Documents.PutAsync(m.memberID, json);


            }
            
            throw new NotImplementedException();
        }
    }
}
