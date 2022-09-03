using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Moq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Triggers;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;
using Strem.Twitch.Flows.Triggers.Chat;
using Strem.Twitch.Services.Client;
using Strem.Twitch.Types;
using Strem.Twitch.Variables;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models.Builders;

namespace Strem.UnitTests.Twitch.Triggers;

public class OnTwitchChatMessageTriggerTests
{
    [Fact]
    public void should_not_trigger_when_no_oauth_token_is_available()
    {
        var mockLogger = new Mock<ILogger<FlowTrigger<OnTwitchChatMessageTriggerData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<IObservableTwitchClient>();
        
        var trigger = new OnTwitchChatMessageTrigger(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        var canExecute = trigger.CanExecute();
        Assert.False(canExecute);
    }
    
    [Fact]
    public void should_trigger_when_oauth_token_is_available_and_has_scope()
    {
        var mockLogger = new Mock<ILogger<FlowTrigger<OnTwitchChatMessageTriggerData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<IObservableTwitchClient>();
        
        dummyAppState.AppVariables.Set(TwitchVars.OAuthToken, "valid");
        dummyAppState.AppVariables.Set(TwitchVars.OAuthScopes, ChatScopes.ReadChat);
        
        var trigger = new OnTwitchChatMessageTrigger(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        var canExecute = trigger.CanExecute();
        Assert.True(canExecute);
    }
    
    [Fact]
    public void should_not_trigger_when_oauth_token_is_available_but_has_no_read_chat_scope()
    {
        var mockLogger = new Mock<ILogger<FlowTrigger<OnTwitchChatMessageTriggerData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<IObservableTwitchClient>();
        
        dummyAppState.AppVariables.Set(TwitchVars.OAuthToken, "valid");
        
        var trigger = new OnTwitchChatMessageTrigger(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        var canExecute = trigger.CanExecute();
        Assert.False(canExecute);
    }
    
    [Fact]
    public async Task should_trigger_on_chat_message_with_no_restrictions_and_matching_channel()
    {
        var mockLogger = new Mock<ILogger<FlowTrigger<OnTwitchChatMessageTriggerData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<IObservableTwitchClient>();

        var expectedChannel = "some-channel";

        var onChatMessageSubject = new Subject<OnMessageReceivedArgs>();
        mockTwitchClient
            .Setup(x => x.OnMessageReceived)
            .Returns(onChatMessageSubject);
        
        mockFlowStringProcessor
            .Setup(x => x.Process(expectedChannel, It.IsAny<IVariables>()))
            .Returns(expectedChannel);
        
        var triggerData = new OnTwitchChatMessageTriggerData
        {
            Channel = expectedChannel,
            MatchTypeType = TextMatchType.None,
            MinimumUserType = UserType.Viewer
        };
        var trigger = new OnTwitchChatMessageTrigger(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        var observable = await trigger.Execute(triggerData);

        var wasCalled = false;
        var sub = observable.Subscribe(x => wasCalled = true);
        onChatMessageSubject.OnNext(new OnMessageReceivedArgs { ChatMessage = ChatMessageBuilder.Create().WithChannel(expectedChannel).Build() });
        
        Assert.True(wasCalled);
    }
    
    [Fact]
    public async Task should_not_trigger_on_chat_message_with_no_restrictions_and_not_matching_channel()
    {
        var mockLogger = new Mock<ILogger<FlowTrigger<OnTwitchChatMessageTriggerData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<IObservableTwitchClient>();

        var expectedChannel = "some-channel";

        var onChatMessageSubject = new Subject<OnMessageReceivedArgs>();
        mockTwitchClient
            .Setup(x => x.OnMessageReceived)
            .Returns(onChatMessageSubject);

        var triggerData = new OnTwitchChatMessageTriggerData
        {
            MatchTypeType = TextMatchType.None,
            MinimumUserType = UserType.Viewer
        };
        var trigger = new OnTwitchChatMessageTrigger(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        var observable = await trigger.Execute(triggerData);

        var wasCalled = false;
        var sub = observable.Subscribe(x => wasCalled = true);
        onChatMessageSubject.OnNext(new OnMessageReceivedArgs { ChatMessage = ChatMessageBuilder.Create().WithChannel(expectedChannel).Build() });
        
        Assert.False(wasCalled);
    }
    
    [Fact]
    public void should_meet_chat_criteria_with_no_restrictions_and_channel_matches()
    {
        var mockLogger = new Mock<ILogger<FlowTrigger<OnTwitchChatMessageTriggerData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<IObservableTwitchClient>();

        var expectedChannel = "some-channel";
        var triggerData = new OnTwitchChatMessageTriggerData
        {
            Channel = expectedChannel,
            MatchTypeType = TextMatchType.None,
            MinimumUserType = UserType.Viewer
        };

        mockFlowStringProcessor
            .Setup(x => x.Process(expectedChannel, It.IsAny<IVariables>()))
            .Returns(expectedChannel);
        
        var trigger = new OnTwitchChatMessageTrigger(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        var meetsCriteria = trigger.DoesMessageMeetCriteria(triggerData, ChatMessageBuilder.Create().WithChannel(expectedChannel).Build());
        Assert.True(meetsCriteria);
    }
    
    [Fact]
    public void should_not_meet_chat_criteria_with_no_restrictions_when_channel_doesnt_match_users_channel()
    {
        var mockLogger = new Mock<ILogger<FlowTrigger<OnTwitchChatMessageTriggerData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<IObservableTwitchClient>();

        var expectedChannel = "some-channel";
        var triggerData = new OnTwitchChatMessageTriggerData
        {
            MatchTypeType = TextMatchType.None,
            MinimumUserType = UserType.Viewer
        };
        var trigger = new OnTwitchChatMessageTrigger(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        var meetsCriteria = trigger.DoesMessageMeetCriteria(triggerData, ChatMessageBuilder.Create().WithChannel(expectedChannel).Build());
        Assert.False(meetsCriteria);
    }
    
    [Theory]
    [InlineData(UserType.Viewer, UserType.Viewer, true)]
    [InlineData(UserType.Viewer, UserType.GlobalModerator, true)]
    [InlineData(UserType.Moderator, UserType.Viewer, false)]
    [InlineData(UserType.Moderator, UserType.GlobalModerator, true)]
    [InlineData(UserType.Broadcaster, UserType.GlobalModerator, false)]
    [InlineData(UserType.Broadcaster, UserType.Admin, true)]
    [InlineData(UserType.Broadcaster, UserType.Staff, true)]
    [InlineData(UserType.Staff, UserType.Moderator, false)]
    [InlineData(UserType.Staff, UserType.Staff, true)]
    public void should_meet_user_restrictions_correctly(UserType requiredType, UserType providedType, bool shouldPass)
    {
        var mockLogger = new Mock<ILogger<FlowTrigger<OnTwitchChatMessageTriggerData>>>();
        var mockFlowStringProcessor = new Mock<IFlowStringProcessor>();
        var mockEventBus = new Mock<IEventBus>();
        var dummyAppState = new AppState(new Variables(), new Variables(), new Variables());
        var mockTwitchClient = new Mock<IObservableTwitchClient>();

        var trigger = new OnTwitchChatMessageTrigger(mockLogger.Object, mockFlowStringProcessor.Object, dummyAppState, mockEventBus.Object, mockTwitchClient.Object);
        var meetsCriteria = trigger.IsUserAboveMinimumRequired(requiredType, providedType);
        Assert.Equal(shouldPass, meetsCriteria);
    }
}