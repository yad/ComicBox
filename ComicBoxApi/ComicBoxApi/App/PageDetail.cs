namespace ComicBoxApi.App
{
    public class PageDetail
    {
        public string Content { get; private set; }

        public PageDetail(string page)
        {
            Content = page;
        }
    }
}