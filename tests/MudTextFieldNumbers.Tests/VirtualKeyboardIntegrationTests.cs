using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using Xunit;

namespace MudTextFieldNumbers.Tests;

/// <summary>
/// Integration tests for MudVirtualKeyboard with MudTextFieldInteger and MudTextFieldDecimal
/// </summary>
public class VirtualKeyboardIntegrationTests : TestContext
{
    public VirtualKeyboardIntegrationTests()
    {
        // Register MudBlazor services
        Services.AddMudServices();
        
        // Setup JSInterop for MudBlazor components
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void VirtualKeyboard_WithIntegerField_UpdatesValue_WhenDigitsClicked()
    {
        // Arrange
        var cut = RenderComponent<TestIntegerFieldWithKeyboard>();

        // Act - Click digits 1, 2, 3 (find buttons fresh each time to avoid stale references)
        var buttons1 = cut.FindAll("button");
        var button1 = buttons1.First(b => b.TextContent.Trim() == "1");
        button1.Click();
        
        var buttons2 = cut.FindAll("button");
        var button2 = buttons2.First(b => b.TextContent.Trim() == "2");
        button2.Click();
        
        var buttons3 = cut.FindAll("button");
        var button3 = buttons3.First(b => b.TextContent.Trim() == "3");
        button3.Click();

        // Assert
        var input = cut.Find("input");
        Assert.Equal("123", input.GetAttribute("value"));
    }

    [Fact]
    public void VirtualKeyboard_WithDecimalField_UpdatesValue_WhenDigitsAndDecimalClicked()
    {
        // Arrange
        var cut = RenderComponent<TestDecimalFieldWithKeyboard>();

        // Act - Click 1, 2, decimal, 3, 4 (find buttons fresh each time)
        var buttons1 = cut.FindAll("button");
        buttons1.First(b => b.TextContent.Trim() == "1").Click();
        
        var buttons2 = cut.FindAll("button");
        buttons2.First(b => b.TextContent.Trim() == "2").Click();
        
        var buttons3 = cut.FindAll("button");
        buttons3.First(b => b.TextContent.Contains(".")).Click();
        
        var buttons4 = cut.FindAll("button");
        buttons4.First(b => b.TextContent.Trim() == "3").Click();
        
        var buttons5 = cut.FindAll("button");
        buttons5.First(b => b.TextContent.Trim() == "4").Click();

        // Assert
        var input = cut.Find("input");
        Assert.Equal("12.34", input.GetAttribute("value"));
    }

    [Fact]
    public void VirtualKeyboard_WithIntegerField_Backspace_RemovesLastCharacter()
    {
        // Arrange
        var cut = RenderComponent<TestIntegerFieldWithKeyboard>();

        // Act - Click 1, 2, 3, then backspace (find buttons fresh each time)
        var buttons1 = cut.FindAll("button");
        buttons1.First(b => b.TextContent.Trim() == "1").Click();
        
        var buttons2 = cut.FindAll("button");
        buttons2.First(b => b.TextContent.Trim() == "2").Click();
        
        var buttons3 = cut.FindAll("button");
        buttons3.First(b => b.TextContent.Trim() == "3").Click();
        
        var buttons4 = cut.FindAll("button");
        var backspaceButton = buttons4.FirstOrDefault(b => 
        {
            var buttonHtml = b.OuterHtml.ToLower();
            return buttonHtml.Contains("mud-icon") && b.TextContent.Trim() == "";
        });
        Assert.NotNull(backspaceButton);
        backspaceButton.Click();

        // Assert
        var input = cut.Find("input");
        Assert.Equal("12", input.GetAttribute("value"));
    }

    [Fact]
    public void VirtualKeyboard_WithIntegerField_Clear_ResetsValue()
    {
        // Arrange
        var cut = RenderComponent<TestIntegerFieldWithKeyboard>();

        // Act - Click 1, 2, 3, then clear (find buttons fresh each time)
        var buttons1 = cut.FindAll("button");
        buttons1.First(b => b.TextContent.Trim() == "1").Click();
        
        var buttons2 = cut.FindAll("button");
        buttons2.First(b => b.TextContent.Trim() == "2").Click();
        
        var buttons3 = cut.FindAll("button");
        buttons3.First(b => b.TextContent.Trim() == "3").Click();
        
        var buttons4 = cut.FindAll("button");
        var clearButton = buttons4.First(b => b.TextContent.Contains("Clear"));
        clearButton.Click();

        // Assert
        var input = cut.Find("input");
        var value = input.GetAttribute("value");
        Assert.True(string.IsNullOrEmpty(value));
    }

    [Fact]
    public void VirtualKeyboard_WithDecimalField_CustomSeparator_UsesComma()
    {
        // Arrange
        var cut = RenderComponent<TestDecimalFieldWithKeyboardComma>();

        // Act - Click 1, 2, comma, 5 (find buttons fresh each time)
        var buttons1 = cut.FindAll("button");
        buttons1.First(b => b.TextContent.Trim() == "1").Click();
        
        var buttons2 = cut.FindAll("button");
        buttons2.First(b => b.TextContent.Trim() == "2").Click();
        
        var buttons3 = cut.FindAll("button");
        buttons3.First(b => b.TextContent.Contains(",")).Click();
        
        var buttons4 = cut.FindAll("button");
        buttons4.First(b => b.TextContent.Trim() == "5").Click();

        // Assert
        var input = cut.Find("input");
        Assert.Contains(",", input.GetAttribute("value"));
    }
}

/// <summary>
/// Test component with integer field and keyboard
/// </summary>
public class TestIntegerFieldWithKeyboard : ComponentBase
{
    private int? _value;
    private string _currentText = "";

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        var seq = 0;
        
