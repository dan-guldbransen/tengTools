using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Security;

namespace Litium.Accelerator.Deployments
{
    /// <summary>
    ///     Cms structure package
    /// </summary>
    /// <remarks>
    ///     Disclaimer: Class is still under development and can be changed without notification and with breaking changes.
    /// </remarks>
    public class EcommerceStructurePackage : IStructurePackage<StructureInfo.ECommerceStructure>
    {
        private readonly SecurityToken _securityToken;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public EcommerceStructurePackage()
        {
            _securityToken = ModuleECommerce.Instance.AdminToken;
        }

        /// <summary>
        ///     Exports the specified package info.
        /// </summary>
        /// <param name="packageInfo"> The package info. </param>
        /// <returns> </returns>
        public virtual StructureInfo.ECommerceStructure Export(PackageInfo packageInfo)
        {
            var structure = new StructureInfo.ECommerceStructure();
            ExportMethods(structure);
            return structure;
        }

        /// <summary>
        ///     Imports the specified structure info.
        /// </summary>
        /// <param name="structureInfo"> The structure info. </param>
        /// <param name="packageInfo"> The package info. </param>
        public virtual void Import(StructureInfo structureInfo, PackageInfo packageInfo)
        {
            foreach (var delivery in structureInfo.ECommerce.DeliveryMethod)
            {
                CreateDeliveryMethod(delivery, structureInfo);
            }

            //foreach (var payment in structureInfo.ECommerce.PaymentMethod)
            //{
            //	CreatePaymentMethod(payment, structureInfo);
            //}
        }

        /// <summary>
        ///     Prepares the import.
        /// </summary>
        /// <param name="structureInfo"> The structure info. </param>
        /// <param name="packageInfo"> The package info. </param>
        public virtual void PrepareImport(StructureInfo structureInfo, PackageInfo packageInfo)
        {
            foreach (var item in structureInfo.ECommerce.DeliveryMethod)
            {
                structureInfo.Mappings.Add(item.ID, Guid.NewGuid());
            }
            foreach (var item in structureInfo.ECommerce.PaymentMethod)
            {
                structureInfo.Mappings.Add(item.ID, Guid.NewGuid());
            }
        }

        private void CreateDeliveryMethod(DeliveryMethodCarrier delivery, StructureInfo structureInfo)
        {
            var delMethod = ModuleECommerce.Instance.DeliveryMethods.Get(delivery.ID, _securityToken);
            if (delMethod == null)
            {
                var carrier = new DeliveryMethodCarrier();
                carrier.Name = delivery.Name;
                carrier.DeliveryProviderID = delivery.DeliveryProviderID;
                carrier.ImageID = Guid.Empty;
                carrier.ID = delivery.ID;
                DeliveryMethodSaveNamesAndDescriptions(carrier, delivery);
                carrier.Costs = UpdateDeliveryCurrency(delivery.Costs, structureInfo);
                carrier.Translations = UpdateLanguageId(delivery.Translations, structureInfo);
                ModuleECommerce.Instance.DeliveryMethods.Create(carrier, _securityToken);
            }
        }

        private ICollection<DeliveryMethodTranslationCarrier> UpdateLanguageId(ICollection<DeliveryMethodTranslationCarrier> deliveryTranslations, StructureInfo structureInfo)
        {
            foreach (var translation in deliveryTranslations)
            {
                translation.LanguageID = structureInfo.Id(translation.LanguageID);
                translation.DeliveryMethodID = structureInfo.Id(translation.DeliveryMethodID);
            }
            return deliveryTranslations;
        }

        private ICollection<DeliveryMethodCostCarrier> UpdateDeliveryCurrency(ICollection<DeliveryMethodCostCarrier> deliveryCosts, StructureInfo structureInfo)
        {
            foreach (var cost in deliveryCosts)
            {
                cost.CurrencyID = structureInfo.Id(cost.CurrencyID);
                cost.DeliveryMethodID = structureInfo.Id(cost.DeliveryMethodID);
            }
            return deliveryCosts;
        }

