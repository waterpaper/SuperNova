using Supernova.Unity;

public class CallBacks
{
    static CallBacks instance;
    public static CallBacks Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new CallBacks();
            }

            return instance;
        }
    }

    public CallBacks()
    {

    }

    public System.Action<Character, Enemy> beforeAttack;
    public System.Action<Character, Enemy> afterAttack;
    public System.Action<Character, Enemy> monsterDie;
    public System.Action<Character, Enemy> earnGold;
}
