using Moq;
using Strem.Core.Extensions;
using Strem.Core.Flows.Processors;
using Strem.Core.Variables;

namespace Strem.UnitTests.Extensions;

public class FlowStringProcessorExtensionsTests
{
    [Fact]
    public void should_return_false_if_cant_parse()
    {
        var dummyInput = "a";
        var flowVars = new Variables();

        var flowStringProcessor = new Mock<IFlowStringProcessor>();
        flowStringProcessor
            .Setup(x => x.Process(dummyInput, flowVars))
            .Returns("not-a-number");

        var hasConverted = IFlowStringProcessorExtensions.TryProcessInt(flowStringProcessor.Object, dummyInput, flowVars, out var actualValue);
        Assert.False(hasConverted);
    }
    
    [Fact]
    public void should_return_true_with_var_when_can_parse()
    {
        var dummyInput = "a";
        var expectedNumber = 231;
        var flowVars = new Variables();

        var flowStringProcessor = new Mock<IFlowStringProcessor>();
        flowStringProcessor
            .Setup(x => x.Process(dummyInput, flowVars))
            .Returns(expectedNumber.ToString());

        var hasConverted = IFlowStringProcessorExtensions.TryProcessInt(flowStringProcessor.Object, dummyInput, flowVars, out var actualValue);
        Assert.True(hasConverted);
        Assert.Equal(expectedNumber, actualValue);
    }
}