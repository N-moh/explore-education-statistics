using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static GovUk.Education.ExploreEducationStatistics.Common.Services.CollectionUtils;
using static GovUk.Education.ExploreEducationStatistics.Content.Model.Database.ContentDbUtils;

namespace GovUk.Education.ExploreEducationStatistics.Content.Model.Tests
{
    public class PublicationMethodologyPersistenceTests
    {
        [Fact]
        public async Task DeletingPublicationDoesNotDeleteMethodology()
        {
            var contextId = Guid.NewGuid().ToString();

            var methodologyId = Guid.NewGuid();
            var publication1 = new Publication();
            var publication2 = new Publication();
            
            // Store a new Methodology, linked to 2 Publications
            await using (var context = InMemoryContentDbContext(contextId))
            {
                var methodology = new Methodology
                {
                    Id = methodologyId,
                    MethodologyParent = new MethodologyParent
                    {
                        Publications = new List<PublicationMethodology>
                        {
                            new PublicationMethodology
                            {
                                Publication = publication1
                            },
                            new PublicationMethodology
                            {
                                Publication = publication2
                            }
                        }
                    }
                };

                await context.Publications.AddRangeAsync(publication1, publication2);
                await context.Methodologies.AddAsync(methodology);
                await context.SaveChangesAsync();
            }

            // Retrieve the Methodology, and ensure it remains linked to the 2 Publications
            await using (var context = InMemoryContentDbContext(contextId))
            {
                var methodology = await context
                    .Methodologies
                    .Include(m => m.MethodologyParent)
                    .ThenInclude(m => m.Publications)
                    .FirstAsync(m => m.Id == methodologyId);

                Assert.Equal(methodology
                    .MethodologyParent
                    .Publications
                    .Select(p => p.PublicationId), 
                    AsList(publication1.Id, publication2.Id));
            }

            // Delete one of the Publications
            await using (var context = InMemoryContentDbContext(contextId))
            {
                context.Publications.Remove(publication1);
                await context.SaveChangesAsync();
            }
            
            // Retrieve the Methodology, and ensure it remains linked to the non-deleted Publication
            await using (var context = InMemoryContentDbContext(contextId))
            {
                var methodology = await context
                    .Methodologies
                    .Include(m => m.MethodologyParent)
                    .ThenInclude(m => m.Publications)
                    .FirstAsync(m => m.Id == methodologyId);

                Assert.Equal(methodology
                        .MethodologyParent
                        .Publications
                        .Select(p => p.PublicationId), 
                    AsList(publication2.Id));
            }
        }
    }
}