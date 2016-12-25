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

        private static Member populateContact(string xmldata)
        {
            var m = new Member();
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
                        m.address = xr.ReadInnerXml();
                    if (xr.Name == "city")
                        m.city = xr.ReadInnerXml();
                    if (xr.Name == "state")
                        m.state = xr.ReadInnerXml();
                    if (xr.Name == "zip")
                        m.zip = xr.ReadInnerXml();
                    if (xr.Name == "emailAddress")
                        m.email = xr.ReadInnerXml();
                    if (xr.Name == "phone")
                        m.phone = xr.ReadInnerXml();
                    if (xr.Name == "description")
                        m.Description = xr.ReadInnerXml();
                    if (xr.Name == "notes")
                        m.Notes = xr.ReadInnerXml();
                    if (xr.Name == "personID" && Int32.TryParse(xr.ReadInnerXml(),out test))
                        popPerson(test, ref m);
                }
            }

                return m;
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
            var db = new MyCouchClient("http://foxjazz:greeper2@localhost:5984/", "members");
            
            DoTask(  rtbStatus,db);

        }
        public static StringContent SetContent(string data)
        {
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            return content;
        }
        public static async Task DoTask( RichTextBox rtbStatus, MyCouchClient db)
        {
            //var result = await db.Documents.PostAsync(data);

            //var http = new HttpClient();
            //Uri uri = new Uri("http://localhost/5984");
            //Uri uriid = new Uri("http://localhost/5984/_uuids");
            

            XmlTextReader reader = new XmlTextReader("hubData.xml");
            int i = 0;
            List<Member> members = new List<Member>();

            while (reader.Read())
            {

                //rtbStatus.AppendText(reader.NodeType.ToString());
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "contact")
                {

                    //                    rtbStatus.AppendText(reader.Name + "  " + reader.Value);
                    var newm = populateContact(reader.ReadOuterXml());


                    string jsondata = Newtonsoft.Json.JsonConvert.SerializeObject(newm);
                    //i++;
                    try
                    {
                        var response = await db.Documents.PostAsync(jsondata);
                        string jresponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                        rtbStatus.AppendText(jresponse);

                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                    //var result = await db.Documents.PostAsync(jsondata);
                    //try
                    //{
                    //    var httpr = await http.GetAsync(uriid);
                    //    if (httpr.IsSuccessStatusCode)
                    //    {
                    //        var response = await http.PutAsync(uri, SetContent(jsondata)).ContinueWith((postTask) => postTask.Result.EnsureSuccessStatusCode());
                    //        string jresponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                    //        rtbStatus.AppendText(jresponse);
                    //    }
                        
                    //}
                    //catch(Exception ex)
                    //{
                    //    Console.Write(ex.Message);
                    //}
                    
                    rtbStatus.AppendText("\r\n");
                }
             
                rtbStatus.Refresh();

                if (i > 50)
                    return;
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
