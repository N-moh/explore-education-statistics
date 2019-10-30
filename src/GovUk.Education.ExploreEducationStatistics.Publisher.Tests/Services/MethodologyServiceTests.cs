﻿using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ExploreEducationStatistics.Content.Model;
using GovUk.Education.ExploreEducationStatistics.Content.Model.Database;
using GovUk.Education.ExploreEducationStatistics.Publisher.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GovUk.Education.ExploreEducationStatistics.Publisher.Tests.Services
{
    public class MethodologyServiceTests
    {
        [Fact(Skip = "Bug with in memory database")]
        public void MethodologyService_GetTree()
        {
            var builder = new DbContextOptionsBuilder<ContentDbContext>();
            builder.UseInMemoryDatabase(databaseName: "GetTree");
            var options = builder.Options;

            using (var context = new ContentDbContext(options))
            {
                var methodologies = new List<Methodology>
                {
                    new Methodology
                    {
                        Id = new Guid("ddcb9b8a-c071-4d19-a315-f742682b1e18"),
                        Title = "Methodology A",
                        Summary = "first methodology"
                    },
                    new Methodology
                    {
                        Id = new Guid("22a27c18-3d09-41e5-88ea-b85eb3268ccc"),
                        Title = "Methodology B",
                        Summary = "second methodology"
                    }
                };

                var publications = new List<Publication>
                {
                    new Publication
                    {
                        Id = new Guid("ed70afba-f7e1-4ab3-bded-74d078b6fca0"),
                        Title = "Publication A",
                        TopicId = new Guid("0144e3f2-41e1-4aec-9c55-2671f454c85f"),
                        Slug = "publication-a",
                        Summary = "first publication",
                        MethodologyId = new Guid("ed70afba-f7e1-4ab3-bded-74d078b6fca0"),
                    },
                    new Publication
                    {
                        Id = new Guid("e45cf030-f29b-42c3-8270-3cc8267026f0"),
                        Title = "Publication B",
                        TopicId = new Guid("0144e3f2-41e1-4aec-9c55-2671f454c85f"),
                        Slug = "publication-b",
                        Summary = "second publication",
                        MethodologyId = new Guid("e45cf030-f29b-42c3-8270-3cc8267026f0"),
                    },
                };

                var topics = new List<Topic>
                {
                    new Topic
                    {
                        Id = new Guid("0144e3f2-41e1-4aec-9c55-2671f454c85f"),
                        Title = "Topic A",
                        ThemeId = new Guid("a7772148-fbbd-4c85-8530-f33c9ef25488"),
                        Slug = "topic-a",
                        Summary = "The first topic",
                        Publications = publications
                    }
                };

                var themes = new List<Theme>
                {
                    new Theme
                    {
                        Id = new Guid("a7772148-fbbd-4c85-8530-f33c9ef25488"),
                        Title = "Theme A",
                        Slug = "theme-a",
                        Summary = "The first theme",
                        Topics = topics
                    }
                };

                context.AddRange(methodologies);
                context.AddRange(themes);
                context.AddRange(topics);
                context.AddRange(publications);
                context.SaveChanges();
            }

            using (var context = new ContentDbContext(options))
            {
                var service = new MethodologyService(context);

                var result = service.GetTree();

                Assert.True(result.Any());
                Assert.Single(result);
                Assert.Equal("Theme A", result.FirstOrDefault().Title);
                Assert.Single(result.FirstOrDefault().Topics);
                Assert.Equal("Topic A", result.FirstOrDefault().Topics.FirstOrDefault().Title);
                Assert.Equal(1, result.FirstOrDefault().Topics.FirstOrDefault().Publications.Count());
            }
        }
    }
}