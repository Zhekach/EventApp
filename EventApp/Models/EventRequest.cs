using System.ComponentModel.DataAnnotations;

namespace EventApp.Models;

public record EventRequest : IValidatableObject
{
    [Required (ErrorMessage =  "Названия мероприятия обязательно для заполнения")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Название должно быть от 2 до 100 символов")]
    public string Title { get; set; }
    
    [Required (ErrorMessage =  "Время начала обязательно для заполнения")]
    public DateTime StartAt { get; set; }
    [Required (ErrorMessage =  "Время окончания обязательно для заполнения")]
    public DateTime EndAt { get; set; }
    public string Description { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndAt <= StartAt)
        {
            yield return new ValidationResult(
                "Дата окончания должна быть позже даты начала", [nameof(EndAt)]);
        }
    }
}