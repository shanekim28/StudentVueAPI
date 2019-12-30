using HtmlAgilityPack;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace StudentVueAPI {
	public class StudentVue : StudentVueAPI {
		private string _districtDomain;

		/// <summary>
		/// Returns a list of Grade objects
		/// </summary>
		public override List<Grade> GetGradeBook() {
			List<Grade> list = new List<Grade>();

			// Get the gradebook and pull data from it
			BrowserSession.GET(string.Format("https://{0}/PXP2_Gradebook.aspx?AGU=0", _districtDomain));
			HtmlNode table = BrowserSession.HtmlDoc.DocumentNode.SelectSingleNode("//tbody");
			HtmlNodeCollection courseNames = table.SelectNodes(".//button[@class='btn btn-link course-title']");
			HtmlNodeCollection courseMarks = table.SelectNodes(".//span[@class='mark']");
			HtmlNodeCollection courseGrades = table.SelectNodes(".//span[@class='score']");
			for (int i = 0; i < courseNames.Count; i++) {
				list.Add(new Grade(courseNames[i].InnerHtml.Substring(3), courseMarks[i].InnerHtml, float.Parse(courseGrades[i].InnerHtml.Substring(0, courseGrades[i].InnerHtml.Length - 1))));
			}

			return list;
		}

		public override GPARank GetGPARank() {
			// Get the course history page and pull data from it
			BrowserSession.GET(string.Format("https://{0}/PXP2_CourseHistory.aspx?AGU=0", _districtDomain));
			HtmlNode score = BrowserSession.HtmlDoc.DocumentNode.SelectSingleNode("//span[@class='gpa-score']");
			HtmlNode rank = BrowserSession.HtmlDoc.DocumentNode.SelectSingleNode("//span[@class='gpa-rank hide']");

			return new GPARank(score.InnerText, rank.InnerText);
		}

		/// <summary>
		/// Returns a list of Course objects
		/// </summary>
		public override List<Course> GetSchedule() {
			List<Course> list = new List<Course>();

			// Init load options
			LoadOptions loadOptions = new LoadOptions();
			loadOptions.group = null;
			loadOptions.requireTotalCount = true;
			loadOptions.searchOperation = "contains";
			loadOptions.searchValue = null;
			loadOptions.skip = 0;
			loadOptions.sort = null;
			loadOptions.take = 15;

			// Get the schedule webpage and pull parameters from it
			BrowserSession.GET(string.Format("https://{0}/PXP2_ClassSchedule.aspx?AGU=0&VDT=None", _districtDomain));
			HtmlNode jsNode = BrowserSession.HtmlDoc.DocumentNode.SelectSingleNode("//script[@type='text/javascript' and contains(., 'PXPRemote')]");
			Match shortenedJsNode = Regex.Match(jsNode.InnerHtml, "new DevExpress.PXPRemoteDataStore\\(.*");
			Match pullJSON = Regex.Match(shortenedJsNode.ToString(), "'([^']*)'");
			string name = pullJSON.Groups[1].ToString();
			string parameters = pullJSON.NextMatch().Groups[1].ToString();

			// Craft a new json request and send it to StudentVue's backend
			RequestObject requestObject = new RequestObject();
			requestObject.request = new Request();
			requestObject.request.agu = 0;
			requestObject.request.dataRequestType = "Load";
			requestObject.request.dataSourceTypeName = name;
			requestObject.request.gridParameters = parameters;
			requestObject.request.loadOptions = loadOptions;
			string postResponse = BrowserSession.POST(string.Format("https://{0}/service/PXP2Communication.asmx/DXDataGridRequest", _districtDomain), JsonConvert.SerializeObject(requestObject), "application/json");

			// Get the data
			Schedule schedule = JsonConvert.DeserializeObject<Schedule>(postResponse);
			foreach (Datum d in schedule.D.Data.data) {
				Teacher t = JsonConvert.DeserializeObject<Teacher>(d.Teacher);
				SynergyMailToInfo s = JsonConvert.DeserializeObject<SynergyMailToInfo>(t.synergyMailToInfo);
				list.Add(new Course(d.Period, d.CourseTitle, t.teacherName, d.RoomName));
			}

			return list;
		}

		/// <summary>
		/// Returns a SchoolInfo object
		/// </summary>
		public override SchoolInfo GetSchoolInfo() {
			Dictionary<string, string> info = new Dictionary<string, string>();

			// Get the school's information page and pull data from it
			BrowserSession.GET(string.Format("https://{0}/PXP2_SchoolInformation.aspx?AGU=0", _districtDomain));
			HtmlNode table = BrowserSession.HtmlDoc.DocumentNode.SelectSingleNode("//table[@class='info_tbl table table-bordered']");
			HtmlNodeCollection infoNodes = table.SelectNodes(".//td");
			foreach (HtmlNode node in infoNodes) {
				string key = node.SelectSingleNode(".//span").InnerText;
				node.RemoveChild(node.SelectSingleNode(".//span"));
				node.InnerHtml = node.InnerHtml.Replace("<br>", " ");
				node.InnerHtml = node.InnerHtml.Replace("<i>", "");
				info.Add(key, node.InnerText.Trim());
			}

			return JsonConvert.DeserializeObject<SchoolInfo>(JsonConvert.SerializeObject(info));

		}

		/// <summary>
		/// Returns a StudentInfo object
		/// </summary>
		public override StudentInfo GetStudentInfo() {
			Dictionary<string, string> info = new Dictionary<string, string>();

			// Get the student's account page and pull data from it
			BrowserSession.GET(string.Format("https://{0}/PXP2_MyAccount.aspx?AGU=0", _districtDomain));
			HtmlNode table = BrowserSession.HtmlDoc.DocumentNode.SelectSingleNode("//table[@class='info_tbl table table-bordered']");
			HtmlNodeCollection infoNodes = table.SelectNodes(".//td");
			foreach (HtmlNode node in infoNodes) {
				string key = node.SelectSingleNode(".//span").InnerText;
				node.RemoveChild(node.SelectSingleNode(".//span"));
				node.InnerHtml = node.InnerHtml.Replace("<br>", " ");
				node.InnerHtml = node.InnerHtml.Replace("<i>", "");
				info.Add(key, node.InnerText.Trim());
			}

			// Separate phone numbers through a Regex match
			MatchCollection matches = Regex.Matches(info["Phone Numbers"], "(\\d{3}-\\d{3}-\\d{4})");
			string a = "";
			foreach (Match match in matches) {
				a += " " + match;
			}
			info["Phone Numbers"] = a.Substring(1);

			return JsonConvert.DeserializeObject<StudentInfo>(JsonConvert.SerializeObject(info));
		}

		/// <summary>
		/// Logs into StudentVue. The districtDomain is the domain name of the school's StudentVue portal
		/// </summary>
		public override void Login(string username, string password, string districtDomain) {
			_districtDomain = districtDomain;
			BrowserSession.GET(string.Format("https://{0}/PXP2_Login_Student.aspx?regenerateSessionId=True", districtDomain));
			FormElementCollection formElements = new FormElementCollection(BrowserSession.HtmlDoc);
			formElements["ctl00$MainContent$username"] = username;
			formElements["ctl00$MainContent$password"] = password;
			string loginData = formElements.AssembleLoginPayload();

			string postLogin = BrowserSession.POST(string.Format("https://{0}/PXP2_Login_Student.aspx?regenerateSessionId=True", districtDomain), loginData, "application/x-www-form-urlencoded");
		}
	}

}
