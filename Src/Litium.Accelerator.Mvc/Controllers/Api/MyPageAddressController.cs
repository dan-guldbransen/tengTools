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
    [RoutePrefix("api/mypageaddress")]
    public class MyPageAddressController : ApiControllerBase
    {
        private readonly BusinessAddressViewModelBuilder _businessAddressViewModelBuilder;
        private readonly AddressViewModelService _addressViewModelService;

        private readonly ModelState _modelState;

        public MyPageAddressController(BusinessAddressViewModelBuilder businessAddressViewModelBuilder, AddressViewModelService addressViewModelService)
        {
            _businessAddressViewModelBuilder = businessAddressViewModelBuilder;
            _addressViewModelService = addressViewModelService;

            _modelState = new ApiModelState(ModelState);
        }

        /// <summary>
        /// Gets all addresses of the organization that the current user belongs to.
        /// </summary>
        [HttpGet]
        [Route]
        public IHttpActionResult Get()
        {
            var model = _businessAddressViewModelBuilder.Build();
            return Ok(model);
        }

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <param name="id">The address system identifier.</param>
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(Guid id)
        {
            var model = _businessAddressViewModelBuilder.Build(id);
            return Ok(model);
        }

        /// <summary>
        /// Creates the address.
        /// </summary>
        /// <param name="viewModel">Object containing the address information.</param>
        [HttpPost]
        [Route]
        [ApiValidateAntiForgeryToken]
        public IHttpActionResult Add(AddressViewModel viewModel)
        {
            if (_addressViewModelService.Validate(viewModel, _modelState) && 
                _addressViewModelService.Create(viewModel, _modelState))
            {
                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Updates the address.
        /// </summary>
        /// <param name="viewModel">Object containing the address information.</param>
        [HttpPut]
        [Route]
        [ApiValidateAntiForgeryToken]
        public IHttpActionResult Update(AddressViewModel viewModel)
        {
            if (_addressViewModelService.Validate(viewModel, _modelState) && _addressViewModelService.Update(viewModel, _modelState))
            {
                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Deletes the address.
        /// </summary>
        /// <param name="id">The address system identifier.</param>
        [HttpDelete]
        [Route]
        [ApiValidateAntiForgeryToken]
        public IHttpActionResult Delete([FromBody]Guid id)
        {
            _addressViewModelService.Delete(id);
            return Ok();
        }
    }
}
