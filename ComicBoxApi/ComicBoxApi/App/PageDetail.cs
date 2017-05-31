using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ComicBoxApi.App
{
    public enum NextPageType
    {
        Page,

        Chapter,

        End
    }

    public class PageDetail
    {
        public string Content { get; private set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public NextPageType NextPageType { get; private set; }

        public PageDetail(string page, NextPageType nextPageType)
        {
            Content = page;
            NextPageType = nextPageType;
        }
    }
}