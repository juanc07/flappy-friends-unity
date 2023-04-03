#import "GpgIosConfig.h"
#import "GpgIosAdapter.h"

#define MakeNSString(x) ([[NSString stringWithFormat:@"%s", x] retain])

NSString * _returnedString;
NSString * _callbackObjectName;
UIViewController * _viewController;
BOOL _connected;
BOOL _connecting;
NSArray * _scores = nil;
int _scoresCollection;
int _scoresSeed;
int _scoresSpan;
NSData * _localState = nil;
NSData * _remoteState = nil;
NSData * _resolvedState = nil;
BOOL _justResolved = NO;

@implementation GpgUnityAdapter

+ (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation
{
    return [GPPURLHandler handleURL:url
                  sourceApplication:sourceApplication
                         annotation:annotation];
}

-(id)initWithCallbackObjectName:(NSString*)callbackObjectName
                        verbose:(BOOL)verbose
                   gamesEnabled:(BOOL)gamesEnabled
                appStateEnabled:(BOOL)appStateEnabled
                 viewController:(UIViewController*)viewController
{
    if (self = [super init])
    {
        _callbackObjectName = [callbackObjectName retain];
        _viewController = viewController;
        _connected = NO;
        _connecting = NO;
        
        GPPSignIn *signIn = [GPPSignIn sharedInstance];
        
        signIn.clientID = MakeNSString(GPG_CLIENT_ID);
        if (appStateEnabled)
        {
            signIn.scopes = [NSArray arrayWithObjects:
                             @"https://www.googleapis.com/auth/games",
                             @"https://www.googleapis.com/auth/appstate",
                             nil];
        }
        else
        {
            signIn.scopes = [NSArray arrayWithObjects:
                             @"https://www.googleapis.com/auth/games",
                             nil];
        }
        signIn.language = [[NSLocale preferredLanguages] objectAtIndex:0];
        signIn.delegate = self;
        signIn.shouldFetchGoogleUserID = YES;
    }
    return self;
}


-(void)connectSilent:(BOOL)silent
{
    _connecting = YES;
    if (silent)
    {
        [[GPPSignIn sharedInstance] trySilentAuthentication];
    }
    else
    {
        [[GPPSignIn sharedInstance] authenticate];
    }
}

-(void)startGoogleGamesSignIn
{
    [[GPGManager sharedInstance] signIn:[GPPSignIn sharedInstance]
                     reauthorizeHandler:^(BOOL requiresKeychainWipe, NSError *error)
     {
         if (requiresKeychainWipe)
         {
             [[GPPSignIn sharedInstance] signOut];
         }
         [[GPPSignIn sharedInstance] authenticate];
     }];
    _connected = YES;
    UnitySendMessage([_callbackObjectName UTF8String], "onConnected", "0");
}

- (void)finishedWithAuth:(GTMOAuth2Authentication *)auth error:(NSError *)error
{
    NSLog(@"Finished with auth.");
    _connecting = NO;
    if (error == nil && auth)
    {
        NSLog(@"Success signing in to Google! Auth object is %@", auth);
        [self startGoogleGamesSignIn];
    }
    else
    {
        NSLog(@"Failed to log into Google\n\tError=%@\n\tAuthObj=%@", error, auth);
        UnitySendMessage([_callbackObjectName UTF8String], "onFailedToConnect", ([[NSString stringWithFormat:@"%i", error.code] UTF8String]));
    }
}


-(void)disconnect
{
    [[GPPSignIn sharedInstance] disconnect];
    _connected = NO;
    UnitySendMessage([_callbackObjectName UTF8String], "onDisconnected", "0");
}

-(void)signOut
{
    [[GPGManager sharedInstance] signout];
    [self disconnect];
}

-(BOOL)isConnected
{
    return _connected;
}

-(BOOL)isConnecting
{
    return _connecting;
}



-(void)achievementViewControllerDidFinish: (GPGAchievementController *)viewController
{
    [_viewController dismissViewControllerAnimated:YES completion:nil];
}

-(void)showAchievements
{
    GPGAchievementController *achievementController = [[GPGAchievementController alloc] init];
    achievementController.achievementDelegate = self;
    [_viewController presentViewController:achievementController animated:YES completion:nil];
}

-(void)loadAchievements
{
    UnitySendMessage([_callbackObjectName UTF8String], "onAchievementsLoaded", "0");
}

-(int)getAchievementsCount
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    return allAchievements.count;
}

