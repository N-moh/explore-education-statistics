﻿using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Admin.Security.AuthorizationHandlers;
using GovUk.Education.ExploreEducationStatistics.Admin.Services;
using GovUk.Education.ExploreEducationStatistics.Content.Model;
using Xunit;
using static GovUk.Education.ExploreEducationStatistics.Admin.Security.SecurityClaimTypes;
using static GovUk.Education.ExploreEducationStatistics.Admin.Tests.Security.AuthorizationHandlers.Utils.ReleaseAuthorizationHandlersTestUtil;
using static GovUk.Education.ExploreEducationStatistics.Content.Model.ReleaseRole;
using static GovUk.Education.ExploreEducationStatistics.Content.Model.ReleaseApprovalStatus;

namespace GovUk.Education.ExploreEducationStatistics.Admin.Tests.Security.AuthorizationHandlers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PublishSpecificReleaseAuthorizationHandlerTests
    {
        public class PublishSpecificReleaseAuthorizationHandlerClaimTests
        {
            [Fact]
            public async Task PublishSpecificReleaseAuthorizationHandler_FailsWhenDraft()
            {
                // Assert that no claims will allow a draft Release to be published
                await AssertReleaseHandlerSucceedsWithCorrectClaims<PublishSpecificReleaseRequirement>(
                    contentDbContext =>
                        new PublishSpecificReleaseAuthorizationHandler(new UserReleaseRoleRepository(contentDbContext)),
                    new Release
                    {
                        ApprovalStatus = Draft
                    }
                );
            }

            [Fact]
            public async Task PublishSpecificReleaseAuthorizationHandler_SucceedsWhenApproved()
            {
                // Assert that the PublishAllReleases claim will allow an approved Release to be published
                await AssertReleaseHandlerSucceedsWithCorrectClaims<PublishSpecificReleaseRequirement>(
                    contentDbContext =>
                        new PublishSpecificReleaseAuthorizationHandler(new UserReleaseRoleRepository(contentDbContext)),
                    new Release
                    {
                        ApprovalStatus = Approved
                    },
                    PublishAllReleases
                );
            }
        }

        public class PublishSpecificReleaseAuthorizationHandlerReleaseRoleTests
        {
            [Fact]
            public async Task PublishSpecificReleaseAuthorizationHandler_FailsWhenDraft()
            {
                // Assert that no User Release roles will allow a draft Release to be published
                await AssertReleaseHandlerSucceedsWithCorrectReleaseRoles<PublishSpecificReleaseRequirement>(
                    contentDbContext =>
                        new PublishSpecificReleaseAuthorizationHandler(new UserReleaseRoleRepository(contentDbContext)),
                    new Release
                    {
                        ApprovalStatus = Draft
                    }
                );
            }

            [Fact]
            public async Task PublishSpecificReleaseAuthorizationHandler_SucceedsWhenApproved()
            {
                // Assert that only the Approver User Release role will allow an approved Release to be published
                await AssertReleaseHandlerSucceedsWithCorrectReleaseRoles<PublishSpecificReleaseRequirement>(
                    contentDbContext => 
                        new PublishSpecificReleaseAuthorizationHandler(new UserReleaseRoleRepository(contentDbContext)),
                    new Release
                    {
                        ApprovalStatus = Approved
                    },
                    Approver);
            }
        }
    }
}