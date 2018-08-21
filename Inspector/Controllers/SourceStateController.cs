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
        public IEnumerable<object> GetAll()
        {
            if (!_modelContext.InspectionLink.Any())
                throw new ArgumentException("Для получения конкретного снэпшота вебсайта, " +
                                            "необходимо начать слежение, используя метод POST метод api/SourceState");

            return _modelContext.SourceStates.Select(state => new {state.Id, state.FixationTime});
        }

        // GET: api/SourceState/5
        [HttpGet("{id}", Name = "Get")]
        [Produces(typeof(SourceState))]
        public ActionResult Get(int id)
        {
            if (!_modelContext.InspectionLink.Any())
                throw new ArgumentException("Для получения конкретного снэпшота вебсайта, " +
                                            "необходимо начать слежение, используя метод POST метод api/SourceState");
            if (id < 0)
                throw new ArgumentException("Id снэпшота не может быть отрицательным");

            var sourceState = _modelContext.SourceStates.Find(id);

            if (sourceState == null)
                throw new ArgumentException("Не найден снэпшот вебсайта для этого id");

            return Ok(sourceState);
        }

        // POST: api/SourceState
        [HttpPost]
        public ActionResult Post([FromBody]InspectionLink value)
        {
            if (!Uri.IsWellFormedUriString(value.Url, UriKind.Absolute))
                throw new ArgumentException("Значение поля Url пустое");

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
