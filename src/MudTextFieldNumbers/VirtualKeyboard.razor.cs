// Copyright © 2025 PLUMETTAZ S.A. - All Rights Reserved

// using MudBlazor;
// using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MudTextFieldNumbers;

/// <summary>
/// Virtual keyboard component for MudBlazor input fields.
/// </summary>
public partial class VirtualKeyboard : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    /// <summary>
    /// Indicates whether the keyboard is visible.
    /// </summary>
    private bool _visible = false;

    /// <summary>
    /// Indicates whether caps lock is activated (uppercase mode).
    /// </summary>
    private bool _capsLock = false;

    /// <summary>
    /// Reference to this component for JS interop callbacks.
    /// </summary>
    private DotNetObjectReference<VirtualKeyboard>? _dotNetRef;

    /// <summary>
    /// Registers JS callbacks for showing and hiding the keyboard on first render.
    /// </summary>
    /// <param name="firstRender">True if this is the first render.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Create and store reference for disposal
            _dotNetRef = DotNetObjectReference.Create(this);
            
            // Register JS callbacks to show/hide the keyboard
            await JS.InvokeVoidAsync(
                "virtualKeyboard.registerShowCallback",
                _dotNetRef,
                nameof(ShowKeyboard));
            await JS.InvokeVoidAsync(
                "virtualKeyboard.registerHideCallback",
                _dotNetRef,
                nameof(HideKeyboard));
        }
    }

    /// <summary>
    /// JS-invokable method to show the keyboard.
    /// </summary>
    [JSInvokable]
    public async Task ShowKeyboard()
    {
        _visible = true;
        StateHasChanged();

        // Adjust viewport for keyboard (Android-like behavior)
        await JS.InvokeVoidAsync("virtualKeyboard.adjustViewportForKeyboard", true);
    }

    /// <summary>
    /// JS-invokable method to hide the keyboard.
    /// </summary>
    [JSInvokable]
    public async Task HideKeyboard()
    {
        _visible = false;
        StateHasChanged();

        // Remove viewport adjustment when keyboard is hidden
        await JS.InvokeVoidAsync("virtualKeyboard.adjustViewportForKeyboard", false);
    }

    /// <summary>
    /// Closes the virtual keyboard when the close button is clicked.
    /// </summary>
    private async Task CloseKeyboard()
    {
        await HideKeyboard();
    }

    /// <summary>
    /// Toggles caps lock mode (uppercase/lowercase).
    /// </summary>
    private void ToggleCapsLock()
    {
        _capsLock = !_capsLock;
        StateHasChanged();
    }

    /// <summary>
    /// Sends a key press to the currently focused input via JS interop.
    /// </summary>
    /// <param name="key">The key to send.</param>
    private async Task SendKey(string key)
    {
        await JS.InvokeVoidAsync("virtualKeyboard.sendKey", key);
    }

    /// <summary>
    /// The layout of the virtual keyboard in QWERTZ format.
    /// </summary>
    private readonly string[][] _keys = new[]
    {
        new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" },
        new[] { "Q", "W", "E", "R", "T", "Z", "U", "I", "O", "P" },
        new[] { "A", "S", "D", "F", "G", "H", "J", "K", "L" },
        new[] { "Y", "X", "C", "V", "B", "N", "M", ",", "-" },
    };

    /// <summary>
    /// Gets the transformed key text, applying caps lock transformation if needed.
    /// </summary>
    /// <param name="key">The original key.</param>
    /// <returns>The transformed key text.</returns>
    private string GetTransformedKey(string key)
    {
        // Only apply caps lock to letters
        if (key.Length == 1 && char.IsLetter(key[0]))
        {
            return _capsLock ? key.ToUpper() : key.ToLower();
        }

        return key;
    }

    /// <summary>
    /// Disposes the component and releases resources.
    /// </summary>
    public void Dispose()
    {
        _dotNetRef?.Dispose();
    }
}
