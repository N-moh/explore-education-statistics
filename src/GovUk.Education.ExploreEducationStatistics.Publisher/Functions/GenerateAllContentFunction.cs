﻿using System;
using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Publisher.Model;
using GovUk.Education.ExploreEducationStatistics.Publisher.Services.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace GovUk.Education.ExploreEducationStatistics.Publisher.Functions
{
    /**
     * Development / BAU Function which performs a full content refresh
     */
    // ReSharper disable once UnusedType.Global
    public class GenerateAllContentFunction
    {
        private readonly IContentService _contentService;

        private const string QueueName = "generate-all-content";

        public GenerateAllContentFunction(IContentService contentService)
        {
            _contentService = contentService;
        }

        [FunctionName("GenerateAllContent")]
        // ReSharper disable once UnusedMember.Global
        public async Task GenerateAllContent(
            [QueueTrigger(QueueName)] GenerateAllContentMessage message,
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