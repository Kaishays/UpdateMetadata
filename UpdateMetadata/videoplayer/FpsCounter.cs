using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace KlvExtractor.VideoPlayer
{
    public static class FpsCounter
    {
        private static int _frameCount = 0;
        private static DateTime _lastTime = DateTime.Now;

        /// <summary>
        /// Call this method once every frame to update the FPS counter.
        /// </summary>
        public static void OnFrameArrived()
        {
            _frameCount++;
            Application.Current.Dispatcher.Invoke(() =>
            {
               // MainWindow.current.fps.Text = VidConVars.FPS.ToString();
            });
            // Compute elapsed time since last FPS update
            var now = DateTime.Now;
            var elapsed = (now - _lastTime).TotalSeconds;

            // Update FPS every 1 second or more
            if (elapsed >= 1.0)
            {
                // Frames per second = number of frames / seconds
                //VidConVars.FPS = _frameCount / elapsed;

                // Reset for the next cycle
                _frameCount = 0;
                _lastTime = now;
            }
        }
    }
}
