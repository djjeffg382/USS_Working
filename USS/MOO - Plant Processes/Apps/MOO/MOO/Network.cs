using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel;

namespace MOO
{
    /// <summary>
    /// functions used for IP based calculations
    /// </summary>
    public static class Network
    {
        /// <summary>
        /// gets the start and end IP address of a given network and mask
        /// </summary>
        /// <param name="NetworkAddress"></param>
        /// <param name="BitMask"></param>
        /// <returns></returns>
        public static IPAddress[] GetIpRange(IPAddress NetworkAddress, int BitMask)
        {
            uint mask = ~(uint.MaxValue >> BitMask);

            // Convert the IP address to bytes.
            byte[] ipBytes = NetworkAddress.GetAddressBytes();

            // BitConverter gives bytes in opposite order to GetAddressBytes().
            byte[] maskBytes = BitConverter.GetBytes(mask).Reverse().ToArray();

            byte[] startIPBytes = new byte[ipBytes.Length];
            byte[] endIPBytes = new byte[ipBytes.Length];

            // Calculate the bytes of the start and end IP addresses.
            for (int i = 0; i < ipBytes.Length; i++)
            {
                startIPBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
                endIPBytes[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
            }

            // Convert the bytes to IP addresses.
            IPAddress startIP = new(startIPBytes);
            IPAddress endIP = new(endIPBytes);

            return new IPAddress[] { startIP, endIP };
        }

        /// <summary>
        /// gets the start and end IP address of a given network and mask
        /// </summary>
        /// <param name="NetworkAddress"></param>
        /// <param name="SubNet"></param>
        /// <returns></returns>
        public static IPAddress[] GetIpRange(IPAddress NetworkAddress, IPAddress SubNet)
        {
            int mask = SubnetToBitMask(SubNet);
            return GetIpRange(NetworkAddress, mask);
        }

        /// <summary>
        /// Gets the broadcast address on a given subnet
        /// </summary>
        /// <param name="address"></param>
        /// <param name="subnetMask"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }


        /// <summary>
        /// Gets the network address of the specified IP and subnet
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="SubnetMask"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IPAddress GetNetworkAddress(IPAddress Address, IPAddress SubnetMask)
        {
            byte[] ipAdressBytes = Address.GetAddressBytes();
            byte[] subnetMaskBytes = SubnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }

        /// <summary>
        /// Determines if 2 ip addresses are in the same subnet
        /// </summary>
        /// <param name="Address1"></param>
        /// <param name="Address2"></param>
        /// <param name="SubnetMask"></param>
        /// <returns></returns>
        public static bool IsInSameSubnet(IPAddress Address1, IPAddress Address2, IPAddress SubnetMask)
        {
            IPAddress network1 = GetNetworkAddress(Address1, SubnetMask);
            IPAddress network2 = GetNetworkAddress(Address2, SubnetMask);

            return network1.Equals(network2);
        }




        /// <summary>
        /// converts an IP Subnet mask to bit count
        /// </summary>
        /// <param name="SubnetMask"></param>
        /// <returns></returns>
        public static int SubnetToBitMask(IPAddress SubnetMask)
        {

            byte[] ipBytes = SubnetMask.GetAddressBytes();
            return NumberOfSetBits(ipBytes[0]) + NumberOfSetBits(ipBytes[1]) + NumberOfSetBits(ipBytes[2]) + NumberOfSetBits(ipBytes[3]);
        }

        private static int NumberOfSetBits(int i)
        {
            i -= ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }



        /// <summary>
        /// Returns a list of all IPs in a subnet
        /// </summary>
        /// <param name="startIP"></param>
        /// <param name="endIP"></param>
        /// <returns></returns>
        public static List<string> GetIpList(IPAddress startIP,
            IPAddress endIP)
        {
            uint sIP = IpToUint(startIP.GetAddressBytes());
            uint eIP = IpToUint(endIP.GetAddressBytes());
            List<string> result = new();
            while (sIP <= eIP)
            {
                result.Add( new IPAddress(ReverseBytesArray(sIP)).ToString());
                sIP++;
            }
            return result;
        }


        /* reverse byte order in array */
        private static uint ReverseBytesArray(uint ip)
        {
            byte[] bytes = BitConverter.GetBytes(ip);
            bytes = bytes.Reverse().ToArray();
            return (uint)BitConverter.ToInt32(bytes, 0);
        }


        /* Convert bytes array to 32 bit long value */
        private static uint IpToUint(byte[] ipBytes)
        {
            ByteConverter bConvert = new();
            uint ipUint = 0;

            int shift = 24; // indicates number of bits left for shifting
            foreach (byte b in ipBytes)
            {
                if (ipUint == 0)
                {
                    ipUint = (uint)bConvert.ConvertTo(b, typeof(uint)) << shift;
                    shift -= 8;
                    continue;
                }

                if (shift >= 8)
                    ipUint += (uint)bConvert.ConvertTo(b, typeof(uint)) << shift;
                else
                    ipUint += (uint)bConvert.ConvertTo(b, typeof(uint));

                shift -= 8;
            }

            return ipUint;
        }


    }
}
