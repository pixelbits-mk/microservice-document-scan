using Common.Util.Interfaces;

namespace Common.Util.Services
{
    // Application Layer
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
