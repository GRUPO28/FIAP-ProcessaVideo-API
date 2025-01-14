using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Infrastructure.Repositories.DynamoDb;

public class DatabaseSettings
{
    public const string KeyName = "Database";

    public string TableName { get; set; } = string.Empty;
}
