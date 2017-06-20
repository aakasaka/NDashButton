using PacketDotNet;
using SharpPcap;
using SharpPcap.WinPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Sgk.Libs.NDashButton
{
    /// <summary>
    /// A listener class to handle the dash button pushes.
    /// </summary>
    public class DashButtonListener : IDisposable
    {
        private readonly ICaptureDevice epDevice;

        private readonly Dictionary<PhysicalAddress, DashButton> buttonDic
            = new Dictionary<PhysicalAddress, DashButton>();

        /// <summary>
        /// Fired when the reigistered buttons pushed.
        /// </summary>
        public event EventHandler<DashButtonPushedEventArgs> Pushed;

        /// <summary>
        /// Gets or sets interval to ignore the push as duplication.
        /// Default: 10 seconds.
        /// </summary>
        public TimeSpan DuplicateIgnoreInterval { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Gets or sets read timeout span.
        /// Default: 1 second.
        /// </summary>
        public TimeSpan ReadTimeout { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Constructs a listener. 
        /// </summary>
        /// <param name="deviceIpAddress">IP address of the endpoint which dash buttons connect to.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Thrown when no device matches with deviceIpAddress.</exception>
        public DashButtonListener(string deviceIpAddress)
        {
            if (deviceIpAddress == null)
            {
                throw new ArgumentNullException(nameof(deviceIpAddress));
            }
            if (deviceIpAddress == IPAddress.Any.ToString())
            {
                throw new ArgumentException("You should specify one IP address.");
            }

            //find device
            this.epDevice = CaptureDeviceList.Instance
                                             .OfType<WinPcapDevice>()
                                             .FirstOrDefault(d => d.Addresses.Any(a => a.Addr.ipAddress?.ToString() == deviceIpAddress));

            if (this.epDevice == null)
            {
                throw new ArgumentException("No device found : IP = " + deviceIpAddress);
            }

            this.epDevice.OnPacketArrival += handlePacketArrival;
        }

        /// <summary>
        /// Register a dash button.
        /// </summary>
        /// <param name="button"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Thrown when the button's mac address already registered.</exception>
        public void Register(DashButton button)
        {
            if (button == null) throw new ArgumentNullException(nameof(button));

            lock (buttonDic)
            {
                this.buttonDic.Add(button.MacAddress, button);  //throws ArgumentException
            }
        }

        /// <summary>
        /// Start capturing the packets.
        /// </summary>
        public void Start()
        {
            this.epDevice.Open(DeviceMode.Promiscuous, (int)this.ReadTimeout.TotalMilliseconds);
            this.epDevice.StartCapture();
        }


        private void handlePacketArrival(object sender, CaptureEventArgs e)
        {
            var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            while (packet != null)
            {
                if (packet is ARPPacket arp)
                {
                    handleArp(arp);
                }
                packet = packet.PayloadPacket;
            }
        }

        private void handleArp(ARPPacket arp)
        {
            DashButton button;
            lock (this.buttonDic)
            {
                if (!this.buttonDic.TryGetValue(arp.SenderHardwareAddress, out button))
                {
                    return;
                }
            }

            //fire the event on another thread.
            Task.Run(() =>
            {
                this.Pushed?.Invoke(this, new DashButtonPushedEventArgs(button));
            });
        }

        /// <summary>
        /// Stop capturing.
        /// </summary>
        public void Stop()
        {
            if (this.epDevice.Started)
            {
                this.epDevice.StopCapture();
            }
            this.epDevice.Close();
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }
}
