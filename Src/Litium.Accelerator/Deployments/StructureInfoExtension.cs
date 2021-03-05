using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Media;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Deployments
{
    public static class StructureInfoExtension
    {
        private static readonly Regex RegexPageId = new Regex("href=\"PageID:([0-9a-f]{32})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegexpStorage = new Regex(@"\""((http\://.*/storage/|https\://.*/storage/|/storage/)(cms|ma|rel|pc|nl)/([0-9a-f]{32})/([0-9a-f]{32})/(.*)/(.*)/(.*))\""", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegexpTags = new Regex(@"<(.*?)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly FileService FileService = ServiceLocator.ServiceProvider.GetService(typeof(FileService)) as FileService;
        /// <summary>
        ///     Replaces the text.
        /// </summary>
        /// <param name="structureInfo"></param>
        /// <param name="text"> The text. </param>
        /// <returns> </returns>
        public static string ReplaceText([NotNull] this StructureInfo structureInfo, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            foreach (Match hit in RegexPageId.Matches(text))
            {
                var value = hit.Groups[1].Value;
                var newPageId = structureInfo.Id(new Guid(value)).ToString("N");
                if (!newPageId.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    text = text.Replace(value, newPageId);
                }
            }

            var replacedMatch = new HashSet<string>();
            var tags = RegexpTags.Matches(text);
            foreach (Match tag in tags)
            {
                var storageMatches = RegexpStorage.Matches(tag.Value);
                foreach (Match storageMatch in storageMatches)
                {
                    string fullMatch;
                    Guid ownerId;
                    string formatPath;

                    try
                    {
                        fullMatch = storageMatch.Groups[1].Value;
                        ownerId = structureInfo.Id(new Guid(storageMatch.Groups[4].Value));
                        formatPath = storageMatch.Groups[6].Value;
                    }
                    catch
                    {
                        continue;
                    }

                    if (fullMatch.Contains("\""))
                    {
                        fullMatch = fullMatch.Substring(0, fullMatch.IndexOf('"'));
                    }

                    if (replacedMatch.Contains(fullMatch))
                    {
                        continue;
                    }

                    replacedMatch.Add(fullMatch);

                    var file = FileService.Get(ownerId);
                    if (file == null)
                    {
                        continue;
                    }

                    Size size;
                    bool keepAspectRatio;
                    ImageFormat resizeFormat;
                    FileExtensions.DecodeFormatPart(formatPath, out size, out keepAspectRatio, out resizeFormat);

                    if (file.GetFileType().IsImage())
                    {
                        var url = FileExtensions.GetUrl(file.SystemId, file.BlobUri.ToString(), file.Name, null, size, keepAspectRatio, false);
                        text = text.Replace(fullMatch, url);
                    }
                    else
                    {
                        var url = FileExtensions.GetUrl(file.SystemId, file.BlobUri.ToString(), file.Name, null, Size.Empty, false, false); ;
                        text = text.Replace(fullMatch, url);
                    }
                }
            }

            return text;
        }
    }
}
