namespace EasyRoute
{
    public static class Utils
    {
        public static bool IsValidIdentifier(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }
            if (char.IsDigit(id[0]))
            {
                return false;
            }
            foreach (var c in id)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                {
                    return false;
                }
            }
            return true;
        }
    }
}