-(NSString*)getAchievementId:(int)index
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    GPGAchievementMetadata * achievement = allAchievements[index];
    return achievement.achievementId;
}

-(int)getAchievementState:(int)index
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    GPGAchievementMetadata * achievement = allAchievements[index];
    return achievement.state;
}

-(int)getAchievementType:(int)index
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    GPGAchievementMetadata * achievement = allAchievements[index];
    return achievement.type;
}

-(NSString*)getAchievementName:(int)index
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    GPGAchievementMetadata * achievement = allAchievements[index];
    return achievement.name;
}

-(NSString*)getAchievementDescription:(int)index
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    GPGAchievementMetadata * achievement = allAchievements[index];
    return achievement.achievementDescription;
}

-(NSString*)getAchievementRevealedIconUrl:(int)index
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    GPGAchievementMetadata * achievement = allAchievements[index];
    return [achievement.revealedIconUrl absoluteString];
}

-(NSString*)getAchievementUnlockedIconUrl:(int)index
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    GPGAchievementMetadata * achievement = allAchievements[index];
    return [achievement.unlockedIconUrl absoluteString];
}

-(int)getAchievementCompletedSteps:(int)index
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    GPGAchievementMetadata * achievement = allAchievements[index];
    return achievement.completedSteps;
}

-(int)getAchievementNumberOfSteps:(int)index
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    GPGAchievementMetadata * achievement = allAchievements[index];
    return achievement.numberOfSteps;
}

-(NSString*)getAchievementFormattedCompletedSteps:(int)index
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    GPGAchievementMetadata * achievement = allAchievements[index];
    return achievement.formattedCompletedSteps;
}

-(NSString*)getAchievementFormattedNumberOfSteps:(int)index
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    GPGAchievementMetadata * achievement = allAchievements[index];
    return achievement.formattedNumberOfSteps;
}

-(long)getAchievementLastUpdatedTimestamp:(int)index
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    GPGAchievementMetadata * achievement = allAchievements[index];
    return achievement.lastUpdatedTimestamp;
}

-(float)getAchievementProgress:(int)index
{
    NSArray *allAchievements = [[GPGManager sharedInstance].applicationModel.achievement allMetadata];
    GPGAchievementMetadata * achievement = allAchievements[index];
    return achievement.progress;
}



-(void)incrementAchievement:(NSString*)achievementId by:(int)steps
{
    GPGAchievement * achievement = [GPGAchievement achievementWithId:achievementId];
    [achievement incrementAchievementNumSteps:steps completionHandler:nil];
}

-(void)revealAchievement:(NSString*)achievementId
{
    GPGAchievement * achievement = [GPGAchievement achievementWithId:achievementId];
    [achievement revealAchievementWithCompletionHandler:nil];
}

-(void)unlockAchievement:(NSString*)achievementId
{
    GPGAchievement * achievement = [GPGAchievement achievementWithId:achievementId];
    [achievement unlockAchievementWithCompletionHandler:nil];
}



-(void)showLeaderboards
{
    GPGLeaderboardsController *leaderboardsController = [[GPGLeaderboardsController alloc] init];
    leaderboardsController.leaderboardsDelegate = self;
    [_viewController presentViewController:leaderboardsController animated:YES completion:nil];
}

-(void)leaderboardsViewControllerDidFinish: (GPGAchievementController *)viewController
{
    [_viewController dismissViewControllerAnimated:YES completion:nil];
}

-(void)showLeaderboard:(NSString*)leaderboardId
{
    GPGLeaderboardController *leaderboardController = [[GPGLeaderboardController alloc] initWithLeaderboardId:leaderboardId];
    leaderboardController.leaderboardDelegate = self;
    [_viewController presentViewController:leaderboardController animated:YES completion:nil];
}

