#define ENVIRONMENT_STAGING

public static class Constants
{
    public static string GetItemIconResPath(long itemID) => string.Format("Assets/res/img/Equipment/{0}.png", itemID);
}
