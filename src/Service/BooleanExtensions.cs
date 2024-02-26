namespace Service;

internal static class BooleanExtensions
{
    public static void Match(this bool value, Action onTrue, Action onFalse)
    {
        if (value)
        {
            onTrue();
        }
        else
        {
            onFalse();
        }
    }
}