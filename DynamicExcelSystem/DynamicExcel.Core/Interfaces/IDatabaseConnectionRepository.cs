using DynamicExcel.Core.Entities;
using System.Collections.Generic;

namespace DynamicExcel.Core.Interfaces
{
    public interface IDatabaseConnectionRepository
    {
        IEnumerable<DatabaseConnection> GetAll();
        DatabaseConnection GetById(int id);
        int Add(DatabaseConnection connection);
        void Update(DatabaseConnection connection);
        void Delete(int id);
        DatabaseConnection GetDefaultConnection();
        void SetDefaultConnection(int id);
    }
}
