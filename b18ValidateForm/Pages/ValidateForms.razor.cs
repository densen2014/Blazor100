using System.Diagnostics.CodeAnalysis;

namespace b18ValidateForm.Pages;

/// <summary>
/// 
/// </summary>
public sealed partial class ValidateForms
{
    [NotNull]
    private Foo? Model { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnInitialized()
    {
        Model = new Foo()
        {
            Name = "",
            Count = 1,
            Address = "",
            DateTime = new DateTime(1997, 12, 05),
            Education = EnumEducation.Middle
        };
    }

}
