using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IMessagingService
    {
        Task PublishAsync<T>(T message, string queueName = null) where T : class;
        void Subscribe<T>(string queueName, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken) where T : class;
    }
}
