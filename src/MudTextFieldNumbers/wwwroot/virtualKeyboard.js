window.MudTextFieldNumbers = window.MudTextFieldNumbers || {};

window.MudTextFieldNumbers.setupFocusHandlers = function(inputId, dotNetHelper) {
    const input = document.getElementById(inputId);
    if (!input) {
        console.warn('Input element not found:', inputId);
        return;
    }
    
    input.addEventListener('focus', function() {
        dotNetHelper.invokeMethodAsync('NotifyFocusIn');
    });
    
    input.addEventListener('blur', function() {
        dotNetHelper.invokeMethodAsync('NotifyFocusOut');
    });
    
    console.log('Focus handlers setup for:', inputId);
};

window.MudTextFieldNumbers.setupFocusHandlersBySelector = function(selector, dotNetHelper) {
    const inputs = document.querySelectorAll(selector);
    inputs.forEach(function(input) {
        input.addEventListener('focus', function() {
            dotNetHelper.invokeMethodAsync('NotifyFocusIn');
        });
        
        input.addEventListener('blur', function() {
            dotNetHelper.invokeMethodAsync('NotifyFocusOut');
        });
    });
    
    console.log('Focus handlers setup for', inputs.length, 'inputs');
};
