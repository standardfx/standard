using System.Collections.ObjectModel;

namespace Standard.Data.StringMetrics
{
    public interface ITokenizer
    {
        Collection<string> Tokenize(string word);
        Collection<string> TokenizeToSet(string word);

        string Delimiters { get; }

        ITermHandler StopWordHandler { get; set; }
    }
}
