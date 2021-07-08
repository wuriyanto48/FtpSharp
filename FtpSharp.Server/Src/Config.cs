using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FtpSharp.Server
{
    public class Config
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("rootDir")]
        public string RootDir { get; set; }

        public static Config FromFile(string fileName)
        {
            try
            {
                string configContent = File.ReadAllText(fileName);
                Config config = JsonSerializer.Deserialize<Config>(configContent);
                return config;
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

}