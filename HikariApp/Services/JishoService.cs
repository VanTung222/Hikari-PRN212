using HikariApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Media;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.RegularExpressions;
using System.Linq; // Added for .Any()

namespace HikariApp.Services
{
    public class JishoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _jishoBaseUrl = "https://jisho.org/api/v1/search/words";
        private readonly string _googleTranslateBaseUrl = "https://translate.googleapis.com/translate_a/single";
        private List<RecentSearch> _recentSearches = new List<RecentSearch>();
        private const int MaxRecentSearches = 10;

        public JishoService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        }

        // Tìm kiếm từ qua Jisho API
        public async Task<JishoResponse?> SearchWordAsync(string word)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[JISHO] Searching for: {word}");
                
                if (string.IsNullOrWhiteSpace(word))
                {
                    System.Diagnostics.Debug.WriteLine("[JISHO] Empty search term");
                    return CreateSimpleResponse("Lỗi", "Vui lòng nhập từ cần tra cứu");
                }
                
                // Kiểm tra xem từ có phải tiếng Việt không
                bool isVietnamese = ContainsVietnameseCharacters(word);
                bool isJapanese = ContainsJapaneseCharacters(word);
                
                System.Diagnostics.Debug.WriteLine($"[JISHO] isVietnamese: {isVietnamese}, isJapanese: {isJapanese}");
                
                string originalWord = word;
                List<string> meanings = new List<string>();
                string? japaneseTranslation = null;
                string? vietnameseTranslation = null;
                
                // Xử lý dịch trực tiếp trước khi tìm kiếm trong Jisho
                if (isVietnamese)
                {
                    // Dịch từ tiếng Việt sang tiếng Nhật
                    System.Diagnostics.Debug.WriteLine("[JISHO] Translating Vietnamese to Japanese directly");
                    japaneseTranslation = await TranslateAsync(word, "vi", "ja");
                    System.Diagnostics.Debug.WriteLine($"[JISHO] Vietnamese to Japanese: {word} -> {japaneseTranslation}");
                    
                    if (!string.IsNullOrEmpty(japaneseTranslation))
                    {
                        meanings.Add($"Tiếng Việt: {word}");
                    }
                }
                else if (isJapanese)
                {
                    // Dịch từ tiếng Nhật sang tiếng Việt
                    System.Diagnostics.Debug.WriteLine("[JISHO] Translating Japanese to Vietnamese directly");
                    vietnameseTranslation = await TranslateAsync(word, "ja", "vi");
                    System.Diagnostics.Debug.WriteLine($"[JISHO] Japanese to Vietnamese: {word} -> {vietnameseTranslation}");
                    
                    if (!string.IsNullOrEmpty(vietnameseTranslation))
                    {
                        meanings.Add($"Tiếng Việt: {vietnameseTranslation}");
                    }
                }
                
                // Tìm thêm thông tin từ Jisho API nếu là tiếng Nhật hoặc có thể dịch sang tiếng Anh
                JishoResponse? jishoResult = null;
                
                if (isJapanese || !isVietnamese)
                {
                    // Nếu là tiếng Nhật hoặc tiếng Anh, tìm trực tiếp trong Jisho
                    var queryParams = new Dictionary<string, string?>
                    {
                        { "keyword", word }
                    };
                    
                    var requestUri = QueryHelpers.AddQueryString(_jishoBaseUrl, queryParams);
                    System.Diagnostics.Debug.WriteLine($"[JISHO] Request URI: {requestUri}");
                    
                    try
                    {
                        var response = await _httpClient.GetAsync(requestUri);
                        System.Diagnostics.Debug.WriteLine($"[JISHO] Response status: {response.StatusCode}");
                        
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            System.Diagnostics.Debug.WriteLine($"[JISHO] Response content: {content.Substring(0, Math.Min(content.Length, 200))}...");
                            
                            jishoResult = JsonConvert.DeserializeObject<JishoResponse>(content);
                            System.Diagnostics.Debug.WriteLine($"[JISHO] Deserialized result: {(jishoResult != null ? "success" : "null")}");
                            
                            if (jishoResult?.Data != null && jishoResult.Data.Count > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"[JISHO] Found {jishoResult.Data.Count} results in Jisho");
                                
                                // Nếu tìm thấy kết quả trong Jisho, thêm nghĩa tiếng Việt nếu có
                                if (isJapanese && !string.IsNullOrEmpty(vietnameseTranslation))
                                {
                                    foreach (var data in jishoResult.Data)
                                    {
                                        if (data.Senses != null && data.Senses.Count > 0)
                                        {
                                            if (data.Senses[0].EnglishDefinitions == null)
                                            {
                                                data.Senses[0].EnglishDefinitions = new List<string>();
                                            }
                                            
                                            // Thêm nghĩa tiếng Việt vào đầu danh sách
                                            data.Senses[0].EnglishDefinitions.Insert(0, $"Tiếng Việt: {vietnameseTranslation}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[JISHO] Error querying Jisho API: {ex.Message}");
                        // Tiếp tục xử lý với kết quả dịch trực tiếp
                    }
                }
                
                // Nếu không tìm thấy kết quả trong Jisho hoặc có lỗi, tạo kết quả từ dịch trực tiếp
                if ((jishoResult == null || jishoResult.Data == null || jishoResult.Data.Count == 0) && 
                    (isVietnamese || isJapanese))
                {
                    System.Diagnostics.Debug.WriteLine("[JISHO] Creating response from direct translation");
                    
                    // Tạo kết quả từ dịch trực tiếp
                    var japaneseEntry = new JapaneseWord();
                    
                    if (isVietnamese && !string.IsNullOrEmpty(japaneseTranslation))
                    {
                        // Nếu từ gốc là tiếng Việt, hiển thị kết quả dịch tiếng Nhật
                        japaneseEntry.Word = japaneseTranslation;
                        japaneseEntry.Reading = japaneseTranslation;
                        
                        // Thêm nghĩa tiếng Việt gốc
                        if (!meanings.Contains($"Tiếng Việt: {originalWord}"))
                        {
                            meanings.Add($"Tiếng Việt: {originalWord}");
                        }
                    }
                    else if (isJapanese)
                    {
                        // Nếu từ gốc là tiếng Nhật, giữ nguyên từ tiếng Nhật
                        japaneseEntry.Word = originalWord;
                        japaneseEntry.Reading = originalWord;
                        
                        // Thêm nghĩa tiếng Việt đã dịch
                        if (!string.IsNullOrEmpty(vietnameseTranslation) && 
                            !meanings.Contains($"Tiếng Việt: {vietnameseTranslation}"))
                        {
                            meanings.Add($"Tiếng Việt: {vietnameseTranslation}");
                        }
                    }
                    
                    // Tạo kết quả giả lập để hiển thị
                    var sense = new Sense
                    {
                        EnglishDefinitions = meanings
                    };
                    
                    var data = new JishoData
                    {
                        Japanese = new List<JapaneseWord> { japaneseEntry },
                        Senses = new List<Sense> { sense }
                    };
                    
                    jishoResult = new JishoResponse
                    {
                        Data = new List<JishoData> { data }
                    };
                }
                
                // Nếu vẫn không có kết quả, tạo thông báo không tìm thấy
                if (jishoResult == null || jishoResult.Data == null || jishoResult.Data.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("[JISHO] No results found, creating empty response");
                    return CreateSimpleResponse("Không tìm thấy", "Không tìm thấy kết quả cho từ khóa này");
                }
                
                // Thêm vào lịch sử tìm kiếm nếu có kết quả
                if (jishoResult.Data.Count > 0 && jishoResult.Data[0].Japanese?.Count > 0)
                {
                    var firstResult = jishoResult.Data[0];
                    var firstJapanese = firstResult.Japanese[0];
                    
                    string displayWord = originalWord;
                    string reading = string.Empty;
                    
                    if (isVietnamese && !string.IsNullOrEmpty(japaneseTranslation))
                    {
                        // Nếu từ gốc là tiếng Việt, hiển thị cả từ tiếng Việt và tiếng Nhật
                        displayWord = originalWord;
                        reading = japaneseTranslation;
                    }
                    else if (isJapanese)
                    {
                        // Nếu từ gốc là tiếng Nhật, hiển thị từ tiếng Nhật
                        displayWord = firstJapanese.Word ?? originalWord;
                        reading = firstJapanese.Reading ?? originalWord;
                    }
                    else
                    {
                        // Trường hợp khác
                        displayWord = firstJapanese.Word ?? originalWord;
                        reading = firstJapanese.Reading ?? string.Empty;
                    }
                    
                    // Lấy nghĩa từ kết quả
                    var resultMeanings = new List<string>();
                    
                    if (firstResult.Senses?.Count > 0 && firstResult.Senses[0].EnglishDefinitions?.Count > 0)
                    {
                        resultMeanings.AddRange(firstResult.Senses[0].EnglishDefinitions);
                    }
                    
                    // Thêm nghĩa tiếng Việt nếu chưa có
                    if (isJapanese && !string.IsNullOrEmpty(vietnameseTranslation) && 
                        !resultMeanings.Any(m => m.Contains(vietnameseTranslation)))
                    {
                        resultMeanings.Add($"Tiếng Việt: {vietnameseTranslation}");
                    }
                    
                    // Lấy URL âm thanh
                    string? audioUrl = null;
                    
                    if (isVietnamese && !string.IsNullOrEmpty(japaneseTranslation))
                    {
                        // Nếu từ gốc là tiếng Việt, phát âm từ tiếng Nhật đã dịch
                        audioUrl = GetAudioUrlForWord(japaneseTranslation);
                    }
                    else if (!string.IsNullOrEmpty(firstJapanese.Reading))
                    {
                        audioUrl = GetAudioUrlForWord(firstJapanese.Reading);
                    }
                    else if (!string.IsNullOrEmpty(firstJapanese.Word))
                    {
                        audioUrl = GetAudioUrlForWord(firstJapanese.Word);
                    }
                    
                    // Thêm vào lịch sử tìm kiếm
                    AddRecentSearch(new RecentSearch(
                        displayWord,
                        reading,
                        resultMeanings,
                        audioUrl
                    ));
                }
                
                return jishoResult;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[JISHO] Error searching word: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[JISHO] Stack trace: {ex.StackTrace}");
                
                return CreateSimpleResponse("Lỗi", $"Lỗi khi tìm kiếm: {ex.Message}");
            }
        }
        
        // Tạo kết quả đơn giản với thông báo
        private JishoResponse CreateSimpleResponse(string title, string message)
        {
            var japaneseEntry = new JapaneseWord
            {
                Word = title,
                Reading = string.Empty
            };
            
            var sense = new Sense
            {
                EnglishDefinitions = new List<string> { message }
            };
            
            var data = new JishoData
            {
                Japanese = new List<JapaneseWord> { japaneseEntry },
                Senses = new List<Sense> { sense }
            };
            
            return new JishoResponse
            {
                Data = new List<JishoData> { data }
            };
        }
        
        // Dịch văn bản giữa các ngôn ngữ sử dụng Google Translate API không chính thức
        private async Task<string> TranslateAsync(string text, string sourceLanguage, string targetLanguage)
        {
            try
            {
                var queryParams = new Dictionary<string, string?>
                {
                    { "client", "gtx" },
                    { "sl", sourceLanguage },
                    { "tl", targetLanguage },
                    { "dt", "t" },
                    { "q", text }
                };
                
                var requestUri = QueryHelpers.AddQueryString(_googleTranslateBaseUrl, queryParams);
                System.Diagnostics.Debug.WriteLine($"[TRANSLATE] Request URI: {requestUri}");
                
                var response = await _httpClient.GetAsync(requestUri);
                System.Diagnostics.Debug.WriteLine($"[TRANSLATE] Response status: {response.StatusCode}");
                
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[TRANSLATE] Response content: {content.Substring(0, Math.Min(content.Length, 200))}...");
                
                // Phân tích kết quả JSON từ Google Translate API
                // Kết quả có dạng [[["translated text","original text",null,null,1]],null,"source-lang"]
                dynamic? result = JsonConvert.DeserializeObject(content);
                if (result != null && result[0] != null && result[0][0] != null)
                {
                    string translation = result[0][0][0]?.ToString() ?? string.Empty;
                    System.Diagnostics.Debug.WriteLine($"[TRANSLATE] Translated: {text} -> {translation}");
                    return translation;
                }
                
                return string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TRANSLATE] Error translating: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[TRANSLATE] Stack trace: {ex.StackTrace}");
                return string.Empty;
            }
        }
        
        // Kiểm tra xem chuỗi có chứa ký tự tiếng Việt không
        private bool ContainsVietnameseCharacters(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            
            // Các ký tự đặc trưng của tiếng Việt
            string vietnameseChars = "ăâêôơưáàảãạắằẳẵặấầẩẫậéèẻẽẹếềểễệíìỉĩịóòỏõọốồổỗộớờởỡợúùủũụứừửữựýỳỷỹỵđ";
            
            text = text.ToLower();
            foreach (char c in text)
            {
                if (vietnameseChars.Contains(c))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        // Kiểm tra xem chuỗi có chứa ký tự tiếng Nhật không
        private bool ContainsJapaneseCharacters(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            
            // Kiểm tra bằng regex cho các ký tự Hiragana, Katakana và Kanji
            return Regex.IsMatch(text, @"[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]");
        }

        // Lấy URL âm thanh cho từ tiếng Nhật
        private string GetAudioUrlForWord(string word)
        {
            if (string.IsNullOrEmpty(word)) return string.Empty;
            
            // Sử dụng Google Text-to-Speech API
            var queryParams = new Dictionary<string, string?>
            {
                { "ie", "UTF-8" },
                { "q", word },
                { "tl", "ja" },
                { "client", "tw-ob" }
            };
            
            return QueryHelpers.AddQueryString("https://translate.google.com/translate_tts", queryParams);
        }

        // Phát âm thanh từ URL
        public async Task PlayAudioAsync(string? audioUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(audioUrl))
                    return;

                System.Diagnostics.Debug.WriteLine($"[AUDIO] Playing audio from URL: {audioUrl}");
                
                // Tạo thư mục temp nếu chưa tồn tại
                var tempFolder = Path.Combine(Path.GetTempPath(), "HikariAudio");
                if (!Directory.Exists(tempFolder))
                    Directory.CreateDirectory(tempFolder);

                // Tạo file tạm để lưu âm thanh
                var tempFile = Path.Combine(tempFolder, $"audio_{DateTime.Now.Ticks}.mp3");

                // Tải file âm thanh
                var audioBytes = await _httpClient.GetByteArrayAsync(audioUrl);
                await File.WriteAllBytesAsync(tempFile, audioBytes);

                System.Diagnostics.Debug.WriteLine($"[AUDIO] Audio saved to: {tempFile}");
                
                // Phát âm thanh
                using (var player = new SoundPlayer(tempFile))
                {
                    player.Play();
                }

                // Xóa file tạm sau khi phát xong (không chờ đợi)
                _ = Task.Run(async () => {
                    await Task.Delay(10000); // Chờ 10 giây
                    try { File.Delete(tempFile); } catch { }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUDIO] Error playing audio: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[AUDIO] Stack trace: {ex.StackTrace}");
            }
        }

        // Thêm từ vào danh sách tìm kiếm gần đây
        private void AddRecentSearch(RecentSearch search)
        {
            // Xóa nếu từ đã tồn tại trong danh sách
            _recentSearches.RemoveAll(s => s.Word == search.Word);

            // Thêm vào đầu danh sách
            _recentSearches.Insert(0, search);

            // Giữ danh sách không quá số lượng tối đa
            if (_recentSearches.Count > MaxRecentSearches)
            {
                _recentSearches.RemoveAt(_recentSearches.Count - 1);
            }
        }

        // Lấy danh sách các từ đã tìm kiếm gần đây
        public List<RecentSearch> GetRecentSearches()
        {
            return _recentSearches;
        }
    }
} 