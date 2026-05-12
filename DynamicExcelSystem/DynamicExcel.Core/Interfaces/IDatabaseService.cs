namespace DynamicExcel.Core.Interfaces
{
    public interface IDatabaseService
    {
        bool TestConnection(string connectionString, out string errorMessage);
    }
}
