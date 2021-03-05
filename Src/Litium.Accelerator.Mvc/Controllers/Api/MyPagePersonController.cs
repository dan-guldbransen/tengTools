using System;
using System.Web.Http;
using Litium.Accelerator.Builders.Person;
using Litium.Accelerator.Mvc.Attributes;
using Litium.Accelerator.Mvc.ModelStates;
using Litium.Accelerator.Services;
using Litium.Accelerator.ViewModels.Persons;

namespace Litium.Accelerator.Mvc.Controllers.Api
{
    [OrganizationRole(true, false)]
    [RoutePrefix("api/mypageperson")]
    public class MyPagePersonController : ApiControllerBase
    {
        private readonly BusinessPersonViewModelBuilder _b2BPersonViewModelBuilder;
        private readonly PersonViewModelService _personViewModelService;

        private readonly ModelState _modelState;

        public MyPagePersonController(BusinessPersonViewModelBuilder b2BPersonViewModelBuilder, 
            PersonViewModelService personViewModelService)
        {
            _b2BPersonViewModelBuilder = b2BPersonViewModelBuilder;
            _personViewModelService = personViewModelService;

            _modelState = new ApiModelState(ModelState);
        }

        /// <summary>
        /// Gets all persons of the organization that the current user belongs to.
        /// </summary>
        [HttpGet]
        [Route]
        public IHttpActionResult Get()
        {
            var model = _b2BPersonViewModelBuilder.Build();
            return Ok(model);
        }

        /// <summary>
        /// Gets the person.
        /// </summary>
        /// <param name="id">The person system identifier.</param>
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(Guid id)
        {
            var model = _b2BPersonViewModelBuilder.Build(id);
            return Ok(model);
        }

        /// <summary>
        /// Updates the person.
        /// </summary>
        /// <param name="viewModel">Object containing the person information.</param>
        [HttpPut]
        [Route]
        [ApiValidateAntiForgeryToken]
        public IHttpActionResult Update(BusinessPersonViewModel viewModel)
        {
            if (_personViewModelService.Validate(viewModel, _modelState) 
                && _personViewModelService.Update(viewModel, _modelState))
            {
                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Deletes the link between the person and the organization.
        /// </summary>
        /// <param name="personId">The person system identifier.</param>
        [HttpDelete]
        [Route]
        [ApiValidateAntiForgeryToken]
        public IHttpActionResult Delete([FromBody]Guid personId)
        {
            _personViewModelService.Delete(personId);
            return Ok();
        }

        /// <summary>
        /// Creates the person.
        /// </summary>
        /// <param name="viewModel">Object containing the person information.</param>
        [HttpPost]
        [Route]
        [ApiValidateAntiForgeryToken]
        public IHttpActionResult Add(BusinessPersonViewModel viewModel)
        {
            if (_personViewModelService.Validate(viewModel, _modelState) 
                && _personViewModelService.Create(viewModel, _modelState))
            {
                return Ok();
            }

            return BadRequest(ModelState);
        }
    }
}
