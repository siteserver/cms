using System;
using System.Net;
using System.Management;

namespace BaiRong.Core.Diagnostics
{
	/// <summary>
	/// 获取运行程序的计算机信息的类
	/// </summary>
	public sealed class ComputerUtils
	{
		private ComputerUtils()
		{
		}


		/// <summary>
		/// 获取计算机的网卡地址，并将网卡地址中的分隔符“:”除去
		/// </summary>
		/// <returns>计算机的网卡地址</returns>
		public static string GetMacAddress()
		{
			var macAddress = CacheUtils.Get("ComputerUtils_MacAddress") as String;
			if (string.IsNullOrEmpty(macAddress))
			{
				macAddress = string.Empty;
				try
				{
					var mcMAC = new ManagementClass("Win32_NetworkAdapterConfiguration");
					var mocMAC = mcMAC.GetInstances();
					foreach(ManagementObject m in mocMAC)
					{
						if((bool)m["IPEnabled"])
						{
							macAddress = m["MacAddress"].ToString();
							break;
						}
					}
				}
				catch{}
				macAddress = macAddress.Replace(":", "");
				CacheUtils.Max("ComputerUtils_MacAddress", macAddress);
			}
			return macAddress;
		}


		/// <summary>
		/// 获取计算机的CPU标识
		/// </summary>
		/// <returns>计算机的CPU标识</returns>
		public static string GetProcessorId()
		{
			var processorId = CacheUtils.Get("ComputerUtils_ProcessorId") as String;
			if (string.IsNullOrEmpty(processorId))
			{
				processorId = string.Empty;
				try
				{
					var mcCpu = new ManagementClass("win32_Processor");
					var mocCpu = mcCpu.GetInstances();
					foreach(ManagementObject m in mocCpu)
					{
						processorId = m["ProcessorId"].ToString();
					}
				}
				catch{}
				processorId = processorId.Replace(":", "");
				CacheUtils.Max("ComputerUtils_ProcessorId", processorId);
			}
			return processorId;
		}


		/// <summary>
		/// 获取计算机的硬盘序列号
		/// </summary>
		/// <returns>计算机的硬盘序列号</returns>
		public static string GetColumnSerialNumber()
		{
			var columnSerialNumber = CacheUtils.Get("ComputerUtils_ColumnSerialNumber") as String;
			if (string.IsNullOrEmpty(columnSerialNumber))
			{
				columnSerialNumber = string.Empty;
				try
				{
					var mcHD = new ManagementClass("win32_logicaldisk");
					var mocHD = mcHD.GetInstances();
					foreach(ManagementObject m in mocHD)
					{
						if(m["DeviceID"].ToString() == "C:")
						{
							columnSerialNumber = m["VolumeSerialNumber"].ToString();
							break;
						}
					}
				}
				catch{}
				columnSerialNumber = columnSerialNumber.Replace(":", "");
				CacheUtils.Max("ComputerUtils_ColumnSerialNumber", columnSerialNumber);
			}
			return columnSerialNumber;
		}


		/// <summary>
		/// 获取计算机的标识，标识由网卡地址、CPU标识以及硬盘序列号组成，中间用“:”分隔
		/// </summary>
		/// <returns></returns>
		public static string GetComputerID()
		{
			return $"{GetMacAddress()}:{GetProcessorId()}:{GetColumnSerialNumber()}";
		}


		/// <summary>
		/// 得到主机名称
		/// </summary>
		/// <returns></returns>
		public static string GetHostName()
		{
			return Dns.GetHostName().ToUpper();
		}

        //public static string GetIP()
        //{
        //    if (HttpContext.Current != null)
        //    {
        //        if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
        //        {
        //            return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
        //        }
        //        else
        //        {
        //            return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //        }
        //    }
        //    return string.Empty;
        //}
	}
}
