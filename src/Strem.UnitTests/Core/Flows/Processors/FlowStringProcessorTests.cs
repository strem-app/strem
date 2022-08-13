using Strem.Core.Extensions;
using Strem.Core.Flows.Processors;
using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.UnitTests.Core.Flows.Processors;

public class FlowStringProcessorTests
{
    [Fact]
    public void should_correctly_template_variables()
    {
        var appState = new AppState(new Variables(), new Variables(), new Variables());
        appState.UserVariables.Set("name", "lipstickdreamer");
        appState.UserVariables.Set("points", "lipstickdreamer", "1000");
        appState.UserVariables.Set("score", "lipstickdreamer", "243");
        appState.UserVariables.Set("test1", "243", "test-context");
        appState.UserVariables.Set("test", "test-value-2");
        appState.UserVariables.Set("test-value-2", "test-context", "roger and");
        
        var inputText = "Hey there V(name), you have V(points, V(name)) points available, V(V(test), V(test1, V(score, V(name)))) out!";
        var expectedText = "Hey there lipstickdreamer, you have 1000 points available, roger and out!";
        var flowStringProcessor = new FlowStringProcessor(appState);

        var actualText = flowStringProcessor.Process(inputText, new Variables());
        Assert.NotNull(actualText);
        Assert.Equal(expectedText, actualText);
    }

    [Fact]
    public void should_correctly_pick_correct_vars_in_order()
    {
        var flowArguments = new Variables();
        flowArguments.Set("user", "grofit");
        
        var appState = new AppState(new Variables(), new Variables(), new Variables());
        appState.UserVariables.Set("user", "lipstickdreamer");
        appState.UserVariables.Set("points", "1000");
        appState.TransientVariables.Set("user", "theisal");
        appState.TransientVariables.Set("points", "222");
        appState.TransientVariables.Set("test", "woop");

        var input = "V(user), V(points), V(test)";
        var expectedOutput = "grofit, 1000, woop";
        var flowStringProcessor = new FlowStringProcessor(appState);
        
        var actualOutput = flowStringProcessor.Process(input, flowArguments);
        Assert.NotNull(actualOutput);
        Assert.Equal(expectedOutput, actualOutput);
    }
}