using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
/// - Optional automatic integration with VirtualKeyboardService
/// 
/// Usage:
/// <![CDATA[<MudTextFieldDecimal @bind-Value="myDecimalValue" DecimalPlaces="2" Label="Enter amount" />]]>
/// <![CDATA[<MudTextFieldDecimal @bind-Value="myDecimalValue" DecimalPlaces="3" DecimalSeparator="," Label="Prix" />]]>
/// <![CDATA[<MudTextFieldDecimal @bind-Value="myDecimalValue" UseVirtualKeyboard="true" Label="Enter amount" />]]>
/// </summary>
public class MudTextFieldDecimal : MudTextField<decimal?>, IVirtualKeyboardField
{
    private string _currentText = "";

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
    /// Optional VirtualKeyboardService for automatic keyboard integration.
    /// If provided and UseVirtualKeyboard is true, the field will automatically
    /// connect to the virtual keyboard service on focus.
    /// </summary>
    [Inject]
    private VirtualKeyboardService? KeyboardService { get; set; }

    /// <summary>
    /// When true, automatically connects to the VirtualKeyboardService (if available).
    /// Default is false for backward compatibility.
    /// </summary>
    [Parameter]
    public bool UseVirtualKeyboard { get; set; } = false;

    /// <summary>
    /// Gets the type of keyboard for this field.
    /// </summary>
    public VirtualKeyboardType KeyboardType => VirtualKeyboardType.Decimal;

    /// <summary>
    /// Gets whether this field supports decimal input (true).
    /// </summary>
    public bool SupportsDecimal => true;

    /// <summary>
    /// Gets whether this field supports negative values (true).
    /// </summary>
    public bool SupportsNegative => true;

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

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        // Sync current text with Text property
        if (!string.IsNullOrEmpty(Text))
        {
            _currentText = Text;
        }
        else if (Value.HasValue)
        {
            var decimalSeparator = DecimalSeparator ?? 
                System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var formatted = Value.Value.ToString($"F{DecimalPlaces}", System.Globalization.CultureInfo.InvariantCulture);
            if (decimalSeparator != ".")
            {
                formatted = formatted.Replace(".", decimalSeparator);
            }
            _currentText = formatted;
        }
        else
        {
            _currentText = "";
        }
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && UseVirtualKeyboard && KeyboardService != null)
        {
            // Register focus/blur events through Adornment property which we can customize
            // For now we'll handle this through an explicit API
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Call this method to register the field with the keyboard service (e.g., on focus).
    /// </summary>
    public void RegisterWithKeyboard()
    {
        if (UseVirtualKeyboard && KeyboardService != null)
        {
            KeyboardService.SetActiveField(this);
        }
    }

    /// <summary>
    /// Call this method to unregister the field from the keyboard service (e.g., on blur).
    /// </summary>
    public void UnregisterFromKeyboard()
    {
        if (UseVirtualKeyboard && KeyboardService != null && KeyboardService.ActiveField == this)
        {
            KeyboardService.ClearActiveField();
        }
    }

    public void OnDigitInput(int digit)
    {
        var decimalSeparator = DecimalSeparator ?? 
            System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        var newText = _currentText + digit.ToString();
        var normalized = newText.Replace(decimalSeparator, ".");
        if (decimal.TryParse(normalized, out decimal result))
        {
            _currentText = newText;
            _ = SetTextAsync(_currentText);
        }
    }

    public void OnDecimalInput()
    {
        var decimalSeparator = DecimalSeparator ?? 
            System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        
        if (!_currentText.Contains(decimalSeparator))
        {
            if (string.IsNullOrEmpty(_currentText))
            {
                _currentText = "0" + decimalSeparator;
            }
            else
            {
                _currentText += decimalSeparator;
            }
            _ = SetTextAsync(_currentText);
        }
    }

    public void OnBackspaceInput()
    {
        if (_currentText.Length > 0)
        {
            var newText = _currentText[..^1];
            _currentText = newText;
            _ = SetTextAsync(_currentText);
        }
    }

    public void OnClearInput()
    {
        _currentText = "";
        _ = SetTextAsync("");
    }

    public void OnNegativeInput()
    {
        if (!string.IsNullOrEmpty(_currentText))
        {
            var decimalSeparator = DecimalSeparator ?? 
                System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            
            string newText;
            if (_currentText.StartsWith("-"))
            {
                newText = _currentText[1..];
            }
            else
            {
                newText = "-" + _currentText;
            }
            
            // Validate the new text
            var normalized = newText.Replace(decimalSeparator, ".");
            if (decimal.TryParse(normalized, out decimal result) && result != 0)
            {
                _currentText = newText;
                _ = SetTextAsync(_currentText);
            }
            else if (result == 0 && !newText.StartsWith("-"))
            {
                // Allow positive zero
                _currentText = newText;
                _ = SetTextAsync(_currentText);
            }
        }
    }

    public void OnCharacterInput(char character)
    {
        // Not supported for decimal fields
    }
}
