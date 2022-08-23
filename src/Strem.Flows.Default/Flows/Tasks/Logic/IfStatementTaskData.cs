﻿using Newtonsoft.Json;
using Strem.Core.Flows;
using Strem.Core.Flows.Tasks;
using Strem.Core.Types;
using Strem.Flows.Default.Types;

namespace Strem.Flows.Default.Flows.Tasks.Logic;

public class IfStatementTaskData : IFlowTaskData, IHasSubTaskData
{
    public static readonly string TaskCode = "if-statement";
    public static readonly string TaskVersion = "1.0.0";
    public static readonly string MatchSubTaskKey = "match";
    public static readonly string NoMatchSubTaskKey = "no-match";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string ValueA { get; set; } = string.Empty;
    public string ValueB { get; set; } = string.Empty;
    public ComparisonType ComparisonType { get; set; }
    public OperatorType NumberOperator { get; set; }
    public TextMatchType TextOperator { get; set; }

    [JsonIgnore]
    public string[] SubTaskKeys { get; } = new[] { MatchSubTaskKey, NoMatchSubTaskKey };
    public Dictionary<string, List<IFlowTaskData>> SubTasks { get; set; }

    public IfStatementTaskData()
    {
        SubTasks ??= SubTaskKeys.ToDictionary(x => x, x => new List<IFlowTaskData>());
    }
}