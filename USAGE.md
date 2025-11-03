# Usage Guide - MudTextFieldNumbers

## Quick Start

### 1. Installation

```bash
dotnet add package MudTextFieldNumbers
```

### 2. Setup

Add to your `Program.cs`:
```csharp
builder.Services.AddMudServices();
```

Add to your `_Imports.razor`:
```razor
@using MudBlazor
@using MudTextFieldNumbers
```

Include MudBlazor resources in your `App.razor` or `index.html`:
```html
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
```

## MudTextFieldInteger

### Basic Usage

```razor
@code {
    private int? myValue = 42;
}

<MudTextFieldInteger @bind-Value="myValue" Label="Enter a number" />
```

### With Validation

```razor
<MudTextFieldInteger @bind-Value="myValue" 
                     Label="Required Integer" 
                     Required="true"
                     RequiredError="This field is required" />
```

### With All Common Properties

```razor
<MudTextFieldInteger @bind-Value="myValue"
                     Label="Quantity"
                     Variant="Variant.Outlined"
                     Placeholder="Enter quantity"
                     HelperText="Enter a whole number"
                     Disabled="false"
                     ReadOnly="false"
                     Required="true"
                     RequiredError="Quantity is required"
                     Immediate="false"
                     Adornment="Adornment.End"
                     AdornmentIcon="@Icons.Material.Filled.Numbers"
                     AdornmentColor="Color.Primary" />
```

## MudTextFieldDecimal

### Basic Usage (2 decimal places)

```razor
@code {
    private decimal? myValue = 123.45m;
}

<MudTextFieldDecimal @bind-Value="myValue" Label="Enter amount" />
```

### With Custom Decimal Places

```razor
<MudTextFieldDecimal @bind-Value="myValue" 
                     DecimalPlaces="3"
                     Label="Precise Value" />
```

### With Custom Decimal Separator (Comma)

```razor
<MudTextFieldDecimal @bind-Value="myValue" 
                     DecimalPlaces="2"
                     DecimalSeparator=","
                     Label="Prix (€)" />
```

### Complete Example with Validation

```razor
<MudTextFieldDecimal @bind-Value="myValue"
                     DecimalPlaces="2"
                     Label="Price"
                     Variant="Variant.Filled"
                     Placeholder="0.00"
                     HelperText="Enter the price"
                     Required="true"
                     RequiredError="Price is required"
                     Adornment="Adornment.Start"
                     AdornmentText="$" />
```

## Common Use Cases

### Currency Input (USD)

```razor
<MudTextFieldDecimal @bind-Value="price"
                     DecimalPlaces="2"
                     Label="Price"
                     Adornment="Adornment.Start"
                     AdornmentText="$"
                     Format="C2" />
```

### Currency Input (EUR with comma)

```razor
<MudTextFieldDecimal @bind-Value="prix"
                     DecimalPlaces="2"
                     DecimalSeparator=","
                     Label="Prix"
                     Adornment="Adornment.End"
                     AdornmentText="€" />
```

### Percentage with 1 decimal

```razor
<MudTextFieldDecimal @bind-Value="percentage"
                     DecimalPlaces="1"
                     Label="Percentage"
                     Adornment="Adornment.End"
                     AdornmentText="%" />
```

### Quantity Counter

```razor
<MudTextFieldInteger @bind-Value="quantity"
                     Label="Quantity"
                     Variant="Variant.Outlined"
                     HelperText="How many items?" />
```

### Weight with 3 decimals

```razor
<MudTextFieldDecimal @bind-Value="weight"
                     DecimalPlaces="3"
                     Label="Weight"
                     Adornment="Adornment.End"
                     AdornmentText="kg" />
```

## Form Integration

```razor
<EditForm Model="@model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />
    
    <MudTextFieldInteger @bind-Value="model.Quantity"
                         For="@(() => model.Quantity)"
                         Label="Quantity"
                         Required="true" />
    
    <MudTextFieldDecimal @bind-Value="model.Price"
                         For="@(() => model.Price)"
                         DecimalPlaces="2"
                         Label="Price"
                         Required="true" />
    
    <MudButton ButtonType="ButtonType.Submit" 
               Variant="Variant.Filled" 
               Color="Color.Primary">
        Submit
    </MudButton>
</EditForm>

@code {
    private OrderModel model = new();
    
    private void HandleSubmit()
    {
        // Handle form submission
    }
    
    public class OrderModel
    {
        [Required]
        public int? Quantity { get; set; }
        
        [Required]
        public decimal? Price { get; set; }
    }
}
```

## Virtual Keyboard - Automatic Integration

The automatic integration approach allows you to have a single virtual keyboard that automatically connects to any focused field without manually wiring event callbacks.

### Setup

**1. Register the service in Program.cs:**

```csharp
using MudTextFieldNumbers;

builder.Services.AddSingleton<VirtualKeyboardService>();
```

**2. Add the keyboard manager to your layout (e.g., MainLayout.razor footer):**

```razor
@* In your layout footer or wherever you want the keyboard to appear *@
<MudContainer Class="mt-4" MaxWidth="MaxWidth.Medium">
    <MudVirtualKeyboardManager />
</MudContainer>
```

**3. Wrap your fields with VirtualKeyboardFieldWrapper:**

