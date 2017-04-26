// Copyright by Bo Norgaard, All rights reserved.

using System;
using System.Text;
using System.Collections;


namespace BaiRong.Core
{
    /// <summary>
    /// Internal class for storing a range of IP numbers with the same IP mask
    /// </summary>
    internal class IPArrayList {
        private bool isSorted = false;
        private ArrayList ipNumList = new ArrayList();
        private uint ipmask;

        /// <summary>
        /// Constructor that sets the mask for the list
        /// </summary>
        public IPArrayList(uint mask) {
            ipmask = mask;
        }

        /// <summary>
        /// Add a new IP numer (range) to the list
        /// </summary>
        public void Add(uint IPNum) {
            isSorted = false;
            ipNumList.Add(IPNum & ipmask);
        }

        /// <summary>
        /// Checks if an IP number is within the ranges included by the list
        /// </summary>
        public bool Check(uint IPNum) {
            var found = false;
            if (ipNumList.Count>0) {
                if (!isSorted) {
                    ipNumList.Sort();
                    isSorted=true;
                }
                IPNum = IPNum & ipmask;
                if (ipNumList.BinarySearch(IPNum)>=0) found=true;
            }
            return found;
        }

        /// <summary>
        /// Clears the list
        /// </summary>
        public void Clear() {
            ipNumList.Clear();
            isSorted = false;
        }

        /// <summary>
        /// The ToString is overriden to generate a list of the IP numbers
        /// </summary>
        public override string ToString() {
            var buf = new StringBuilder();
            foreach (uint ipnum in ipNumList) {
                if (buf.Length>0) buf.Append("\r\n");
                buf.Append(((int)ipnum & 0xFF000000) >> 24).Append('.');
                buf.Append(((int)ipnum & 0x00FF0000) >> 16).Append('.');
                buf.Append(((int)ipnum & 0x0000FF00) >> 8).Append('.');
                buf.Append(((int)ipnum & 0x000000FF));
            }
            return buf.ToString();
        }

        /// <summary>
        /// The IP mask for this list of IP numbers
        /// </summary>
        public uint Mask => ipmask;
    }