-(void)leaderboardViewControllerDidFinish: (GPGAchievementController *)viewController
{
    [_viewController dismissViewControllerAnimated:YES completion:nil];
}

-(void)submitScore:(long)score toLeaderboard:(NSString*)leaderboardId
{
    GPGScore * gpgScore = [[GPGScore alloc] initWithLeaderboardId:leaderboardId];
    gpgScore.value = score;
    [gpgScore submitScoreWithCompletionHandler: nil];
}

-(void)loadLeaderboards
{
    UnitySendMessage([_callbackObjectName UTF8String], "onLeaderboardMetadataLoaded", "0");
}


-(int)getLeaderboardsCount
{
    NSArray *allLeaderboards = [[GPGManager sharedInstance].applicationModel.leaderboard allMetadata];
    return allLeaderboards.count;
}

-(NSString*)getLeaderboardId:(int)index
{
    NSArray *allLeaderboards = [[GPGManager sharedInstance].applicationModel.leaderboard allMetadata];
    GPGLeaderboardMetadata * leaderboard = allLeaderboards[index];
    return leaderboard.leaderboardId;
}

-(NSString*)getLeaderboardTitle:(int)index
{
    NSArray *allLeaderboards = [[GPGManager sharedInstance].applicationModel.leaderboard allMetadata];
    GPGLeaderboardMetadata * leaderboard = allLeaderboards[index];
    return leaderboard.title;
}

-(NSString*)getLeaderboardIconImageUrl:(int)index
{
    NSArray *allLeaderboards = [[GPGManager sharedInstance].applicationModel.leaderboard allMetadata];
    GPGLeaderboardMetadata * leaderboard = allLeaderboards[index];
    return [leaderboard.iconUrl absoluteString];
}

-(int)getLeaderboardScoreOrder:(int)index
{
    NSArray *allLeaderboards = [[GPGManager sharedInstance].applicationModel.leaderboard allMetadata];
    GPGLeaderboardMetadata * leaderboard = allLeaderboards[index];
    return leaderboard.order;
}

-(void)loadScoresForLeaderboard:(NSString*)leaderboardId span:(int)span collection:(int)collection seed:(int)seed maxResults:(int)maxResults
{
    _scoresCollection = collection;
    _scoresSeed = seed;
    _scoresSpan = span;
    
    GPGLeaderboard *leaderboard = [GPGLeaderboard leaderboardWithId:leaderboardId];
    leaderboard.social = (collection == 1);
    leaderboard.timeScope = (GPGLeaderboardTimeScope)(span + 1);
    leaderboard.personalWindow = (seed == 1);
    
    [leaderboard loadScoresWithCompletionHandler:^(NSArray *scores, NSError *error)
    {
        if (error != nil || scores == nil)
        {
            UnitySendMessage([_callbackObjectName UTF8String], "onLeaderboardScoresLoaded", "1");
            return;
        }
        if (_scores != nil)
            [_scores release];
        _scores = [scores retain];
        UnitySendMessage([_callbackObjectName UTF8String], "onLeaderboardScoresLoaded", [[NSString stringWithFormat:@"0 %@", leaderboardId] UTF8String]);
    }];
}

-(int)getScoresCount
{
    if (_scores == nil)
        return 0;
    
    return _scores.count;
}

-(int)getScoresCollection
{
    return _scoresCollection;
}

-(int)getScoresSeed
{
    return _scoresSeed;
}

-(int)getScoresSpan
{
    return _scoresSpan;
}

-(long)getScoreValue:(int)index
{
    return ((GPGScore*)_scores[index]).value;
}

-(NSString*)getScoreAvatarUrl:(int)index
{
    return [((GPGScore*)_scores[index]).avatarUrl absoluteString];
}

-(NSString*)getScoreFormattedRank:(int)index
{
    return ((GPGScore*)_scores[index]).formattedRank;
}

-(NSString*)getScoreFormattedScore:(int)index
{
    return ((GPGScore*)_scores[index]).formattedScore;
}

-(NSString*)getScoreDisplayName:(int)index
{
    return ((GPGScore*)_scores[index]).displayName;
}

-(NSString*)getScorePlayerId:(int)index
{
    return ((GPGScore*)_scores[index]).playerId;
}

