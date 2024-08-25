namespace webauthdemo.unggoy;

/// <summary>
/// Attribute that sets the Unggoy action name for authorization with the 'Unggoy' policy.
/// </summary>
/// <param name="name">Name of the action used by the authorization handler (<see cref="UnggoyAuthorizationHandler"/>)
/// to authorize the received call.</param>
public class UnggoyActionAttribute(string name) : Attribute
{
    public string Name { get; set; } = name;
}