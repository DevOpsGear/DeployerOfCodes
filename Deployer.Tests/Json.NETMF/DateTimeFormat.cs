namespace Json.NETMF
{
	/// <summary>
	/// Enumeration of the popular formats of time and date
	/// within Json.  It's not a standard, so you have to
	/// know which on you're using.
	/// </summary>
	public enum DateTimeFormat
	{
		Default = 0,
		ISO8601 = 1,
		Ajax = 2
	}
}