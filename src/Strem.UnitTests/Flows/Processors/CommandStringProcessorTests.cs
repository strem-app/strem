using Strem.Flows.Processors;

namespace Strem.UnitTests.Flows.Processors;

public class CommandStringProcessorTests
{
    [Theory]
    [InlineData("wooples", "", "")]
    [InlineData("wooples !monkey face !woop", "", "")]
    [InlineData("wehionwe weoinvwioev wevwe vinm.fwe .f/ewf. we", "", "")]
    [InlineData("!clip", "clip", "")]
    [InlineData("!clip heya", "clip", "heya")]
    [InlineData("!clip !heya", "clip", "!heya")]
    [InlineData("!cli.p !heya", "cli.p", "!heya")]
    [InlineData("!clip:massive !heya", "clip:massive", "!heya")]
    [InlineData("!_1.-:22whoop", "_1.-:22whoop", "")]
    public void should_parse_commands_correctly(string messageText, string expectedCommand, string expectedArgs)
    {
        var processor = new CommandStringProcessor();
        var parsedCommand = processor.Process(messageText);
        
        Assert.NotNull(parsedCommand);
        Assert.Equal(expectedCommand, parsedCommand.CommandName);
        Assert.Equal(expectedArgs, parsedCommand.CommandArgs);
    }
}