using System;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace Footswitch
{

    [Serializable]
    public class FootswitchException : Exception
    {
        public FootswitchException() {}
        public FootswitchException(string message) : base(message) {}
    }

    public class FootswitchListener
    {

        public delegate void PedalReturn();
        public static UsbDevice UsbDevice;
        public static UsbDeviceFinder UsbFinder;
        private bool _isListening;
        private Thread _workThread;
        private const int Delay = 200;
        private UsbEndpointReader _reader;

        public FootswitchListener()
        {
            UsbFinder = new UsbDeviceFinder(0x0C45, 0x7403);
        }

        public FootswitchListener(int? vendorId, int? productId)
        {
            var vendor = vendorId ?? 0x0C45;
            var product = productId ?? 0x7403;

            UsbFinder = new UsbDeviceFinder(vendor, product);
        }

        public event PedalReturn Press;

        public event PedalReturn Release;

        protected virtual void OnPress()
        {
            var handler = Press;
            if (handler != null) handler();
        }

        protected virtual void OnRelease()
        {
            var handler = Release;
            if (handler != null) handler();
        }

        public void StartListen()
        {
            UsbDevice = UsbDevice.OpenUsbDevice(UsbFinder);
            if (UsbDevice == null) throw new FootswitchException("Footswitch not found");

            var wholeUsbDevice = UsbDevice as IUsbDevice;
            if (!ReferenceEquals(wholeUsbDevice, null))
            {
                // Выбираем конфигурацию #1
                wholeUsbDevice.SetConfiguration(1);
                // Выбираем интерфейс #0.
                wholeUsbDevice.ClaimInterface(0);
            }

            // Выбираем первый эндпоинт
            _reader = UsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);

            // Запускаем тред
            var ts = new ThreadStart(Work);
            _workThread = new Thread(ts);
            _workThread.Start();
            _isListening = true;
        }

        private void Work()
        {
            while (_isListening)
            {
                var readBuffer = new byte[8];

                int bytesRead;

                _reader.Read(readBuffer, Delay, out bytesRead);

                // Действие на сигналу
                // 15000000
                // 20000000
                if (readBuffer[0] + readBuffer[1] == 6 ) OnPress();
                // Действие на сигналу
                // 20000000
                // 10000000
                if (readBuffer[0] + readBuffer[1] == 1) OnRelease();
            }
        }

        public void StopListen()
        {
            if (!_isListening) throw new FootswitchException("Footswitch is not listening now.");
            while (_workThread.ThreadState == ThreadState.Running)
            {
            }
            _isListening = false;
        }

    }
}
