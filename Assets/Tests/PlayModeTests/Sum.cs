using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Sum
{
    // Method to sum two integers
    public int Add(int a, int b)
    {
        return a + b;
    }

    // A Test behaves as an ordinary method
    [Test]
    public void SumSimplePasses()
    {
        // Arrange
        Sum sum = new Sum();
        int expected = 5;

        // Act
        int result = sum.Add(2, 3);

        // Assert
        Assert.AreEqual(expected, result);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator SumWithEnumeratorPasses()
    {
        // Arrange
        Sum sum = new Sum();
        int expected = 10;

        // Act
        int result = sum.Add(4, 6);

        // Use yield to skip a frame.
        yield return null;

        // Assert
        Assert.AreEqual(expected, result);
    }
}