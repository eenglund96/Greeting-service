namespace GreetingService.API.Core
{
    public class Greeting
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Message { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
