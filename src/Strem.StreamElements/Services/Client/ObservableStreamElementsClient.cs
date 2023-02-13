using System.Reactive;
using System.Reactive.Linq;
using StreamElementsNET.Models.Cheer;
using StreamElementsNET.Models.Follower;
using StreamElementsNET.Models.Host;
using StreamElementsNET.Models.Internal;
using StreamElementsNET.Models.Subscriber;
using StreamElementsNET.Models.Tip;
using Strem.Core.Extensions;
using ErrorEventArgs = SuperSocket.ClientEngine.ErrorEventArgs;

namespace Strem.StreamElements.Services.Client;

using StreamElementsClient = StreamElementsNET.Client;

public class ObservableStreamElementsClient : IObservableStreamElementsClient
{
    public CustomStreamElementsClient Client { get; }
    
    public IObservable<Unit> OnConnected { get; private set; }
    public IObservable<Unit> OnDisconnected { get; private set; }
    public IObservable<Authenticated> OnAuthenticated { get; private set; }
    public IObservable<ErrorEventArgs> OnError { get; private set; }
    public IObservable<Unit> OnAuthenticationFailure { get; private set; }
    public IObservable<Follower> OnFollower { get; private set; }
    public IObservable<string> OnFollowerLatest { get; private set; }
    public IObservable<int> OnFollowerGoal { get; private set; }
    public IObservable<int> OnFollowerMonth { get; private set; }
    public IObservable<int> OnFollowerWeek { get; private set; }
    public IObservable<int> OnFollowerTotal { get; private set; }
    public IObservable<int> OnFollowerSession { get; private set; }
    public IObservable<Cheer> OnCheer { get; private set; }
    public IObservable<CheerLatest> OnCheerLatest { get; private set; }
    public IObservable<int> OnCheerGoal { get; private set; }
    public IObservable<int> OnCheerCount { get; private set; }
    public IObservable<int> OnCheerTotal { get; private set; }
    public IObservable<int> OnCheerSession { get; private set; }
    public IObservable<CheerSessionTopDonator> OnCheerSessionTopDonator { get; private set; }
    public IObservable<CheerSessionTopDonation> OnCheerSessionTopDonation { get; private set; }
    public IObservable<int> OnCheerMonth { get; private set; }
    public IObservable<int> OnCheerWeek { get; private set; }
    public IObservable<Host> OnHost { get; private set; }
    public IObservable<HostLatest> OnHostLatest { get; private set; }
    public IObservable<Tip> OnTip { get; private set; }
    public IObservable<int> OnTipCount { get; private set; }
    public IObservable<TipLatest> OnTipLatest { get; private set; }
    public IObservable<double> OnTipSession { get; private set; }
    public IObservable<double> OnTipGoal { get; private set; }
    public IObservable<double> OnTipWeek { get; private set; }
    public IObservable<double> OnTipTotal { get; private set; }
    public IObservable<double> OnTipMonth { get; private set; }
    public IObservable<TipSessionTopDonator> OnTipSessionTopDonator { get; private set; }
    public IObservable<TipSessionTopDonation> OnTipSessionTopDonation { get; private set; }
    public IObservable<Subscriber> OnSubscriber { get; private set; }
    public IObservable<SubscriberLatest> OnSubscriberLatest { get; private set; }
    public IObservable<int> OnSubscriberSession { get; private set; }
    public IObservable<int> OnSubscriberGoal { get; private set; }
    public IObservable<int> OnSubscriberMonth { get; private set; }
    public IObservable<int> OnSubscriberWeek { get; private set; }
    public IObservable<int> OnSubscriberTotal { get; private set; }
    public IObservable<int> OnSubscriberPoints { get; private set; }
    public IObservable<int> OnSubscriberResubSession { get; private set; }
    public IObservable<SubscriberResubLatest> OnSubscriberResubLatest { get; private set; }
    public IObservable<int> OnSubscriberNewSession { get; private set; }
    public IObservable<int> OnSubscriberGiftedSession { get; private set; }
    public IObservable<SubscriberNewLatest> OnSubscriberNewLatest { get; private set; }
    public IObservable<SubscriberAlltimeGifter> OnSubscriberAlltimeGifter { get; private set; }
    public IObservable<SubscriberGiftedLatest> OnSubscriberGiftedLatest { get; private set; }
    public IObservable<string> OnUnknownComplexObject { get; private set; }
    public IObservable<string> OnUnknownSimpleUpdate { get; private set; }
    
    public ObservableStreamElementsClient(CustomStreamElementsClient client)
    {
        Client = client;
        SetupObservables();
    }

