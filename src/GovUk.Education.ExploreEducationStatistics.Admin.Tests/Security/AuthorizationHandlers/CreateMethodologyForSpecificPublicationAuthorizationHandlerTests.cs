using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Admin.Security.AuthorizationHandlers;
using GovUk.Education.ExploreEducationStatistics.Admin.Services.Interfaces;
using GovUk.Education.ExploreEducationStatistics.Content.Model;
using GovUk.Education.ExploreEducationStatistics.Content.Model.Database;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;
using static GovUk.Education.ExploreEducationStatistics.Admin.Security.SecurityClaimTypes;
using static GovUk.Education.ExploreEducationStatistics.Admin.Tests.Security.AuthorizationHandlers.Utils.AuthorizationHandlersTestUtil;
using static GovUk.Education.ExploreEducationStatistics.Admin.Tests.Services.DbUtils;
using static GovUk.Education.ExploreEducationStatistics.Common.Services.CollectionUtils;
using static GovUk.Education.ExploreEducationStatistics.Common.Tests.Utils.MockUtils;
using static Moq.MockBehavior;

namespace GovUk.Education.ExploreEducationStatistics.Admin.Tests.Security.AuthorizationHandlers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CreateMethodologyForSpecificPublicationAuthorizationHandlerTests
    {
        private static readonly Guid UserId = Guid.NewGuid();
            
        private static readonly Publication Publication = new Publication
        {
            Id = Guid.NewGuid(),
            Methodologies = new List<PublicationMethodology>()
        };
            
        private static readonly Publication PublicationWithExternalMethodology = new Publication
        {
            Id = Guid.NewGuid(),
            Methodologies = new List<PublicationMethodology>(),
            ExternalMethodology = new ExternalMethodology()
        };
            
        private static readonly Publication PublicationWithOwnedMethodology = new Publication
        {
            Id = Guid.NewGuid(),
            Methodologies = AsList(new PublicationMethodology
            {
                Owner = true
            })
        };
            
        private static readonly Publication PublicationWithAdoptedMethodology = new Publication
        {
            Id = Guid.NewGuid(),
            Methodologies = AsList(new PublicationMethodology
            {
                Owner = false
            })
        };

        public class CreateMethodologyForSpecificPublicationAuthorizationHandlerClaimTests
        {
            [Fact]
            public async Task UserWithCorrectClaimCanCreateMethodologyForAnyPublication()
            {
                await AssertUserWithCorrectClaimCanCreateMethodology(Publication);
            }

            [Fact]
            public async Task UserWithCorrectClaimCanCreateMethodologyForAnyPublication_AdoptedAnotherMethodologyButNotOwned()
            {
                await AssertUserWithCorrectClaimCanCreateMethodology(PublicationWithAdoptedMethodology);
            }
            
            [Fact]
            public async Task UserWithCorrectClaimCannotCreateMethodologyForAnyPublication_OwnsAnotherMethodology()
            {
                await AssertUserWithCorrectClaimCannotCreateMethodology(PublicationWithOwnedMethodology);
            }

            private static async Task AssertUserWithCorrectClaimCanCreateMethodology(Publication publication)
            {
                await ForEachSecurityClaimAsync(async claim =>
                {
                    await using var context = InMemoryApplicationDbContext(Guid.NewGuid().ToString());
                    context.Attach(publication);
                    
                    var (handler, publicationRoleRepository) = CreateHandlerAndDependencies(context);

                    var user = CreateClaimsPrincipal(UserId, claim);
                    var authContext = CreateAuthContext(user, publication);

                    var expectedToPassByClaimAlone = claim == CreateAnyMethodology;

                    if (!expectedToPassByClaimAlone)
                    {
                        publicationRoleRepository
                            .Setup(s => s.GetAllRolesByUser(UserId, publication.Id))
                            .ReturnsAsync(AsList<PublicationRole>());
                    }

                    await handler.HandleAsync(authContext);
                    VerifyAllMocks(publicationRoleRepository);

                    // Verify that the presence of the "CreateAnyMethodology" Claim will pass the handler test, without
                    // the need for a specific Publication to be provided
                    Assert.Equal(expectedToPassByClaimAlone, authContext.HasSucceeded);
                });
            }

            private static async Task AssertUserWithCorrectClaimCannotCreateMethodology(
                Publication publication)
            {
                await ForEachSecurityClaimAsync(async claim =>
                {
                    await using var context = InMemoryApplicationDbContext(Guid.NewGuid().ToString());
                    context.Attach(publication);
                    
                    var (handler, publicationRoleRepository) = CreateHandlerAndDependencies(context);

                    var user = CreateClaimsPrincipal(UserId, claim);
                    var authContext = CreateAuthContext(user, publication);

                    await handler.HandleAsync(authContext);
                    VerifyAllMocks(publicationRoleRepository);

                    Assert.False(authContext.HasSucceeded);
                });
            }
        }
        
        public class CreateMethodologyForSpecificPublicationAuthorizationHandlerPublicationRoleTests
        {
            [Fact]
            public async Task UserCanCreateMethodologyForPublicationWithPublicationOwnerRole()
            {
                await AssertPublicationOwnerCanCreateMethodology(Publication);
            }            
            
            [Fact]
            public async Task UserCanCreateMethodologyForPublicationWithPublicationOwnerRole_AdoptedAnotherMethodologyButNotOwned()
            {
                await AssertPublicationOwnerCanCreateMethodology(PublicationWithAdoptedMethodology);
            }

            [Fact]
            public async Task UserCannotCreateMethodologyForPublicationWithoutPublicationOwnerRole()
            {
                await using var context = InMemoryApplicationDbContext(Guid.NewGuid().ToString());
                context.Attach(Publication);
                    
                var (handler, publicationRoleRepository) = CreateHandlerAndDependencies(context);
                
                var user = CreateClaimsPrincipal(UserId);
                var authContext = CreateAuthContext(user, Publication);
                
                publicationRoleRepository
                    .Setup(s => s.GetAllRolesByUser(UserId, Publication.Id))
                    .ReturnsAsync(AsList<PublicationRole>());

                await handler.HandleAsync(authContext);
                VerifyAllMocks(publicationRoleRepository);

                // Verify that the user can't create a Methodology for this Publication because they don't have 
                // Publication Owner role on it
                Assert.False(authContext.HasSucceeded);
            }
            
            [Fact]
            public async Task UserCannotCreateMethodologyForPublication_OwnsAnotherMethodology()
            {
                await AssertPublicationOwnerCannotCreateMethodology(PublicationWithOwnedMethodology);
            }

            private static async Task AssertPublicationOwnerCanCreateMethodology(Publication publication)
            {
                await using var context = InMemoryApplicationDbContext(Guid.NewGuid().ToString());
                context.Attach(publication);
                    
                var (handler, publicationRoleRepository) = CreateHandlerAndDependencies(context);

                var user = CreateClaimsPrincipal(UserId);
                var authContext = CreateAuthContext(user, publication);

                publicationRoleRepository
                    .Setup(s => s.GetAllRolesByUser(UserId, publication.Id))
                    .ReturnsAsync(AsList(PublicationRole.Owner));

                await handler.HandleAsync(authContext);
                VerifyAllMocks(publicationRoleRepository);

                // Verify that the user can create a Methodology for this Publication by virtue of having a Publication
                // Owner role on the Publication
                Assert.True(authContext.HasSucceeded);
            }

            private static async Task AssertPublicationOwnerCannotCreateMethodology(Publication publication)
            {
                await using var context = InMemoryApplicationDbContext(Guid.NewGuid().ToString());
                context.Attach(publication);
                    
                var (handler, publicationRoleRepository) = CreateHandlerAndDependencies(context);

                var user = CreateClaimsPrincipal(UserId);
                var authContext = CreateAuthContext(user, publication);

                await handler.HandleAsync(authContext);
                VerifyAllMocks(publicationRoleRepository);

                // Verify that the user can create a Methodology for this Publication by virtue of having a Publication
                // Owner role on the Publication
                Assert.False(authContext.HasSucceeded);
            }
        }

        private static AuthorizationHandlerContext CreateAuthContext(ClaimsPrincipal user, Publication publication)
        {
            return CreateAuthorizationHandlerContext<CreateMethodologyForSpecificPublicationRequirement, Publication>
                (user, publication);
        }

        private static (CreateMethodologyForSpecificPublicationAuthorizationHandler, Mock<IUserPublicationRoleRepository>)
            CreateHandlerAndDependencies(ContentDbContext context)
        {
            var publicationRoleRepository = new Mock<IUserPublicationRoleRepository>(Strict);

            var handler = new CreateMethodologyForSpecificPublicationAuthorizationHandler(
                publicationRoleRepository.Object, context);

            return (handler, publicationRoleRepository);
        }
    }
}