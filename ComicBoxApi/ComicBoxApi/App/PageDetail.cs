namespace ComicBoxApi.App
{
    public class PageDetail
    {
        public string Content { get; private set; }

        public string NextPageOrChapter { get; private set; }

        public PageDetail(string page, string nextPageOrChapter)
        {
            Content = page;
            NextPageOrChapter = nextPageOrChapter;
        }
    }
}