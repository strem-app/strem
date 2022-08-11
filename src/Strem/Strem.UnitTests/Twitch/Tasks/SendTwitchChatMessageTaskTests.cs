using Microsoft.Extensions.Logging;
using Moq;
using Strem.Core.Events.Bus;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Default.Flows.Tasks.Utility;
using Strem.Twitch.Flows.Tasks.Chat;
using Strem.Twitch.Variables;
using Strem.UnitTests.Extensions;
using TwitchLib.Client.Interfaces;

namespace Strem.UnitTests.Twitch.Tasks;

public class SendTwitchChatMessageTaskTests
{
    [Fact]
    public void should_not_execute_when_no_oauth_token_is_available()
    {
        var mockLogger = new Mock<ILogger<FlowTask<SendTwitchChatMessageTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<ITwitchClient>();
        
        var task = new SendTwitchChatMessageTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        var canExecute = task.CanExecute();
        Assert.False(canExecute);
    }
    
    [Fact]
    public void should_allow_execute_when_oauth_token_is_available()
    {
        var mockLogger = new Mock<ILogger<FlowTask<SendTwitchChatMessageTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<ITwitchClient>();
        
        dummyAppState.AppVariables.Set(TwitchVars.OAuthToken, "valid");
        var task = new SendTwitchChatMessageTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        var canExecute = task.CanExecute();
        Assert.True(canExecute);
    }
    
    [Fact]
    public async Task should_process_message_and_send_to_users_twitch_channel_when_no_channel_provided()
    {
        var mockLogger = new Mock<ILogger<FlowTask<SendTwitchChatMessageTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<ITwitchClient>();
        
        var inputString = "this is a test";
        var expectedString = "this text has been processed";
        mockFlowStringProcessor
            .Setup(x => x.Process(inputString, It.IsAny<IVariables>()))
            .Returns(expectedString);

        var expectedChannel = "my-channel";
        dummyAppState.AppVariables.Set(TwitchVars.Username, expectedChannel);
        
        var taskData = new SendTwitchChatMessageTaskData { Message = inputString };
        var flowVars = new Variables();
        var task = new SendTwitchChatMessageTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        await task.Execute(taskData, flowVars);
        
        mockTwitchClient.Verify(x => x.SendMessage(expectedChannel, expectedString, false), Times.Once());
    }
    
     
    [Fact]
    public async Task should_process_message_and_send_to_custom_twitch_channel_when_channel_provided()
    {
        var mockLogger = new Mock<ILogger<FlowTask<SendTwitchChatMessageTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<ITwitchClient>();
        
        var inputString = "this is a test";
        var expectedString = "this text has been processed";
        mockFlowStringProcessor
            .Setup(x => x.Process(inputString, It.IsAny<IVariables>()))
            .Returns(expectedString);

        var expectedChannel = "custom-channel";
        dummyAppState.AppVariables.Set(TwitchVars.Username, "my-channel");
        
        var taskData = new SendTwitchChatMessageTaskData { Channel = expectedChannel, Message = inputString };
        var flowVars = new Variables();
        var task = new SendTwitchChatMessageTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        await task.Execute(taskData, flowVars);
        
        mockTwitchClient.Verify(x => x.SendMessage(expectedChannel, expectedString, false), Times.Once());
    }
}