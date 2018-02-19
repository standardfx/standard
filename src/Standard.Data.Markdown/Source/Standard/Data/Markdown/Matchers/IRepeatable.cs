namespace Standard.Data.Markdown.Matchers
{
    internal interface IRepeatable
    {
        Matcher Repeat(int minOccur, int maxOccur);
    }
}
