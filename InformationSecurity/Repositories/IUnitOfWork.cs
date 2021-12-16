namespace InformationSecurity.Repositories
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
