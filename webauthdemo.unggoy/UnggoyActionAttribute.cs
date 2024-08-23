namespace webauthdemo.unggoy;

public class UnggoyActionAttribute(string? name = null) : Attribute
{
    public string? Name { get; set; } = name;
}