﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Content.Api.Services.Interfaces;
using GovUk.Education.ExploreEducationStatistics.Content.Model.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ExploreEducationStatistics.Content.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly IContentCacheService _contentCacheService;

        public DownloadController(IContentCacheService contentCacheService)
        {
            _contentCacheService = contentCacheService;
        }

        /// <response code="204">If the item is null</response>    
        [HttpGet("tree")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [Produces("application/json")]
        public async Task<ActionResult<string>> GetDownloadTree()
        {
            var tree = await _contentCacheService.GetDownloadTreeAsync();

            if (string.IsNullOrWhiteSpace(tree))
            {
                return NoContent();
            }

            return tree;
        }
    }
}