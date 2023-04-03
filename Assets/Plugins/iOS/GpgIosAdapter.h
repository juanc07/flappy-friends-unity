#import <GooglePlus/GooglePlus.h>
#import <PlayGameServices/PlayGameServices.h>

@interface GpgUnityAdapter : NSObject
<
GPPSignInDelegate,
GPGAchievementControllerDelegate,
GPGLeaderboardControllerDelegate,
GPGLeaderboardsControllerDelegate
>
{
    
}

+ (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;

@end