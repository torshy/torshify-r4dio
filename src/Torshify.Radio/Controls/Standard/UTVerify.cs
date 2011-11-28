namespace Standard
{
    using System;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    
    /// <summary>
    /// A UnitTest class to supplement Visual Studio's Assert facilities.
    /// </summary>
    /// <remarks>
    /// As this becomes more complete it may replace some uses of the VS Assert class.  Some
    /// aspects of the VS test framework, such as ExpectedExceptionAttribute, have deep limitations
    /// that this tries to address.  So this class can be used as a drop-in replacement for
    /// Assert it generally throws AssertFailedExceptions which are understood by the MSTest harness.
    /// </remarks>
    internal static class UTVerify
    {
        public delegate void ExceptionableAction();

        public static void ExpectException<TException>(ExceptionableAction action) where TException : Exception
        {
            ExpectException<TException>(action, true);
        }

        public static void ExpectException<TException>(ExceptionableAction action, bool supportSubclasses) where TException : Exception
        {
            // Throw the ArgumentException if action is null.  Don't want this to get caught in our try block.
            try
            {
                action();
            }
            catch (TException e)
            {
                // If the caller specified that they want exactly the TException type thrown then don't accept derived exceptions.
                if (!supportSubclasses && (e.GetType() != typeof(TException)))
                {
                    throw;
                }
                // Caught the expected exception type.
                // If code past the catch block gets executed then the action didn't throw.
                return;
            }
            throw new Exception(
                string.Format("Expected an exception of type {0}{1} to be thrown but the operation completed without raising one.",
                    typeof(TException).ToString(),
                    supportSubclasses ? " (or a derived exception type)" : ""));
        }

        public static void ExpectComException(ExceptionableAction action, params HRESULT[] expectedErrorCodes)
        {
            // Throw the ArgumentException if action is null.  Don't want this to get caught in our try block.
            Verify.IsNotNull(action, "action");
            foreach (HRESULT expectedErrorCode in expectedErrorCodes)
            {
                Assert.IsFalse(expectedErrorCode.Succeeded);
            }

            try
            {
                action();
            }
            catch (COMException e)
            {
                // Only catch this if it maps to the expected HRESULT.
                foreach (HRESULT expectedErrorCode in expectedErrorCodes)
                {
                    if (expectedErrorCode.Equals(e))
                    {
                        return;
                    }
                }

                // Caught the expected exception type.
                // If code past the catch block gets executed then the action didn't throw.
                throw;
            }
            throw new Exception("Expected a COMException to be thrown but the operation completed without raising one.");
        }

        public static void CollectionsAreEqual<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            IEnumerator<T> firstEnumerator = first.GetEnumerator();
            IEnumerator<T> secondEnumerator = second.GetEnumerator();

            for (int i = 0; true; ++i)
            {
                bool hasNextFirst = firstEnumerator.MoveNext();
                bool hasNextSecond = secondEnumerator.MoveNext();

                if (hasNextFirst != hasNextSecond)
                {
                    throw new Exception("The two collections are of different sizes.");
                }

                if (!hasNextFirst)
                {
                    return;
                }

                Assert.AreEqual(firstEnumerator.Current, secondEnumerator.Current);
            }
        }

        public static void AreReferenceEqual(object first, object second)
        {
            if (!object.ReferenceEquals(first, second))
            {
                throw new Exception("The objects were expected to be the same reference.");
            }
        }
    }
}
