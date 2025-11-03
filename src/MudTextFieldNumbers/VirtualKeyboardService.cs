using Microsoft.AspNetCore.Components;

namespace MudTextFieldNumbers;

/// <summary>
/// Service that manages the state and behavior of virtual keyboards.
/// Allows automatic coordination between input fields and a single virtual keyboard instance.
/// </summary>
public class VirtualKeyboardService
{
    private IVirtualKeyboardField? _activeField;
    
    /// <summary>
    /// Event raised when the active field changes (focused or blurred).
    /// </summary>
    public event EventHandler<IVirtualKeyboardField?>? ActiveFieldChanged;

    /// <summary>
    /// Gets the currently active field that should receive keyboard input.
    /// </summary>
    public IVirtualKeyboardField? ActiveField => _activeField;

    /// <summary>
    /// Registers a field as the active field that should receive keyboard input.
    /// </summary>
    /// <param name="field">The field to activate</param>
    public void SetActiveField(IVirtualKeyboardField? field)
    {
        if (_activeField != field)
        {
            _activeField = field;
            ActiveFieldChanged?.Invoke(this, _activeField);
        }
    }

    /// <summary>
    /// Clears the active field (typically when a field loses focus).
    /// </summary>
    public void ClearActiveField()
    {
        SetActiveField(null);
    }

    /// <summary>
    /// Sends a digit to the currently active field.
    /// </summary>
    /// <param name="digit">The digit (0-9) to send</param>
    public void SendDigit(int digit)
    {
        _activeField?.OnDigitInput(digit);
    }

    /// <summary>
    /// Sends a decimal separator to the currently active field.
    /// </summary>
    public void SendDecimal()
    {
        _activeField?.OnDecimalInput();
    }

    /// <summary>
    /// Sends a backspace command to the currently active field.
    /// </summary>
    public void SendBackspace()
    {
        _activeField?.OnBackspaceInput();
    }

    /// <summary>
    /// Sends a clear command to the currently active field.
    /// </summary>
    public void SendClear()
    {
        _activeField?.OnClearInput();
    }

    /// <summary>
    /// Toggles the sign (positive/negative) of the currently active field.
    /// </summary>
    public void SendNegativeToggle()
    {
        _activeField?.OnNegativeInput();
    }

    /// <summary>
    /// Sends a character to the currently active field (for text fields).
    /// </summary>
    /// <param name="character">The character to send</param>
    public void SendCharacter(char character)
    {
        _activeField?.OnCharacterInput(character);
    }
}

/// <summary>
/// Interface for fields that can receive input from a virtual keyboard.
/// </summary>
public interface IVirtualKeyboardField
{
    /// <summary>
    /// Gets the type of keyboard that should be displayed for this field.
    /// </summary>
    VirtualKeyboardType KeyboardType { get; }

    /// <summary>
    /// Gets whether the field supports decimal input.
    /// </summary>
    bool SupportsDecimal { get; }

    /// <summary>
    /// Gets whether the field supports negative values.
    /// </summary>
    bool SupportsNegative { get; }

    /// <summary>
    /// Gets the decimal separator character for this field.
    /// </summary>
    string? DecimalSeparator { get; }

    /// <summary>
    /// Called when a digit button is pressed on the virtual keyboard.
    /// </summary>
    /// <param name="digit">The digit (0-9)</param>
    void OnDigitInput(int digit);

    /// <summary>
    /// Called when the decimal separator button is pressed.
    /// </summary>
    void OnDecimalInput();

    /// <summary>
    /// Called when the backspace button is pressed.
    /// </summary>
    void OnBackspaceInput();

    /// <summary>
    /// Called when the clear button is pressed.
    /// </summary>
    void OnClearInput();

    /// <summary>
    /// Called when the negative toggle button is pressed.
    /// </summary>
    void OnNegativeInput();

    /// <summary>
    /// Called when a character is typed (for text keyboards).
    /// </summary>
    /// <param name="character">The character that was typed</param>
    void OnCharacterInput(char character);
}

/// <summary>
/// The type of virtual keyboard to display.
/// </summary>
public enum VirtualKeyboardType
{
    /// <summary>
    /// Numeric keyboard for integer values.
    /// </summary>
    Numeric,

    /// <summary>
    /// Decimal keyboard for decimal values.
    /// </summary>
    Decimal,

    /// <summary>
    /// Text keyboard for text input.
    /// </summary>
    Text
}
