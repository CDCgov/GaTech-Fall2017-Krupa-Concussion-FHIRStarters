using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using fhirStartersApp.Data;
using fhirStartersApp.DecisionSupport;
using fhirStartersApp.DecisionSupport.MtbiRules;
using Microsoft.AspNetCore.Hosting;
using Path = System.IO.Path;

namespace fhirStartersApp.Services
{
    public class ManagementPlanService : IManagementPlanService
    {
        private readonly string _templatePath;

        public ManagementPlanService(IHostingEnvironment environment)
        {
            _templatePath = Path.Combine(environment.ContentRootPath, "Assets", "management-plan-template.docx");
        }

        public ManagementPlan Generate(IEnumerable<DecisionSupportResult> decisionSupportResults)
        {
            using (var fileStream = new FileStream(_templatePath, FileMode.Open))
            using (var memoryStream = new MemoryStream())
            {
                CopyStream(fileStream, memoryStream);
                using (var doc = WordprocessingDocument.Open(memoryStream, true))
                {
                    var body = doc.MainDocumentPart.Document.Body;
                    
                    var daysRestTemplate = DischargeRule.DaysRestTemplate.Substring(0, DischargeRule.DaysRestTemplate.IndexOf('{') - 1); // stop at the placeholder for number of days
                    var cdsResults = decisionSupportResults as IList<DecisionSupportResult> ?? decisionSupportResults.ToList();
                    cdsResults = RemoveOrReplace("{recommendedRest}", daysRestTemplate, cdsResults, body);

                    cdsResults = RemoveOrReplace("{headache}", DischargeRule.HeadacheDrugs, cdsResults, body);
                    cdsResults = RemoveOrReplace("{sunglasses}", DischargeRule.SunglassesEarplugs, cdsResults, body);
                    cdsResults = RemoveOrReplace("{accommodations}", DischargeRule.SchoolRecommendations, cdsResults, body);
                    
                    var templateSearchResult = body.ChildElements.FirstOrDefault(e =>
                        e.InnerText != null && e.InnerText.Contains("{customRecommendations}"));
                    if (cdsResults.All(cdsResult => cdsResult.Type != DecisionSupportResultType.ManagementPlanRecommendation))
                    {
                        body.RemoveChild(templateSearchResult);
                    }
                    else if (templateSearchResult is Paragraph paragraph)
                    {
                        body.ReplaceChild(CreateParagraph(paragraph, cdsResults), paragraph);
                    }
                }
                // At this point, the memory stream contains the modified document.

                return new ManagementPlan(memoryStream.ToArray(), "docx");
            }
        }

        private static IList<DecisionSupportResult> RemoveOrReplace(string templateSearchString, string resultSearchString,
            IList<DecisionSupportResult> cdsResults, Body body)
        {
            var templateSearchResult = body.ChildElements.FirstOrDefault(e =>
                e.InnerText != null && e.InnerText.Contains(templateSearchString));
            var result = cdsResults.FirstOrDefault(dsr =>
                dsr.Type == DecisionSupportResultType.ManagementPlanRecommendation &&
                dsr.Description.Contains(resultSearchString));
            if (result == null)
            {
                body.RemoveChild(templateSearchResult);
            }
            else if (templateSearchResult is Paragraph paragraph)
            {
                body.ReplaceChild(CreateParagraph(paragraph, result.Description), paragraph);
                cdsResults.Remove(result);
            }
            return cdsResults;
        }

        private static Paragraph CreateParagraph(Paragraph basis, string text)
        {
            var clone = (Paragraph) basis.Clone();
            foreach (var child in clone.ChildElements.ToList())
            {
                if (child is Run) clone.RemoveChild(child);
            }
            clone.AppendChild(new Run(new Text(text)));
            return clone;
        }

        private static OpenXmlElement CreateParagraph(Paragraph basis, IList<DecisionSupportResult> cdsResults)
        {
            var clone = (Paragraph)basis.Clone();
            foreach (var child in clone.ChildElements.ToList())
            {
                if (child is Run) clone.RemoveChild(child);
            }
            clone.Append(cdsResults
                .Where(cdsResult => cdsResult.Type == DecisionSupportResultType.ManagementPlanRecommendation)
                .Select(result => new Run(new Text(result.Description))));
            return clone;
        }

        private static void CopyStream(Stream source, Stream destination)
        {
            var buffer = new byte[32768];
            int bytesRead;
            do
            {
                bytesRead = source.Read(buffer, 0, buffer.Length);
                destination.Write(buffer, 0, bytesRead);
            } while (bytesRead != 0);
        }
    }
}
