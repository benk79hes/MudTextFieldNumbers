window.MudTextFieldNumbers = window.MudTextFieldNumbers || {};

// Map to store cleanup functions for each wrapper element
window.MudTextFieldNumbers._cleanupFunctions = new Map();

// Setup focus handlers for a wrapper element
window.MudTextFieldNumbers.setupFocusHandlersForWrapper = function(wrapperElement, dotNetHelper) {
    if (!wrapperElement) {
        console.warn('Wrapper element not found');
        return;
    }
    
    // Function to attach handlers to input elements
    function attachHandlers(input) {
        if (input._vkbHandlersAttached) {
            return; // Already attached
        }
        
        const focusHandler = function() {
            dotNetHelper.invokeMethodAsync('NotifyFocusIn');
        };
        
        const blurHandler = function() {
            dotNetHelper.invokeMethodAsync('NotifyFocusOut');
        };
        
        input.addEventListener('focus', focusHandler);
        input.addEventListener('blur', blurHandler);
        input._vkbHandlersAttached = true;
        input._vkbFocusHandler = focusHandler;
        input._vkbBlurHandler = blurHandler;
        
        console.log('Attached focus handlers to input:', input);
    }
    
    // Find and attach to existing input elements
    const inputs = wrapperElement.querySelectorAll('input[type="text"]');
    inputs.forEach(attachHandlers);
    
    // Watch for new input elements being added
    const observer = new MutationObserver(function(mutations) {
        mutations.forEach(function(mutation) {
            mutation.addedNodes.forEach(function(node) {
                if (node.nodeType === 1) { // Element node
                    // Check if the node itself is an input
                    if (node.tagName === 'INPUT' && node.type === 'text') {
                        attachHandlers(node);
                    }
                    // Check for input elements within the node
                    const inputs = node.querySelectorAll ? node.querySelectorAll('input[type="text"]') : [];
                    inputs.forEach(attachHandlers);
                }
            });
        });
    });
    
    observer.observe(wrapperElement, {
        childList: true,
        subtree: true
    });
    
    // Store cleanup function
    window.MudTextFieldNumbers._cleanupFunctions.set(wrapperElement, function() {
        observer.disconnect();
        const inputs = wrapperElement.querySelectorAll('input[type="text"]');
        inputs.forEach(function(input) {
            if (input._vkbHandlersAttached) {
                input.removeEventListener('focus', input._vkbFocusHandler);
                input.removeEventListener('blur', input._vkbBlurHandler);
                delete input._vkbHandlersAttached;
                delete input._vkbFocusHandler;
                delete input._vkbBlurHandler;
            }
        });
    });
    
    console.log('Focus handler setup complete for wrapper with', inputs.length, 'inputs');
};

// Cleanup focus handlers for a wrapper element
window.MudTextFieldNumbers.cleanupFocusHandlers = function(wrapperElement) {
    if (!wrapperElement) return;
    
    const cleanup = window.MudTextFieldNumbers._cleanupFunctions.get(wrapperElement);
    if (cleanup) {
        cleanup();
        window.MudTextFieldNumbers._cleanupFunctions.delete(wrapperElement);
        console.log('Cleaned up focus handlers for wrapper');
    }
};

// Legacy functions kept for backward compatibility
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

