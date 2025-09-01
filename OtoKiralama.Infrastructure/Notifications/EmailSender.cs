using System.Threading;
using System.Threading.Tasks;

namespace OtoKiralama.Infrastructure.Notifications
{
    internal sealed class EmailSender
    {
        public Task SendAsync(string to, string subject, string body, CancellationToken ct) => Task.CompletedTask;
    }
}