using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Litium.Accelerator.ViewModels.Media;
using Litium.FieldFramework;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Media;

namespace Litium.Accelerator.Extensions
{
    public static class FieldFrameworkExtensions
    {
        private const string FieldDefinitionConstantSize = "Size";
        private const string FieldDefinitionConstantColor = "Color";
        public static List<string> GetImageUrls(this IFieldFramework fields, FileService fileService, Func<int, ImageSize> getImageSize = null)
        {
            if (fields == null)
            {
                return null;
            }

            var imageIds = (fields[SystemFieldDefinitionConstants.Images] as IReadOnlyCollection<Guid>)?.ToArray();
            if (imageIds?.Length > 0)
            {
                var result = new List<string>();
                for (int i = 0; i < imageIds.Length; i++)
                {
                    ImageSize imageSize = null;
                    if (getImageSize != null)
                    {
                        imageSize = getImageSize(i);
                    }
                    var file = fileService.Get(imageIds[i]);
                    var link = (imageSize != null)
                        ? file.GetUrlToImage(imageSize.MaxSize, imageSize.MinSize, out Size actualSize, false)
                        : file.GetUrlToImage(out actualSize, false);

                    result.Add(link);
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        public static string GetName(this IFieldFramework fields, string culture)
        {
            if (fields == null || string.IsNullOrWhiteSpace(culture))
                return null;

            return fields[SystemFieldDefinitionConstants.Name, culture] as string;
        }

        public static string GetDescription(this IFieldFramework fields, string culture)
        {
            if (fields == null || string.IsNullOrWhiteSpace(culture))
                return null;

            return fields[SystemFieldDefinitionConstants.Description, culture] as string;
        }

        public static string GetSize(this IFieldFramework fields)
        {
            return fields[FieldDefinitionConstantSize] as string;
        }

        public static string GetColor(this IFieldFramework fields)
        {
            return fields[FieldDefinitionConstantColor] as string;
        }
    }
}
