using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;

namespace Strem.Twitter.Flow.Tasks;

public class SendTweetTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "send-tweet-task-data";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string TweetContent { get; set; }
}