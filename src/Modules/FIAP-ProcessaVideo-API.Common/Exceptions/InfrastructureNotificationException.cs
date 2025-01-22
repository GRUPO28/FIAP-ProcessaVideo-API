using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Common.Exceptions;

public class InfrastructureNotificationException : NotificationException
{
    public InfrastructureNotificationException(string message) : base(message)
    {
    }
}
