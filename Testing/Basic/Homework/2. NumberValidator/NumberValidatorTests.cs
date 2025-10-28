using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase(0, 2, TestName = "Precision is zero")]
    [TestCase(1, -1, TestName = "Scale is negative")]
    [TestCase(1, 2, TestName = "Precision is less than scale")]
    [TestCase(1, 1, TestName = "Precision equals scale")]
    public void Constructor_ThrowsException_OnInvalidArguments(int precision, int scale)
    {
        Action constructor = () => new NumberValidator(precision, scale);
        constructor.Should().Throw<ArgumentException>();
    }

    [TestCase(1, 0, TestName = "Precision and scale are minimum possible")]
    [TestCase(1000, 500, TestName = "Precision and scale are big enough")]
    public void Constructor_DoesNotThrowException_OnValidArguments(int precision, int scale)
    {
        Action constructor = () => new NumberValidator(precision, scale, true);
        constructor.Should().NotThrow();
    }

    [TestCase("a.sd", TestName = "Input contains letters")]
    [TestCase("", TestName = "Input is empty string")]
    [TestCase(null, TestName = "Input is null")]
    [TestCase("12+3", TestName = "Input contains invalid characters")]
    public void Validator_ReturnsFalse_OnInvalidInputFormats(string input)
    {
        var validator = new NumberValidator(5);
        validator.IsValidNumber(input).Should().BeFalse();
    }

    [TestCase(1, 0, "0", TestName = "One zero")]
    [TestCase(2, 1, "0.0", TestName = "One zero with fraction part")]
    [TestCase(3, 1, "00.0", TestName = "Two leading zeros with fraction part")]
    public void Validator_ReturnsTrue_WithProperZeros(int precision, int scale, string input)
    {
        var validator = new NumberValidator(precision, scale);
        validator.IsValidNumber(input).Should().BeTrue();
    }

    [TestCase(6, 0, "1234", TestName = "Simple integer")]
    [TestCase(20, 10, "123.54", TestName = "Number with fraction part")]
    public void Validator_ReturnsTrue_WithProperNumbers(int precision, int scale, string input)
    {
        var validator = new NumberValidator(precision, scale);
        validator.IsValidNumber(input).Should().BeTrue();
    }

    [TestCase("1234567", TestName = "Precision overflow with numerals")]
    [TestCase("0000000", TestName = "Precision overflow with zeros")]
    public void Validator_ReturnsFalse_WithPrecisionOverflow(string input)
    {
        var validator = new NumberValidator(6);
        validator.IsValidNumber(input).Should().BeFalse();
    }

    [TestCase("1.23456", TestName = "Scale overflow with numerals")]
    [TestCase("0.00000", TestName = "Scale overflow with zeros")]
    public void Validator_ReturnsFalse_WithScaleOverflow(string input)
    {
        var validator = new NumberValidator(10, 4);
        validator.IsValidNumber(input).Should().BeFalse();
    }

    [TestCase("-123", TestName = "Negative integer")]
    [TestCase("-0", TestName = "Negative zero")]
    public void Validator_ReturnsFalse_WithNegativeNumberWhenNotAllowed(string input)
    {
        var validator = new NumberValidator(10, 0, true);
        validator.IsValidNumber(input).Should().BeFalse();
    }

    [TestCase("-1.23", TestName = "Negative float")]
    [TestCase("+1.23", TestName = "Positive float")]
    public void Validator_ReturnsFalse_WithSignPrecisionOverflow(string input)
    {
        var validator = new NumberValidator(3, 2);
        validator.IsValidNumber(input).Should().BeFalse();
    }

    [TestCase]
    public void Validator_ReturnsTrue_WithCommaSeparator()
    {
        var validator = new NumberValidator(5, 2);
        validator.IsValidNumber("12,34").Should().BeTrue();
    }
}