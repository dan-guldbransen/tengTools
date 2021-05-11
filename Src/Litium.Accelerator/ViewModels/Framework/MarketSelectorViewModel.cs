﻿using Litium.Accelerator.Builders;
using Litium.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.ViewModels.Framework
{
    public class MarketSelectorViewModel : IViewModel
    {
        public IList<ContentLinkModel> ChannelLinkList { get; set; } = new List<ContentLinkModel>();

        public IList<ContentLinkModel> PartnerLinkList { get; set; } = new List<ContentLinkModel>();
    }
}
