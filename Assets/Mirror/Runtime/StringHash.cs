namespace Mirror
{
    public static class StringHash
    {
        // string.GetHashCode is not guaranteed to be the same on all machines, but
        // we need one that is the same on all machines. simple and stupid:
        public static int GetStableHashCode(this string text)
        {
            unchecked
            {
                var hash = 23;
                foreach (var c in text)
                    hash = hash * 31 + c;
                return hash;
            }
        }
    }
}