public class SingletonMgr <T> where T : class, new()
{
    protected static object _instanceLock = new object();
    protected static volatile T _instance;
    public static T GetInstance
    {
        get
        {
            lock (_instanceLock)
            {
                if (_instance == null)
                    _instance = new T();
            }
            return _instance;
        }
    }
}