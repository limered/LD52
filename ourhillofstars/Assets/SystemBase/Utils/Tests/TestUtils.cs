using System.Collections;

namespace SystemBase.Utils.Tests
{
    public class TestUtils
    {
        public static IEnumerator SkipFrames(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return null;
            }
        }
    }
}