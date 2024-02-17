namespace Ingression;

public static class MathExtend
{
    public static Int32 Decode(string input, int fromBase)
    {
        var CharList = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().ToList();
        var reversed = input.ToUpper().Reverse();
        int result = 0;
        int pos = 0;
        foreach (char c in reversed)
        {
            result += CharList.IndexOf(c) * (int)Math.Pow(fromBase, pos);
            pos++;
        }
        return result;
    }

}