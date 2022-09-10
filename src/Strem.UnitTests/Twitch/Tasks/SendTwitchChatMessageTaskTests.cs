using Microsoft.Extensions.Logging;
using Moq;
using Strem.Core.Events.Bus;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Twitch.Flows.Tasks.Chat;
using Strem.Twitch.Types;
using Strem.Twitch.Variables;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

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
    public void should_not_execute_when_oauth_token_is_available_but_is_missing_scope()
    {
        var mockLogger = new Mock<ILogger<FlowTask<SendTwitchChatMessageTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<ITwitchClient>();
        
        dummyAppState.AppVariables.Set(TwitchVars.OAuthToken, "valid");
        
        var task = new SendTwitchChatMessageTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        var canExecute = task.CanExecute();
        Assert.False(canExecute);
    }
    
    [Fact]
    public void should_allow_execute_when_oauth_token_is_available_and_has_scope()
    {
        var mockLogger = new Mock<ILogger<FlowTask<SendTwitchChatMessageTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<ITwitchClient>();
        
        dummyAppState.AppVariables.Set(TwitchVars.OAuthToken, "valid");
        dummyAppState.AppVariables.Set(TwitchVars.OAuthScopes, ChatScopes.SendChat);
        var task = new SendTwitchChatMessageTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        var canExecute = task.CanExecute();
        Assert.True(canExecute);
    }
    
    [Fact]
    public async Task should_process_data_and_send_to_users_twitch_channel_when_no_channel_provided()
    {
        var mockLogger = new Mock<ILogger<FlowTask<SendTwitchChatMessageTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<ITwitchClient>();
        
        var inputString = "this is a test";
        var inputChannel = "my-channel";
        var expectedChannel = "my-processed-channel";
        var expectedString = "this text has been processed";
        
        mockTwitchClient.Setup(x => x.JoinedChannels)
            .Returns(new List<JoinedChannel>());
        
        mockFlowStringProcessor
            .Setup(x => x.Process(inputString, It.IsAny<IVariables>()))
            .Returns(expectedString);
        
        mockFlowStringProcessor
            .Setup(x => x.Process(inputChannel, It.IsAny<IVariables>()))
            .Returns(expectedChannel);

        dummyAppState.AppVariables.Set(TwitchVars.Username, inputChannel);
        
        var taskData = new SendTwitchChatMessageTaskData { Message = inputString };
        var flowVars = new Variables();
        var task = new SendTwitchChatMessageTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        await task.Execute(taskData, flowVars);
        
        mockTwitchClient.Verify(x => x.SendMessage(expectedChannel, expectedString, false), Times.Once());
    }
    
     
    [Fact]
    public async Task should_process_data_and_send_to_custom_twitch_channel_when_channel_provided()
    {
        var mockLogger = new Mock<ILogger<FlowTask<SendTwitchChatMessageTaskData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<ITwitchClient>();
        
        var inputMessage = "this is a test";
        var inputChannel = "tmy-channel";
        var expectedChannel = "my-processed-channel";
        var expectedMessage = "this text has been processed";

        mockTwitchClient.Setup(x => x.JoinedChannels)
            .Returns(new List<JoinedChannel>());
        
        mockFlowStringProcessor
            .Setup(x => x.Process(inputMessage, It.IsAny<IVariables>()))
            .Returns(expectedMessage);
        
        mockFlowStringProcessor
            .Setup(x => x.Process(inputChannel, It.IsAny<IVariables>()))
            .Returns(expectedChannel);

        dummyAppState.AppVariables.Set(TwitchVars.Username, "some-other-channel-not-used");
        
        var taskData = new SendTwitchChatMessageTaskData { Channel = inputChannel, Message = inputMessage };
        var flowVars = new Variables();
        var task = new SendTwitchChatMessageTask(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        await task.Execute(taskData, flowVars);
        
        mockTwitchClient.Verify(x => x.SendMessage(expectedChannel, expectedMessage, false), Times.Once());
    }
}