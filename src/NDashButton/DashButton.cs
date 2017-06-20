using System;
using System.Net.NetworkInformation;

namespace Sgk.Libs.NDashButton
{
    /// <summary>
    /// A class to hold dash button properties.
    /// </summary>
    public class DashButton
    {
        /// <summary>
        /// Gets name to identify the button.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets physical address of the button.
        /// </summary>
        public PhysicalAddress MacAddress { get; private set; }

        /// <summary>
        /// Constructs DashButton object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="macAddress"></param>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public DashButton(string name, string macAddress)
            : this(name, PhysicalAddress.Parse(macAddress))
        {
        }
        /// <summary>
        /// Constructs DashButton object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="macAddress"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DashButton(string name, PhysicalAddress macAddress)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.MacAddress = macAddress ?? throw new ArgumentNullException(nameof(macAddress));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = obj as DashButton;
            return this.Name == other.Name
                && this.MacAddress == other.MacAddress;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ MacAddress.GetHashCode();
        }
    }
}
