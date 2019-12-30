using HtmlAgilityPack;
using System.IO;
using System.Net;
using System.Text;

namespace StudentVueAPI {
	public static class BrowserSession {
		public static HtmlDocument HtmlDoc { get; set; }
		public static string ResponseUri { get; set; }

		static CookieContainer cookieContainer = new CookieContainer();

		/// <summary>
		/// Performs an HTTP GET request to a given URL. Returns the response body
		/// </summary>
		public static string GET(string _url) {
			// Create the GET request
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_url);
			request.Method = "GET";
			AddCookiesTo(request);

			// Get the response
			HttpWebResponse response = (HttpWebResponse) request.GetResponse();
			ResponseUri = response.ResponseUri.ToString();
			SaveCookiesFrom(response);

			// Read the response
			StreamReader sr = new StreamReader(response.GetResponseStream());
			string responseBodyText = sr.ReadToEnd();
			ParseResponseBody(responseBodyText);
			sr.Close();

			return responseBodyText;
		}

		/// <summary>
		/// Performs an HTTP POST request to a given URL with a given request body and content type. Returns the response body
		/// </summary>
		public static string POST(string _url, string _data, string _contentType) {
			// Create the POST request
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_url);
			AddCookiesTo(request);
			request.Method = "POST";
			request.ContentType = _contentType;
			AddPostData(request, _data);

			// Get the response
			HttpWebResponse response = (HttpWebResponse) request.GetResponse();
			ResponseUri = response.ResponseUri.ToString();
			SaveCookiesFrom(response);

			// Read the response
			StreamReader sr = new StreamReader(response.GetResponseStream());
			string responseBodyText = sr.ReadToEnd();
			ParseResponseBody(responseBodyText);
			sr.Close();

			return responseBodyText;
		}

		/// <summary>
		/// Adds the body content to the POST request
		/// </summary>
		private static void AddPostData(HttpWebRequest _request, string _data) {
			byte[] buffer = Encoding.UTF8.GetBytes(_data.ToCharArray());
			_request.ContentLength = buffer.Length;
			Stream requestStream = _request.GetRequestStream();
			requestStream.Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Adds existing cookies to the request
		/// </summary>
		private static void AddCookiesTo(HttpWebRequest _request) {
			_request.CookieContainer = cookieContainer;
		}

		/// <summary>
		/// Saves new cookies from the response
		/// </summary>
		private static void SaveCookiesFrom(HttpWebResponse _response) {
			if (_response.Cookies.Count > 0) {
				cookieContainer.Add(_response.Cookies);
			}
		}

		/// <summary>
		/// Saves the response body. Could also be a JSON body
		/// </summary>
		/// <param name="_responseBodyText"></param>
		private static void ParseResponseBody(string _responseBodyText) {
			HtmlDoc = new HtmlDocument();
			HtmlDoc.LoadHtml(_responseBodyText);
		}
	}
}
