using System;
using System.Net.Sockets;
using System.IO.Ports;

namespace ModbusSnt
{
    abstract class ModbusADU
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Abstract Class
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        public abstract byte[] SendReceiveMessage(ushort idTransaction, byte idUnit, byte[] pdu);

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Fields
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        public class ModbusADUExcepcion : ModbusExcepcion
        {
            public ModbusADUExcepcion(string message, string modbusADU_type) : base("ModbusADU: " + modbusADU_type + ": " + message) { }
        }
        #endregion
    }
}
