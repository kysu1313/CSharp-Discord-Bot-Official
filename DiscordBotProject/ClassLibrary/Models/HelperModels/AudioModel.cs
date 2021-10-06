using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ClassLibrary.Models.HelperModels
{
    
    // REFERECNCE: https://github.com/sorashi/youtube-play/blob/d116a5705b6f902462af53d74c031ba69341d931/youtube-play/LinkResolver.cs#L99
    
    public class VideoFormat
    {
        public string FormatCode { get; set; }
        public string Extension { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string Note { get; set; }
        public string Url { get; set; }
        public float? Bitrate { get; set; }
    }
    
    public class AudioModel
    {

        public string id { get; set; }
        public string title { get; set; }
        private Task<IEnumerable<VideoFormat>> Formats { get; set; }
        private VideoFormat bestAudioFormat;
        
        public AudioModel(string _id, string _title = null) {
            id = _id;
            title = _title;
        }
        
        public Task<IEnumerable<VideoFormat>> GetFormats() {
            if (Formats != null) return Formats;
            var t = Task.Factory.StartNew(() => {
                var iteratorProcess = new Process {
                    StartInfo = new ProcessStartInfo {
                        FileName = "youtube-dl",
                        Arguments = "-j " + id,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                iteratorProcess.Start();
                var stdout = iteratorProcess.StandardOutput.ReadToEnd();
                var stderr = iteratorProcess.StandardError.ReadToEnd();
                iteratorProcess.WaitForExit();
                var code = iteratorProcess.ExitCode;
                if (code != 0) {
                    // Error Occurred
                    return null;
                }
                var jo = JObject.Parse(stdout);
                return ResolveFormats(jo["formats"]?.ToString());
            });
            Formats = t;
            return t;
        }
        
        private static IEnumerable<VideoFormat> ResolveFormats(string jsonArray) {
            var ja = JArray.Parse(jsonArray);
            return ja
                .Select(x =>
                    new VideoFormat {
                        FormatCode = x["format_id"].ToString(),
                        Extension = x["ext"].ToString(),
                        Width = x.Value<int?>("width"),
                        Height = x.Value<int?>("height"),
                        Note = x["format_note"]?.ToString(),
                        Bitrate = x.Value<float?>("abr") ?? x.Value<float?>("tbr"),
                        Url = x["url"].ToString()
                    }
                );
        }
        
        public static AudioModel FromJson(string json) {
            var jo = JObject.Parse(json);
            var vid = new AudioModel(jo["id"].ToString(), jo["title"].ToString()) {
                Formats = Task.FromResult(ResolveFormats(jo["formats"].ToString()))
            };
            return vid;
        }
        
        public async Task<VideoFormat> GetBestAudioFormat() {
            if (bestAudioFormat != null) return bestAudioFormat;
            var videoFormats = await GetFormats();
            if (videoFormats == null) return null;
            var audioOnly = videoFormats.Where(x => x.Note == "DASH audio" &&
                                                    (x.Extension == "webm" || x.Extension == "m4a"));
            
            bestAudioFormat = audioOnly.OrderByDescending(x => x.Bitrate).First();
            return bestAudioFormat;
        }
        
    }
}