        // Render MudTextFieldInteger
        builder.OpenComponent<MudTextFieldInteger>(seq++);
        builder.AddAttribute(seq++, "Value", _value);
        builder.AddAttribute(seq++, "ValueChanged", EventCallback.Factory.Create<int?>(this, v => _value = v));
        builder.AddAttribute(seq++, "Text", _currentText);
        builder.AddAttribute(seq++, "TextChanged", EventCallback.Factory.Create<string>(this, t => _currentText = t));
        builder.CloseComponent();
        
        // Render MudVirtualKeyboard
        builder.OpenComponent<MudVirtualKeyboard>(seq++);
        builder.AddAttribute(seq++, "ShowDecimalButton", false);
        builder.AddAttribute(seq++, "DigitClicked", EventCallback.Factory.Create<int>(this, digit =>
        {
            _currentText += digit.ToString();
            if (int.TryParse(_currentText, out int result))
            {
                _value = result;
            }
            StateHasChanged();
        }));
        builder.AddAttribute(seq++, "BackspaceClicked", EventCallback.Factory.Create(this, () =>
        {
            if (_currentText.Length > 0)
            {
                _currentText = _currentText.Substring(0, _currentText.Length - 1);
                if (string.IsNullOrEmpty(_currentText))
                {
                    _value = null;
                }
                else if (int.TryParse(_currentText, out int result))
                {
                    _value = result;
                }
            }
            StateHasChanged();
        }));
        builder.AddAttribute(seq++, "ClearClicked", EventCallback.Factory.Create(this, () =>
        {
            _currentText = "";
            _value = null;
            StateHasChanged();
        }));
        builder.CloseComponent();
    }
}

/// <summary>
/// Test component with decimal field and keyboard
/// </summary>
public class TestDecimalFieldWithKeyboard : ComponentBase
{
    private decimal? _value;
    private string _currentText = "";

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        var seq = 0;
        
        // Render MudTextFieldDecimal
        builder.OpenComponent<MudTextFieldDecimal>(seq++);
        builder.AddAttribute(seq++, "Value", _value);
        builder.AddAttribute(seq++, "ValueChanged", EventCallback.Factory.Create<decimal?>(this, v => _value = v));
        builder.AddAttribute(seq++, "Text", _currentText);
        builder.AddAttribute(seq++, "TextChanged", EventCallback.Factory.Create<string>(this, t => _currentText = t));
        builder.AddAttribute(seq++, "DecimalPlaces", 2);
        builder.CloseComponent();
        
