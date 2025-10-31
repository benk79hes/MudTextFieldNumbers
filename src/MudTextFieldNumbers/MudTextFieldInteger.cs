using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudTextFieldNumbers;

/// <summary>
/// MudTextFieldInteger - A numeric text field component for integer values.
/// 
/// This component extends MudTextField to provide a better user experience for integer input,
/// especially on mobile devices with virtual keyboards. It uses a text input with inputmode="numeric"
/// instead of input type="number" to avoid issues with virtual keyboards.
/// 
/// Key Features:
/// - Compatible with virtual keyboards on mobile devices
/// - Uses InputMode.numeric for optimized keyboard layout
/// - Automatic validation and formatting
/// - Supports all MudTextField properties and events
/// 
/// Usage:
/// <![CDATA[<MudTextFieldInteger @bind-Value="myIntValue" Label="Enter number" />]]>
/// </summary>
public class MudTextFieldInteger : MudTextField<int?>
{
    /// <summary>
    /// Initializes the component with proper settings for integer input.
    /// Configures the input type as text with numeric input mode and sets up
    /// the converter for integer value parsing and formatting.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        // Override input type to text to support virtual keyboards
        InputType = InputType.Text;
        
        // Set input mode to numeric for mobile keyboards
        InputMode = InputMode.numeric;
        
        // Configure converter for integer values
        Converter = new Converter<int?>
        {
            SetFunc = value => value?.ToString(System.Globalization.CultureInfo.InvariantCulture),
            GetFunc = text =>
            {
                if (string.IsNullOrWhiteSpace(text))
                    return null;

                // Remove any whitespace
                text = text.Trim();

                // Try to parse as integer
                if (int.TryParse(text, System.Globalization.NumberStyles.Integer, 
                    System.Globalization.CultureInfo.InvariantCulture, out int result))
                {
                    return result;
                }

                return null;
            }
        };
    }
}
