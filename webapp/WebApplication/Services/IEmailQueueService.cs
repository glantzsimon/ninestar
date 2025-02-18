namespace K9.WebApplication.Services
{
    public interface IEmailQueueService
    {
        void ProcessQueue();
    }
}