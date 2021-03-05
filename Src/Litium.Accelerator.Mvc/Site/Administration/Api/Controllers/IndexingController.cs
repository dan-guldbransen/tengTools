using System.Linq;
using System.Web.Http;
using Litium.Accelerator.Services;
using Litium.Accelerator.WebForm.Site.Administration.Api.ViewModel;
using Litium.FieldFramework;
using Litium.Products;
using Litium.Web.Administration.WebApi;

namespace Litium.Accelerator.Mvc.Site.Administration.Api.Controllers
{
    [RoutePrefix("site/administration/api/indexing")]
    public class IndexingController : BackofficeApiController
    {
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly TemplateSettingService _templateSettingService;
        private readonly FieldDefinitionService _fieldDefinitionService;

        public IndexingController(FieldTemplateService fieldTemplateService, TemplateSettingService templateSettingService, FieldDefinitionService fieldDefinitionService)
        {
            _fieldTemplateService = fieldTemplateService;
            _templateSettingService = templateSettingService;
            _fieldDefinitionService = fieldDefinitionService;
        }

        [Route]
        [HttpGet]
        public IndexingModel Get()
        {
            return new IndexingModel
            {
                Templates = _fieldTemplateService.GetAll().OfType<ProductFieldTemplate>().Select(x => new IndexingModel.Template
                {
                    Title = x.Localizations.CurrentUICulture.Name ?? x.Id,
                    TemplateId = x.Id,
                    GroupingFieldId = _templateSettingService.GetTemplateGroupingField(x.Id)?.ToLowerInvariant(),
                    Fields = x.VariantFieldGroups.SelectMany(z => z.Fields)
                        .Distinct()
                        .Select(z => _fieldDefinitionService.Get<ProductArea>(z))
                        .Where(z => z != null)
                        .Select(z => new IndexingModel.FieldGroup {Title = z.Localizations.CurrentUICulture.Name ?? z.Id, FieldId = z.Id.ToLowerInvariant() })
                        .OrderBy(z => z.Title)
                        .ToList()
                })
                .OrderBy(x => x.Title)
                .ToList()
            };
        }

        [Route]
        [HttpPut]
        public IHttpActionResult Put([FromBody] IndexingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var item in model.Templates)
            {
                _templateSettingService.SetTemplateGroupings(item.TemplateId, item.GroupingFieldId);
            }

            AddSuccessMessage(GetString("General.Item.Updated") + " " + GetString("pim.RebuildSearchIndex"));
            return Ok(model);
        }
    }
}