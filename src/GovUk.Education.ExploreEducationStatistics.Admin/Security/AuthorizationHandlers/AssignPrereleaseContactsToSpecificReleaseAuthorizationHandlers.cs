using GovUk.Education.ExploreEducationStatistics.Common.Security.AuthorizationHandlers;
using GovUk.Education.ExploreEducationStatistics.Content.Model;
using GovUk.Education.ExploreEducationStatistics.Content.Model.Database;
using Microsoft.AspNetCore.Authorization;
using static GovUk.Education.ExploreEducationStatistics.Admin.Security.AuthorizationHandlers.AuthorizationHandlerUtil;

namespace GovUk.Education.ExploreEducationStatistics.Admin.Security.AuthorizationHandlers
{
    public class AssignPrereleaseContactsToSpecificReleaseRequirement : IAuthorizationRequirement
    {}

    public class AssignPrereleaseContactsToSpecificReleaseAuthorizationHandler : 
        CompoundAuthorizationHandler<AssignPrereleaseContactsToSpecificReleaseRequirement, Release>
    {
        public AssignPrereleaseContactsToSpecificReleaseAuthorizationHandler(ContentDbContext context) : base(
            new CanUpdateAllReleasesAuthorizationHandler(),
            new HasEditorRoleOnReleaseAuthorizationHandler(context))
        {
            
        }
    
        public class CanUpdateAllReleasesAuthorizationHandler : 
            EntityAuthorizationHandler<AssignPrereleaseContactsToSpecificReleaseRequirement, Release>
        {
            public CanUpdateAllReleasesAuthorizationHandler()
                : base(ctx =>
                    ctx.Entity.Status == ReleaseStatus.Approved && 
                    SecurityUtils.HasClaim(ctx.User, SecurityClaimTypes.UpdateAllReleases)
                )
            {
            
            }
        }

        public class HasEditorRoleOnReleaseAuthorizationHandler
            : HasRoleOnReleaseAuthorizationHandler<AssignPrereleaseContactsToSpecificReleaseRequirement>
        {
            public HasEditorRoleOnReleaseAuthorizationHandler(ContentDbContext context) 
                : base(context, ctx => ctx.Release.Status == ReleaseStatus.Approved && ContainsEditorRole(ctx.Roles))
            {}
        }
    }
}
