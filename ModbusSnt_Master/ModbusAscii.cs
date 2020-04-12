using System;
using System.Net.Sockets;
using System.IO.Ports;

namespace ModbusSnt
{
    class ModbusAscii : ModbusADU
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Constructor
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        public ModbusAscii(NetworkStream tcpClient)
        {
            m_physicalLayer = PhysicalLayer.TCP;
        }


        //=====================================================================
        public ModbusAscii(SerialPort serialPort)
        {
            m_physicalLayer = PhysicalLayer.Serial;
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Implementation Abstract Class
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        public override byte[] SendReceiveMessage(ushort idTransaction, byte idUnit, byte[] pdu)
        {
            throw new NotImplementedException();
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Fields 
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        PhysicalLayer m_physicalLayer;
        enum PhysicalLayer
        {
            TCP,
            Serial,
        }

        #endregion
    }
}
