using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;

namespace StudentVueAPI {
	public abstract class StudentVueAPI {
		public abstract void Login(string username, string password, string districtDomain);
		public abstract List<Course> GetSchedule();
		public abstract List<Grade> GetGradeBook();
		public abstract GPARank GetGPARank();
		public abstract StudentInfo GetStudentInfo();
		public abstract SchoolInfo GetSchoolInfo();
	}

	public class Course {
		public long Period { get; set; }
		public string CourseTitle { get; set; }
		public string TeacherName { get; set; }
		public string RoomName { get; set; }

		public Course(long period, string courseTitle, string teacherName, string roomName) {
			Period = period;
			CourseTitle = courseTitle;
			TeacherName = teacherName;
			RoomName = roomName;
		}
	}

	public class GPARank {
		public string GPA { get; set; }
		public string Rank { get; set; }

		public GPARank(string gpa, string rank) {
			GPA = gpa;
			Rank = rank;
		}
	}

	public class Grade {
		[JsonProperty("Course Title")]
		public string CourseTitle { get; set; }
		public string Mark { get; set; }
		public float Score { get; set; }

		public Grade(string courseTitle, string mark, float score) {
			CourseTitle = courseTitle;
			Mark = mark;
			Score = score;
		}
	}

	public class StudentInfo {
		public string Name { get; set; }
		[JsonProperty("User ID")]
		public string UserID { get; set; }
		[JsonProperty("Home Address")]
		public string HomeAddress { get; set; }
		[JsonProperty("Mail Address")]
		public string MailAddress { get; set; }
		[JsonProperty("Phone Numbers")]
		public string PhoneNumbers { get; set; }
	}

	public partial class SchoolInfo {
		public string Principal { get; set; }
		[JsonProperty("School Name")]
		public string SchoolName { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
		public string Fax { get; set; }
		[JsonProperty("Website URL")]
		public string WebsiteURL { get; set; }
	}


	public partial class Schedule {
		public d D { get; set; }
	}

	public partial class d {
		public string __type { get; set; }
		public object Error { get; set; }
		public Data Data { get; set; }
		public string DataType { get; set; }
	}

	public partial class Data {
		public bool success { get; set; }
		public Datum[] data { get; set; }
		public long TotalCount { get; set; }
	}

	public partial class Datum {
		public string ID { get; set; }
		public long Period { get; set; }
		public string CourseTitle { get; set; }
		public string RoomName { get; set; }
		public string Teacher { get; set; }
		public string AdditionalStaffName { get; set; }
	}

	public partial class Teacher {
		public string teacherName { get; set; }
		public string email { get; set; }
		public string agu { get; set; }
		public string sgu { get; set; }
		public string emailSubject { get; set; }
		public string recipientDetails1 { get; set; }
		public string recipientDetails2 { get; set; }
		public TeacherNameAndTeacherEmail[] teacherNameAndTeacherEmail { get; set; }
		public string synergyMailToInfo { get; set; }
		public string dataType { get; set; }
	}

	public partial class TeacherNameAndTeacherEmail {
		public string AdditionalStaffEmail { get; set; }
		public string AdditionalStafeName { get; set; }
		public string AdditionalStaffGU { get; set; }
	}

	public partial class SynergyMailToInfo {
		public To[] to { get; set; }
		public string subject { get; set; }
		public string messageText { get; set; }
	}

	public partial class To {
		public long RecipientList { get; set; }
		public string GU { get; set; }
	}

	public partial class RequestObject {
		public Request request { get; set; }
	}

	public partial class Request {
		public int agu { get; set; }
		public string dataRequestType { get; set; }
		public string dataSourceTypeName { get; set; }
		public object gridParameters { get; set; }
		public LoadOptions loadOptions { get; set; }
	}

	public partial class LoadOptions {
		public object group { get; set; }
		public bool requireTotalCount { get; set; }
		public string searchOperation { get; set; }
		public object searchValue { get; set; }
		public long skip { get; set; }
		public object sort { get; set; }
		public long take { get; set; }
	}

	public class FormElementCollection : Dictionary<string, string> {
		public FormElementCollection(HtmlDocument _doc) {
			HtmlNode form = _doc.DocumentNode.SelectSingleNode("//form[@name='aspnetForm']");
			HtmlNodeCollection inputs = form.SelectNodes(".//input");

			foreach (HtmlNode node in inputs) {
				Add(node.GetAttributeValue("name", ""), node.GetAttributeValue("value", ""));
			}
		}

		public string AssembleLoginPayload() {
			string sB = "";
			foreach (KeyValuePair<string, string> element in this) {
				string value = HttpUtility.UrlEncode(element.Value);
				sB += ("&" + element.Key + "=" + value);
			}

			return sB.ToString().Substring(1);
		}
	}
}
