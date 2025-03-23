namespace WebApi
{
    public interface IPolicySummarizer
    {
        Task<string> SummarizePolicy(string filePath);
    }
}