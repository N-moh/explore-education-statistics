using GovUk.Education.ExploreEducationStatistics.Content.Model;
using GovUk.Education.ExploreEducationStatistics.Content.Model.Database;
using Microsoft.AspNetCore.Authorization;
using static GovUk.Education.ExploreEducationStatistics.Admin.Security.AuthorizationHandlers.AuthorizationHandlerUtil;

namespace GovUk.Education.ExploreEducationStatistics.Admin.Security.AuthorizationHandlers
{
    public class ApproveSpecificReleaseRequirement : IAuthorizationRequirement
    {}
    
    public class ApproveSpecificReleaseAuthorizationHandler : CompoundAuthorizationHandler<ApproveSpecificReleaseRequirement, Release>
    {
        public ApproveSpecificReleaseAuthorizationHandler(ContentDbContext context) : base(
            new ApproveSpecificReleaseCanApproveAllReleasesAuthorizationHandler(),
            new ApproveSpecificReleaseHasApproverRoleOnReleaseAuthorizationHandler(context))
        {
            
        }
    }
    
    public class ApproveSpecificReleaseCanApproveAllReleasesAuthorizationHandler : HasClaimAuthorizationHandler<
        ApproveSpecificReleaseRequirement>
    {
        public ApproveSpecificReleaseCanApproveAllReleasesAuthorizationHandler() 
            : base(SecurityClaimTypes.ApproveAllReleases) {}
    }

    public class ApproveSpecificReleaseHasApproverRoleOnReleaseAuthorizationHandler
        : HasRoleOnReleaseAuthorizationHandler<ApproveSpecificReleaseRequirement>
    {
        public ApproveSpecificReleaseHasApproverRoleOnReleaseAuthorizationHandler(ContentDbContext context) 
            : base(context, ctx => ContainsApproverRole(ctx.Roles))
        {}
    }
    
}