    /// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class IPList
	{   private ArrayList ipRangeList = new ArrayList();
        private SortedList maskList = new SortedList();
        private ArrayList usedList = new ArrayList();

		public IPList()
		{
            // Initialize IP mask list and create IPArrayList into the ipRangeList
            uint mask = 0x00000000;
            for(var level = 1; level<33; level++) {
                mask = (mask >> 1) | 0x80000000;
                maskList.Add(mask,level);
                ipRangeList.Add(new IPArrayList(mask));
            }
        }

        // Parse a String IP address to a 32 bit unsigned integer
        // We can't use System.Net.IPAddress as it will not parse
        // our masks correctly eg. 255.255.0.0 is pased as 65535 !
        private uint parseIP(string IPNumber) {
            uint res = 0;
            var elements = IPNumber.Split(new Char[] {'.'});
            if (elements.Length==4) {
                res = (uint)Convert.ToInt32(elements[0])<<24;
                res += (uint)Convert.ToInt32(elements[1])<<16;
                res += (uint) Convert.ToInt32(elements[2])<<8;
                res += (uint) Convert.ToInt32(elements[3]);
            }
            return res;
        }

        /// <summary>
        /// Add a single IP number to the list as a string, ex. 10.1.1.1
        /// </summary>
        public void Add(string ipNumber) {
            Add(parseIP(ipNumber));
        }

        /// <summary>
        /// Add a single IP number to the list as a unsigned integer, ex. 0x0A010101
        /// </summary>
        public void Add(uint ip) {
            ((IPArrayList)ipRangeList[31]).Add(ip);
            if (!usedList.Contains((int)31)) {
                usedList.Add((int)31);
                usedList.Sort();
            }
        }

        /// <summary>
        /// Adds IP numbers using a mask for range where the mask specifies the number of
        /// fixed bits, ex. 172.16.0.0 255.255.0.0 will add 172.16.0.0 - 172.16.255.255
        /// </summary>
        public void Add(string ipNumber, string mask) {
            Add(parseIP(ipNumber),parseIP(mask));
        }

        /// <summary>
        /// Adds IP numbers using a mask for range where the mask specifies the number of
        /// fixed bits, ex. 0xAC1000 0xFFFF0000 will add 172.16.0.0 - 172.16.255.255
        /// </summary>
        public void Add(uint ip, uint umask) {
            var Level = maskList[umask];
            if (Level!=null) {
                ip = ip & umask;
                ((IPArrayList)ipRangeList[(int)Level-1]).Add(ip);
                if (!usedList.Contains((int)Level-1)) {
                    usedList.Add((int)Level-1);
                    usedList.Sort();
                }
            }
        }

        /// <summary>
        /// Adds IP numbers using a mask for range where the mask specifies the number of
        /// fixed bits, ex. 192.168.1.0/24 which will add 192.168.1.0 - 192.168.1.255
        /// </summary>
        public void Add(string ipNumber, int maskLevel) {
            Add(parseIP(ipNumber),(uint)maskList.GetKey(maskList.IndexOfValue(maskLevel)));
        }

        /// <summary>
        /// Adds IP numbers using a from and to IP number. The method checks the range and
        /// splits it into normal ip/mask blocks.
        /// </summary>
        public void AddRange(string fromIP, string toIP) {
            AddRange(parseIP(fromIP),parseIP(toIP));
        }

        /// <summary>
        /// Adds IP numbers using a from and to IP number. The method checks the range and
        /// splits it into normal ip/mask blocks.
        /// </summary>
        public void AddRange(uint fromIP, uint toIP) {
            // If the order is not asending, switch the IP numbers.
            if (fromIP>toIP) {
                var tempIP = fromIP;
                fromIP=toIP;
                toIP=tempIP;
            }
            if (fromIP==toIP) {
                Add(fromIP);
            } else {
                var diff = toIP-fromIP;
                var diffLevel = 1;
                var range = 0x80000000;
                if (diff<256) {
                    diffLevel = 24;
                    range = 0x00000100;
                }
                while (range>diff) {
                    range = range>>1;
                    diffLevel++;
                }
                var mask = (uint)maskList.GetKey(maskList.IndexOfValue(diffLevel));
                var minIP = fromIP & mask;
                if (minIP<fromIP) minIP+=range;
                if (minIP>fromIP) {
                    AddRange(fromIP,minIP-1);
                    fromIP=minIP;
                }
                if (fromIP==toIP) {
                    Add(fromIP);
                } else {
                    if ((minIP+(range-1))<=toIP) {
                        Add(minIP,mask);
                        fromIP = minIP+range;
                    }
                    if (fromIP==toIP) {
                        Add(toIP);
                    } else {
                        if (fromIP<toIP) AddRange(fromIP,toIP);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if an IP number is contained in the lists, ex. 10.0.0.1
        /// </summary>
        public bool CheckNumber(string ipNumber) {
            return CheckNumber(parseIP(ipNumber));;
        }

        /// <summary>
        /// Checks if an IP number is contained in the lists, ex. 0x0A000001
        /// </summary>
        public bool CheckNumber(uint ip) {
            var found = false;
            var i=0;
            while (!found && i<usedList.Count) {
                found = ((IPArrayList)ipRangeList[(int)usedList[i]]).Check(ip);
                i++;
            }
            return found;
        }

        /// <summary>
        /// Clears all lists of IP numbers
        /// </summary>
        public void Clear() {
            foreach (int i in usedList) {
                ((IPArrayList)ipRangeList[i]).Clear();
            }
            usedList.Clear();
        }

        /// <summary>
        /// Generates a list of all IP ranges in printable format
        /// </summary>
        public override string ToString() {
            var buffer = new StringBuilder();
            foreach (int i in usedList) {
                buffer.Append("\r\nRange with mask of ").Append(i+1).Append("\r\n");
                buffer.Append(((IPArrayList)ipRangeList[i]).ToString());
            }
            return buffer.ToString();
        }


	}
}
