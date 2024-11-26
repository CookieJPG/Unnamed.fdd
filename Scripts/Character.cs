public class Character
{
    public string identity;
    public int index { get; set; }

    public Character(string identity, int index)
    {
        this.identity = identity;
        this.index = index;
    }
}
