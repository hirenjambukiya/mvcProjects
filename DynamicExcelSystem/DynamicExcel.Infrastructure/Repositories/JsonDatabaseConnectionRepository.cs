using DynamicExcel.Core.Entities;
using DynamicExcel.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DynamicExcel.Infrastructure.Repositories
{
    public class JsonDatabaseConnectionRepository : IDatabaseConnectionRepository
    {
        private readonly string _filePath = "connections.json";

        public JsonDatabaseConnectionRepository()
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        private List<DatabaseConnection> LoadData()
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<DatabaseConnection>>(json) ?? new List<DatabaseConnection>();
        }

        private void SaveData(List<DatabaseConnection> data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public int Add(DatabaseConnection connection)
        {
            var data = LoadData();
            connection.Id = data.Any() ? data.Max(x => x.Id) + 1 : 1;
            connection.CreatedDate = System.DateTime.Now;
            
            if (!data.Any() || connection.IsDefault)
            {
                data.ForEach(x => x.IsDefault = false);
                connection.IsDefault = true;
            }

            data.Add(connection);
            SaveData(data);
            return connection.Id;
        }

        public void Delete(int id)
        {
            var data = LoadData();
            var item = data.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                data.Remove(item);
                if (item.IsDefault && data.Any())
                {
                    data.First().IsDefault = true;
                }
                SaveData(data);
            }
        }

        public IEnumerable<DatabaseConnection> GetAll()
        {
            return LoadData().OrderByDescending(x => x.CreatedDate);
        }

        public DatabaseConnection GetById(int id)
        {
            return LoadData().FirstOrDefault(x => x.Id == id);
        }

        public DatabaseConnection GetDefaultConnection()
        {
            var data = LoadData();
            return data.FirstOrDefault(x => x.IsDefault) ?? data.FirstOrDefault();
        }

        public void SetDefaultConnection(int id)
        {
            var data = LoadData();
            foreach (var item in data)
            {
                item.IsDefault = (item.Id == id);
            }
            SaveData(data);
        }

        public void Update(DatabaseConnection connection)
        {
            var data = LoadData();
            var index = data.FindIndex(x => x.Id == connection.Id);
            if (index != -1)
            {
                var existing = data[index];
                connection.CreatedDate = existing.CreatedDate; // Preserve
                if (connection.IsDefault)
                {
                    data.ForEach(x => x.IsDefault = false);
                }
                data[index] = connection;
                SaveData(data);
            }
        }
    }
}
