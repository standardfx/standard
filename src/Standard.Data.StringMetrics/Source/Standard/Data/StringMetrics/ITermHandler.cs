using System.Text;

namespace Standard.Data.StringMetrics
{
    public interface ITermHandler
    {
        void AddWord(string termToAdd);
        bool IsWord(string termToTest);
        void RemoveWord(string termToRemove);

        int NumberOfWords { get; }

        StringBuilder WordsAsBuffer { get; }
    }
}