        private void CreatePaymentMethod(PaymentMethodCarrier payment, StructureInfo structureInfo)
        {
            var paymentMethod = ModuleECommerce.Instance.PaymentMethods.Get(payment.Name, payment.PaymentProviderName, _securityToken);

            if (paymentMethod != null)
            {
                var paymentMethodCarrier = paymentMethod.GetAsCarrier();
                paymentMethodCarrier.ImageID = Guid.Empty;
                //paymentMethodCarrier.ID = structureInfo.Id(paymentMethod.ID);
                PaymentMethodSaveNamesAndDescriptions(paymentMethodCarrier, payment);
                SavePaymentMethodCosts(paymentMethodCarrier, payment);

                paymentMethod.SetValuesFromCarrier(paymentMethodCarrier, _securityToken);
            }
        }

        private void DeliveryMethodSaveNamesAndDescriptions(DeliveryMethodCarrier newCarrier, DeliveryMethodCarrier delivery)
        {
            var displayNames = delivery.Translations.ToDictionary(x => x.LanguageID, x => x.DisplayName);

            foreach (var item in displayNames)
            {
                var translationCarrier = delivery.Translations.FirstOrDefault(x => x.LanguageID == item.Key);
                if (translationCarrier == null)
                {
                    translationCarrier = new DeliveryMethodTranslationCarrier(newCarrier.ID, item.Key, null, null);
                    newCarrier.Translations.Add(translationCarrier);
                }
                translationCarrier.DisplayName = item.Value;
                newCarrier.Translations.Add(translationCarrier);
            }
        }

        private void ExportMethods(StructureInfo.ECommerceStructure structure)
        {
            structure.DeliveryMethod = ModuleECommerce.Instance.DeliveryMethods.GetAll().Select(x => x.GetAsCarrier()).ToList();
            structure.PaymentMethod = ModuleECommerce.Instance.PaymentMethods.GetAll().Select(x => x.GetAsCarrier()).ToList();
        }

        private void PaymentMethodSaveNamesAndDescriptions(PaymentMethodCarrier newCarrier, PaymentMethodCarrier payment)
        {
            var displayNames = payment.Translations.ToDictionary(x => x.LanguageID, x => x.DisplayName);
            var descriptions = payment.Translations.ToDictionary(x => x.LanguageID, x => x.Description);
            foreach (var item in displayNames)
            {
                var translationCarrier = payment.Translations.FirstOrDefault(x => x.LanguageID == item.Key);
                if (translationCarrier == null)
                {
                    translationCarrier = new PaymentMethodTranslationCarrier(payment.ID, item.Key, null, null);
                }
                translationCarrier.DisplayName = item.Value;
                newCarrier.Translations.Add(translationCarrier);
            }
            foreach (var item in descriptions)
            {
                var translationCarrier = newCarrier.Translations.FirstOrDefault(x => x.LanguageID == item.Key);
                if (translationCarrier == null)
                {
                    translationCarrier = new PaymentMethodTranslationCarrier(payment.ID, item.Key, null, null);
                }
                translationCarrier.Description = item.Value;
            }
        }

        private void SavePaymentMethodCosts(PaymentMethodCarrier newCarrier, PaymentMethodCarrier payment)
        {
            foreach (var item in payment.Costs)
            {
                var deliveryMethodCostCarrier = payment.Costs.FirstOrDefault(x => x.CurrencyID == item.CurrencyID);
                if (deliveryMethodCostCarrier == null)
                {
                    deliveryMethodCostCarrier = new PaymentMethodCostCarrier(newCarrier.ID, item.CurrencyID, 0, false, 0);
                    newCarrier.Costs.Add(deliveryMethodCostCarrier);
                }
                deliveryMethodCostCarrier.Cost = item.Cost;
                deliveryMethodCostCarrier.IncludeVat = item.IncludeVat;
                deliveryMethodCostCarrier.VatPercentage = item.VatPercentage;
            }
        }
    }
}
