window.virtualKeyboard = {
    currentTarget: null,
    showCallback: null,
    hideCallback: null,
    originalViewportHeight: window.innerHeight,
    keyboardHeight: 300, // Estimated keyboard height
    init: function () {
        document.addEventListener('focusin', function (e) {
            let el = e.target;
            
            // Don't show keyboard for radio inputs, especially mud-radio-input
            if (el.tagName === 'INPUT' && (el.type === 'radio' || el.classList.contains('mud-radio-input'))) {
                return;
            }
            
            // Don't show keyboard if the target or any parent is a MudRadio or MudRadioGroup
            if (e.target.closest && (
                e.target.closest('.mud-radio') || 
                e.target.closest('.mud-radio-group') ||
                e.target.classList.contains('mud-radio') ||
                e.target.classList.contains('mud-radio-group')
            )) {
                return;
            }

            // If still no input found, check if we're in a MudTextField container
            if (!el && e.target.closest) {
                const mudField = e.target.closest('.mud-input-control, .mud-field, .mud-text-field');
                if (mudField) {
                    el = mudField.querySelector('input, textarea');
                }
            }

            if (el && (el.tagName === 'INPUT' || el.tagName === 'TEXTAREA')) {
                window.virtualKeyboard.currentTarget = el;
                if (window.virtualKeyboard.showCallback) {
                    try {
                        window.virtualKeyboard.showCallback.invokeMethodAsync('ShowKeyboard');
                        // Android-like behavior: scroll input into view
                        window.virtualKeyboard.scrollInputIntoView(el);
                    } catch (error) {
                        console.error('Error showing virtual keyboard:', error);
                    }
                }
            }
        });
        document.addEventListener('focusout', function (e) {
            // Use a delay to allow the new focus to occur
            setTimeout(function () {
                const activeElement = document.activeElement;

                // Don't close if focus goes to a virtual keyboard element
                const isVirtualKeyboardElement = activeElement && (
                    activeElement.closest('.mud-paper') || // Keyboard's MudPaper
                    activeElement.closest('[class*="virtual-keyboard"]') || // Specific keyboard class
                    activeElement.closest('[style*="z-index: 4000"]') || // Keyboard container with high z-index
                    activeElement.type === 'button' && activeElement.textContent &&
                    (activeElement.textContent.includes('⌫') ||
                        activeElement.textContent.includes('⏎') ||
                        activeElement.textContent.includes('✕') ||
                        /^[A-Z0-9]$/.test(activeElement.textContent.trim()))
                );

                // Don't close if focus goes to another input
                const isInputElement = activeElement &&
                    (activeElement.tagName === 'INPUT' || activeElement.tagName === 'TEXTAREA');

                // Close only if it's neither a keyboard element nor an input
                if (!isVirtualKeyboardElement && !isInputElement) {
                    if (window.virtualKeyboard.hideCallback) {
                        try {
                            window.virtualKeyboard.hideCallback.invokeMethodAsync('HideKeyboard');
                        } catch (error) {
                            console.error('Error hiding virtual keyboard:', error);
                        }
                    }
                    // Reset scroll position when keyboard closes
                    window.virtualKeyboard.resetScroll();
                }
            }, 50); // Short delay to allow new focus to stabilize
        });
    },
    sendKey: function (key) {
        let el = window.virtualKeyboard.currentTarget;

        // If no current target or it's not valid, try to find the currently focused input
        if (!el || !el.isConnected) {
            const activeElement = document.activeElement;
            if (activeElement && (activeElement.tagName === 'INPUT' || activeElement.tagName === 'TEXTAREA')) {
                el = activeElement;
                window.virtualKeyboard.currentTarget = el;
            } else {
                // Look for any visible input in password dialogs or forms
                const visibleInputs = Array.from(document.querySelectorAll('input, textarea'))
                    .filter(input => {
                        const style = window.getComputedStyle(input);
                        return style.display !== 'none' && style.visibility !== 'hidden' && input.offsetParent !== null;
                    });

                if (visibleInputs.length > 0) {
                    el = visibleInputs[visibleInputs.length - 1]; // Take the last visible input (likely in a dialog)
                    window.virtualKeyboard.currentTarget = el;
                }
            }
        }
        if (!el) {
            return;
        }

        // Check if this is a numeric field (MudNumericField or number input)
        const isNumericField = el.type === 'number' || 
                              el.closest('.mud-numeric-field') || 
                              el.getAttribute('inputmode') === 'numeric' ||
                              el.classList.contains('mud-input-numeric');
        
        // Ensure the element is focused
        if (document.activeElement !== el) {
            el.focus();
        }

        // Force cursor to end and deselect everything for numeric fields to prevent replacement
        if (isNumericField && el.type !== 'number') {
            const currentLength = el.value.length;
            el.setSelectionRange(currentLength, currentLength);
        }

        // Handle character input directly without KeyboardEvent simulation
        if (key === 'Backspace') {
            const currentValue = el.value;
            let newValue;
            let newCursorPos;

            if (el.type === 'number') {
                // For number inputs, simply remove the last character
                if (currentValue.length > 0) {
                    newValue = currentValue.slice(0, -1);
                } else {
                    return; // Nothing to delete
                }
            } else {
                // For text inputs, use selection logic
                const selectionStart = el.selectionStart || 0;
                const selectionEnd = el.selectionEnd || 0;

                if (selectionStart === selectionEnd) {
                    // No selection, delete character before cursor
                    if (selectionStart > 0) {
                        newValue = currentValue.substring(0, selectionStart - 1) + currentValue.substring(selectionStart);
                        newCursorPos = selectionStart - 1;
                    } else {
                        return; // Nothing to delete
                    }
                } else {
                    // Delete selected text
                    newValue = currentValue.substring(0, selectionStart) + currentValue.substring(selectionEnd);
                    newCursorPos = selectionStart;
                }
            }

            el.value = newValue;
            
            // Only set selection range if the input type supports it
            if (el.type !== 'number' && newCursorPos !== undefined) {
                el.setSelectionRange(newCursorPos, newCursorPos);
            }

        } else if (key === 'Enter') {
            // Trigger form submission or blur
            el.blur();
            if (el.form) {
                el.form.dispatchEvent(new Event('submit'));
            }
            return;
        } else {
            // Get current cursor position and selection
            const selectionStart = el.selectionStart || 0;
            const selectionEnd = el.selectionEnd || 0;
            const currentValue = el.value;

            let newValue;
            let newCursorPos;

            // Handle character input more naturally
            if (/^\d$/.test(key)) { // If it's a digit
                // Check if there's a pending decimal point
                const hasPendingDecimal = el.dataset.pendingDecimal === 'true';
                
                // For number inputs, special handling since they don't support selection
                if (el.type === 'number') {
                    if (currentValue === "0" || currentValue === "") {
                        if (hasPendingDecimal) {
                            newValue = "0." + key;
                        } else {
                            newValue = key;
                        }
                    } else {
                        if (hasPendingDecimal) {
                            newValue = currentValue + "." + key;
                        } else {
                            newValue = currentValue + key;
                        }
                    }
                    // Clear the pending decimal flag
                    delete el.dataset.pendingDecimal;
                } else {
                    // For text inputs (MudNumericField), always append since we forced cursor to end
                    if (currentValue === "0") {
                        // Replace single "0" with the new digit
                        if (hasPendingDecimal) {
                            newValue = "0." + key;
                        } else {
                            newValue = key;
                        }
                        newCursorPos = newValue.length;
                    } else {
                        // Append to the end
                        if (hasPendingDecimal) {
                            newValue = currentValue + "." + key;
                        } else {
                            newValue = currentValue + key;
                        }
                        newCursorPos = newValue.length;
                    }
                    // Clear the pending decimal flag
                    delete el.dataset.pendingDecimal;
                }
            } else if ((key === '.' || key === ',') && isNumericField) {
                // Handle decimal point/comma for numeric fields
                
                // Check if current value already has a decimal separator
                const hasDecimalDot = currentValue.includes('.');
                const hasDecimalComma = currentValue.includes(',');
                
                // Check if the current value is auto-formatted with trailing zeros (e.g., "1,000" or "1.000")
                // If so, replace it with the user's decimal input
                const autoFormattedPattern = /^(\d+)[.,]0+$/;
                const match = currentValue.match(autoFormattedPattern);
                
                if (match && (hasDecimalDot || hasDecimalComma)) {
                    // Remove the auto-formatted decimal part and add user's decimal separator
                    newValue = match[1] + key;
                    newCursorPos = newValue.length;
                } else if (!hasDecimalDot && !hasDecimalComma) {
                    if (el.type === 'number') {
                        // For number inputs, don't add a trailing decimal point
                        // Instead, store the intent to add decimal and wait for next digit
                        el.dataset.pendingDecimal = 'true';
                        return; // Don't update the value yet
                    } else {
                        // For MudNumericField (text-based), use the actual key pressed (respects field's decimal separator)
                        newValue = currentValue + key;
                        newCursorPos = newValue.length;
                    }
                } else {
                    return; // Already has a user-entered decimal value, don't add another
                }
            } else {
                // For non-numeric characters or non-numeric fields
                if (el.type === 'number') {
                    // Number inputs only accept numbers and decimal points
                    return;
                } else {
                    newValue = currentValue.substring(0, selectionStart) + key + currentValue.substring(selectionEnd);
                    newCursorPos = selectionStart + 1;
                }
            }

            // Update the value immediately
            el.value = newValue;
            
            // Only set selection range if the input type supports it
            if (el.type !== 'number' && newCursorPos !== undefined) {
                el.setSelectionRange(newCursorPos, newCursorPos);
            }
        }

        // Trigger input and change events with proper timing for Blazor
        setTimeout(() => {
            const inputEvent = new Event('input', { bubbles: true });
            el.dispatchEvent(inputEvent);
            
            // Simple change event after a short delay
            setTimeout(() => {
                const changeEvent = new Event('change', { bubbles: true });
                el.dispatchEvent(changeEvent);
            }, 50);
        }, 10);
    },
    scrollInputIntoView: function (inputElement) {
        if (!inputElement) return;
        
        setTimeout(() => {
            // Try to get actual keyboard height from DOM
            const keyboardElement = document.querySelector('[style*="position: fixed"][style*="bottom: 0"]');
            let keyboardHeight = window.virtualKeyboard.keyboardHeight;
            
            if (keyboardElement) {
                keyboardHeight = keyboardElement.offsetHeight;
                window.virtualKeyboard.keyboardHeight = keyboardHeight; // Update for future use
            }
            
            // Check for password field in a dialog
            const isPasswordField = inputElement.type === 'password' || 
                                   (inputElement.getAttribute('inputtype') === 'password');
            
            // Handle password dialogs specially - ensure they're high enough in the viewport
            if (isPasswordField) {
                // Find parent dialog if it exists
                const dialogContainer = inputElement.closest('.mud-overlay');
                if (dialogContainer) {
                    // Ensure the dialog is positioned high in the viewport
                    if (!dialogContainer.style.paddingTop || 
                        parseInt(dialogContainer.style.paddingTop) < 10) {
                        dialogContainer.style.display = 'flex';
                        dialogContainer.style.alignItems = 'flex-start';
                        dialogContainer.style.justifyContent = 'center';
                        dialogContainer.style.paddingTop = '10vh';
                    }
                    return; // Skip regular scrolling for password dialogs
                }
            }
            
            const rect = inputElement.getBoundingClientRect();
            const viewportHeight = window.innerHeight;
            const availableHeight = viewportHeight - keyboardHeight;
            
            // Check if input is hidden behind keyboard
            if (rect.bottom > availableHeight) {
                const scrollOffset = rect.bottom - availableHeight + 20; // Add some padding
                
                // Smooth scroll to bring input into view
                window.scrollBy({
                    top: scrollOffset,
                    behavior: 'smooth'
                });
            }
        }, 200); // Increased delay to ensure keyboard is fully rendered
    },
    adjustViewportForKeyboard: function (show) {
        const body = document.body;
        if (show) {
            // Add padding bottom to account for keyboard
            body.style.paddingBottom = window.virtualKeyboard.keyboardHeight + 'px';
            body.style.transition = 'padding-bottom 0.3s ease';
        } else {
            // Remove padding when keyboard is hidden
            body.style.paddingBottom = '0px';
            setTimeout(() => {
                body.style.transition = '';
            }, 300);
        }
    },
    resetScroll: function () {
        // Optionally scroll back to a neutral position when keyboard closes
        // This can be customized based on your app's needs
        setTimeout(() => {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        }, 100);
    },
    registerShowCallback: function (cb) {
        window.virtualKeyboard.showCallback = cb;
    },
    registerHideCallback: function (cb) {
        window.virtualKeyboard.hideCallback = cb;
    }
};

window.addEventListener('DOMContentLoaded', function () {
    window.virtualKeyboard.init();
});
