using System;

namespace Sgk.Libs.NDashButton
{
    /// <summary>
    /// A EventArgs class of pushed event.
    /// </summary>
    public class DashButtonPushedEventArgs
    {
        /// <summary>
        /// Gets the pushed button info.
        /// </summary>
        public DashButton Button { get; private set; }

        /// <summary>
        /// Constructs a object.
        /// </summary>
        /// <param name="button"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DashButtonPushedEventArgs(DashButton button)
        {
            this.Button = button ?? throw new ArgumentNullException(nameof(button));
        }
    }
}
