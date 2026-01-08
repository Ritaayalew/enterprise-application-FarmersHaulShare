namespace MessagingAndNotifications.Domain.ValueObjects;

/// <summary>
/// Value object representing a notification template with placeholders
/// </summary>
public sealed class NotificationTemplate : SharedKernel.Domain.ValueObject
{
    public string TemplateName { get; private init; } = string.Empty;
    public string Subject { get; private init; } = string.Empty;
    public string Body { get; private init; } = string.Empty;
    public string? LanguageCode { get; private init; }

    private NotificationTemplate() { }

    public NotificationTemplate(string templateName, string subject, string body, string? languageCode = null)
    {
        if (string.IsNullOrWhiteSpace(templateName))
            throw new SharedKernel.Domain.DomainException("Template name cannot be empty.");
        if (string.IsNullOrWhiteSpace(subject))
            throw new SharedKernel.Domain.DomainException("Subject cannot be empty.");
        if (string.IsNullOrWhiteSpace(body))
            throw new SharedKernel.Domain.DomainException("Body cannot be empty.");

        TemplateName = templateName.Trim();
        Subject = subject.Trim();
        Body = body.Trim();
        LanguageCode = languageCode?.Trim();
    }

    public string Render(Dictionary<string, string>? placeholders = null)
    {
        if (placeholders == null || placeholders.Count == 0)
            return Body;

        var rendered = Body;
        foreach (var placeholder in placeholders)
        {
            rendered = rendered.Replace($"{{{placeholder.Key}}}", placeholder.Value, StringComparison.OrdinalIgnoreCase);
        }
        return rendered;
    }

    public string RenderSubject(Dictionary<string, string>? placeholders = null)
    {
        if (placeholders == null || placeholders.Count == 0)
            return Subject;

        var rendered = Subject;
        foreach (var placeholder in placeholders)
        {
            rendered = rendered.Replace($"{{{placeholder.Key}}}", placeholder.Value, StringComparison.OrdinalIgnoreCase);
        }
        return rendered;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TemplateName;
        yield return Subject;
        yield return Body;
        yield return LanguageCode ?? string.Empty;
    }

    public override string ToString() => $"{TemplateName} ({LanguageCode ?? "default"})";
}
