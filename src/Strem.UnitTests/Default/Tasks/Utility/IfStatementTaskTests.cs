using Microsoft.Extensions.Logging;
using Moq;
using Strem.Core.Events.Bus;
using Strem.Core.Flows;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;
using Strem.Flows.Default.Flows.Tasks.Logic;
using Strem.Flows.Default.Flows.Tasks.Utility;

namespace Strem.UnitTests.Default.Tasks.Utility;

public class IfStatementTaskTests
{
    [Theory]
    [InlineData("a", "a", false)]
    [InlineData("a", "1", false)]
    [InlineData("1", "a", false)]
    public void should_return_false_if_neither_can_parse(string valueA, string valueB, bool shouldMatch)
    {
        var mockLogger = new Mock<ILogger<FlowTask<IfStatementTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var mockFlowExecutor = new Mock<IFlowExecutor>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var dummyFlowVars = new Variables();

        var dummyA = "a";
        var dummyB = "b";
        mockFlowStringProcessor
            .Setup(x => x.Process(dummyA, dummyFlowVars))
            .Returns(valueA);
        
        mockFlowStringProcessor
            .Setup(x => x.Process(dummyB, dummyFlowVars))
            .Returns(valueB);
        
        var task = new IfStatementTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, 
            mockEventBus.Object, mockFlowExecutor.Object);

        var data = new IfStatementTaskData
        {
            NumberOperator = OperatorType.Equals,
            ValueA = dummyA,
            ValueB = dummyB
        };

        var matches = task.NumericalComparison(data, dummyFlowVars);
        Assert.Equal(shouldMatch, matches);
    }
    
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, true, true)]
    public void should_correctly_run_failure_flow(bool runFailureFlow, bool hasFlowId, bool shouldHaveCalled)
    {
        var mockLogger = new Mock<ILogger<FlowTask<IfStatementTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var mockFlowExecutor = new Mock<IFlowExecutor>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        
        var task = new IfStatementTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, 
            mockEventBus.Object, mockFlowExecutor.Object);

        var flowGuid = Guid.NewGuid();
        var data = new IfStatementTaskData
        {
            FailureFlowId = hasFlowId ? flowGuid : Guid.NewGuid(),
            RunFlowOnFailure = runFailureFlow
        };
        
        task.PossiblyRunFailureFlow(data);

        var times = shouldHaveCalled ? Times.Once() : Times.Never();
        mockFlowExecutor.Verify(x => x.ExecuteFlow(flowGuid, It.IsAny<IVariables>()), times);
    }
}