using Litium.Accelerator.Constants;
using Litium.Accelerator.ViewModels.Block;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models;
using Litium.Web.Models.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Builders.Block
{
    public class HeroBlockViewModelBuilder : IViewModelBuilder<HeroBlockViewModel>
    {
        public virtual HeroBlockViewModel Build(BlockModel blockModel)
        {
           var model = blockModel.MapTo<HeroBlockViewModel>();

            model.BackgroundImage = blockModel.GetValue<Guid?>(BlockFieldNameConstants.BackgroundImage)?.MapTo<ImageModel>();
            model.LinkUrl = blockModel.GetValue<PointerPageItem>(BlockFieldNameConstants.Link)?.MapTo<LinkModel>();

            model.ContentPosition = blockModel.GetValue<string>(BlockFieldNameConstants.ContentPosition);

            return model;
        }
    }
}
