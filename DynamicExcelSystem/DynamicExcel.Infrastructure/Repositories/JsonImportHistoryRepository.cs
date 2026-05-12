using DynamicExcel.Core.Entities;
using DynamicExcel.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DynamicExcel.Infrastructure.Repositories
{
    public class JsonImportHistoryRepository : IImportHistoryRepository
    {
        private readonly string _filePath = "importhistory.json";

        public JsonImportHistoryRepository()
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        private List<ImportHistory> LoadData()
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<ImportHistory>>(json) ?? new List<ImportHistory>();
            }
            catch
            {
                return new List<ImportHistory>();
            }
        }

        private void SaveData(List<ImportHistory> data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void Add(ImportHistory history)
        {
            var data = LoadData();
            history.Id = data.Any() ? data.Max(x => x.Id) + 1 : 1;
            data.Add(history);
            SaveData(data);
        }

        public void Delete(int id)
        {
            var data = LoadData();
            var item = data.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                data.Remove(item);
                SaveData(data);
            }
        }

        public IEnumerable<ImportHistory> GetAll()
        {
            return LoadData().OrderByDescending(x => x.ImportDate);
        }
    }
}
