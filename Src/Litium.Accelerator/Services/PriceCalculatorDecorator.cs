using JetBrains.Annotations;
using Litium.Accelerator.Utilities;
using Litium.Products;
using Litium.Products.PriceCalculator;
using Litium.Runtime.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Litium.Accelerator.Services
{
    [ServiceDecorator(typeof(IPriceCalculator))]
    internal class PriceCalculatorDecorator : IPriceCalculator
    {
        private readonly IPriceCalculator _parentResolver;
        private readonly PersonStorage _personStorage;

        public PriceCalculatorDecorator(
            IPriceCalculator parentResolver,
            PersonStorage personStorage)
        {
            _parentResolver = parentResolver;
            _personStorage = personStorage;
        }

        public IDictionary<Guid, PriceCalculatorResult> GetListPrices([NotNull] PriceCalculatorArgs calculatorArgs, [NotNull] params PriceCalculatorItemArgs[] itemArgs)
        {
            SetOrangizationSystemIds(calculatorArgs);
            return _parentResolver.GetListPrices(calculatorArgs, itemArgs);
        }

        public ICollection<PriceList> GetPriceLists([NotNull]PriceCalculatorArgs calculatorArgs)
        {
            SetOrangizationSystemIds(calculatorArgs);
            return _parentResolver.GetPriceLists(calculatorArgs);
        }

        private void SetOrangizationSystemIds([NotNull] PriceCalculatorArgs calculatorArgs)
        {
            var selectedOrganization = _personStorage.CurrentSelectedOrganization;
            if (selectedOrganization != null)
            {
                calculatorArgs.OrganizationSystemIds = new List<Guid>() { selectedOrganization.SystemId };
            }
        }
    }
}