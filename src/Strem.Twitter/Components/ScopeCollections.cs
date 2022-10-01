using Strem.Twitter.Types;

namespace Strem.Twitter.Components;

public class ScopeCollections
{
    public static string[] ReadTweetScopes = new[] { Scopes.ReadTweet };
    public static string[] ManageTweetScopes = new[] { Scopes.WriteTweet, Scopes.ModerateTweet };
}