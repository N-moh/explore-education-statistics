﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ExploreEducationStatistics.Publisher.Model.ViewModels;

namespace GovUk.Education.ExploreEducationStatistics.Content.Api.ViewModels
{
    public class AllMethodologiesThemeViewModel : IComparable<AllMethodologiesThemeViewModel>
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public List<AllMethodologiesTopicViewModel> Topics { get; set; } = new List<AllMethodologiesTopicViewModel>();

        public void RemoveTopicNodesWithoutMethodologiesAndSort()
        {
            // Remove all publications without any methodologies
            Topics.ForEach(topic => topic.RemovePublicationNodesWithoutMethodologiesAndSort());

            // Remove all topics without any publications
            Topics = Topics
                .Where(topic => topic.Publications.Any())
                .ToList();

            Topics.Sort();
        }

        public int CompareTo(AllMethodologiesThemeViewModel? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Title, other.Title, StringComparison.Ordinal);
        }
    }

    public class AllMethodologiesTopicViewModel : IComparable<AllMethodologiesTopicViewModel>
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public List<AllMethodologiesPublicationViewModel> Publications { get; set; } =
            new List<AllMethodologiesPublicationViewModel>();

        public void RemovePublicationNodesWithoutMethodologiesAndSort()
        {
            Publications = Publications
                .Where(publication => publication.Methodologies.Any())
                .ToList();

            Publications.Sort();
        }


        public int CompareTo(AllMethodologiesTopicViewModel? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Title, other.Title, StringComparison.Ordinal);
        }
    }

    public class AllMethodologiesPublicationViewModel : IComparable<AllMethodologiesPublicationViewModel>
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public List<MethodologySummaryViewModel> Methodologies { get; set; } =
            new List<MethodologySummaryViewModel>();


        public int CompareTo(AllMethodologiesPublicationViewModel? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Title, other.Title, StringComparison.Ordinal);
        }
    }
}