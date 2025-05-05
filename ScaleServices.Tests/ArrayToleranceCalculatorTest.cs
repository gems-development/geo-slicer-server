using NetTopologySuite.Geometries;
using Services.ScaleCalculator;
using Services.ScaleCalculator.Interfaces;
using Xunit;

namespace ScaleServices.Tests;

public class ArrayToleranceCalculatorTest
{
    private static readonly ArrayToleranceCalculator ToleranceCalculator = new ArrayToleranceCalculator();
    
    [Theory]
    [InlineData(50, 50.04, 0)]
    [InlineData(50, 50.06, 4E-5)]
    [InlineData(50, 50.12, 7.27E-5)]
    [InlineData(50, 50.18, 7.27E-5)]
    [InlineData(50, 50.22, 2E-4)]
    [InlineData(50, 50.38, 2E-4)]
    [InlineData(50, 50.42, 5E-4)]
    [InlineData(50, 50.77, 5E-4)]
    [InlineData(50, 50.85, 5.33E-4)]
    [InlineData(45, 55.06, 5.33E-4)]
    public void Test(double left, double right, double expected)
    {
        double actual = ToleranceCalculator.CalculateTolerance(new Point(left, 0), new Point(right, 0));
        Assert.Equal(expected, actual);
    }
}