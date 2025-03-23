namespace WebApi
{
    public interface IPolicySummarizer
    {
        Task<String> SummarizePolicy(string filePath);
    }
}
