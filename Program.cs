using Microsoft.Extensions.DependencyInjection;

namespace PocHttpClient
{
	internal class Program
	{
		#region Helper Data
		public static readonly List<string> Websites = new List<string>()
		{
			"http://google.com",
			"http://facebook.com",
			"http://youtube.com",
			"http://baidu.com",
			"http://wikipedia.org",
			"http://yahoo.com",
			"http://qq.com",
			"http://taobao.com",
			"http://tmall.com",
			"http://twitter.com"

		};
		
		public static readonly List<string> GitHubUsers = new List<string>()
		{
			"timothybrooks",
			"karpathy",
			"RaifordBrookshire",
			"microsoft",
			"miniprofiler",
			"CryptoPowerTools"
		};
		private enum HttpClientNames
		{
			GitHubApi,
			BasicClient,
			DogApi
		}
		#endregion

		static async Task Main(string[] args)
		{
			// Unfortunately the way to Create a factory instance is by setting up
			// the Service  Container and pulling from it.
			var services = new ServiceCollection();
			
			#region Setup HttpClients and Add to Single Factory
			// Exampples of Adding Various Http Setup for each named client.
			// The idea here is that all HttpClient can be configured in a single
			// place in the app startup. Various parts of the code will now 
			// create these from the factory HttpClientFactory.CreateClient("EndpointName")
			services.AddHttpClient(nameof(HttpClientNames.GitHubApi), client =>
			{
				client.BaseAddress = new Uri("https://api.github.com/");
				client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
				client.DefaultRequestHeaders.Add("User-Agent", "PocHttpClientApp");
					
			});
			// Add your other Endpoint Clients as your app required	
			services.AddHttpClient(nameof(HttpClientNames.BasicClient), client =>
			{
				//client.BaseAddress = new Uri("https://api.github.com/");
				//client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
				client.DefaultRequestHeaders.Add("User-Agent", "PocHttpClientApp");
					
			});
			// Test Api that returns a random Image url
			services.AddHttpClient(nameof(HttpClientNames.DogApi), client =>
			{
				client.BaseAddress = new Uri("https://dog.ceo/");
				//client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
				client.DefaultRequestHeaders.Add("User-Agent", "PocHttpClientApp");

			});
			var provider = services.BuildServiceProvider();
			IHttpClientFactory? clientFactory = provider.GetService<IHttpClientFactory>();
			#endregion
			
			// Call your implementations - Just uncomment to try various methods
			await GetDogImage(clientFactory);
			await GetDogImages(clientFactory, 3);
			await GetGitHubInfo(clientFactory, "DotNetOpenAuth");
			await GetGitHubInfoAll(clientFactory, GitHubUsers);
			
			WriteLine("Hit any key...");
			Console.ReadLine();
		}

		async static Task UsingHttpClient(IHttpClientFactory? factory)
		{
			WriteHeader("Using HttpClient class to call single Request");
			try
			{
				string url = "http://yahoo.com";
				var client = factory.CreateClient("TestClient");
				var response = await client.GetAsync(url); 
				response.EnsureSuccessStatusCode();
				var responseContent = await response.Content.ReadAsStringAsync();
				WriteResponse($"Content returned from {url} statuscode:{response.StatusCode} IsSuccessCode:{response.IsSuccessStatusCode}");
				WriteLine(responseContent.Substring(0, 20));
			}
			catch (HttpRequestException e)
			{
				Console.WriteLine("Error: " + e.Message + " " + e?.InnerException?.Message);
			}			
		}

		public static async Task GetGitHubInfo(IHttpClientFactory? factory, string userName)
		{	
			// NOTE: THIS IS NOT THE PREFERRED METHOD
			//		 Since it does not take advantage of the pooled HttpClientFactory
			//		 I am leaving out all error trapping for clarity
			HttpClient client = factory.CreateClient(nameof(HttpClientNames.GitHubApi));
			var response = await client.GetAsync($"/users/{userName}");
			var json = response.Content.ReadAsStringAsync().Result;
			WriteHeader($"Git Hub Account Info - {userName}");
			WriteJson($"Json: {json}");
		}
		public static async Task GetGitHubInfoAll(IHttpClientFactory? factory, IEnumerable<string> userNames)
		{
			foreach (var userName in userNames)
			{
				await GetGitHubInfo(factory, userName);
			}
		}
		public static async Task GetDogImage(IHttpClientFactory? factory)
		{
			string cid = Guid.NewGuid().ToString().Substring(0, 3);
			WriteLine($"GetDogImage::StartProcess  {cid}");
			
			// NOTE: I am leaving out all error trapping for clarity
			HttpClient client = factory.CreateClient(nameof(HttpClientNames.DogApi));
			var response = await client.GetAsync("/api/breeds/image/random");
			var json = response.Content.ReadAsStringAsync().Result;		
			WriteHeader($"Dog Request Data  {cid}");
			WriteLine(json);
		}
		public static async Task GetDogImages(IHttpClientFactory? factory, int count)
		{
			for (int i = 0; i < count; i++)
			{
				await GetDogImage(factory);
			}
		}
		
		#region Write to Console Wrappers
		private static void WriteResponse(string response, bool error = false)
		{
			Console.ForegroundColor = error ? ConsoleColor.Red: ConsoleColor.Green;
			WriteLine($"");
			WriteLine(response);
			WriteLine($"---------------------------------------------");
			Console.ResetColor();
		}
		private static void WriteJson(string json, ConsoleColor color=ConsoleColor.Cyan)
		{
			Console.ForegroundColor = color;
			WriteLine($"");
			WriteLine(json);
			Console.ResetColor();
		}
		public static void WriteHeader(string header)
		{
			Console.ForegroundColor = ConsoleColor.Magenta;
			WriteLine($"");
			WriteLine(header);
			WriteLine($"---------------------------------------------");
			Console.ResetColor();
		}
		public static void WriteLine(string line, ConsoleColor color = ConsoleColor.DarkGray)
		{
			Console.WriteLine(line);
		}
		#endregion
	}
}