        // Render MudVirtualKeyboard
        builder.OpenComponent<MudVirtualKeyboard>(seq++);
        builder.AddAttribute(seq++, "ShowDecimalButton", true);
        builder.AddAttribute(seq++, "DecimalSeparator", ".");
        builder.AddAttribute(seq++, "DigitClicked", EventCallback.Factory.Create<int>(this, digit =>
        {
            _currentText += digit.ToString();
            if (decimal.TryParse(_currentText, out decimal result))
            {
                _value = result;
            }
            StateHasChanged();
        }));
        builder.AddAttribute(seq++, "DecimalClicked", EventCallback.Factory.Create(this, () =>
        {
            if (!_currentText.Contains("."))
            {
                _currentText += ".";
            }
            StateHasChanged();
        }));
        builder.AddAttribute(seq++, "BackspaceClicked", EventCallback.Factory.Create(this, () =>
        {
            if (_currentText.Length > 0)
            {
                _currentText = _currentText.Substring(0, _currentText.Length - 1);
                if (string.IsNullOrEmpty(_currentText))
                {
                    _value = null;
                }
                else if (decimal.TryParse(_currentText, out decimal result))
                {
                    _value = result;
                }
            }
            StateHasChanged();
        }));
        builder.AddAttribute(seq++, "ClearClicked", EventCallback.Factory.Create(this, () =>
        {
            _currentText = "";
            _value = null;
            StateHasChanged();
        }));
        builder.CloseComponent();
    }
}

/// <summary>
/// Test component with decimal field and keyboard using comma separator
/// </summary>
public class TestDecimalFieldWithKeyboardComma : ComponentBase
{
    private decimal? _value;
    private string _currentText = "";

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        var seq = 0;
        
        // Render MudTextFieldDecimal
        builder.OpenComponent<MudTextFieldDecimal>(seq++);
        builder.AddAttribute(seq++, "Value", _value);
        builder.AddAttribute(seq++, "ValueChanged", EventCallback.Factory.Create<decimal?>(this, v => _value = v));
        builder.AddAttribute(seq++, "Text", _currentText);
        builder.AddAttribute(seq++, "TextChanged", EventCallback.Factory.Create<string>(this, t => _currentText = t));
        builder.AddAttribute(seq++, "DecimalPlaces", 2);
        builder.AddAttribute(seq++, "DecimalSeparator", ",");
        builder.CloseComponent();
        
        // Render MudVirtualKeyboard
        builder.OpenComponent<MudVirtualKeyboard>(seq++);
        builder.AddAttribute(seq++, "ShowDecimalButton", true);
        builder.AddAttribute(seq++, "DecimalSeparator", ",");
        builder.AddAttribute(seq++, "DigitClicked", EventCallback.Factory.Create<int>(this, digit =>
        {
            _currentText += digit.ToString();
            if (decimal.TryParse(_currentText.Replace(",", "."), out decimal result))
            {
                _value = result;
            }
            StateHasChanged();
        }));
        builder.AddAttribute(seq++, "DecimalClicked", EventCallback.Factory.Create(this, () =>
        {
            if (!_currentText.Contains(","))
            {
                _currentText += ",";
            }
            StateHasChanged();
        }));
        builder.AddAttribute(seq++, "BackspaceClicked", EventCallback.Factory.Create(this, () =>
        {
            if (_currentText.Length > 0)
            {
                _currentText = _currentText.Substring(0, _currentText.Length - 1);
                if (string.IsNullOrEmpty(_currentText))
                {
                    _value = null;
                }
                else if (decimal.TryParse(_currentText.Replace(",", "."), out decimal result))
                {
                    _value = result;
                }
            }
            StateHasChanged();
        }));
        builder.AddAttribute(seq++, "ClearClicked", EventCallback.Factory.Create(this, () =>
        {
            _currentText = "";
            _value = null;
            StateHasChanged();
        }));
        builder.CloseComponent();
    }
}
