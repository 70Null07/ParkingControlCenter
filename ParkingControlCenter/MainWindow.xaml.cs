using CarsRecognitionLibrary;
using CarsMovementLibrary;
using MediaFileProcessor.Models.Common;
using MediaFileProcessor.Models.Enums;
using MediaFileProcessor.Processors;
using System.Windows;
using System.IO;
using System.Net.Http;
using System.Diagnostics;

namespace ParkingControlCenter
{
    public partial class MainWindow : Window
    {
		private HttpClient httpClient;
		private string playlistUrl = "http://krkvideo2.orionnet.online/cam3209/tracks-v1/mono.m3u8"; // URL плейлиста .m3u8
		string[] segmentUrls;
		string tempFilePath = string.Empty;
		bool isThreadsStarted = false;

		private static volatile int nextMediaElement = 0;

        private CarsMovement CarsMovement = new CarsMovement();

        public MainWindow()
        {
            //InitializeComponent();

			httpClient = new HttpClient();

			Thread t1 = new(new ThreadStart(GetSegmentTask));
			t1.Start();
		}

		private async void GetSegmentTask()
		{
			string playlistContent = await httpClient.GetStringAsync(playlistUrl);
			string baseUrl = "http://krkvideo2.orionnet.online/cam3209/tracks-v1/";
			segmentUrls = await ParsePlaylist(playlistContent, baseUrl);

			var watch = Stopwatch.StartNew();
			await GetSegment();
			System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
			{
				if (!isThreadsStarted)
				{
					//Process.Start("C:\\ffmpeg\\bin\\ffmpeg.exe", "-re -f concat -i C:\\ffmpeg\\bin\\list_1.txt -f mpegts udp://127.0.0.1:23000?pkt_size=188\"&\"buffer_size=65535");

					//Process.Start("powershell.exe", "vlc.exe http://krkvideo2.orionnet.online/cam3209/tracks-v1/mono.m3u8 --sout='#transcode{vcodec=theo,vb=1000}:http{access=https,mux=ogg,dst=:24000/video.ogg}'" +
					//	" --sout-keep --http-cert='C://temp/Certificate/certificate.pem' --http-key='C://temp/Certificate/parkingcontrolcomplex.pem'");

			//		Process.Start("powershell.exe", "vlc.exe http://krkvideo2.orionnet.online/cam3209/tracks-v1/mono.m3u8" +
	  //" --sout='#transcode{vcodec=VP80,vb=2000}:http{access=https," +
	  //" mux=webm,dst=:24000/cam.webm}' --no-sout-all --sout-keep --http-cert='C://temp/Certificate/certificate.pem'" +
	  //" --http-key='C://temp/Certificate/parkingcontrolcomplex.pem'");

					//Process.Start("C:\\ffmpeg\\bin\\ffmpeg.exe", "-re -f concat -i C:\\ffmpeg\\bin\\list_3.txt -f mpegts -r 20 udp://127.0.0.1:24000?pkt_size=188\"&\"buffer_size=65535");
					isThreadsStarted = true;
				}

				if (nextMediaElement == 0)
					nextMediaElement = 1;
				else
					nextMediaElement = 0;
			}));
			Thread.Sleep(74000);
			watch.Stop();
			var elapsedMs = watch.ElapsedMilliseconds;
			if (elapsedMs + 74000 < 80000)
			{
				Thread.Sleep((int)(80000 - (74000 + elapsedMs)));
			}
			GetSegmentTask();
		}

		private async Task<bool> GetSegment()
		{
			var byteArray = new List<byte[]>();
			int segmentLength = 0;

			for (int i = 0; i < segmentUrls.Length; i++)
			{
				byteArray.Add(await httpClient.GetByteArrayAsync(segmentUrls[i]));
				segmentLength += byteArray[i].Length;
			};

			using (MemoryStream memoryStream = new MemoryStream(byteArray.SelectMany(x => x).ToArray()))
			{
				// Загрузка сегмента .ts в буфер
				byte[] buffer = new byte[memoryStream.Length];
				memoryStream.Read(buffer, 0, buffer.Length);

				// Сохранение сегмента во временный файл
				tempFilePath = System.IO.Path.GetTempFileName();

				tempFilePath = tempFilePath.Remove(tempFilePath.Length - 4) + ".wmv";

				File.WriteAllBytes("C:\\ffmpeg\\bin\\" + tempFilePath.Split('\\').Last(), buffer);

				var videoFileProcessor = new VideoFileProcessor();

				for (int j = 0; j < 8; j++)
				{
					await videoFileProcessor.ExtractFrameFromVideoAsync(TimeSpan.FromMilliseconds(j * 10000),
												new MediaFile("C:\\ffmpeg\\bin\\" + tempFilePath.Split('\\').Last()),
												"C:/temp/parking.jpg",
												FileFormatType.JPG);

					CarRecognition carRecognition = new("C:/temp/parking.jpg");
					Thread.Sleep(1000);
					carRecognition.Recognize();
					carRecognition.DrawPlace();
					File.Copy("./parkingload.jpg", "C:\\Users\\dmitr\\source\\repos\\TestWebApplicationHTTP\\TestWebApplicationHTTP\\wwwroot\\images\\parkingload_" + j + ".jpg", true);
				}

				CarsMovement.VideoPath = "C:\\ffmpeg\\bin\\" + tempFilePath.Split('\\').Last();
				//CarsMovement.VideoPath = "C:\\ffmpeg\\bin\\tmprf351v.wmv";
				//CarsMovement.GenerateNewData();
				Thread t2 = new(new ThreadStart(CarsMovement.GetCarsMovement));
				t2.Start();

				if (nextMediaElement == 0)
				{
					string[] arrLine = File.ReadAllLines("C:\\ffmpeg\\bin\\list_1.txt");
					arrLine[2 - 1] = "file '" + tempFilePath.Split('\\').Last() + "'";
					File.WriteAllLines("C:\\ffmpeg\\bin\\list_1.txt", arrLine);
				}
				else
				{
					string[] arrLine = File.ReadAllLines("C:\\ffmpeg\\bin\\list_2.txt");
					arrLine[2 - 1] = "file '" + tempFilePath.Split('\\').Last() + "'";
					File.WriteAllLines("C:\\ffmpeg\\bin\\list_2.txt", arrLine);
				}
			}
			return true;
		}

		private Task<string[]> ParsePlaylist(string playlistContent, string baseUrl)
		{
			List<string> segmentUrlsList = new List<string>();

			string[] lines = playlistContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];

				if (line.StartsWith("#EXTINF:"))
				{
					string segmentRelativeUrl = lines[i + 1];
					string segmentUrl = new Uri(new Uri(baseUrl), segmentRelativeUrl).ToString();
					segmentUrlsList.Add(segmentUrl);
				}
			}
			return Task.FromResult(segmentUrlsList.ToArray());
		}
	}
}