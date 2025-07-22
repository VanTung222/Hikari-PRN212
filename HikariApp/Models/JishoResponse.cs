using System.Collections.Generic;
using Newtonsoft.Json;

namespace HikariApp.Models
{
    public class JishoResponse
    {
        [JsonProperty("meta")]
        public JishoMeta? Meta { get; set; }

        [JsonProperty("data")]
        public List<JishoData>? Data { get; set; }
    }

    public class JishoMeta
    {
        [JsonProperty("status")]
        public int Status { get; set; }
    }

    public class JishoData
    {
        [JsonProperty("slug")]
        public string? Slug { get; set; }

        [JsonProperty("is_common")]
        public bool IsCommon { get; set; }

        [JsonProperty("tags")]
        public List<string>? Tags { get; set; }

        [JsonProperty("jlpt")]
        public List<string>? Jlpt { get; set; }

        [JsonProperty("japanese")]
        public List<JapaneseWord>? Japanese { get; set; }

        [JsonProperty("senses")]
        public List<Sense>? Senses { get; set; }

        [JsonProperty("attribution")]
        public Attribution? Attribution { get; set; }
    }

    public class JapaneseWord
    {
        [JsonProperty("word")]
        public string? Word { get; set; }

        [JsonProperty("reading")]
        public string? Reading { get; set; }
    }

    public class Sense
    {
        [JsonProperty("english_definitions")]
        public List<string>? EnglishDefinitions { get; set; }

        [JsonProperty("parts_of_speech")]
        public List<string>? PartsOfSpeech { get; set; }

        [JsonProperty("links")]
        public List<Link>? Links { get; set; }

        [JsonProperty("tags")]
        public List<string>? Tags { get; set; }

        [JsonProperty("restrictions")]
        public List<string>? Restrictions { get; set; }

        [JsonProperty("see_also")]
        public List<string>? SeeAlso { get; set; }

        [JsonProperty("antonyms")]
        public List<string>? Antonyms { get; set; }

        [JsonProperty("source")]
        public List<Source>? Source { get; set; }

        [JsonProperty("info")]
        public List<string>? Info { get; set; }
    }

    public class Link
    {
        [JsonProperty("text")]
        public string? Text { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }
    }

    public class Source
    {
        [JsonProperty("language")]
        public string? Language { get; set; }

        [JsonProperty("word")]
        public string? Word { get; set; }
    }

    public class Attribution
    {
        [JsonProperty("jmdict")]
        public bool Jmdict { get; set; }

        [JsonProperty("jmnedict")]
        public bool Jmnedict { get; set; }

        [JsonProperty("dbpedia")]
        public bool Dbpedia { get; set; }
    }

    // Helper class để lưu lịch sử tìm kiếm
    public class RecentSearch
    {
        public string? Word { get; set; }
        public string? Reading { get; set; }
        public List<string> Meanings { get; set; } = new List<string>();
        public string? AudioUrl { get; set; }

        public RecentSearch(string word, string reading, List<string> meanings, string? audioUrl = null)
        {
            Word = word;
            Reading = reading;
            Meanings = meanings;
            AudioUrl = audioUrl;
        }
    }
} 