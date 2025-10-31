# MudTextFieldNumbers

[![Screenshot](https://github.com/user-attachments/assets/0841d937-99ea-494f-a1fb-19ec1a73a8bd)](https://github.com/benk79hes/MudTextFieldNumbers)

Components pour gérer des types entiers et décimals avec un MudTextField de manière à pouvoir utiliser un clavier virtuel en évitant le input[type=number].

Components to handle integer and decimal input with MudTextField, compatible with virtual keyboards by avoiding `input[type=number]`.

## Features

- ✅ **Virtual Keyboard Compatible**: Uses text input instead of number input for better virtual keyboard compatibility on mobile devices
- ✅ **Smart Input Mode**: Proper InputMode attribute for mobile keyboard optimization (numeric/decimal)
- ✅ **Automatic Formatting**: Automatic formatting and validation of numeric values
- ✅ **Configurable**: Configurable decimal places and separators
- ✅ **Full MudBlazor API**: Inherits all MudTextField features and properties

## Components

### MudTextFieldInteger

Component for integer input values.

**Usage:**
```razor
<MudTextFieldInteger @bind-Value="myIntValue" 
                     Label="Integer Value" 
                     Variant="Variant.Outlined" />
```

### MudTextFieldDecimal

Component for decimal input values with configurable decimal places and separator.

**Usage:**
```razor
<MudTextFieldDecimal @bind-Value="myDecimalValue" 
                     DecimalPlaces="2"
                     Label="Decimal Value" 
                     Variant="Variant.Outlined" />
```

**With custom separator:**
```razor
<MudTextFieldDecimal @bind-Value="myDecimalValue" 
                     DecimalPlaces="3"
                     DecimalSeparator=","
                     Label="Decimal Value (comma)" 
                     Variant="Variant.Outlined" />
```

## Installation

1. Add the NuGet package to your project:
```bash
dotnet add package MudTextFieldNumbers
```

2. Add MudBlazor services to your `Program.cs`:
```csharp
builder.Services.AddMudServices();
```

3. Add the namespace to your `_Imports.razor`:
```razor
@using MudTextFieldNumbers
@using MudBlazor
```

4. Include MudBlazor CSS and JS in your `App.razor` or `index.html`:
```html
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
```

## Why avoid input[type=number]?

The HTML `<input type="number">` has several issues with virtual keyboards on mobile devices:
- Inconsistent behavior across different mobile browsers
- Limited control over input validation
- Issues with decimal separators in different locales
- Poor user experience with spinners on mobile

This library solves these problems by using text inputs with proper `inputmode` attributes and custom validation.

## Demo

Check out the sample application in the `samples/MudTextFieldNumbers.Sample` directory for a complete working example.

## License

MIT License - feel free to use in your projects!

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

