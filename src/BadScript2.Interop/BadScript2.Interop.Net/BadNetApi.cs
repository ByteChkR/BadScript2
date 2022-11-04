using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Net
{
    public class BadNetApi : BadInteropApi
    {
        public BadNetApi() : base("Net") { }

        public override void Load(BadTable target)
        {
            target.SetFunction<string>("Get", Get);
            target.SetFunction<string, string>("Post", Post);
            target.SetFunction<string>("EncodeUriComponent", s => Uri.EscapeDataString(s));
            target.SetFunction<string>("DecodeUriComponent", s => Uri.UnescapeDataString(s));
        }


        private BadTask Post(string url, string content)
        {
            HttpClient cl = new HttpClient();

            return new BadTask(BadTaskUtils.WaitForTask(cl.PostAsync(url, new StringContent(content))), $"Net.Post(\"{url}\")");
        }

        private BadTask Get(string url)
        {
            HttpClient cl = new HttpClient();
            Task<HttpResponseMessage>? task = cl.GetAsync(url);

            return new BadTask(BadTaskUtils.WaitForTask(task), $"Net.Get(\"{url}\")");
        }
    }
}