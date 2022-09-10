using Microsoft.Extensions.Logging;
using Moq;
using Strem.Core.Events.Bus;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Default.Flows.Tasks.Utility;
using Strem.UnitTests.Extensions;

namespace Strem.UnitTests.Default.Tasks.Utility;

public class WriteToLogTaskTests
{
    [Fact]
    public async Task should_be_executable_without_any_variables()
    {
        var mockLogger = new Mock<ILogger<FlowTask<WriteToLogTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var task = new WriteToLogTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object);

        var canExecute = task.CanExecute();
        Assert.True(canExecute);
    }
    
    [Fact]
    public async Task should_attempt_to_pass_process_message_and_pass_to_logger()
    {
        var mockLogger = new Mock<ILogger<FlowTask<WriteToLogTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());

        var inputString = "this is a test";
        var expectedString = "this text has been processed";
        mockFlowStringProcessor
            .Setup(x => x.Process(inputString, It.IsAny<IVariables>()))
            .Returns(expectedString);
        
        var taskData = new WriteToLogTaskData { Text = inputString };
        var flowVars = new Variables();
        var task = new WriteToLogTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object);
        await task.Execute(taskData, flowVars);
        mockLogger.VerifyLogsFor(LogLevel.Information, expectedString);
    }
}