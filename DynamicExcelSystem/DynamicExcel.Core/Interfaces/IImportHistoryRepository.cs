using DynamicExcel.Core.Entities;
using System.Collections.Generic;

namespace DynamicExcel.Core.Interfaces
{
    public interface IImportHistoryRepository
    {
        IEnumerable<ImportHistory> GetAll();
        void Add(ImportHistory history);
        void Delete(int id);
    }
}
