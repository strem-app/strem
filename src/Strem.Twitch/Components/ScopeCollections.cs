using Strem.Twitch.Types;

namespace Strem.Twitch.Components;

public class ScopeCollections
{
    public static string[] ReadChatScopes = new[] { ChatScopes.ReadChat, ChatScopes.ReadWhispers };
    
    public static string[] ManageChatScopes = new[] { ChatScopes.SendChat, ChatScopes.SendWhisper, ChatScopes.ModerateChannel };
    
    public static string[] ReadChannelScopes = new[]
    {
        ApiScopes.ReadChannelGoals, ApiScopes.ReadBits, ApiScopes.ReadChannelPolls, ApiScopes.ReadChannelPredictions, 
        ApiScopes.ReadChannelRedemptions, ApiScopes.ReadChannelSubscriptions, ApiScopes.ReadChannelVips,
        ApiScopes.ReadChannelHypeTrain, ApiScopes.ReadCharity
    };
    
    public static string[] ManageChannelScopes = new[]
    {
        ApiScopes.ManageChannelBroadcast, ApiScopes.ManageChannelEditors, ApiScopes.ManageChannelExtensions,
        ApiScopes.ManageChannelModerators, ApiScopes.ManageChannelPolls, ApiScopes.ManageChannelPredications,
        ApiScopes.ManageChannelRaids, ApiScopes.ManageChannelRedemptions, ApiScopes.ManageChannelSchedule,
        ApiScopes.ManageChannelVideos, ApiScopes.ManageChannelVips, ApiScopes.RunChannelCommercials,
        ApiScopes.ManageClips
    };

    public static string[] ReadModerationScopes = new[]
    {
        ApiScopes.ReadModeration, ApiScopes.ReadModerationAutomodSettings,
        ApiScopes.ReadModerationBlockedTerms, ApiScopes.ReadModerationChatSettings
    };

    public static string[] ManageModerationScopes = new[]
    {
        ApiScopes.ManageModerationAnnouncements, ApiScopes.ManageModerationAutomod,
        ApiScopes.ManageModerationAutomodSettings, ApiScopes.ManageModerationBannedUsers,
        ApiScopes.ManageModerationBlockedTerms, ApiScopes.ManageModerationChatMessages,
        ApiScopes.ManageModerationChatSettings
    };

    public static string[] ReadUserScopes = new[]
    {
        ApiScopes.ReadUsersBroadcast, ApiScopes.ReadUsersEmail, ApiScopes.ReadUsersFollows,
        ApiScopes.ReadUsersSubscriptions, ApiScopes.ReadUsersBlockedUsers
    };

    public static string[] ManageUserScopes = new[]
    {
        ApiScopes.ManageUser, ApiScopes.ManageUserFollows, ApiScopes.ManageUsersWhispers,
        ApiScopes.ManageUsersBlockedUsers, ApiScopes.ManageUsersChatColours
    };
}