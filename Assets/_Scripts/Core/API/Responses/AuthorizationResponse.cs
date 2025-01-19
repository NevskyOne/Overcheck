public class AuthorizationResponse
{
    public readonly string Name;
    public readonly bool WasRegistered;

    public AuthorizationResponse(string name, bool wasRegistered)
    {
        Name = name;
        WasRegistered = wasRegistered;
    }
}