-(int)getScoreRank:(int)index
{
    return ((GPGScore*)_scores[index]).rank;
}

-(long)getScoreWriteTimestamp:(int)index
{
    return ((GPGScore*)_scores[index]).writeTimestamp;
}


-(int)getMaxCloudSaveKeys
{
    return 4;
}

-(int)getMaxCloudSaveStateSize
{
    return 128 * 1024;
}

-(NSData*)getLocalState
{
    return _localState;
}

-(NSData*)getRemoteState
{
    return _remoteState;
}

-(void)loadStates
{
    NSLog(@"loading states inside");
    GPGAppStateModel *model = [GPGManager sharedInstance].applicationModel.appState;
    
    [model listStateKeysWithCompletionHandler:^(NSArray *states, NSNumber *maxKeyCount, NSError *error)
    {
        NSLog(@"  states list loaded: states = %@, maxKeyCount = %@", states, maxKeyCount);
        
        if (states != nil)
        {
            NSString * arg = @"0";
            for (int i = 0; i < [states count]; ++i)
            {
                arg = [NSString stringWithFormat:@"%@ %i", arg, (int)[states objectAtIndex:i]];
            }
            NSLog(@"  states list loaded: %@", arg);
            UnitySendMessage([_callbackObjectName UTF8String], "onStatesListLoaded", ([arg UTF8String]));
        }
        else if (error != nil)
        {
            NSLog(@"  states list loading failed: %@", error);
            UnitySendMessage([_callbackObjectName UTF8String], "onStatesListLoaded", ([[NSString stringWithFormat:@"%i", error.code] UTF8String]));
        }
    }];
    
    
    NSLog(@"loading states finished");
}

-(void)setLocalState:(NSData*)data
{
    if (_localState != nil)
        [_localState release];
    
    _localState = data;
    if (_localState != nil)
        [_localState retain];
}

-(void)setRemoteState:(NSData*)data
{
    if (_remoteState != nil)
        [_remoteState release];
    
    _remoteState = data;
    if (_remoteState != nil)
        [_remoteState retain];
}

-(void)loadState:(int)key
{
    NSLog(@"loading state: %i", key);
    GPGAppStateModel *model = [GPGManager sharedInstance].applicationModel.appState;
    NSNumber *keyNumber = [NSNumber numberWithInt:key];
    
    [model loadForKey:keyNumber completionHandler:^(GPGAppStateLoadStatus status, NSError *error)
    {
        if (status == GPGAppStateLoadStatusSuccess)
        {
            [self setLocalState: [model stateDataForKey:keyNumber]];
            NSString * arg = [NSString stringWithFormat:@"0 %i", key];
            NSLog(@"  state loaded: %@", arg);
            UnitySendMessage([_callbackObjectName UTF8String], "onStateLoaded", [arg UTF8String]);
        }
        else if (status == GPGAppStateLoadStatusNotFound)
        {
            NSString * arg = [NSString stringWithFormat:@"-1 %i", key];
            NSLog(@"  state not found: %@", arg);
            UnitySendMessage([_callbackObjectName UTF8String], "onStateLoaded", [arg UTF8String]);
        }
        else if (error != nil)
        {
            NSString * arg = [NSString stringWithFormat:@"%i %i", error.code, key];
            NSLog(@"  state load failed: %@", arg);
            UnitySendMessage([_callbackObjectName UTF8String], "onStateLoaded", [arg UTF8String]);
        }
    }
    conflictHandler:^NSData *(NSNumber *k, NSData *localState, NSData *remoteState)
    {
        return remoteState;
    }];
}

