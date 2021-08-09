using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
namespace snaps.wms.notify {
    public class LineNotify {
        private readonly string _baseuri;
        private string _token { get; set; }
        /// <summary>
        /// proxyservr:port
        /// </summary>
        private string _proxy { get; set; }
        public LineNotify(string token,string proxy) {
            _token = token;
            _proxy = proxy;
            _baseuri = "https://notify-api.line.me/api/notify";
        }
        public async Task Send(string messages,bool sticker = false,bool isOk = true) {
            await RequestLineNotify(messages,sticker,isOk);
        }

        private async Task<bool> RequestLineNotify(string message,bool issticker,bool isOk) {
            try {
                // set proxy
                using(HttpClientHandler handler = new HttpClientHandler())
                using(HttpClient client = new HttpClient(handler)) {
                    if(!string.IsNullOrEmpty(_proxy)) {
                        handler.UseProxy = true;
                        handler.Proxy = new WebProxy(_proxy,true);
                    }
                    client.Timeout = TimeSpan.FromMinutes(1);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",_token);
                    using(var form = new MultipartFormDataContent()) {
                        form.Add(new StringContent(message),"message");
                        if(issticker) {
                            form.Add(new StringContent("11538"),"stickerPackageId");
                            form.Add(new StringContent(isOk ? "51626501" : "51626531"),"stickerId");
                        }

                        Console.WriteLine("Connecting");
                        var response = await client.PostAsync(new Uri(_baseuri),form);
                        Console.WriteLine("Status : {0}",response.StatusCode);
                        // need to get response uncomment here
                        //if(response.IsSuccessStatusCode) {
                            //var contents = await response.Content.ReadAsStringAsync();
                            //var jsconten = JsonConvert.DeserializeObject<lineResponse>(contents);
                            //Console.WriteLine("Send status {0} {1}",jsconten.status,jsconten.message);
                        //}
          
                        return response.IsSuccessStatusCode;
                    }
                }
            } catch(Exception ex) {
                Console.WriteLine("Error : {0} ",ex.Message);
                return false;
            }
        }
    }

    public class lineResponse {
        public int status { get; set; }
        public string message { get; set; }
    }
}
