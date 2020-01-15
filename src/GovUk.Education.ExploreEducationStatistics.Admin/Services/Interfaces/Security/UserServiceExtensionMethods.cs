using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Admin.Security;
using GovUk.Education.ExploreEducationStatistics.Common.Model;
using GovUk.Education.ExploreEducationStatistics.Content.Model;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ExploreEducationStatistics.Admin.Services.Interfaces.Security
{
    public static class UserServiceExtensionMethods
    {
        public static Task<Either<ActionResult, bool>> CheckCanViewAllTopics(this IUserService userService)
        {
            return userService.DoCheck(SecurityPolicies.CanViewAllTopics);
        }
        
        public static Task<Either<ActionResult, bool>> CheckCanViewAllReleases(this IUserService userService)
        {
            return userService.DoCheck(SecurityPolicies.CanViewAllReleases);
        }
        
        public static Task<Either<ActionResult, Topic>> CheckCanCreatePublicationForTopic(
            this IUserService userService, Topic topic)
        {
            return userService.DoCheck(topic, SecurityPolicies.CanCreatePublicationForSpecificTopic);
        }
        
        public static Task<Either<ActionResult, Publication>> CheckCanCreateReleaseForPublication(
            this IUserService userService, Publication publication)
        {
            return userService.DoCheck(publication, SecurityPolicies.CanCreateReleaseForSpecificPublication);
        }
        
        public static Task<Either<ActionResult, Release>> CheckCanViewRelease(
            this IUserService userService, Release release)
        {
            return userService.DoCheck(release, SecurityPolicies.CanViewSpecificRelease);
        }
        
        public static Task<Either<ActionResult, Release>> CheckCanUpdateRelease(
            this IUserService userService, Release release)
        {
            return userService.DoCheck(release, SecurityPolicies.CanUpdateSpecificRelease);
        }

        public static Task<Either<ActionResult, Release>> CheckCanUpdateReleaseStatus(
            this IUserService userService, Release release, ReleaseStatus status)
        {
            switch (status)
            {
                case ReleaseStatus.Draft:
                {
                    return userService.CheckCanMarkReleaseAsDraft(release);
                }
                case ReleaseStatus.HigherLevelReview:
                {
                    return userService.CheckCanSubmitReleaseToHigherApproval(release);
                }
                case ReleaseStatus.Approved:
                {
                    return userService.CheckCanApproveRelease(release);
                }
                default:
                {
                    return Task.FromResult(new Either<ActionResult, Release>(release));
                }
            }
        }

        public static Task<Either<ActionResult, Release>> CheckCanMarkReleaseAsDraft(
            this IUserService userService, Release release)
        {
            return userService.DoCheck(release, SecurityPolicies.CanMarkSpecificReleaseAsDraft);
        }

        public static Task<Either<ActionResult, Release>> CheckCanSubmitReleaseToHigherApproval(
            this IUserService userService, Release release)
        {
            return userService.DoCheck(release, SecurityPolicies.CanSubmitSpecificReleaseToHigherReview);
        }
        
        public static Task<Either<ActionResult, Release>> CheckCanApproveRelease(
            this IUserService userService, Release release)
        {
            return userService.DoCheck(release, SecurityPolicies.CanApproveSpecificRelease);
        }
        
        private static async Task<Either<ActionResult, T>> DoCheck<T>(this IUserService userService, T resource, SecurityPolicies policy) 
        {
            var result = await userService.MatchesPolicy(resource, policy);
            return result ? new Either<ActionResult, T>(resource) : new ForbidResult();
        }
        
        private static async Task<Either<ActionResult, bool>> DoCheck(this IUserService userService, SecurityPolicies policy) 
        {
            var result = await userService.MatchesPolicy(policy);
            return result ? new Either<ActionResult, bool>(true) : new ForbidResult();
        }
    }
}