using System;
using Xunit;

namespace SharpFileSystem.Tests
{
    public static class EAssert
    {
        public static void Throws<T>(Action a)
            where T : Exception
        {
            try
            {
                a();
            }
            catch (T)
            {
                return;
            }

            Assert.True(false, string.Format("The exception '{0}' was not thrown.", typeof(T).FullName));
        }
    }
}