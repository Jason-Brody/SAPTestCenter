using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ReportAutoUpload
{
    public class AutoUpload
    {
        public bool StartUpload(string fileName,int assetId)
        {
            var handler = new HttpClientHandler() { UseDefaultCredentials = true };
            string url = "http://c0040597.itcs.hp.com:7860/Report/Upload";
            HttpClient client = new HttpClient(handler);

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent(assetId.ToString()), "asset");
            byte[] fileContent = File.ReadAllBytes(fileName);

            form.Add(new ByteArrayContent(fileContent), "file", fileName);
            var task = client.PostAsync(url, form);
          
            task.Wait();
            var response = task.Result;
            
            client.Dispose();
            handler.Dispose();
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }
    }
}
