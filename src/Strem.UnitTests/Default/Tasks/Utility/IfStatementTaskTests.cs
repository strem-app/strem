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
    [InlineData("a", "a", OperatorType.Equals, false)]
    [InlineData("a", "1", OperatorType.Equals, false)]
    [InlineData("1", "1", OperatorType.Equals, true)]
    public void should_process_values_numerically(string valueA, string valueB, OperatorType operatorType, bool shouldMatch)
    {
        var mockLogger = new Mock<ILogger<FlowTask<IfStatementTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var mockFlowExecutionEngine = new Mock<IFlowExecutionEngine>();
        var mockFlowStore = new Mock<IFlowStore>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var dummyFlowVars = new Variables();
        
        var task = new IfStatementTask(
            mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, 
            mockEventBus.Object, mockFlowExecutionEngine.Object, mockFlowStore.Object);

        var data = new IfStatementTaskData
        {
            NumberOperator = OperatorType.Equals,
            ValueA = "100",
            ValueB = "100"
        };

        var matches = task.NumericalComparison(data, dummyFlowVars);
        Assert.Equal(false, false);

    }
}