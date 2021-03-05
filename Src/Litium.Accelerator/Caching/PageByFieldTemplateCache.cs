using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Caching;
using Litium.Common;
using Litium.Data;
using Litium.Events;
using Litium.FieldFramework;
using Litium.FieldFramework.Events;
using Litium.Runtime.DependencyInjection;
using Litium.Security;
using Litium.Web.Routing;
using Litium.Websites;
using Litium.Websites.Events;
using Litium.Websites.Queryable;
using Microsoft.Extensions.DependencyInjection;

namespace Litium.Accelerator.Caching
{
    [Service(ServiceType = typeof(PageByFieldTemplateCache<>))]
    public class PageByFieldTemplateCache<T>
        where T : PageByFieldTemplateCacheType
    {
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly DataService _dataService;
        private readonly MemoryCacheService _memoryCacheService;
        private readonly AuthorizationService _authorizationService;
        private readonly PageService _pageService;
        private readonly string _cacheKey;
        private readonly string _fieldTemplateName;
        private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;

        public PageByFieldTemplateCache(
            FieldTemplateService fieldTemplateService,
            DataService dataService,
            EventBroker eventBroker,
            MemoryCacheService memoryCacheService,
            AuthorizationService authorizationService,
            PageService pageService,
            IServiceProvider serviceProvider,
            RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor,
            WebsiteService websiteService)
        {
            var fieldType = ActivatorUtilities.CreateInstance<T>(serviceProvider);
            _fieldTemplateService = fieldTemplateService;
            _dataService = dataService;
            _memoryCacheService = memoryCacheService;
            _authorizationService = authorizationService;
            _pageService = pageService;
            _fieldTemplateName = fieldType.Name;

            _cacheKey = GetType().FullName + ":" + _fieldTemplateName;

            eventBroker.Subscribe<FieldTemplateCreated>(_ => RemoveCacheForWebsites());
            eventBroker.Subscribe<FieldTemplateDeleted>(_ => RemoveCacheForWebsites());
            eventBroker.Subscribe<DraftPageCreated>(_ => RemoveCacheForWebsites());
            eventBroker.Subscribe<PageCreated>(_ => RemoveCacheForWebsites());
            eventBroker.Subscribe<PageDeleted>(_ => RemoveCacheForWebsites());
            eventBroker.Subscribe<PageUpdated>(x =>
            {
                if (x.OriginalFieldTemplateSystemId != null)
                {
                    RemoveCacheForWebsites();
                }
            });
            _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;

            void RemoveCacheForWebsites()
            {
                foreach (var website in websiteService.GetAll())
                {
                    _memoryCacheService.Remove(_cacheKey + website.SystemId);
                }
            }
        }

        public bool TryFindPage(Func<Page, bool> predicate, bool ignorePermission = false)
            => TryGetPage(predicate, out var page, ignorePermission: ignorePermission);

        public bool TryGetPage(Func<Page, bool> predicate, out Page page, bool ignorePermission = false)
        {
            if (_routeRequestLookupInfoAccessor.RouteRequestLookupInfo != null)
            {
                var pagesSystemIds = GetCacheItems(_routeRequestLookupInfoAccessor.RouteRequestLookupInfo.Channel.WebsiteSystemId.GetValueOrDefault());
                var previewDataBackup = _routeRequestLookupInfoAccessor.RouteRequestLookupInfo.PreviewPageData;
                try
                {
                    _routeRequestLookupInfoAccessor.RouteRequestLookupInfo.PreviewPageData = null; // we should not get preview Url to the page
                    foreach (var item in _pageService.Get(pagesSystemIds.Where(x => ignorePermission || _authorizationService.HasOperation<Page>(Operations.Entity.Read, x)))
                                                     .Where(x => x.IsActive(_routeRequestLookupInfoAccessor.RouteRequestLookupInfo.Channel.SystemId)))
                    {
                        if (predicate(item))
                        {
                            page = item;
                            return true;
                        }
                    }
                }
                finally
                {
                    _routeRequestLookupInfoAccessor.RouteRequestLookupInfo.PreviewPageData = previewDataBackup;
                }
            }
            page = null;
            return false;
        }

        private List<Guid> GetCacheItems(Guid websiteSystemId)
        {
            return _memoryCacheService.GetOrAdd(_cacheKey + websiteSystemId, () =>
            {
                var fieldTemplate = _fieldTemplateService.Get<PageFieldTemplate>(_fieldTemplateName);
                if (fieldTemplate == null)
                {
                    return null;
                }

                using (var db = _dataService.CreateQuery<Page>())
                {
                    return db
                        .Filter(f => f
                            .TemplateSystemId("eq", fieldTemplate.SystemId)
                            .WebsiteSystemId(websiteSystemId)
                        )
                        .ToSystemIdList();
                }
            });
        }
    }

    public abstract class PageByFieldTemplateCacheType
    {
        public abstract string Name { get; }
    }

    public class ErrorPageByFieldTypeResolverType : PageByFieldTemplateCacheType
    {
        public override string Name { get; } = PageTemplateNameConstants.Error;
    }

    public class LandingPageByFieldTemplateCache : PageByFieldTemplateCacheType
    {
        public override string Name { get; } = PageTemplateNameConstants.Landing;
    }

    public class LoginPageByFieldTemplateCache : PageByFieldTemplateCacheType
    {
        public override string Name { get; } = PageTemplateNameConstants.Login;
    }

    public class PageNotFoundByFieldTemplateCache : PageByFieldTemplateCacheType
    {
        public override string Name { get; } = PageTemplateNameConstants.PageNotFound;
    }

    public class MegaMenuPageFieldTemplateCache : PageByFieldTemplateCacheType
    {
        public override string Name { get; } = PageTemplateNameConstants.MegaMenu;
    }

    public class BrandPageFieldTemplateCache : PageByFieldTemplateCacheType
    {
        public override string Name { get; } = PageTemplateNameConstants.Brand;
    }

    public class WelcomeEmailPageTemplateCache : PageByFieldTemplateCacheType
    {
        public override string Name { get; } = PageTemplateNameConstants.WelcomeEmail;
    }

    public class OrderConfirmationPageByFieldTemplateCache : PageByFieldTemplateCacheType
    {
        public override string Name { get; } = PageTemplateNameConstants.OrderConfirmation;
    }
}
