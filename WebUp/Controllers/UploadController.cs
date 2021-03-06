﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebUp.Models;

namespace WebUp.Controllers
{
    public class UploadController : ApiController
    {

        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> PostFile()
        {
            var request = HttpContext.Current.Request;
            HttpResponseMessage result = null;
            if (request.Files.Count == 0)
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest,"Please select a file... ");
                return result;
            }


            for (int i = 0; i < request.Files.Count; i++)
            {
                HttpPostedFile file = request.Files[i]; //Uploaded file
                //var filecontent = file.InputStream;
                //int fileSize = file.ContentLength;
                string fileName = file.FileName;
                //string mimeType = file.ContentType;

                var order = new Order
                {
                    property = "Richard Duan",
                    action = "order created",
                    file = fileName
                };

                StreamReader csvreader = new StreamReader(file.InputStream);
                
                UpLoadResult upLoadResult=new UpLoadResult
                {
                    TotalNum = 0,
                    UploadedNum = 0,
                    failedNum = 0

                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://evilapi.afdcloud.com.au/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    while (!csvreader.EndOfStream)
                    {
                        var line = csvreader.ReadLine();
                        if (line != "")
                        {
                            var values = line.Split(',');
                            order.customer = values[0];
                            order.value = values[1];
                            upLoadResult.TotalNum++;

                
                            var response = await client.PostAsJsonAsync("/upload", order);
                            if (response.IsSuccessStatusCode)
                            {
                                upLoadResult.UploadedNum++;
                            }
                            else
                            {
                                upLoadResult.failedNum++;
                            }
                        }
                    }
                    
                }
                string rs = "Total Orders: " + upLoadResult.TotalNum + " ;     Successfully uploaded: " +
                                upLoadResult.UploadedNum;
                result = Request.CreateResponse(HttpStatusCode.Created, rs);
            
            }
            return result;
        }


       
    }
}