    public void SetupObservables()
    {
        OnConnected = Observable.FromEventPattern(
                e => Client.OnConnected += e,
                e => Client.OnConnected -= e)
            .ToUnit();
        
        OnDisconnected = Observable.FromEventPattern(
                e => Client.OnDisconnected += e,
                e => Client.OnDisconnected -= e)
            .ToUnit();
        
        OnCheer = Observable.FromEventPattern<Cheer>(
                e => Client.OnCheer += e,
                e => Client.OnCheer -= e)
            .Select(x => x.EventArgs);
        
        OnAuthenticated = Observable.FromEventPattern<Authenticated>(
                e => Client.OnAuthenticated += e,
                e => Client.OnAuthenticated -= e)
            .Select(x => x.EventArgs);
        
        OnError = Observable.FromEventPattern<ErrorEventArgs>(
                e => Client.OnError += e,
                e => Client.OnError -= e)
            .Select(x => x.EventArgs);
        
        OnFollower = Observable.FromEventPattern<Follower>(
                e => Client.OnFollower += e,
                e => Client.OnFollower -= e)
            .Select(x => x.EventArgs);
        
        OnFollowerWeek = Observable.FromEventPattern<int>(
                e => Client.OnFollowerWeek += e,
                e => Client.OnFollowerWeek -= e)
            .Select(x => x.EventArgs);
        
        OnFollowerGoal = Observable.FromEventPattern<int>(
                e => Client.OnFollowerGoal += e,
                e => Client.OnFollowerGoal -= e)
            .Select(x => x.EventArgs);
        
        OnFollowerMonth = Observable.FromEventPattern<int>(
                e => Client.OnFollowerMonth += e,
                e => Client.OnFollowerMonth -= e)
            .Select(x => x.EventArgs);
        
        OnFollowerLatest = Observable.FromEventPattern<string>(
                e => Client.OnFollowerLatest += e,
                e => Client.OnFollowerLatest -= e)
            .Select(x => x.EventArgs);
        
        OnFollowerSession = Observable.FromEventPattern<int>(
                e => Client.OnFollowerSession += e,
                e => Client.OnFollowerSession -= e)
            .Select(x => x.EventArgs);
        
        OnFollowerTotal = Observable.FromEventPattern<int>(
                e => Client.OnFollowerTotal += e,
                e => Client.OnFollowerTotal -= e)
            .Select(x => x.EventArgs);
        
        OnHost = Observable.FromEventPattern<Host>(
                e => Client.OnHost += e,
                e => Client.OnHost -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriber = Observable.FromEventPattern<Subscriber>(
                e => Client.OnSubscriber += e,
                e => Client.OnSubscriber -= e)
            .Select(x => x.EventArgs);
        
        OnTip = Observable.FromEventPattern<Tip>(
                e => Client.OnTip += e,
                e => Client.OnTip -= e)
            .Select(x => x.EventArgs);
        
        OnCheerCount = Observable.FromEventPattern<int>(
                e => Client.OnCheerCount += e,
                e => Client.OnCheerCount -= e)
            .Select(x => x.EventArgs);
                
        OnCheerGoal = Observable.FromEventPattern<int>(
                e => Client.OnCheerGoal += e,
                e => Client.OnCheerGoal -= e)
            .Select(x => x.EventArgs);
        
        OnCheerSession = Observable.FromEventPattern<int>(
                e => Client.OnCheerSession += e,
                e => Client.OnCheerSession -= e)
            .Select(x => x.EventArgs);
        
        OnCheerMonth = Observable.FromEventPattern<int>(
                e => Client.OnCheerMonth += e,
                e => Client.OnCheerMonth -= e)
            .Select(x => x.EventArgs);
        
        OnCheerTotal = Observable.FromEventPattern<int>(
                e => Client.OnCheerTotal += e,
                e => Client.OnCheerTotal -= e)
            .Select(x => x.EventArgs);
        
        OnCheerWeek = Observable.FromEventPattern<int>(
                e => Client.OnCheerWeek += e,
                e => Client.OnCheerWeek -= e)
            .Select(x => x.EventArgs);
        
        OnCheerLatest = Observable.FromEventPattern<CheerLatest>(
                e => Client.OnCheerLatest += e,
                e => Client.OnCheerLatest -= e)
            .Select(x => x.EventArgs);
        
        OnCheerSessionTopDonation = Observable.FromEventPattern<CheerSessionTopDonation>(
                e => Client.OnCheerSessionTopDonation += e,
                e => Client.OnCheerSessionTopDonation -= e)
            .Select(x => x.EventArgs);
                
        OnCheerSessionTopDonator = Observable.FromEventPattern<CheerSessionTopDonator>(
                e => Client.OnCheerSessionTopDonator += e,
                e => Client.OnCheerSessionTopDonator -= e)
            .Select(x => x.EventArgs);
        
        OnHostLatest = Observable.FromEventPattern<HostLatest>(
                e => Client.OnHostLatest += e,
                e => Client.OnHostLatest -= e)
            .Select(x => x.EventArgs);
        
        OnAuthenticationFailure = Observable.FromEventPattern(
                e => Client.OnAuthenticationFailure += e,
                e => Client.OnAuthenticationFailure -= e)
            .ToUnit();
        
        OnTipCount = Observable.FromEventPattern<int>(
                e => Client.OnTipCount += e,
                e => Client.OnTipCount -= e)
            .Select(x => x.EventArgs);
        
        OnTipLatest = Observable.FromEventPattern<TipLatest>(
                e => Client.OnTipLatest += e,
                e => Client.OnTipLatest -= e)
            .Select(x => x.EventArgs);
        
        OnTipSession = Observable.FromEventPattern<double>(
                e => Client.OnTipSession += e,
                e => Client.OnTipSession -= e)
            .Select(x => x.EventArgs);
        
        OnTipGoal = Observable.FromEventPattern<double>(
                e => Client.OnTipGoal += e,
                e => Client.OnTipGoal -= e)
            .Select(x => x.EventArgs);
        
        OnTipWeek = Observable.FromEventPattern<double>(
                e => Client.OnTipWeek += e,
                e => Client.OnTipWeek -= e)
            .Select(x => x.EventArgs);
        
        OnTipTotal = Observable.FromEventPattern<double>(
                e => Client.OnTipTotal += e,
                e => Client.OnTipTotal -= e)
            .Select(x => x.EventArgs);
        
        OnTipMonth = Observable.FromEventPattern<double>(
                e => Client.OnTipMonth += e,
                e => Client.OnTipMonth -= e)
            .Select(x => x.EventArgs);
        
        OnTipSessionTopDonator = Observable.FromEventPattern<TipSessionTopDonator>(
                e => Client.OnTipSessionTopDonator += e,
                e => Client.OnTipSessionTopDonator -= e)
            .Select(x => x.EventArgs);
        
        OnTipSessionTopDonation = Observable.FromEventPattern<TipSessionTopDonation>(
                e => Client.OnTipSessionTopDonation += e,
                e => Client.OnTipSessionTopDonation -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberLatest = Observable.FromEventPattern<SubscriberLatest>(
                e => Client.OnSubscriberLatest += e,
                e => Client.OnSubscriberLatest -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberSession = Observable.FromEventPattern<int>(
                e => Client.OnSubscriberSession += e,
                e => Client.OnSubscriberSession -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberGoal = Observable.FromEventPattern<int>(
                e => Client.OnSubscriberGoal += e,
                e => Client.OnSubscriberGoal -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberMonth = Observable.FromEventPattern<int>(
                e => Client.OnSubscriberMonth += e,
                e => Client.OnSubscriberMonth -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberWeek = Observable.FromEventPattern<int>(
                e => Client.OnSubscriberWeek += e,
                e => Client.OnSubscriberWeek -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberTotal = Observable.FromEventPattern<int>(
                e => Client.OnSubscriberTotal += e,
                e => Client.OnSubscriberTotal -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberPoints = Observable.FromEventPattern<int>(
                e => Client.OnSubscriberPoints += e,
                e => Client.OnSubscriberPoints -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberResubSession = Observable.FromEventPattern<int>(
                e => Client.OnSubscriberResubSession += e,
                e => Client.OnSubscriberResubSession -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberResubLatest = Observable.FromEventPattern<SubscriberResubLatest>(
                e => Client.OnSubscriberResubLatest += e,
                e => Client.OnSubscriberResubLatest -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberNewSession = Observable.FromEventPattern<int>(
                e => Client.OnSubscriberNewSession += e,
                e => Client.OnSubscriberNewSession -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberGiftedSession = Observable.FromEventPattern<int>(
                e => Client.OnSubscriberGiftedSession += e,
                e => Client.OnSubscriberGiftedSession -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberNewLatest = Observable.FromEventPattern<SubscriberNewLatest>(
                e => Client.OnSubscriberNewLatest += e,
                e => Client.OnSubscriberNewLatest -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberAlltimeGifter = Observable.FromEventPattern<SubscriberAlltimeGifter>(
                e => Client.OnSubscriberAlltimeGifter += e,
                e => Client.OnSubscriberAlltimeGifter -= e)
            .Select(x => x.EventArgs);
        
        OnSubscriberGiftedLatest = Observable.FromEventPattern<SubscriberGiftedLatest>(
                e => Client.OnSubscriberGiftedLatest += e,
                e => Client.OnSubscriberGiftedLatest -= e)
            .Select(x => x.EventArgs);
        
        OnUnknownComplexObject = Observable.FromEventPattern<string>(
                e => Client.OnUnknownComplexObject += e,
                e => Client.OnUnknownComplexObject -= e)
            .Select(x => x.EventArgs);
        
        OnUnknownSimpleUpdate = Observable.FromEventPattern<string>(
                e => Client.OnUnknownSimpleUpdate += e,
                e => Client.OnUnknownSimpleUpdate -= e)
            .Select(x => x.EventArgs);

    }
}