# Running the Demo Application

This guide shows you how to install and run the MudTextFieldNumbers demo application locally.

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later

## Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/benk79hes/MudTextFieldNumbers.git
cd MudTextFieldNumbers
```

### 2. Build the Solution

```bash
dotnet build
```

This will:
- Restore all NuGet packages
- Build the component library (`src/MudTextFieldNumbers`)
- Build the sample application (`samples/MudTextFieldNumbers.Sample`)

### 3. Run the Demo

```bash
cd samples/MudTextFieldNumbers.Sample
dotnet run
```

The application will start and display output similar to:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5098
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 4. Open in Browser

Open your web browser and navigate to:

```
http://localhost:5098
```

You should see the MudTextFieldNumbers demo page with several examples:

- **Integer Input** - Basic integer input field
- **Decimal Input** - Decimal input with 2 decimal places
- **Decimal with Custom Separator** - Decimal input using comma as separator
- **Required Fields Example** - Fields with validation

## What You'll See

The demo application showcases all the features of the MudTextFieldNumbers components:

1. **Text Input Fields** that look like number inputs but use `inputmode` for better mobile keyboard support
2. **Real-time Value Display** showing the current bound value below each input
3. **Different Configurations** demonstrating various use cases
4. **Validation Examples** with required fields
5. **Benefits List** explaining why these components are useful

## Testing the Components

Try these interactions in the demo:

- **Integer Field**: Type any whole number (e.g., 42, 100, 999)
- **Decimal Field**: Type decimal values with dot separator (e.g., 123.45)
- **Comma Separator Field**: Type decimals with comma (e.g., 99,999)
- **Required Fields**: Leave fields empty and blur to see validation errors

## Troubleshooting

### Port Already in Use

If port 5098 is already in use, you can specify a different port:

```bash
dotnet run --urls "http://localhost:5099"
```

### Build Errors

If you encounter build errors, try cleaning and rebuilding:

```bash
dotnet clean
dotnet restore
dotnet build
```

### Components Not Appearing

If the input fields don't appear:
1. Make sure you've built the solution (`dotnet build`)
2. Clear your browser cache
3. Check the browser console for JavaScript errors
4. Verify that the component library is referenced correctly in the sample project

## Development

To make changes to the components:

1. Edit files in `src/MudTextFieldNumbers/`
2. Rebuild: `dotnet build`
3. Restart the demo application

The components are implemented as C# classes that extend `MudTextField<T>`:
- `MudTextFieldInteger.cs` - Integer input component
- `MudTextFieldDecimal.cs` - Decimal input component

## Next Steps

- Read [USAGE.md](../../USAGE.md) for detailed usage examples
- Read [README.md](../../README.md) for component documentation
- Try integrating the components into your own Blazor application

## Getting Help

If you encounter issues:
- Check the [Issues](https://github.com/benk79hes/MudTextFieldNumbers/issues) page
- Review the [MudBlazor documentation](https://mudblazor.com/)
- Open a new issue with details about your problem
