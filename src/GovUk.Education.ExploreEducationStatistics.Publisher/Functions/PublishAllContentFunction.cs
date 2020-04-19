﻿using System;
using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Publisher.Model;
using GovUk.Education.ExploreEducationStatistics.Publisher.Services.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using static GovUk.Education.ExploreEducationStatistics.Publisher.Model.PublisherQueues;

namespace GovUk.Education.ExploreEducationStatistics.Publisher.Functions
{
    // ReSharper disable once UnusedType.Global
    public class PublishAllContentFunction
    {
        private readonly IContentService _contentService;

        public PublishAllContentFunction(IContentService contentService)
        {
            _contentService = contentService;
        }

        /**
         * Development / BAU Function to generate and publish the content of all Releases immediately.
         * Depends on the download files existing.
         */
        [FunctionName("PublishAllContent")]
        // ReSharper disable once UnusedMember.Global
        public async Task PublishAllContent(
            [QueueTrigger(PublishAllContentQueue)] PublishAllContentMessage message,
            ExecutionContext executionContext,
            ILogger logger)
        {
            logger.LogInformation($"{executionContext.FunctionName} triggered: {message}");
            try
            {
                await _contentService.UpdateAllContentAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Exception occured while executing {executionContext.FunctionName}");
                throw;
            }

            logger.LogInformation($"{executionContext.FunctionName} completed");
        }
    }
}