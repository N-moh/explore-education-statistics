using GovUk.Education.ExploreEducationStatistics.Data.Model.Query;
using GovUk.Education.ExploreEducationStatistics.Data.Services.Interfaces;
using GovUk.Education.ExploreEducationStatistics.Data.Services.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ExploreEducationStatistics.Data.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IDataService<ResultWithMetaViewModel> _dataService;

        public DataController(IDataService<ResultWithMetaViewModel> dataService)
        {
            _dataService = dataService;
        }

        [HttpPost]
        public ActionResult<ResultWithMetaViewModel> Query([FromBody] ObservationQueryContext query)
        {
            return _dataService.Query(query);
        }
    }
}