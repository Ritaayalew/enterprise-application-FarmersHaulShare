namespace BatchPostingAndGrouping.Domain.ValueObjects;

/// <summary>
/// Value object representing quality grade of produce
/// Common grades: Premium, Grade A, Grade B, Standard
/// </summary>
public sealed class QualityGrade : SharedKernel.Domain.ValueObject
{
    public string Grade { get; private init; } = string.Empty;
    public string? Description { get; private init; }

    private QualityGrade() { } // EF Core

    public QualityGrade(string grade, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(grade))
            throw new SharedKernel.Domain.DomainException("Quality grade cannot be empty.");

        Grade = grade.Trim();
        Description = description?.Trim();
    }

    // Common grade constants
    public static QualityGrade Premium() => new("Premium", "Highest quality, uniform appearance");
    public static QualityGrade GradeA() => new("Grade A", "High quality, minor imperfections");
    public static QualityGrade GradeB() => new("Grade B", "Good quality, acceptable for processing");
    public static QualityGrade Standard() => new("Standard", "Average quality, suitable for general use");

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Grade.ToLowerInvariant();
    }

    public override string ToString() => Grade;
}
