using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
namespace snaps.wms.notify {
    public class LineNotify {
        private readonly string _baseurl;
        private string _token { get; set; }
        /// <summary>
        /// proxyservr:port
        /// </summary>
        private string _proxy { get; set; }
        public LineNotify(string token,string proxy) {
            _token = token;
            _proxy = proxy;
            _baseurl = "https://notify-api.line.me/api/notify";
        }
        public async Task Send(string messages,bool isOk = true) {
            await RequestLineNotify(messages,isOk);
        }

        private async Task<bool> RequestLineNotify(string message,bool isOk) {
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
                        form.Add(new StringContent("11538"),"stickerPackageId");
                        form.Add(new StringContent(isOk ? "51626501" : "51626531"),"stickerId");
                        form.Add(new StringContent(message),"message");
                        Console.WriteLine("Connecting");
                        var response = await client.PostAsync(new Uri(_baseurl),form);
                        //var contents = await response.Content.ReadAsStringAsync();
                        //var jsconten = JsonSerializer.Deserialize<lineResponse>(contents);
                        Console.WriteLine("Send status {0}",response.StatusCode);
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
