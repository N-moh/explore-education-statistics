using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Admin.Security;
using GovUk.Education.ExploreEducationStatistics.Admin.Security.AuthorizationHandlers;
using GovUk.Education.ExploreEducationStatistics.Admin.Services;
using GovUk.Education.ExploreEducationStatistics.Admin.Services.Interfaces;
using GovUk.Education.ExploreEducationStatistics.Common.Extensions;
using GovUk.Education.ExploreEducationStatistics.Content.Model;
using GovUk.Education.ExploreEducationStatistics.Content.Model.Database;
using Moq;
using Xunit;
using PublisherReleaseStatus = GovUk.Education.ExploreEducationStatistics.Publisher.Model.ReleaseStatus;
using static GovUk.Education.ExploreEducationStatistics.Admin.Tests.Security.AuthorizationHandlers.Utils.ReleaseAuthorizationHandlersTestUtil;
using static GovUk.Education.ExploreEducationStatistics.Common.Services.EnumUtil;
using static GovUk.Education.ExploreEducationStatistics.Content.Model.PublicationRole;
using static GovUk.Education.ExploreEducationStatistics.Content.Model.ReleaseStatus;
using static GovUk.Education.ExploreEducationStatistics.Publisher.Model.ReleaseStatusOverallStage;

namespace GovUk.Education.ExploreEducationStatistics.Admin.Tests.Security.AuthorizationHandlers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UpdateSpecificReleaseAuthorizationHandlerTests
    {
        public class UpdateSpecificReleaseAuthorizationHandlerClaimTests
        {
            [Fact]
            public async Task UpdateSpecificReleaseAuthorizationHandler_ReleasePublishingNotStarted()
            {
                // Assert that only users with the "UpdateAllReleases" claim can
                // update an arbitrary Release if it has not started publishing
                await GetEnumValues<ReleaseStatus>().ForEachAsync(
                    async status =>
                    {
                        var release = new Release
                        {
                            Id = Guid.NewGuid(),
                            Status = status
                        };

                        await AssertReleaseHandlerSucceedsWithCorrectClaims<UpdateSpecificReleaseRequirement>(
                            HandlerSupplier(release),
                            release,
                            SecurityClaimTypes.UpdateAllReleases
                        );
                    }
                );
            }
            
            [Fact]
            public async Task UpdateSpecificReleaseAuthorizationHandler_ReleasePublishing()
            {
                // Assert that no users can update an arbitrary Release
                // if it has started publishing
                await GetEnumValues<ReleaseStatus>().ForEachAsync(
                    async status =>
                    {
                        var release = new Release
                        {
                            Id = Guid.NewGuid(),
                            Status = status,
                        };

                        await AssertReleaseHandlerSucceedsWithCorrectClaims<UpdateSpecificReleaseRequirement>(
                            HandlerSupplierWhenPublishing(release),
                            release
                        );
                    }
                );
            }
            
            [Fact]
            public async Task UpdateSpecificReleaseAuthorizationHandler_ReleasePublished()
            {
                // Assert that no users can update an arbitrary
                // Release if it has been published
                await GetEnumValues<ReleaseStatus>().ForEachAsync(
                    async status =>
                    {
                        var release = new Release
                        {
                            Id = Guid.NewGuid(),
                            Status = status,
                            Published = DateTime.UtcNow
                        };

                        await AssertReleaseHandlerSucceedsWithCorrectClaims<UpdateSpecificReleaseRequirement>(
                            HandlerSupplier(release),
                            release
                        );
                    }
                );
            }
            
        }

        public class UpdateSpecificReleaseAuthorizationHandlerPublicationRoleTests
        {
            [Fact]
            public async Task UpdateSpecificReleaseAuthorizationHandler_ReleasePublishingNotStarted()
            {
                await GetEnumValues<ReleaseStatus>().ForEachAsync(
                    async status =>
                    {
                        var release = new Release
                        {
                            Id = Guid.NewGuid(),
                            Publication = new Publication
                            {
                                Id = Guid.NewGuid()
                            },
                            Published = null,
                            Status = status
                        };

                        // Assert that a User who has the Publication Owner role on a
                        // Release can update it if it is not Approved
                        if (status != Approved)
                        {
                            await AssertReleaseHandlerSucceedsWithCorrectPublicationRoles<UpdateSpecificReleaseRequirement>(
                                HandlerSupplier(release),
                                release,
                                Owner
                            );
                        }
                        else
                        {
                            // Assert that a User who has the Publication Owner role on a
                            // Release cannot update it if it is not Approved
                            await AssertReleaseHandlerSucceedsWithCorrectPublicationRoles<UpdateSpecificReleaseRequirement>(
                                HandlerSupplier(release),
                                release
                            );
                        }
                    }
                );
            }

            [Fact]
            public async Task UpdateSpecificReleaseAuthorizationHandler_ReleasePublishing()
            {
                await GetEnumValues<ReleaseStatus>().ForEachAsync(
                    async status =>
                    {
                        var release = new Release
                        {
                            Id = Guid.NewGuid(),
                            Publication = new Publication
                            {
                                Id = Guid.NewGuid()
                            },
                            Published = null,
                            Status = status
                        };

                        // Assert that no User Publication roles will allow updating a Release once it has started publishing
                        await AssertReleaseHandlerSucceedsWithCorrectPublicationRoles<UpdateSpecificReleaseRequirement>(
                            HandlerSupplierWhenPublishing(release),
                            release
                        );
                    }
                );
            }

            [Fact]
            public async Task UpdateSpecificReleaseAuthorizationHandler_ReleasePublished()
            {
                await GetEnumValues<ReleaseStatus>().ForEachAsync(
                    async status =>
                    {
                        var release = new Release
                        {
                            Id = Guid.NewGuid(),
                            Publication = new Publication
                            {
                                Id = Guid.NewGuid()
                            },
                            Status = status,
                            Published = DateTime.UtcNow
                        };

                        // Assert that no User Publication roles will allow updating a Release once it has been published
                        await AssertReleaseHandlerSucceedsWithCorrectPublicationRoles<UpdateSpecificReleaseRequirement>(
                            HandlerSupplier(release), 
                            release
                        );
                    }
                );
            }
        }

        public class UpdateSpecificReleaseAuthorizationHandlerReleaseRoleTests
        {
            [Fact]
            public async Task UpdateSpecificReleaseAuthorizationHandler_ReleasePublishingNotStarted()
            {
                await GetEnumValues<ReleaseStatus>().ForEachAsync(
                    async status =>
                    {
                        var release = new Release
                        {
                            Id = Guid.NewGuid(),
                            Publication = new Publication
                            {
                                Id = Guid.NewGuid()
                            },
                            Published = null,
                            Status = status
                        };

                        // Assert that a User who has the "Contributor", "Lead" or "Approver" role on a
                        // Release can update it if it is not Approved
                        if (status != Approved)
                        {
                            await AssertReleaseHandlerSucceedsWithCorrectReleaseRoles<UpdateSpecificReleaseRequirement>(
                                HandlerSupplier(release),
                                release,
                                ReleaseRole.Contributor,
                                ReleaseRole.Lead,
                                ReleaseRole.Approver
                            );
                        }
                        else
                        {
                            // Assert that a User who has the "Approver" role on a
                            // Release can update it if it is Approved
                            await AssertReleaseHandlerSucceedsWithCorrectReleaseRoles<UpdateSpecificReleaseRequirement>(
                                HandlerSupplier(release),
                                release,
                                ReleaseRole.Approver
                            );
                        }
                    }
                );
            }

            [Fact]
            public async Task UpdateSpecificReleaseAuthorizationHandler_ReleasePublishing()
            {
                await GetEnumValues<ReleaseStatus>().ForEachAsync(
                    async status =>
                    {
                        var release = new Release
                        {
                            Id = Guid.NewGuid(),
                            Publication = new Publication
                            {
                                Id = Guid.NewGuid()
                            },
                            Published = null,
                            Status = status
                        };

                        // Assert that no User Release roles will allow updating a Release once it has started publishing
                        await AssertReleaseHandlerSucceedsWithCorrectReleaseRoles<UpdateSpecificReleaseRequirement>(
                            HandlerSupplierWhenPublishing(release),
                            release
                        );
                    }
                );
            }

            [Fact]
            public async Task UpdateSpecificReleaseAuthorizationHandler_ReleasePublished()
            {
                await GetEnumValues<ReleaseStatus>().ForEachAsync(
                    async status =>
                    {
                        var release = new Release
                        {
                            Id = Guid.NewGuid(),
                            Publication = new Publication
                            {
                                Id = Guid.NewGuid()
                            },
                            Status = status,
                            Published = DateTime.UtcNow
                        };

                        // Assert that no User Release roles will allow updating a Release once it has been published
                        await AssertReleaseHandlerSucceedsWithCorrectReleaseRoles<UpdateSpecificReleaseRequirement>(
                            HandlerSupplier(release), 
                            release
                        );
                    }
                );
            }
        }

        private static Func<ContentDbContext, UpdateSpecificReleaseAuthorizationHandler> HandlerSupplierWhenPublishing(
            Release release)
        { 
            var statusListWhenPublishing = new List<PublisherReleaseStatus>
            {
                new PublisherReleaseStatus()
            };
            
            return HandlerSupplier(release, statusListWhenPublishing);
        }

        private static Func<ContentDbContext, UpdateSpecificReleaseAuthorizationHandler> HandlerSupplier(
            Release release,
            List<PublisherReleaseStatus> publishingStatuses = null)
        {
            var releaseStatusRepository = new Mock<IReleaseStatusRepository>();

            releaseStatusRepository.Setup(
                    s => s.GetAllByOverallStage(
                        release.Id,
                        Started,
                        Complete
                    )
                )
                .ReturnsAsync(publishingStatuses ?? new List<PublisherReleaseStatus>());

            return contentDbContext =>
            {
                contentDbContext.Add(release);
                contentDbContext.SaveChanges();
                
                return new UpdateSpecificReleaseAuthorizationHandler(
                    releaseStatusRepository.Object,
                    new UserPublicationRoleRepository(contentDbContext),
                    new UserReleaseRoleRepository(contentDbContext)
                );
            };
        }
    }
}