```razor
@inject VirtualKeyboardService KeyboardService

<VirtualKeyboardFieldWrapper Field="@_quantityField">
    <MudTextFieldInteger @ref="_quantityField"
                        @bind-Value="_quantity"
                        UseVirtualKeyboard="true"
                        Label="Quantity" 
                        Variant="Variant.Outlined" />
</VirtualKeyboardFieldWrapper>

<VirtualKeyboardFieldWrapper Field="@_priceField">
    <MudTextFieldDecimal @ref="_priceField"
                        @bind-Value="_price"
                        UseVirtualKeyboard="true"
                        DecimalPlaces="2"
                        Label="Price" 
                        Variant="Variant.Outlined" />
</VirtualKeyboardFieldWrapper>

@code {
    private MudTextFieldInteger? _quantityField;
    private MudTextFieldDecimal? _priceField;
    private int? _quantity;
    private decimal? _price;
}
```

### Multiple Fields Example

```razor
@page "/order-form"
@rendermode InteractiveServer

<MudGrid>
    <MudItem xs="12" md="6">
        <VirtualKeyboardFieldWrapper Field="@_field1">
            <MudTextFieldInteger @ref="_field1"
                                @bind-Value="_quantity"
                                UseVirtualKeyboard="true"
                                Label="Quantity"
                                Variant="Variant.Outlined" />
        </VirtualKeyboardFieldWrapper>
    </MudItem>
    
    <MudItem xs="12" md="6">
        <VirtualKeyboardFieldWrapper Field="@_field2">
            <MudTextFieldDecimal @ref="_field2"
                                @bind-Value="_unitPrice"
                                UseVirtualKeyboard="true"
                                DecimalPlaces="2"
                                Label="Unit Price"
                                Variant="Variant.Outlined" />
        </VirtualKeyboardFieldWrapper>
    </MudItem>
    
    <MudItem xs="12" md="6">
        <VirtualKeyboardFieldWrapper Field="@_field3">
            <MudTextFieldDecimal @ref="_field3"
                                @bind-Value="_taxRate"
                                UseVirtualKeyboard="true"
                                DecimalPlaces="2"
                                Label="Tax Rate (%)"
                                Variant="Variant.Outlined" />
        </VirtualKeyboardFieldWrapper>
    </MudItem>
</MudGrid>

@* Keyboard appears in layout footer automatically *@

@code {
    private MudTextFieldInteger? _field1;
    private MudTextFieldDecimal? _field2;
    private MudTextFieldDecimal? _field3;
    
    private int? _quantity;
    private decimal? _unitPrice;
    private decimal? _taxRate;
}
```

### Benefits of Automatic Integration

- **Zero Event Wiring**: No need to manually wire up `DigitClicked`, `BackspaceClicked`, etc.
- **Single Keyboard**: One keyboard instance serves all fields throughout your application
- **Automatic Adaptation**: Keyboard automatically shows/hides decimal button based on field type
- **Focus Management**: Keyboard appears when field is focused, hides when blurred
- **Flexible Placement**: Place the keyboard manager anywhere (footer, modal, sidebar, etc.)
- **Per-Field Configuration**: Each field can have its own settings (decimal places, separator, etc.)

### Customizing the Keyboard Manager

```razor
<MudVirtualKeyboardManager 
    Title="Enter Value"
    KeyboardClass="custom-keyboard-style"
    KeyboardStyle="max-width: 400px; margin: 0 auto;" />
```

**Available Parameters:**
- `Title`: Custom title (default is based on field type)
- `Class`: CSS class for the container
- `Style`: Inline styles for the container
- `KeyboardClass`: CSS class for the keyboard itself
- `KeyboardStyle`: Inline styles for the keyboard itself



## Advanced Customization

Both components inherit from `MudTextField`, so all MudTextField properties are available:

- `Label`, `Placeholder`, `HelperText`
- `Variant` (Text, Filled, Outlined)
- `Margin`, `Class`, `Style`
- `Disabled`, `ReadOnly`
- `Required`, `RequiredError`
- `Adornment`, `AdornmentIcon`, `AdornmentText`, `AdornmentColor`
- `Lines` (for multiline, though not recommended for numeric input)
- `OnBlur`, `OnKeyDown`, `OnKeyUp`
- And many more!

## Best Practices

1. **Always use nullable types** (`int?`, `decimal?`) to handle empty inputs properly
2. **Set appropriate DecimalPlaces** for decimal inputs (default is 2)
3. **Use DecimalSeparator** parameter when your app targets specific locales
4. **Add validation** with `Required` and `RequiredError` for mandatory fields
5. **Provide HelperText** to guide users on expected input format
6. **Use Adornments** for currency symbols, units, or icons
7. **Test on mobile devices** to verify virtual keyboard behavior

## Troubleshooting

### Value not updating
- Ensure you're using `@bind-Value` not `Value`
- Check that your property is nullable (`int?` or `decimal?`)

### Validation not working
- Add `DataAnnotationsValidator` to your `EditForm`
- Use the `For` parameter: `For="@(() => model.Property)"`
- Ensure Required attribute is set if needed

### Wrong decimal separator displayed
- Set the `DecimalSeparator` parameter explicitly
- Check your culture settings if not using custom separator

### Virtual keyboard not showing numbers
- This shouldn't happen as components use proper `inputmode`
- If it does, check your browser/device compatibility
- Report as an issue on GitHub

## Support

For issues, questions, or contributions:
- GitHub: https://github.com/benk79hes/MudTextFieldNumbers
- MudBlazor Documentation: https://mudblazor.com/
