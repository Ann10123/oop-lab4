using System;
using System.Numerics;
using System.Collections.Generic;

public interface IMyNumber<T> where T : IMyNumber<T>
{
    T Add(T b);
    T Subtract(T b);
    T Multiply(T b);
    T Divide(T b);
}
public class MyFrac : IMyNumber<MyFrac>, IComparable<MyFrac>
{
    public BigInteger numerator {get;}
    public BigInteger denominator {get;}
    public MyFrac(BigInteger numerator, BigInteger denominator)
    {
        if (denominator == 0)
            throw new DivideByZeroException("Denominator cannot be zero.");
        this.numerator = numerator;
        this.denominator = denominator;
        var gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);
        numerator = numerator / gcd;
        denominator = denominator / gcd;
        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }
    }
    public MyFrac(int numerator, int denominator)
        : this((BigInteger)numerator, (BigInteger)denominator) { }

    public MyFrac Add(MyFrac b)
    {
        return new MyFrac(numerator * b.denominator + b.numerator * denominator,
            denominator * b.denominator);
    }
    public MyFrac Subtract(MyFrac b)
    {
        return new MyFrac(numerator * b.denominator - b.numerator * denominator,
            denominator * b.denominator);
    }
    public MyFrac Multiply(MyFrac b)
    {
        return new MyFrac(numerator * b.numerator, denominator * b.denominator);
    }
    public MyFrac Divide(MyFrac b)
    {
        if (b.numerator == 0)
        {
            throw new DivideByZeroException("Cannot divide by zero.");
        }
        return new MyFrac(numerator * b.denominator, denominator * b.numerator);
    }
    public override string ToString() => $"{numerator}/{denominator}";
    public int CompareTo(MyFrac other)
    {
        BigInteger left = numerator * other.denominator;
        BigInteger right = other.numerator * denominator;
        return left.CompareTo(right);
    }
}
public class MyComplex : IMyNumber<MyComplex>
{
    public double real { get; }
    public double imaginary { get; }
    public MyComplex(double real, double imaginary)
    {
        this.real = real;
        this.imaginary = imaginary;
    }
    public MyComplex Add(MyComplex b)
    {
        return new MyComplex(real + b.real, imaginary + b.imaginary);
    }
    public MyComplex Subtract(MyComplex b)
    {
        return new MyComplex(real - b.real, imaginary - b.imaginary);
    }
    public MyComplex Multiply(MyComplex b)
    {
        return new MyComplex(real * b.real - imaginary * b.imaginary,
            real * b.imaginary + imaginary * b.real);
    }
    public MyComplex Divide(MyComplex b)
    {
        double denominator = b.real * b.real + b.imaginary * b.imaginary;
        if (denominator == 0)
        {
            throw new DivideByZeroException("Cannot divide by zero.");
        }
        return new MyComplex((real * b.real + imaginary * b.imaginary) / denominator,
            (imaginary * b.real - real * b.imaginary) / denominator);
    }
    public override string ToString() => $"{real} + {imaginary}i";
}
class Program
{
    static void testAPlusBSquare<T>(T a, T b) where T : IMyNumber<T>
    {
        Console.WriteLine("=== Starting testing (a+b)^2=a^2+2ab+b^2 with a = " + a + ", b = " + b + " ===");
        T aPlusB = a.Add(b);
        Console.WriteLine("a = " + a);
        Console.WriteLine("b = " + b);
        Console.WriteLine("(a + b) = " + aPlusB);
        Console.WriteLine("(a+b)^2 = " + aPlusB.Multiply(aPlusB));
        Console.WriteLine(" = = = ");
        T curr = a.Multiply(a);
        Console.WriteLine("a^2 = " + curr);
        T wholeRightPart = curr;
        curr = a.Multiply(b);
        curr = curr.Add(curr);
        Console.WriteLine("2*a*b = " + curr);
        wholeRightPart = wholeRightPart.Add(curr);
        curr = b.Multiply(b);
        Console.WriteLine("b^2 = " + curr);
        wholeRightPart = wholeRightPart.Add(curr);
        Console.WriteLine("a^2+2ab+b^2 = " + wholeRightPart);
        Console.WriteLine("=== Finishing testing (a+b)^2=a^2+2ab+b^2 with a = " + a + ", b = " + b + " ===");
    }
    static void testSquaresDifference<T>(T a, T b) where T : IMyNumber<T>
    {
        Console.WriteLine("=== Starting testing (a-b) and (a^2 - b^2)/(a+b) with a = " + a + ", b = " + b + " ===");
        T aMinusB = a.Subtract(b);
        Console.WriteLine("a = " + a);
        Console.WriteLine("b = " + b);
        Console.WriteLine("(a - b) = " + aMinusB);
        T aSquared = a.Multiply(a);
        T bSquared = b.Multiply(b);
        T differenceOfSquares = aSquared.Subtract(bSquared);
        Console.WriteLine("a^2 = " + aSquared);
        Console.WriteLine("b^2 = " + bSquared);
        Console.WriteLine("(a^2 - b^2) = " + differenceOfSquares);
        try
        {
            T aPlusB = a.Add(b);
            Console.WriteLine("(a + b) = " + aPlusB);
            T divisionResult = differenceOfSquares.Divide(aPlusB);
            Console.WriteLine("(a^2 - b^2) / (a + b) = " + divisionResult);
        }
        catch (DivideByZeroException)
        {
            Console.WriteLine("Division by zero occurred when calculating (a^2 - b^2) / (a + b)");
        }
        Console.WriteLine("=== Finishing testing (a-b) and (a^2 - b^2)/(a+b) with a = " + a + ", b = " + b + " ===");
    }
    static void Main(string[] args)
    {
        testAPlusBSquare(new MyFrac(1, 3), new MyFrac(1, 6));
        testAPlusBSquare(new MyComplex(1, 3), new MyComplex(1, 6));
        testSquaresDifference(new MyFrac(1, 3), new MyFrac(1, 6));
        testSquaresDifference(new MyComplex(1, 3), new MyComplex(1, 6));
        Console.ReadKey();
    }
}
