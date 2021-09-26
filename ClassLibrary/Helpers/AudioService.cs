using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models.HelperModels;
using Discord;
using Discord.Audio;
using NBitcoin;
using Newtonsoft.Json.Linq;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExtractor;

namespace ClassLibrary.Helpers
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private Queue<string> _playlist;
        private readonly ConcurrentQueue<AudioModel> _playQueue = new ConcurrentQueue<AudioModel>();
        private readonly ConcurrentQueue<Process> _processQueue = new ConcurrentQueue<Process>();
        private string _youtubeUrl = "https://www.youtube.com/get_video_info?html5=1&video_id=";
        private string _prefetchUrl = "https://www.youtube.com/watch?v=IDGOESHERE&spf=prefetch";
        
        // btw: the data is encoded twice, you have to decode->parse->decode to get a working link

        public async Task<Task> PlayTrack(int volume = 20)
        {
        
            return Task.Factory.StartNew(async () => {
                while (_playQueue.Count > 0) {
                    var current = _playQueue.TryDequeue(out var success) 
                                  == false ? new AudioModel("","") : success;
                    var playerProcess = new Process {
                        StartInfo = new ProcessStartInfo {
                            FileName = "ffplay",
                            Arguments = $"-volume {volume} -nodisp -vn -autoexit \"{(await current.GetBestAudioFormat()).Url}\"",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Now playing: {current.title}");
                    Console.ResetColor();
                    playerProcess.Start();
                    playerProcess.WaitForExit();
                }
            });
        }
        
        public static IEnumerable<AudioModel> ResolveLink(string link) {
            var iteratorProcess = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "youtube-dl",
                    Arguments = $"-j --flat-playlist \"{link}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            iteratorProcess.Start();
            
            while (!iteratorProcess.StandardOutput.EndOfStream) {
                var line = iteratorProcess.StandardOutput.ReadLine();
                var jo = JObject.Parse(line);
                if (jo.Value<string>("_type") == "url") // we are dealing with a playlist
                    yield return new AudioModel(jo["id"].ToString(), jo["title"].ToString());
                else yield return AudioModel.FromJson(line); // a single video
            }
        }

        public async Task<Stream> CallYoutube(string id)
        {
            var youtube = new YoutubeClient();
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(id);
            var streamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();
            await using var input = await youtube.Videos.Streams.GetAsync(streamInfo);

            return input;
            // await using var output = client.CreatePCMStream();

            // var cmd = input | Cli.Wrap("ffmpeg").WithArguments("-i pipe:0 ...") | output;
            // await cmd.ExecuteAsync();
        }
        
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
        
        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string link)
        {
            // !play https://www.youtube.com/watch?v=aIHF7u9Wwiw
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                
                var audio = await CallYoutube(GetVideoId(link));
                var audioLength = audio.Length;
                var bytes = streamToByteArray(audio);
                using (var output = client.CreatePCMStream(AudioApplication.Mixed))
                {
                    var process = Process.Start(new ProcessStartInfo { // FFmpeg requires us to spawn a process and hook into its stdout, so we will create a Process
                        FileName = "ffmpeg",
                        Arguments = $"-i {link} " + // Here we provide a list of arguments to feed into FFmpeg. -i means the location of the file/URL it will read from
                                    "-f s16le -ar 48000 -ac 2 pipe:1", // Next, we tell it to output 16-bit 48000Hz PCM, over 2 channels, to stdout.
                        UseShellExecute = false,
                        RedirectStandardOutput = true // Capture the stdout of the process
                    });
                    Thread.Sleep(2000); // Sleep for a few seconds to FFmpeg can start processing data.

                    int blockSize = 3840; // The size of bytes to read per frame; 1920 for mono
                    byte[] buffer = new byte[blockSize];
                    int byteCount;

                    while (true) // Loop forever, so data will always be read
                    {
                        byteCount = process.StandardOutput.BaseStream // Access the underlying MemoryStream from the stdout of FFmpeg
                            .Read(buffer, 0, blockSize); // Read stdout into the buffer

                        if (byteCount == 0) // FFmpeg did not output anything
                            break; // Break out of the while(true) loop, since there was nothing to read.

                        output.WriteAsync(buffer, 0, byteCount);
                        // client.Send(buffer, 0, byteCount); // Send our data to Discord
                    }

                    // output..Wait(); 
                    
                    
                    // await output.WriteAsync(bytes, 0, bytes.Length); // Send the buffer to Discord
                }
                    
                
                
                
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                // using (var ffmpeg = CreateProcess(link))
                // using (var stream = client.CreatePCMStream(AudioApplication.Music))
                // {
                    
                    // byte currByte = 0;
                    // while (currByte < audioLength)
                    // {
                    //     stream.WriteByte(currByte);
                    //     currByte++;
                    // }
                    

                    
                    // await stream.FlushAsync();
                    // try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(
                    //     await CallYoutube(GetVideoId(link))); }
                    // stream.BeginWrite(audio.ReadBytes(1000))
                    // finally { await stream.FlushAsync(); }
                // }
            }
        }
        
        public static byte[] streamToByteArray(Stream input)
        {
            MemoryStream ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }

        private static string GetVideoId(string url)
        {
            var id = url.Split("=")[^1];
            return id;
        }

        private Process CreateProcess(string link)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{link}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
                
                // FileName = "ffmpeg.exe",
                // Arguments = $"-hide_banner -loglevel panic -i \"{link}\" -ac 2 -f s16le -ar 48000 pipe:1",
                // UseShellExecute = false,
                // RedirectStandardOutput = true
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