-(void)saveState:(int)key data:(NSData*)data
{
    NSLog(@"saving state: %i", key);
    GPGAppStateModel *model = [GPGManager sharedInstance].applicationModel.appState;
    NSNumber * keyNumber = [NSNumber numberWithInt:key];
    [model setStateData:data forKey:keyNumber];
    
    _justResolved = NO;
    
    [model updateForKey:keyNumber completionHandler:^(GPGAppStateWriteStatus status, NSError *error)
    {
        if (status == GPGAppStateWriteStatusSuccess)
        {
            if (!_justResolved)
            {
                _justResolved = NO;
                [self setLocalState: [model stateDataForKey:keyNumber]];
                NSString * arg = [NSString stringWithFormat:@"0 %i", key];
                NSLog(@"  state saved: %@", arg);
                UnitySendMessage([_callbackObjectName UTF8String], "onStateSaved", [arg UTF8String]);
            }
        }
        if (error != nil)
        {
            NSString * arg = [NSString stringWithFormat:@"%i %i", error.code, key];
            NSLog(@"  state save failed: %@", arg);
            UnitySendMessage([_callbackObjectName UTF8String], "onStateSaved", [arg UTF8String]);
        }
    }
    conflictHandler:^NSData *(NSNumber *k, NSData *localState, NSData *remoteState)
    {
        [self setLocalState: localState];
        [self setRemoteState: remoteState];
        NSString * arg = [NSString stringWithFormat:@"0 %i", key];
        NSLog(@"  conflict on save: %@", arg);
        UnitySendMessage([_callbackObjectName UTF8String], "onStateConflicted", [arg UTF8String]);
        _justResolved = YES;
        return localState;
    }];
}

-(void)resolveState:(int)key data:(NSData*)data
{
    NSLog(@"resolving state: %i", key);
    GPGAppStateModel *model = [GPGManager sharedInstance].applicationModel.appState;
    NSNumber * keyNumber = [NSNumber numberWithInt:key];
    [model setStateData:data forKey:keyNumber];
    
    [model updateForKey:keyNumber completionHandler:^(GPGAppStateWriteStatus status, NSError *error)
    {
        [self setLocalState: [model stateDataForKey:keyNumber]];
        NSString * arg = [NSString stringWithFormat:@"0 %i", key];
        NSLog(@"  state resolved: %@", arg);
        UnitySendMessage([_callbackObjectName UTF8String], "onStateResolved", [arg UTF8String]);
    }
    conflictHandler:^NSData *(NSNumber *k, NSData *localState, NSData *remoteState)
    {
        NSLog(@"  state conflicted on resolve: %@", k);
        return data;
    }];
}

-(void)deleteState:(int)key
{
    NSLog(@"deleting state inside");
    GPGAppStateModel *model = [GPGManager sharedInstance].applicationModel.appState;
    NSNumber * keyNumber = [NSNumber numberWithInt:key];
    [model deleteForKey:keyNumber completionHandler:^(GPGAppStateWriteStatus status, NSError *error)
     {
         if (status == GPGAppStateWriteStatusSuccess)
         {
             NSString * arg = [NSString stringWithFormat:@"0 %@", keyNumber];
             UnitySendMessage([_callbackObjectName UTF8String], "onStateDeleted", [arg UTF8String]);
         }
         if (error != nil)
         {
             NSString * arg = [NSString stringWithFormat:@"%i %@", error.code, keyNumber];
             UnitySendMessage([_callbackObjectName UTF8String], "onStateDeleted", [arg UTF8String]);
         }
     }];
}

@end

extern UIViewController *UnityGetGLViewController();
static GpgUnityAdapter * _instance = nil;

static char * _returnString = NULL;

char * MakeReturnString(const char* string)
{
    if (string == NULL)
        return NULL;
    
    int size = strlen(string) + 1;
    _returnString = (char*)malloc(size);
    memset(_returnString, 0, size);
    strcpy(_returnString, string);
    return _returnString;
}

