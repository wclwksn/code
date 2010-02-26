using System;
namespace UltraTutorial09
{
	public interface IAlphaWebService
	{
		void DownloadData(string url, DownloadDataResult result);
		void GetTime(string prefix, GetTimeResult result);
	}

	public interface IWebServiceEnabled
	{
		// primitives not yet supporter
		string IsEnabled { get; }
	}
}
