using NUnit.Framework;
using FluentAssertions;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase(0, 2, "positive", TestName = "Precision is zero")]
    [TestCase(1, -1, "non-negative", TestName = "Scale is negative")]
    [TestCase(1, 2, "less or equal", TestName = "Precision is less than scale")]
    [TestCase(1, 1, "less or equal", TestName = "Precision equals scale")]
    public void Constructor_ThrowsException_OnInvalidArguments_Test(int precision, int scale, string expectedMessage)
    {
        var constructor = () => new NumberValidator(precision, scale);
        constructor.Should().Throw<ArgumentException>().WithMessage($"*{expectedMessage}*");
    }

    [TestCase(1, 0, TestName = "Precision and scale are minimum possible")]
    [TestCase(2, 1, TestName = "Highest valid scale below precision")]
    public void Constructor_DoesNotThrowException_OnBoundaryValidArguments_Test(int precision, int scale)
    {
        var constructor = () => new NumberValidator(precision, scale, true);
        constructor.Should().NotThrow();
    }

    [TestCase("a.sd", TestName = "Input contains letters")]
    [TestCase("", TestName = "Input is empty string")]
    [TestCase(null, TestName = "Input is null")]
    [TestCase("12+3", TestName = "Input contains invalid characters")]
    public void Validator_ReturnsFalse_OnInvalidInputFormats_Test(string input)
    {
        var validator = new NumberValidator(5);
        validator.IsValidNumber(input).Should().BeFalse();
    }

    [TestCase(1, 0, "0", TestName = "One zero")]
    [TestCase(2, 1, "0.0", TestName = "One zero with fraction part")]
    [TestCase(3, 1, "00.0", TestName = "Two leading zeros with fraction part")]
    public void Validator_ReturnsTrue_WithProperZeros_Test(int precision, int scale, string input)
    {
        var validator = new NumberValidator(precision, scale);
        validator.IsValidNumber(input).Should().BeTrue();
    }

    [TestCase(4, 0, "1234", TestName = "Simple integer")]
    [TestCase(5, 2, "123.54", TestName = "Number with fraction part")]
    public void Validator_ReturnsTrue_WithProperNumbers_Test(int precision, int scale, string input)
    {
        var validator = new NumberValidator(precision, scale);
        validator.IsValidNumber(input).Should().BeTrue();
    }

    [TestCase("123", TestName = "Precision overflow with numerals")]
    [TestCase("000", TestName = "Precision overflow with zeros")]
    public void Validator_ReturnsFalse_WithPrecisionOverflow_Test(string input)
    {
        var validator = new NumberValidator(2);
        validator.IsValidNumber(input).Should().BeFalse();
    }

    [TestCase("1.23456", TestName = "Scale overflow with numerals")]
    [TestCase("0.00000", TestName = "Scale overflow with zeros")]
    public void Validator_ReturnsFalse_WithScaleOverflow_Test(string input)
    {
        var validator = new NumberValidator(10, 4);
        validator.IsValidNumber(input).Should().BeFalse();
    }

    [TestCase("-123", TestName = "Negative integer")]
    [TestCase("-0", TestName = "Negative zero")]
    public void Validator_ReturnsFalse_WithNegativeNumberWhenNotAllowed_Test(string input)
    {
        var validator = new NumberValidator(10, 0, true);
        validator.IsValidNumber(input).Should().BeFalse();
    }

    [TestCase("-1.23", TestName = "Negative float")]
    [TestCase("+1.23", TestName = "Positive float")]
    public void Validator_ReturnsFalse_WithSignPrecisionOverflow_Test(string input)
    {
        var validator = new NumberValidator(3, 2);
        validator.IsValidNumber(input).Should().BeFalse();
    }

    [TestCase("12,34", TestName = "Comma separator")]
    [TestCase("12.34", TestName = "Dot separator")]
    public void Validator_ReturnsTrue_WithCommaSeparator_Test(string input)
    {
        var validator = new NumberValidator(5, 2);
        validator.IsValidNumber(input).Should().BeTrue();
    }
}