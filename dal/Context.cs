using chess.api.models;
using System.Text.Json;

namespace chess.api.dal
{
    public class Context
    {
        private readonly string _dataDirectory;

        public Context(string folder)
        {
            _dataDirectory = folder;
        }

        public void Save(Study study)
        {
            try
            {
                var filePath = Path.Join(_dataDirectory, MakeStudyFileName(study));
                File.Delete(filePath);

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    var json = JsonSerializer.Serialize(study);
                    writer.Write(json);
                }
            }
            catch ( Exception ex ) { }
        }

        public IList<Study> LoadAll()
        {
            IList<Study> studies = new List<Study>();

            foreach (string path in Directory.GetFiles(_dataDirectory, "*.json"))
            {
                var json = File.ReadAllText(path);
                studies.Add(JsonSerializer.Deserialize<Study>(json));
            }

            return studies;
        }

        public Study Load(Guid studyId)
        {
            try
            {
                var filePath = Path.Join(_dataDirectory, $"{studyId}.json");
                var json = File.ReadAllText(filePath);

                return JsonSerializer.Deserialize<Study>(json);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string MakeStudyFileName(Study study)
        {
            return $"{study.Id}.json";
        }
    }
}
