using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Models.HelperModels;
using Discord;
using Discord.Audio;
using Newtonsoft.Json.Linq;
using YoutubeExtractor;

namespace ClassLibrary.Helpers
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private Queue<string> _playlist;
        private readonly Queue<AudioModel> _playQueue = new Queue<AudioModel>();
        private string _youtubeUrl = "https://www.youtube.com/get_video_info?html5=1&video_id=";
        private string _prefetchUrl = "https://www.youtube.com/watch?v=IDGOESHERE&spf=prefetch";
        
        // btw: the data is encoded twice, you have to decode->parse->decode to get a working link

        // public async Task GetSound()
        // {
        //
        //     return Task.Factory.StartNew(async () => {
        //         while (_playQueue.Count > 0) {
        //             var current = _playQueue.Dequeue();
        //             var playerProcess = new Process {
        //                 StartInfo = new ProcessStartInfo {
        //                     FileName = "ffplay",
        //                     Arguments = $"-volume {volume} -nodisp -vn -autoexit \"{(await current.GetBestStreamableAudioFormat()).Url}\"",
        //                     UseShellExecute = false,
        //                     CreateNoWindow = true
        //                 }
        //             };
        //             Console.ForegroundColor = ConsoleColor.Cyan;
        //             Console.WriteLine($"Now playing: {current.Title}");
        //             Console.ResetColor();
        //             playerProcess.Start();
        //             ChildProcessTracker.AddProcess(playerProcess);
        //             playerProcess.WaitForExit();
        //         }
        //     });
        // }
        //
        // public static IEnumerable<Video> ResolveLink(string rawLink) {
        //     var iteratorProcess = new Process {
        //         StartInfo = new ProcessStartInfo {
        //             FileName = "youtube-dl",
        //             Arguments = $"-j --flat-playlist \"{rawLink}\"",
        //             UseShellExecute = false,
        //             RedirectStandardOutput = true,
        //             CreateNoWindow = true
        //         }
        //     };
        //     iteratorProcess.Start();
        //     while (!iteratorProcess.StandardOutput.EndOfStream) {
        //         var line = iteratorProcess.StandardOutput.ReadLine();
        //         var jo = JObject.Parse(line);
        //         if (jo.Value<string>("_type") == "url") // we are dealing with a playlist
        //             yield return new Video(jo["id"].ToString(), jo["title"].ToString());
        //         else yield return Video.FromJson(line); // a single video
        //     }
        // }
        
        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                return;
            }
            if (target.Guild.Id != guild.Id)
            {
                return;
            }

            var audioClient = await target.ConnectAsync();

            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
            {
                // If you add a method to log happenings from this service,
                // you can uncomment these commented lines to make use of that.
                //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
            }
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
                //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
            }
        }
        
        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
            // Your task: Get a full path to the file if the value of 'path' is only a filename.
            if (!File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                using (var ffmpeg = CreateProcess(path))
                using (var stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                    finally { await stream.FlushAsync(); }
                }
            }
        }

        private Process CreateProcess(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        private async Task GetAudio()
        {
        
            
            
            // Our test youtube link
            string link = "insert youtube link";

            /*
             * Get the available video formats.
             * We'll work with them in the video and audio download examples.
             */
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link);
            /*
             * We want the first extractable video with the highest audio quality.
             */
            VideoInfo video = videoInfos
                .Where(info => info.CanExtractAudio)
                .OrderByDescending(info => info.AudioBitrate)
                .First();
    
            /*
             * If the video has a decrypted signature, decipher it
            */
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            /*
             * Create the audio downloader.
             * The first argument is the video where the audio should be extracted from.
             * The second argument is the path to save the audio file.
             */
            var audioDownloader = new AudioDownloader(video, Path.Combine("D:/Downloads", video.Title + video.AudioExtension));

            // Register the progress events. We treat the download progress as 85% of the progress and the extraction progress only as 15% of the progress,
            // because the download will take much longer than the audio extraction.
            audioDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage * 0.85);
            audioDownloader.AudioExtractionProgressChanged += (sender, args) => Console.WriteLine(85 + args.ProgressPercentage * 0.15);

            /*
             * Execute the audio downloader.
             * For GUI applications note, that this method runs synchronously.
             */
            audioDownloader.Execute();
        }
    }    
}
