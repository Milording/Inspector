using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Inspector.Controllers
{
    [Produces("application/json")]
    [Route("api/SourceState")]
    public class SourceStateController : Controller
    {
        private readonly ModelContext _modelContext;

        public SourceStateController(ModelContext modelContext)
        {
            _modelContext = new ModelContext();
        }

        // GET: api/SourceState
        [HttpGet]
        public IEnumerable<LightSourceState> GetAll()
            => _modelContext.SourceStates.Select(state => new LightSourceState(state.Id, state.FixationTime));

        // GET: api/SourceState/5
        [HttpGet("{id}", Name = "Get")]
        [Produces(typeof(SourceState))]
        public ActionResult Get(int id)
        {
            var sourceState = _modelContext.SourceStates.Find(id);
            if (sourceState == null)
                return NotFound();

            return Ok(sourceState);
        }

        // POST: api/SourceState
        [HttpPost]
        public ActionResult Post([FromBody]InspectionLink value)
        {
            if (!Uri.IsWellFormedUriString(value.Url, UriKind.Absolute))
                return BadRequest();

            var inspectedLink = _modelContext.InspectionLink.SingleOrDefault();
            if (inspectedLink != null)
            {
                _modelContext.DeleteAll<InspectedLink>();
                _modelContext.DeleteAll<SourceState>();

                _modelContext.SaveChanges();
            }

            _modelContext.InspectionLink.Add(new InspectedLink { InspectionLink = value.Url });
            _modelContext.SaveChanges();

            return Ok();
        }
    }

    public class InspectionLink
    {
        public string Url { get; set; }
    }
}
