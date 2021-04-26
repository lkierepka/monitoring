using System.ComponentModel.DataAnnotations;

namespace WorkerService
{
    public class PostgresConfiguration
    {
        [Required] public string ConnectionString { get; set; }
    }
}