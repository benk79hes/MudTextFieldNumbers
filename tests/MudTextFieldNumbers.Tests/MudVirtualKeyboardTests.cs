using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using Xunit;

namespace MudTextFieldNumbers.Tests;

/// <summary>
/// Tests for the MudVirtualKeyboard component
/// </summary>
public class MudVirtualKeyboardTests : TestContext
{
    public MudVirtualKeyboardTests()
    {
        // Register MudBlazor services
        Services.AddMudServices();
    }

    [Fact]
    public void MudVirtualKeyboard_Renders_Successfully()
    {
        // Act
        var cut = RenderComponent<MudVirtualKeyboard>();

        // Assert
        Assert.NotNull(cut);
        var container = cut.Find(".mud-virtual-keyboard");
        Assert.NotNull(container);
    }

    [Fact]
    public void MudVirtualKeyboard_Renders_WithCustomTitle()
    {
        // Arrange
        var title = "Custom Keyboard";

        // Act
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.Title, title));

        // Assert
        var titleElement = cut.Find(".mud-typography");
        Assert.Contains(title, titleElement.TextContent);
    }

    [Fact]
    public void MudVirtualKeyboard_Renders_AllDigitButtons()
    {
        // Act
        var cut = RenderComponent<MudVirtualKeyboard>();

        // Assert - Check for buttons 0-9
        for (int i = 0; i <= 9; i++)
        {
            var buttons = cut.FindAll("button");
            Assert.Contains(buttons, b => b.TextContent.Trim() == i.ToString());
        }
    }

    [Fact]
    public void MudVirtualKeyboard_Renders_DecimalButton_WhenShowDecimalButtonIsTrue()
    {
        // Act
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.ShowDecimalButton, true)
            .Add(p => p.DecimalSeparator, "."));

        // Assert
        var buttons = cut.FindAll("button");
        Assert.Contains(buttons, b => b.TextContent.Contains("."));
    }

    [Fact]
    public void MudVirtualKeyboard_DoesNotRenderDecimalButton_WhenShowDecimalButtonIsFalse()
    {
        // Act
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.ShowDecimalButton, false));

        // Assert
        var buttons = cut.FindAll("button");
        var decimalButton = buttons.FirstOrDefault(b => b.TextContent.Contains("."));
        Assert.Null(decimalButton);
    }

    [Fact]
    public void MudVirtualKeyboard_Renders_CustomDecimalSeparator()
    {
        // Arrange
        var separator = ",";

        // Act
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.ShowDecimalButton, true)
            .Add(p => p.DecimalSeparator, separator));

        // Assert
        var buttons = cut.FindAll("button");
        Assert.Contains(buttons, b => b.TextContent.Contains(separator));
    }

    [Fact]
    public void MudVirtualKeyboard_Renders_BackspaceButton()
    {
        // Act
        var cut = RenderComponent<MudVirtualKeyboard>();

        // Assert - Check that there's a button with MudIcon component (backspace is rendered as icon)
        var mudIcons = cut.FindAll(".mud-icon-root");
        Assert.True(mudIcons.Count > 0, "Should have at least one icon (backspace button)");
    }

    [Fact]
    public void MudVirtualKeyboard_Renders_ClearButton()
    {
        // Act
        var cut = RenderComponent<MudVirtualKeyboard>();

        // Assert
        var buttons = cut.FindAll("button");
        Assert.Contains(buttons, b => b.TextContent.Contains("Clear"));
    }

    [Fact]
    public void MudVirtualKeyboard_DigitClicked_EventFires_WhenDigitButtonClicked()
    {
        // Arrange
        int? clickedDigit = null;
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.DigitClicked, (int digit) => clickedDigit = digit));

        // Act - Find and click the "5" button
        var buttons = cut.FindAll("button");
        var fiveButton = buttons.First(b => b.TextContent.Trim() == "5");
        fiveButton.Click();

        // Assert
        Assert.Equal(5, clickedDigit);
    }

    [Fact]
    public void MudVirtualKeyboard_DigitClicked_EventFires_ForAllDigits()
    {
        // Test all digits 0-9
        for (int expectedDigit = 0; expectedDigit <= 9; expectedDigit++)
        {
            // Arrange
            int? clickedDigit = null;
            var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
                .Add(p => p.DigitClicked, (int digit) => clickedDigit = digit));

            // Act
            var buttons = cut.FindAll("button");
            var digitButton = buttons.First(b => b.TextContent.Trim() == expectedDigit.ToString());
            digitButton.Click();

            // Assert
            Assert.Equal(expectedDigit, clickedDigit);
        }
    }

    [Fact]
    public void MudVirtualKeyboard_DecimalClicked_EventFires_WhenDecimalButtonClicked()
    {
        // Arrange
        bool decimalClicked = false;
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.ShowDecimalButton, true)
            .Add(p => p.DecimalSeparator, ".")
            .Add(p => p.DecimalClicked, () => decimalClicked = true));

        // Act
        var buttons = cut.FindAll("button");
        var decimalButton = buttons.First(b => b.TextContent.Contains("."));
        decimalButton.Click();

        // Assert
        Assert.True(decimalClicked);
    }

    [Fact]
    public void MudVirtualKeyboard_BackspaceClicked_EventFires_WhenBackspaceButtonClicked()
    {
        // Arrange
        bool backspaceClicked = false;
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.ShowNegativeButton, false)  // Disable negative button to keep backspace in its original position
            .Add(p => p.BackspaceClicked, () => backspaceClicked = true));

        // Act - Find backspace button by checking for button with icon
        var allButtons = cut.FindAll("button");
        var backspaceButton = allButtons.FirstOrDefault(b => 
        {
            var buttonHtml = b.OuterHtml.ToLower();
            // Check if button has an icon child element (backspace icon)
            return buttonHtml.Contains("mud-icon");
        });
        
        Assert.NotNull(backspaceButton);
        backspaceButton.Click();

        // Assert
        Assert.True(backspaceClicked);
    }

    [Fact]
    public void MudVirtualKeyboard_ClearClicked_EventFires_WhenClearButtonClicked()
    {
        // Arrange
        bool clearClicked = false;
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.ClearClicked, () => clearClicked = true));

        // Act
        var buttons = cut.FindAll("button");
        var clearButton = buttons.First(b => b.TextContent.Contains("Clear"));
        clearButton.Click();

        // Assert
        Assert.True(clearClicked);
    }

    [Fact]
    public void MudVirtualKeyboard_AppliesCustomClass()
    {
        // Arrange
        var customClass = "my-custom-class";

        // Act
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.Class, customClass));

        // Assert
        var container = cut.Find(".mud-virtual-keyboard");
        Assert.Contains(customClass, container.ClassName);
    }

    [Fact]
    public void MudVirtualKeyboard_AppliesCustomStyle()
    {
        // Arrange
        var customStyle = "width: 300px;";

        // Act
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.Style, customStyle));

        // Assert
        var container = cut.Find(".mud-virtual-keyboard");
        Assert.Contains(customStyle, container.GetAttribute("style"));
    }

    [Fact]
    public void MudVirtualKeyboard_IntegerMode_DoesNotShowDecimalButton()
    {
        // Act - Simulate integer mode
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.ShowDecimalButton, false));

        // Assert
        var buttons = cut.FindAll("button");
        var decimalButton = buttons.FirstOrDefault(b => b.TextContent.Contains(".") || b.TextContent.Contains(","));
        Assert.Null(decimalButton);
    }

    [Fact]
    public void MudVirtualKeyboard_DecimalMode_ShowsDecimalButton()
    {
        // Act - Simulate decimal mode
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.ShowDecimalButton, true)
            .Add(p => p.DecimalSeparator, "."));

        // Assert
        var buttons = cut.FindAll("button");
        var decimalButton = buttons.FirstOrDefault(b => b.TextContent.Contains("."));
        Assert.NotNull(decimalButton);
    }

    [Fact]
    public void MudVirtualKeyboard_Renders_NegativeButton_WhenShowNegativeButtonIsTrue()
    {
        // Act
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.ShowDecimalButton, false)
            .Add(p => p.ShowNegativeButton, true));

        // Assert
        var buttons = cut.FindAll("button");
        Assert.Contains(buttons, b => b.TextContent.Contains("+/−") || b.TextContent.Contains("+/-"));
    }

    [Fact]
    public void MudVirtualKeyboard_NegativeClicked_EventFires_WhenNegativeButtonClicked()
    {
        // Arrange
        bool negativeClicked = false;
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.ShowDecimalButton, false)
            .Add(p => p.ShowNegativeButton, true)
            .Add(p => p.NegativeClicked, () => negativeClicked = true));

        // Act
        var buttons = cut.FindAll("button");
        var negativeButton = buttons.First(b => b.TextContent.Contains("+/−") || b.TextContent.Contains("+/-"));
        negativeButton.Click();

        // Assert
        Assert.True(negativeClicked);
    }

    [Fact]
    public void MudVirtualKeyboard_ShowsNegativeAndDecimal_WhenBothEnabled()
    {
        // Act - When both ShowDecimalButton and ShowNegativeButton are true
        var cut = RenderComponent<MudVirtualKeyboard>(parameters => parameters
            .Add(p => p.ShowDecimalButton, true)
            .Add(p => p.ShowNegativeButton, true)
            .Add(p => p.DecimalSeparator, "."));

        // Assert - Should show both buttons (negative in its own row)
        var buttons = cut.FindAll("button");
        var decimalButton = buttons.FirstOrDefault(b => b.TextContent.Contains("."));
        var negativeButton = buttons.FirstOrDefault(b => b.TextContent.Contains("+/−") || b.TextContent.Contains("+/-"));
        
        Assert.NotNull(decimalButton);
        Assert.NotNull(negativeButton);
    }
}
