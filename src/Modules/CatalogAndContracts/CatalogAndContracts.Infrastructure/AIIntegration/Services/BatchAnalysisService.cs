using BatchPostingAndGrouping.Application.DTOs;
using CatalogAndContracts.Infrastructure.AIIntegration.DTOs;

namespace CatalogAndContracts.Infrastructure.AIIntegration.Services
{
    public class BatchAnalysisService
    {
        public BatchAnalysisResultDto Analyze(BatchDto batch)
        {
            // Parse QualityGrade string into enum if needed
            var grade = Enum.TryParse<QualityGrade>(batch.QualityGrade, out var parsed)
                ? parsed
                : QualityGrade.Medium;

            double suggestedPrice = grade switch
            {
                QualityGrade.High => batch.WeightInKg * 5.0,
                QualityGrade.Medium => batch.WeightInKg * 3.0,
                QualityGrade.Low => batch.WeightInKg * 1.5,
                _ => batch.WeightInKg * 2.0
            };

            return new BatchAnalysisResultDto
            {
                BatchId = batch.Id,
                QualityGrade = batch.QualityGrade,
                SuggestedPricePerKg = suggestedPrice
            };
        }
    }

    public enum QualityGrade
    {
        Low = 1,
        Medium = 2,
        High = 3
    }
}