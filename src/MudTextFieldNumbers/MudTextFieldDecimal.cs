using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudTextFieldNumbers;

/// <summary>
/// MudTextFieldDecimal - A numeric text field component for decimal values.
/// 
/// This component extends MudTextField to provide a better user experience for decimal input,
/// especially on mobile devices with virtual keyboards. It uses a text input with inputmode="decimal"
/// instead of input type="number" to avoid issues with virtual keyboards.
/// 
/// Key Features:
/// - Compatible with virtual keyboards on mobile devices
/// - Uses InputMode.decimal for optimized keyboard layout
/// - Configurable decimal places
/// - Configurable decimal separator (e.g., comma or dot)
/// - Automatic validation and formatting
/// - Supports all MudTextField properties and events
/// 
/// Usage:
/// <![CDATA[<MudTextFieldDecimal @bind-Value="myDecimalValue" DecimalPlaces="2" Label="Enter amount" />]]>
/// <![CDATA[<MudTextFieldDecimal @bind-Value="myDecimalValue" DecimalPlaces="3" DecimalSeparator="," Label="Prix" />]]>
/// </summary>
public class MudTextFieldDecimal : MudTextField<decimal?>
{
    /// <summary>
    /// Number of decimal places to display and accept. Default is 2.
    /// The value will be automatically rounded to this number of decimal places.
    /// </summary>
    [Parameter]
    public int DecimalPlaces { get; set; } = 2;

    /// <summary>
    /// The decimal separator character to use for input and display.
    /// If not set, uses the current culture's decimal separator.
    /// Common values: "." (dot) or "," (comma).
    /// </summary>
    [Parameter]
    public string? DecimalSeparator { get; set; }

    /// <summary>
    /// Initializes the component with proper settings for decimal input.
    /// Configures the input type as text with decimal input mode and sets up
    /// the converter for decimal value parsing and formatting with the specified
    /// decimal places and separator.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        // Override input type to text to support virtual keyboards
        InputType = InputType.Text;
        
        // Set input mode to decimal for mobile keyboards
        InputMode = InputMode.@decimal;
        
        // Get the decimal separator to use
        var decimalSeparator = DecimalSeparator ?? 
            System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        
        // Configure converter for decimal values
        Converter = new Converter<decimal?>
        {
            SetFunc = value =>
            {
                if (value == null)
                    return null;

                // Format with specified decimal places
                var formatted = value.Value.ToString($"F{DecimalPlaces}", System.Globalization.CultureInfo.InvariantCulture);
                
                // Replace decimal separator if custom one is specified
                if (decimalSeparator != ".")
                {
                    formatted = formatted.Replace(".", decimalSeparator);
                }

                return formatted;
            },
            GetFunc = text =>
            {
                if (string.IsNullOrWhiteSpace(text))
                    return null;

                // Remove any whitespace
                text = text.Trim();

                // Normalize decimal separator
                var normalizedValue = text.Replace(decimalSeparator, ".");

                // Try to parse as decimal
                if (decimal.TryParse(normalizedValue, 
                    System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal result))
                {
                    // Round to specified decimal places
                    return Math.Round(result, DecimalPlaces);
                }

                return null;
            }
        };
    }
}
