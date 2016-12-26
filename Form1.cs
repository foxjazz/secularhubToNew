using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using MyCouch;
using System.Net.Http;

namespace ConvertDataToNewFormat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
     
        public static Member populateContact(string xmldata)
        {

            var contact = new Member();
            {
                var data = xmldata.Trim();
                data = data.Replace("\n  ", "");
                data = data.Replace("\n ", "");
                data = data.Replace("\n", "");
                Int32 test;
                var xr = XmlReader.Create(new StringReader(data));

                while (xr.Read())
                {
                    if (xr.Name == "addressLine1")
                        contact.address = xr.ReadInnerXml();
                    if (xr.Name == "city")
                        contact.city = xr.ReadInnerXml();
                    if (xr.Name == "state")
                        contact.state = xr.ReadInnerXml();
                    if (xr.Name == "zip")
                        contact.zip = xr.ReadInnerXml();
                    if (xr.Name == "emailAddress")
                        contact.email = xr.ReadInnerXml();
                    if (xr.Name == "phone")
                        contact.phone = xr.ReadInnerXml();
                    if (xr.Name == "description")
                        contact.Description = xr.ReadInnerXml();
                    if (xr.Name == "notes")
                        contact.Notes = xr.ReadInnerXml();
                    if (xr.Name == "personID" && Int32.TryParse(xr.ReadInnerXml(),out test))
                        popPerson(test, ref contact);
                }
            }

                return contact;
        }

        private static void popPerson(Int32 id, ref Member m)
        {
            var xr1 = new XmlTextReader("hubData.xml");
            bool stay = true;
            Int32 test;
            bool isIN = false;
            while(xr1.Read() && stay)
            {
                if(xr1.Name == "person" && isIN == false)
                {
                    isIN = true;
                    xr1.Read();
                    if(xr1.Name != "id")
                    {
                        xr1.Read();
                    }
                       
                        if ( xr1.Name == "id")
                        {
                            Int32.TryParse(xr1.ReadInnerXml(), out test);
                            if (test == id)
                            {
                                xr1.Read();
                                m.firstName = xr1.ReadInnerXml();
                                xr1.Read();
                                m.lastName = xr1.ReadInnerXml();
                                stay = false;
                            }
                        }
                        

                    
                }
                if(xr1.Name == "person" && isIN)
                {
                    isIN = false;
                }
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            
            
            DoTask(  rtbStatus);

        }
        public static StringContent SetContent(string data)
        {
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            return content;
        }
        public static async Task DoTask( RichTextBox rtbStatus)
        {
            //var result = await db.Documents.PostAsync(data);

            //var http = new HttpClient();
            //Uri uri = new Uri("http://localhost/5984");
            //Uri uriid = new Uri("http://localhost/5984/_uuids");
            MemberProc mproc = new MemberProc();
            mproc.begin();

            XmlTextReader reader = new XmlTextReader("hubData.xml");


            var db = new MyCouchClient("http://foxjazz:greeper2@localhost:5984/", "members");

            foreach (var m in mproc.ml)
            {
                var json = JsonConvert.SerializeObject(m);

                await db.Documents.PutAsync(m.memberID, json);


            }
            

        }
        private void Form1_Load(object sender, EventArgs e)
        {
      
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}
