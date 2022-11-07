namespace BadScript2.VirtualMachine.Managing;

public class BadVirtualMachineUser
{
    public static readonly BadVirtualMachineUser Anonymous = new BadVirtualMachineUser("anon");

    public BadVirtualMachineUser(string name, string password = "")
    {
        Name = name;
        Password = password;
    }

    public string Name { get; set; }
    public string Password { get; set; }
}