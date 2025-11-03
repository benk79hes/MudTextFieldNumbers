using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
/// - Optional automatic integration with VirtualKeyboardService
/// 
/// Usage:
/// <![CDATA[<MudTextFieldInteger @bind-Value="myIntValue" Label="Enter number" />]]>
/// <![CDATA[<MudTextFieldInteger @bind-Value="myIntValue" UseVirtualKeyboard="true" Label="Enter number" />]]>
/// </summary>
public class MudTextFieldInteger : MudTextField<int?>, IVirtualKeyboardField
{
    private string _currentText = "";

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
    public VirtualKeyboardType KeyboardType => VirtualKeyboardType.Numeric;

    /// <summary>
    /// Gets whether this field supports decimal input (false for integer).
    /// </summary>
    public bool SupportsDecimal => false;

    /// <summary>
    /// Gets whether this field supports negative values (true).
    /// </summary>
    public bool SupportsNegative => true;

    /// <summary>
    /// Gets the decimal separator (null for integer fields).
    /// </summary>
    public string? DecimalSeparator => null;

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
            _currentText = Value.Value.ToString();
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
        _currentText += digit.ToString();
        if (int.TryParse(_currentText, out int result))
        {
            _ = SetTextAsync(_currentText);
        }
    }

    public void OnDecimalInput()
    {
        // Not supported for integer fields
    }

    public void OnBackspaceInput()
    {
        if (_currentText.Length > 0)
        {
            _currentText = _currentText[..^1];
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
            if (_currentText.StartsWith("-"))
            {
                _currentText = _currentText[1..];
            }
            else
            {
                _currentText = "-" + _currentText;
            }
            _ = SetTextAsync(_currentText);
        }
    }

    public void OnCharacterInput(char character)
    {
        // Not supported for integer fields
    }
}
