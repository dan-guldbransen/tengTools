using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Litium.Accelerator.Search.Filtering;
using Litium.Accelerator.WebForm.Site.Administration.Api.ViewModel;
using Litium.FieldFramework;
using Litium.Products;
using Litium.Studio.Extenssions;
using Litium.Web.Administration.FieldFramework;
using Litium.Web.Administration.WebApi;

namespace Litium.Accelerator.WebForm.Site.Administration.Api.Controllers
{
    [RoutePrefix("site/administration/api/filtering")]
    public class FilteringController : BackofficeApiController
    {
        private readonly FieldDefinitionService _fieldDefinitionService;
        private readonly FilterService _filterService;

        public FilteringController(FieldDefinitionService fieldDefinitionService, FilterService filterService)
        {
            _fieldDefinitionService = fieldDefinitionService;
            _filterService = filterService;
        }

        [Route("")]
        [HttpGet]
        public IEnumerable<FilteringModel.Item> Get()
        {
            var selected = _filterService.GetProductFilteringFields();
            var all = GetAllFields();

            return all.Where(x => selected.Contains(x.FieldId)).OrderBy(x => selected.IndexOf(x.FieldId)).ToList();
        }

        [Route("setting")]
        [HttpGet]
        public FilteringModel GetSetting()
        {
            var selected = _filterService.GetProductFilteringFields();
            var all = GetAllFields();

            return new FilteringModel
            {
                Filters = all.Where(x => !selected.Contains(x.FieldId)).ToList(),
                Items = all.Where(x => selected.Contains(x.FieldId)).OrderBy(x => selected.IndexOf(x.FieldId)).ToList(),
            };
        }

        [Route("setting")]
        [HttpPost]
        public IHttpActionResult SaveSetting([FromBody] FilteringModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var selected = model.Items.Select(x => x.FieldId).ToList();
            _filterService.SaveProductFilteringFields(selected);

            AddSuccessMessage(GetString("General.Item.Updated"));
            return Ok(model);
        }

        private List<FilteringModel.Item> GetAllFields()
        {
            return _fieldDefinitionService.GetAll<ProductArea>()
                            .Where(x => !x.Hidden && x.CanBeGridFilter)
                            .Select(y => y.MakeWritableClone())
                            .ToList()
                            .ChangeDuplicateFilterItemTitles()
                            .Select(x => new FilteringModel.Item
                            {
                                FieldId = x.Id,
                                Title = x.Localizations.CurrentCulture.Name ?? x.Id,
                                GroupName = (x.SystemDefined ? "pim.template.fieldgroup.systemdefined" : "pim.template.fieldgroup.userdefined").AsAngularResourceString()
                            })
                            .Concat(new[]
                            {
                                new FilteringModel.Item {
                                    FieldId = "#Price",
                                    Title = "accelerator.filterfield.filterprice".AsAngularResourceString(),
                                    GroupName = "accelerator.filterfield.predefined".AsAngularResourceString() },
                                new FilteringModel.Item {
                                    FieldId = "#News",
                                    Title = "accelerator.filterfield.filternews".AsAngularResourceString(),
                                    GroupName = "accelerator.filterfield.predefined".AsAngularResourceString() }
                            })
                            .ToList();
        }
    }
}