extern "C"
{
    void GpgInit(char * callbackObjectName, int verbose, int gamesEnabled, int appStateEnabled)
    {
        _instance = [[GpgUnityAdapter alloc] initWithCallbackObjectName:MakeNSString(callbackObjectName)
                                                                verbose:verbose
                                                           gamesEnabled:gamesEnabled
                                                        appStateEnabled:appStateEnabled
                                                         viewController:UnityGetGLViewController()];
    }

    void GpgConnect(int silent)
    {
        if (_instance == nil)
            return;
        
        [_instance connectSilent: silent];
    }

    void GpgDisconnect()
    {
        if (_instance == nil)
            return;
        
        [_instance disconnect];
    }
    
    void GpgSignOut()
    {
        if (_instance == nil)
            return;
        
        [_instance signOut];
    }
    
    char * GpgGetCurrentAccountName()
    {
        return NULL;
    }
    
    int GpgIsConnected()
    {
        if (_instance == nil)
            return NO;
        
        return [_instance isConnected];
    }
    
    int GpgIsConnecting()
    {
        if (_instance == nil)
            return NO;
        
        return [_instance isConnecting];
    }
    
    
    
    void GpgShowAchievements()
    {
        if (_instance == nil)
            return;
        
        [_instance showAchievements];
    }
    
    void GpgLoadAchievements()
    {
        if (_instance == nil)
            return;
        
        [_instance loadAchievements];
    }
    
    void GpgIncrementAchievement(char * achievementId, int steps)
    {
        if (_instance == nil)
            return;
        
        [_instance incrementAchievement:MakeNSString(achievementId) by:steps];
    }
    
    void GpgRevealAchievement(char * achievementId)
    {
        if (_instance == nil)
            return;
        
        [_instance revealAchievement:MakeNSString(achievementId)];
    }
    
    void GpgUnlockAchievement(char * achievementId)
    {
        if (_instance == nil)
            return;
        
        [_instance unlockAchievement:MakeNSString(achievementId)];
    }
    
    int GpgGetAchievementsCount()
    {
        return [_instance getAchievementsCount];
    }
    
    char * GpgGetAchievementId(int index)
    {
        return MakeReturnString([[_instance getAchievementId:index] UTF8String]);
    }
    
    int GpgGetAchievementState(int index)
    {
        return [_instance getAchievementState:index];
    }
    
    int GpgGetAchievementType(int index)
    {
        return [_instance getAchievementType:index];
    }
    
    char * GpgGetAchievementName(int index)
    {
        return MakeReturnString([[_instance getAchievementName:index] UTF8String]);
    }
    
    char * GpgGetAchievementDescription(int index)
    {
        return MakeReturnString([[_instance getAchievementDescription:index] UTF8String]);
    }
    
    char * GpgGetAchievementRevealedIconUrl(int index)
    {
        return MakeReturnString([[_instance getAchievementRevealedIconUrl:index] UTF8String]);
    }
    
    char * GpgGetAchievementUnlockedIconUrl(int index)
    {
        return MakeReturnString([[_instance getAchievementUnlockedIconUrl:index] UTF8String]);
    }
    
    int GpgGetAchievementCompletedSteps(int index)
    {
        return [_instance getAchievementCompletedSteps:index];
    }
    
    int GpgGetAchievementNumberOfSteps(int index)
    {
        return [_instance getAchievementNumberOfSteps:index];
    }
    
    char * GpgGetAchievementFormattedCompletedSteps(int index)
    {
        return MakeReturnString([[_instance getAchievementFormattedCompletedSteps:index] UTF8String]);
    }
    
    char * GpgGetAchievementFormattedNumberOfSteps(int index)
    {
        return MakeReturnString([[_instance getAchievementFormattedNumberOfSteps:index] UTF8String]);
    }
    
    long GpgGetAchievementLastUpdatedTimestamp(int index)
    {
        return [_instance getAchievementLastUpdatedTimestamp:index];
    }
    
    float GpgGetAchievementProgress(int index)
    {
        return [_instance getAchievementProgress:index];
    }
    
    
    
    void GpgShowLeaderboards()
    {
        [_instance showLeaderboards];
    }
    
    void GpgShowLeaderboard(char * leaderboardId)
    {
        [_instance showLeaderboard: MakeNSString(leaderboardId)];
    }
    
    void GpgSubmitScore(char * leaderboardId, int score)
    {
        [_instance submitScore:score toLeaderboard:MakeNSString(leaderboardId)];
    }
    
    void GpgLoadLeaderboards()
    {
        [_instance loadLeaderboards];
    }
    
    int GpgGetLeaderboardsCount()
    {
        return [_instance getLeaderboardsCount];
    }

    char * GpgGetLeaderboardId(int index)
    {
        return MakeReturnString([[_instance getLeaderboardId:index] UTF8String]);
    }
    
    int GpgGetLeaderboardScoreOrder(int index)
    {
        return [_instance getLeaderboardScoreOrder:index];
    }
    
    char * GpgGetLeaderboardTitle(int index)
    {
        return MakeReturnString([[_instance getLeaderboardTitle:index] UTF8String]);
    }
    
    char * GpgGetLeaderboardIconImageUrl(int index)
    {
        return MakeReturnString([[_instance getLeaderboardIconImageUrl:index] UTF8String]);
    }
    
    void GpgLoadScores(char * leaderboardId, int span, int collection, int seed, int maxResults)
    {
        [_instance loadScoresForLeaderboard:MakeNSString(leaderboardId) span:span collection:collection seed:seed maxResults:maxResults];
    }
    
    int GpgGetScoresCount()
    {
        return [_instance getScoresCount];
    }
    
    int GpgGetScoresCollection()
    {
        return [_instance getScoresCollection];
    }
    
    int GpgGetScoresSeed()
    {
        return [_instance getScoresSeed];
    }
    
    int GpgGetScoresSpan()
    {
        return [_instance getScoresSpan];
    }
    
    long GpgGetScoreValue(int index)
    {
        return [_instance getScoreValue:index];
    }
    
    char * GpgGetScoreAvatarUrl(int index)
    {
        return MakeReturnString([[_instance getScoreAvatarUrl:index] UTF8String]);
    }
    
    char * GpgGetScoreFormattedRank(int index)
    {
        return MakeReturnString([[_instance getScoreFormattedRank:index] UTF8String]);
    }
    
    char * GpgGetScoreFormattedScore(int index)
    {
        return MakeReturnString([[_instance getScoreFormattedScore:index] UTF8String]);
    }
    
    char * GpgGetScoreDisplayName(int index)
    {
        return MakeReturnString([[_instance getScoreDisplayName:index] UTF8String]);
    }
    
    char * GpgGetScorePlayerId(int index)
    {
        return MakeReturnString([[_instance getScorePlayerId:index] UTF8String]);
    }
    
    int GpgGetScoreRank(int index)
    {
        return [_instance getScoreRank:index];
    }
    
    long GpgGetScoreWriteTimestamp(int index)
    {
        return [_instance getScoreWriteTimestamp:index];
    }
    
    
    
    int GpgGetMaxCloudSaveKeys()
    {
        return [_instance getMaxCloudSaveKeys];
    }
    
    int GpgGetMaxCloudSaveStateSize()
    {
        return [_instance getMaxCloudSaveStateSize];
    }
    
    void GpgLoadStates()
    {
        [_instance loadStates];
    }
    
    void GpgLoadState(int key)
    {
        [_instance loadState: key];
    }

    void GpgSaveState(int key, const char * data, int size)
    {
        NSLog(@"saving state");
        
        NSData * nsData = nil;
        
        if (data == nil || size <= 0)
            nsData = [[NSData alloc] init];
        else
            nsData = [[NSData alloc] initWithBytes:data length:size];
        
        [_instance saveState: key data:nsData];
    }
    
    void GpgResolveState(int key, const char * data, int size)
    {
        NSLog(@"resolving state from unity");
        
        NSData * nsData = nil;
        
        if (data == nil || size <= 0)
            nsData = [[NSData alloc] init];
        else
            nsData = [[NSData alloc] initWithBytes:data length:size];
        
        [_instance resolveState:key data:nsData];

    }
    
    void GpgDeleteState(int key)
    {
        [_instance deleteState: key];
    }
    
    int GpgGetLocalState(char * buffer, int bufferSize)
    {
        NSData * data = [_instance getLocalState];
        if (data == nil)
            return 0;
        int size = MIN(bufferSize, data.length);
        memcpy((void*)buffer, data.bytes, size);
        return size;
    }

    int GpgGetRemoteState(char * buffer, int bufferSize)
    {
        NSData * data = [_instance getRemoteState];
        if (data == nil)
            return 0;
        int size = MIN(bufferSize, data.length);
        memcpy((void*)buffer, data.bytes, size);
        return size;
    }
}
