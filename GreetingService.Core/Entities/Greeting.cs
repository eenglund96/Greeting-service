using GreetingService.Core;

namespace GreetingService.API.Core
{
    public class Greeting
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Message { get; set; }

        private string _from;
        public string From
        {
            get
            {
                return _from;
            }

            set
            {
                EmailValidator.IsValid(value);

                _from = value;
            }
        }

        private string _to;
        public string To
        {
            get
            {
                return _to;
            }

            set
            {
                EmailValidator.IsValid(value);

                _to = value;
            }
        }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
