using System.Diagnostics;

namespace Standard.Web.Mqtt
{
	/// <summary>
	/// Log levels
	/// </summary>
	public enum LogLevel
	{
		Error = 0x01,
		Warning = 0x02,
		Information = 0x04,
		Verbose = 0x0F,
		Frame = 0x10,
		Queuing = 0x20
	}

	// delegate for writing trace
	public delegate void WriteLog(string format, params object[] args);

	/// <summary>
	/// MQTT helper class
	/// </summary>
	public static class MqttLogger
    {
		public static WriteLog LogWriter;
		public static LogLevel LogLevelPreference = LogLevel.Error;

		public static void WriteLine(LogLevel level, string format)
		{
			if (LogWriter != null && (level & LogLevelPreference) > 0)
				LogWriter(format);
		}

		public static void WriteLine(LogLevel level, string format, object arg1)
		{
			if (LogWriter != null && (level & LogLevelPreference) > 0)
				LogWriter(format, arg1);
		}

		public static void WriteLine(LogLevel level, string format, object arg1, object arg2)
		{
			if (LogWriter != null && (level & LogLevelPreference) > 0)
				LogWriter(format, arg1, arg2);
		}

		public static void WriteLine(LogLevel level, string format, object arg1, object arg2, object arg3)
		{
			if (LogWriter != null && (level & LogLevelPreference) > 0)
				LogWriter(format, arg1, arg2, arg3);
		}
	}
}
