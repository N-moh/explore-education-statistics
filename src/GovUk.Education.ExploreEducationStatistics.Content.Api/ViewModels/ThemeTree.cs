#nullable enable
using System;
using System.Collections.Generic;

namespace GovUk.Education.ExploreEducationStatistics.Content.Api.ViewModels
{
    public record ThemeTree<TNode> where TNode : BasePublicationTreeNode
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = string.Empty;

        public string Summary { get; init; } = string.Empty;

        public List<TopicTree<TNode>> Topics { get; init; } = new();
    }
}