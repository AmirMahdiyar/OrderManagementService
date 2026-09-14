namespace OrderManagementService.Domain.Services
{
    public interface IPasswordHasher
    {
        string Hash(string password);
    }
}
