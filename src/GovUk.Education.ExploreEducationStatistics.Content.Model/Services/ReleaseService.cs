using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GovUk.Education.ExploreEducationStatistics.Common.Model;
using GovUk.Education.ExploreEducationStatistics.Content.Model.Database;
using GovUk.Education.ExploreEducationStatistics.Content.Model.Services.Interfaces;
using GovUk.Education.ExploreEducationStatistics.Content.Model.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ExploreEducationStatistics.Content.Model.Services
{
    public class ReleaseService : IReleaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;

        public ReleaseService(ApplicationDbContext context, IFileStorageService fileStorageService, IMapper mapper)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }

        public ReleaseViewModel GetRelease(string id)
        {
            var release = Guid.TryParse(id, out var newGuid)
                ? _context.Releases.Include(x => x.Publication).ThenInclude(x => x.LegacyReleases)
                    .Include(x => x.Updates).FirstOrDefault(x => x.Id == newGuid)
                : _context.Releases.Include(x => x.Publication).ThenInclude(x => x.LegacyReleases)
                    .Include(x => x.Updates).FirstOrDefault(x => x.Slug == id);

            if (release != null)
            {
                var releases = _context.Releases.Where(x => x.Publication.Id == release.Publication.Id).ToList();
                release.Publication.Releases = new List<Release>();
                releases.ForEach(r => release.Publication.Releases.Add(new Release
                {
                    Id = r.Id,
                    ReleaseName = r.ReleaseName,
                    Published = r.Published,
                    Slug = r.Slug,
                    Summary = r.Summary,
                    Publication = r.Publication,
                    PublicationId = r.PublicationId,
                    Updates = r.Updates
                }));
            }
            
            var releaseViewModel = _mapper.Map<ReleaseViewModel>(release);
            releaseViewModel.LatestRelease =  IsLatestRelease(release.PublicationId, releaseViewModel.Id);
            
            return releaseViewModel;
        }

        public ReleaseViewModel GetLatestRelease(string id)
        {
            Release release;

            if (Guid.TryParse(id, out var newGuid))
            {
                release = _context.Releases
                    .Include(x => x.Publication).ThenInclude(x => x.LegacyReleases)
                    .Include(x => x.Publication).ThenInclude(p => p.Topic.Theme)
                    .Include(x => x.Publication).ThenInclude(p => p.Contact)
                    .Include(x => x.Updates).OrderBy(x => x.Published)
                    .Last(t => t.PublicationId == newGuid);
            }
            else
            {
                release = _context.Releases
                    .Include(x => x.Publication).ThenInclude(x => x.LegacyReleases)
                    .Include(x => x.Publication).ThenInclude(p => p.Topic.Theme)
                    .Include(x => x.Publication).ThenInclude(p => p.Contact)
                    .Include(x => x.Updates).OrderBy(x => x.Published)
                    .Last(t => t.Publication.Slug == id);
            }

            if (release != null)
            {
                var releases = _context.Releases.Where(x => x.Publication.Id == release.Publication.Id).ToList();
                release.Publication.Releases = new List<Release>();
                releases.ForEach(r => release.Publication.Releases.Add(new Release
                {
                    Id = r.Id,
                    ReleaseName = r.ReleaseName,
                    Published = r.Published,
                    Slug = r.Slug,
                    Summary = r.Summary,
                    Publication = r.Publication,
                    PublicationId = r.PublicationId,
                    Updates = r.Updates
                }));
                
                var releaseViewModel = _mapper.Map<ReleaseViewModel>(release);
                releaseViewModel.DataFiles = ListFiles(release, ReleaseFileTypes.Data);
                releaseViewModel.ChartFiles = ListFiles(release, ReleaseFileTypes.Chart);
                releaseViewModel.AncillaryFiles = ListFiles(release, ReleaseFileTypes.Ancillary);
                releaseViewModel.LatestRelease = true;
                
                return releaseViewModel;
            }

            return null;
        }

        // TODO: This logic is flawed but will provide an accurate result with the current seed data
        private bool IsLatestRelease(Guid publicationId, Guid releaseId)
        {
            return (_context.Releases
                        .Where(x=> x.PublicationId == publicationId)
                        .OrderBy(x => x.Published)
                        .Last().Id == releaseId);
        }
        
        private List<FileInfo> ListFiles(Release release, ReleaseFileTypes type)
        {
            return _fileStorageService.ListFiles(release.Publication.Slug, release.Slug, type).ToList();
        }
    }
}