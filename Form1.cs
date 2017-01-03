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
using System.Threading;

namespace ConvertDataToNewFormat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

            var db = new MyCouchClient("http://74.208.129.62:5984/", "members");
            int count = 1;
            foreach (var m in mproc.ml)
            {
                
                dynamic res = await db.Documents.GetAsync(m.id);
                dynamic resd = await db.Documents.DeleteAsync(m.id, res.Rev);
                var json2 = JsonConvert.SerializeObject(m);
                dynamic resw = await db.Documents.PutAsync(m.id, json2);
                Console.WriteLine("sending document" + count);
                count++;
            }

            /*var db = new MyCouchClient("http://localhost:5984/", "newdata");
            
            foreach (var m in mproc.ml)
            {

                /*
                                var r1 = await db.Documents.GetAsync(m.memberID);

                                dynamic r = JsonConvert.DeserializeObject(r1.Content);

                                r.address = m.address;
                                r.city = m.city;
                                r.state = m.state;
                                r.zip = m.zip;
                                r.phone = m.phone;
                                r.email = m.email;

                                var json2 = JsonConvert.SerializeObject(r);
                #1#
                var json2 = JsonConvert.SerializeObject(m);
                

                dynamic response = await db.Documents.PutAsync(m.id, json2);
                var jsonResult = JsonConvert.SerializeObject(response);
                Console.WriteLine(jsonResult);

            }*/


        }
        private void Form1_Load(object sender, EventArgs e)
        {
      
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}
