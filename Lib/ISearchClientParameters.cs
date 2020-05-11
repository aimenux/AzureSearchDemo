namespace Lib
{
    public interface ISearchClientParameters
    {
        int? Top { get; }
        string Filter { get; }
    }
}