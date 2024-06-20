using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeedleX.Driver.Keyence
{
    public class GT2Class : IDisposable
    {

        public GT2Class() { }
        public GT2Class(string name) {
            _comName = name;
        }

        string _comName = string.Empty;
        SerialPort _serialPort = null;
        bool _GT2Opening = false;
        char[] _GT2Char = new char[512];
        public int GT2Open()
        {
            if (string.IsNullOrEmpty(_comName))
                return -1;

            if (_serialPort == null)
                _serialPort = new SerialPort(_comName, 9600, Parity.None, 8, StopBits.One);

            if (_serialPort.IsOpen)
                _serialPort.Close();

            _serialPort.Open();
            _GT2Opening = true;
            return 0;
        }
        public int GT2Close()
        {
            if (string.IsNullOrEmpty(_comName))
                return -1;
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
            _GT2Opening = false;
            return 0;
        }
        public double GT2GetCurrentValue(int id = 0)
        {
            double ret = 0;
            if (string.IsNullOrEmpty(_comName))
                return 0;
            if (!_GT2Opening)
                return ret;
            if (_serialPort == null)
                return ret;

            if (_serialPort.IsOpen)
            {
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();

                _serialPort.Write($"SR,{id.ToString("00")},002{'\x0D'}{'\x0A'}");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                while (true)
                {
                    if (stopwatch.ElapsedMilliseconds > 500)
                    {
                        stopwatch.Stop();
                        break;
                    }

                    int icount = _serialPort.Read(_GT2Char, 0, _serialPort.BytesToRead);
                    string str = new string(_GT2Char, 0, icount);
                    if (str.IndexOf(",+") > -1 || str.IndexOf(",-") > -1)
                    {
                        string[] strings = str.Split(',');
                        bool bOK = double.TryParse(strings[3], out ret);
                        //ret = Convert.ToDouble(strings[3]);
                        if (!bOK)
                            ret = -9999;
                    }
                }

            }

            return ret;
        }
        public void Dispose()
        {
            GT2Close();
        }
    }
}
