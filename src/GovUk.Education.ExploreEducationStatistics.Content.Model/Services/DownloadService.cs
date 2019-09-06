﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GovUk.Education.ExploreEducationStatistics.Common.Model;
using GovUk.Education.ExploreEducationStatistics.Content.Model.Database;
using GovUk.Education.ExploreEducationStatistics.Content.Model.Services.Interfaces;
using GovUk.Education.ExploreEducationStatistics.Content.Model.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GovUk.Education.ExploreEducationStatistics.Content.Model.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public DownloadService(ApplicationDbContext context,
            IFileStorageService fileStorageService,
            ILogger<DownloadService> logger, IMapper mapper)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _logger = logger;
            _mapper = mapper;
        }

        public IEnumerable<ThemeTree> GetDownloadTree()
        {
//            var config = new MapperConfiguration(cfg =>
//            {
//                cfg.CreateMap<Publication, PublicationTree>()
//                    .ForMember(
//                        dest => dest.DataFiles, m => m.MapFrom(publication =>
//                            ListFiles(publication.Slug, GetLatestRelease(publication).Slug, ReleaseFileTypes.Data)))
//                    .ForMember(
//                        dest => dest.AncillaryFiles, m => m.MapFrom(publication =>
//                            ListFiles(publication.Slug, GetLatestRelease(publication).Slug,
//                                ReleaseFileTypes.Ancillary)))
//                    .ForMember(
//                        dest => dest.ChartFiles, m => m.MapFrom(publication =>
//                            ListFiles(publication.Slug, GetLatestRelease(publication).Slug, ReleaseFileTypes.Chart)));
//            });

            // TODO: not including the topics
            var themes = GetReleases();

            return _mapper.Map<IEnumerable<ThemeTree>>(themes);
        }

        private static Release GetLatestRelease(Publication publication)
        {
            return publication.Releases.ToList()
                .OrderByDescending(release => release.Published)
                .FirstOrDefault();
        }

        private IEnumerable<Theme> GetReleases()
        {
            return _context.Releases.Include(x => x.Publication.Topic).ThenInclude(x => x.Theme)
                .GroupBy(release => release.Publication.Topic.Theme)
                .Select(grouping => grouping.Key).ToList();
        }

        private IEnumerable<FileInfo> ListFiles(string publication, string release, ReleaseFileTypes type)
        {
            return _fileStorageService.ListFiles(publication, release, type);
        }
    }
}