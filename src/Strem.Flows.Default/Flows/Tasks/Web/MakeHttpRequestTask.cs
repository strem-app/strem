using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Data.Tasks;
using Strem.Core.Services.Browsers.Web;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Executors;
using Strem.Flows.Processors;

namespace Strem.Flows.Default.Flows.Tasks.Web;

public class MakeHttpRequestTask : FlowTask<MakeHttpRequestTaskData>
{
    public override string Code => MakeHttpRequestTaskData.TaskCode;
    public override string Version => MakeHttpRequestTaskData.TaskVersion;
    
    public override string Name => "Make A HTTP Request";
    public override string Category => "Web";
    public override string Description => "Makes a HTTP request to a given url";
    
    public static VariableEntry ResponseStatusVariable = new("response.status");
    public static VariableEntry ResponseContentVariable = new("response.content");
    public static VariableEntry ResponseContentTypeVariable = new("response.contentType");
    public static VariableEntry ResponseHeadersVariable = new("response.headers");

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ResponseStatusVariable.ToDescriptor(), ResponseContentVariable.ToDescriptor(),
        ResponseContentTypeVariable.ToDescriptor(), ResponseHeadersVariable.ToDescriptor()
    };

    public IWebBrowser WebBrowser { get; }

    public MakeHttpRequestTask(ILogger<FlowTask<MakeHttpRequestTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IWebBrowser webBrowser) : base(logger, flowStringProcessor, appState, eventBus)
    {
        WebBrowser = webBrowser;
    }

    public override bool CanExecute() => true;
    
    public override async Task<ExecutionResult> Execute(MakeHttpRequestTaskData data, IVariables flowVars)
    {
        var restClient = new RestClient();

        var processedUrl = FlowStringProcessor.Process(data.Url, flowVars);
        var restRequest = new RestRequest(processedUrl, data.Verb);
        
        var headers = data.Headers
            .Select(x => new KeyValuePair<string, string>(x.Name, FlowStringProcessor.Process(x.Value, flowVars)))
            .ToArray();
        
        restRequest.AddHeaders(headers);

        if ((data.Verb is Method.Post or Method.Put) && !string.IsNullOrEmpty(data.Body))
        {
            var content = FlowStringProcessor.Process(data.Body, flowVars);
            restRequest.RequestFormat = data.DataFormat;
            restRequest.AddStringBody(content, data.DataFormat);
        }

        var response = await restClient.ExecuteAsync(restRequest);
        flowVars.Set(ResponseStatusVariable, response.StatusCode.ToString());
        flowVars.Set(ResponseContentVariable, response.Content ?? string.Empty);
        flowVars.Set(ResponseContentTypeVariable, response.ContentType ?? string.Empty);

        var jsonHeaders = JsonConvert.SerializeObject(response.Headers);
        flowVars.Set(ResponseHeadersVariable, jsonHeaders);
        return ExecutionResult.Success();
    }
}