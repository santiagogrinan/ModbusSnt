using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO.Ports;

namespace ModbusSnt
{
    public class ModbusMaster
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Constructor
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        private ModbusMaster() { }

        //=====================================================================
        private ModbusMaster(ModbusADU adu)
        {
            m_modbusADU = adu;
        }

        //=====================================================================
        public static ModbusMaster CreateRtu(NetworkStream networkStream)
        {
            return new ModbusMaster(new ModbusRTU(networkStream));
        }

        //=====================================================================
        public static ModbusMaster CreateRtu(SerialPort serialPort)
        {
            return new ModbusMaster(new ModbusRTU(serialPort));
        }

        //=====================================================================
        public static ModbusMaster CreateAscii(NetworkStream networkStream)
        {
            return new ModbusMaster(new ModbusAscii(networkStream));
        }

        //=====================================================================
        public static ModbusMaster CreateAscii(SerialPort serialPort)
        {
            return new ModbusMaster(new ModbusAscii(serialPort));
        }

        //=====================================================================
        public static ModbusMaster CreateTCP(NetworkStream networkStream)
        {
            return new ModbusMaster(new ModbusTCP(networkStream));
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Public Function
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        /// <summary>
        /// Modbus Read Coils
        /// </summary>
        /// <param name="startAddress"> Start address starting by 1 </param> 
        /// <param name="quantityOfCoils">Number of coils to be read</param> 
        /// <param name="idTransaction">Used in Modbus TCP and Not Used in RTU</param> 
        /// <param name="idUnit">Used in Modbus RTU and TCP</param> 
        /// <returns>Coils Reader</returns> 
        //=====================================================================
        public bool[] ReadCoils(ushort startAddress, ushort quantityOfCoils, ushort idTransaction, byte idUnit)
        {
            byte[] pduRequest = ModbusPDU.GenerateRequest_ReadCoils(startAddress, quantityOfCoils);

            byte[] pduResponse = m_modbusADU.SendReceiveMessage(idTransaction, idUnit, pduRequest);

            ModbusPDU.CheckErrors.CheckExcepcionResponse(pduResponse);

            ModbusPDU.CheckErrors.CheckFunctionCode(pduRequest, pduResponse);

            byte byteCounter;
            bool[] response = ModbusPDU.GetResponse_ReadCoils(pduResponse, quantityOfCoils, out byteCounter);

            ModbusPDU.CheckErrors.CheckParameterLenght(byteCounter, response);

            return response;
        }

        //=====================================================================
        public bool[] ReadDiscreteInputs(ushort startAddress, ushort quantityOfInputs, ushort idTransaction, byte idUnit)
        {
            byte[] pduRequest = ModbusPDU.GenerateRequest_ReadDiscreteInputs(startAddress, quantityOfInputs);

            byte[] pduResponse = m_modbusADU.SendReceiveMessage(idTransaction, idUnit, pduRequest);

            ModbusPDU.CheckErrors.CheckExcepcionResponse(pduResponse);

            ModbusPDU.CheckErrors.CheckFunctionCode(pduRequest, pduResponse);

            byte byteCounter;
            bool[] response = ModbusPDU.GetResponse_ReadDiscreteInputs(pduResponse, quantityOfInputs, out byteCounter);

            ModbusPDU.CheckErrors.CheckParameterLenght(byteCounter, response);

            return response;

        }

        //=====================================================================
        public ushort[] ReadHoldingRegister(ushort startAddress, ushort quantityOfInputs, ushort idTransaction, byte idUnit)
        {
            byte[] pduRequest = ModbusPDU.GenerateRequest_ReadHoldingRegister(startAddress, quantityOfInputs);

            byte[] pduResponse = m_modbusADU.SendReceiveMessage(idTransaction, idUnit, pduRequest);

            ModbusPDU.CheckErrors.CheckExcepcionResponse(pduResponse);

            ModbusPDU.CheckErrors.CheckFunctionCode(pduRequest, pduResponse);

            byte byteCounter;
            ushort[] response = ModbusPDU.GetResponse_ReadHoldingRegisters(pduResponse, out byteCounter);

            ModbusPDU.CheckErrors.CheckParameterLenght(byteCounter, response);

            return response;
        }

        //=====================================================================
        public ushort[] ReadInputRegisters(ushort startAddress, ushort quantityOfInputRegisters, ushort idTransaction, byte idUnit)
        {
            byte[] pduRequest = ModbusPDU.GenerateRequest_ReadInputRegisters(startAddress, quantityOfInputRegisters);

            byte[] pduResponse = m_modbusADU.SendReceiveMessage(idTransaction, idUnit, pduRequest);

            ModbusPDU.CheckErrors.CheckExcepcionResponse(pduResponse);

            ModbusPDU.CheckErrors.CheckFunctionCode(pduRequest, pduResponse);

            byte byteCounter;
            ushort[] response = ModbusPDU.GetResponse_ReadInputRegisters(pduResponse, out byteCounter);

            ModbusPDU.CheckErrors.CheckParameterLenght(byteCounter, response);

            return response;
        }

        //=====================================================================
        /// <summary>
        /// Write Single Coil 
        /// </summary>
        /// <param name="outputAddress"></param>
        /// <param name="outputValue"></param>
        /// <param name="idTransaction"></param>
        /// <param name="idUnit"></param>
        //=====================================================================
        public void WriteSingleCoil(ushort outputAddress, bool outputValue, ushort idTransaction, byte idUnit)
        {
            byte[] pduRequest = ModbusPDU.GenerateRequest_WriteSingleCoil(outputAddress, outputValue);

            byte[] pduResponse = m_modbusADU.SendReceiveMessage(idTransaction, idUnit, pduRequest);

            ModbusPDU.CheckErrors.CheckExcepcionResponse(pduResponse);

            ModbusPDU.CheckErrors.CheckFunctionCode(pduRequest, pduResponse);

            ushort addressResponse;
            bool valueResponse;
            ModbusPDU.GetResponse_WriteSingleCoil(pduResponse, out addressResponse, out valueResponse);


            ModbusPDU.CheckErrors.CheckParameter("Address", outputAddress, addressResponse);
            ModbusPDU.CheckErrors.CheckParameter("Register Value", outputValue, valueResponse);
        }

        //=====================================================================
        /// <summary>
        /// Write Single Register
        /// </summary>
        /// <param name="registerAddress"></param>
        /// <param name="registerValue"></param>
        /// <param name="idTransaction"></param>
        /// <param name="idUnit"></param>
        //=====================================================================
        public void WriteSingleRegister(ushort registerAddress, ushort registerValue, ushort idTransaction, byte idUnit)
        {
            byte[] pduRequest = ModbusPDU.GenerateRequest_WriteSingleRegister(registerAddress, registerValue);

            byte[] pduResponse = m_modbusADU.SendReceiveMessage(idTransaction, idUnit, pduRequest);

            ModbusPDU.CheckErrors.CheckExcepcionResponse(pduResponse);

            ModbusPDU.CheckErrors.CheckFunctionCode(pduRequest, pduResponse);

            ushort addressResponse, valueResponse;
            ModbusPDU.GetResponse_WriteSingleRegister(pduResponse, out addressResponse, out valueResponse);

            ModbusPDU.CheckErrors.CheckParameter("Address", registerAddress, addressResponse);
            ModbusPDU.CheckErrors.CheckParameter("Register Value", registerValue, valueResponse);
        }

        //=====================================================================
        /// <summary>
        /// Write Multiple Coils.
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="quantityOfOutputs"></param>
        /// <param name="outputsValues"> bool array where index 0 is writen in <paramref name= "startAddress"></paramref></param>
        /// <param name="idTransaction"></param>
        /// <param name="idUnit"></param>
        //=====================================================================
        public void WriteMultipleCoils(ushort startAddress, bool[] outputsValues, ushort idTransaction, byte idUnit)
        {
            byte[] pduRequest = ModbusPDU.GenerateRequest_WriteMultipleCoils(startAddress, outputsValues);

            byte[] pduResponse = m_modbusADU.SendReceiveMessage(idTransaction, idUnit, pduRequest);

            ModbusPDU.CheckErrors.CheckExcepcionResponse(pduResponse);

            ModbusPDU.CheckErrors.CheckFunctionCode(pduRequest, pduResponse);

            ushort addressResponse, quantityOfOutputsResponse;
            ModbusPDU.GetResponse_WriteMultipleCoils(pduResponse, out addressResponse, out quantityOfOutputsResponse);

            ModbusPDU.CheckErrors.CheckParameter("Address", startAddress, addressResponse);
            ModbusPDU.CheckErrors.CheckParameter("Quantity Of Outputs", (ushort)outputsValues.Length, quantityOfOutputsResponse);
        }

        //=====================================================================
        public void WriteMultipleRegister(ushort startAddress, ushort[] registersValue, ushort idTransaction, byte idUnit)
        {
            byte[] pduRequest = ModbusPDU.GenerateRequest_WriteMultipleRegister(startAddress, registersValue);

            byte[] pduResponse = m_modbusADU.SendReceiveMessage(idTransaction, idUnit, pduRequest);

            ModbusPDU.CheckErrors.CheckExcepcionResponse(pduResponse);

            ModbusPDU.CheckErrors.CheckFunctionCode(pduRequest, pduResponse);           

            ushort addressResponse, quantityOfRegisterResponse;
            ModbusPDU.GetResponse_WriteMultipleRegister(pduResponse, out addressResponse, out quantityOfRegisterResponse);

            ModbusPDU.CheckErrors.CheckParameter("Address", startAddress, addressResponse);
            ModbusPDU.CheckErrors.CheckParameter("Quantity Of Register", (ushort)registersValue.Length, quantityOfRegisterResponse);
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Fields
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        ModbusADU m_modbusADU;

        #endregion
    }

    public class ModbusExcepcion : Exception
    {
        private ModbusExcepcion() { }

        public ModbusExcepcion(string message) : base(message) { }

        private ModbusExcepcion(string message, Exception inner) : base(message, inner) { }
    }
}
