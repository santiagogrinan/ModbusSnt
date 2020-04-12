using System;

namespace ModbusSnt
{
    static class ModbusPDU
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Request
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::  

        //=====================================================================
        public static byte[] GenerateRequest_ReadCoils(ushort startAddress, ushort quantityOfCoils)
        {
            byte[] result = new byte[5];

            result[0] = (byte)FunctionCode.ReadCoils;

            byte[] addressBytes = UtilsSnt.BitConverterSnt.GetBytes((ushort)(startAddress -1));
            result[1] = addressBytes[0];
            result[2] = addressBytes[1];

            byte[] lenghtBytes = UtilsSnt.BitConverterSnt.GetBytes(quantityOfCoils);
            result[3] = lenghtBytes[0];
            result[4] = lenghtBytes[1];

            return result;
        }

        //=====================================================================
        public static byte[] GenerateRequest_ReadDiscreteInputs(ushort startAddress, ushort quantityOfInputs)
        {
            byte[] result = new byte[5];

            result[0] = (byte)FunctionCode.ReadDiscreteInputs;

            byte[] addressBytes = UtilsSnt.BitConverterSnt.GetBytes((ushort)(startAddress - 1));
            result[1] = addressBytes[0];
            result[2] = addressBytes[1];

            byte[] lenghtBytes = UtilsSnt.BitConverterSnt.GetBytes(quantityOfInputs);
            result[3] = lenghtBytes[0];
            result[4] = lenghtBytes[1];

            return result;
        }

        //=====================================================================
        public static byte[] GenerateRequest_ReadHoldingRegister(ushort startAddress, ushort quantityOfInputs)
        {
            byte[] result = new byte[5];

            result[0] = (byte)FunctionCode.ReadHoldingRegister;

            byte[] addressBytes = UtilsSnt.BitConverterSnt.GetBytes((ushort)(startAddress - 1));
            result[1] = addressBytes[0];
            result[2] = addressBytes[1];

            byte[] lenghtBytes = UtilsSnt.BitConverterSnt.GetBytes(quantityOfInputs);
            result[3] = lenghtBytes[0];
            result[4] = lenghtBytes[1];

            return result;
        }

        //=====================================================================
        public static byte[] GenerateRequest_ReadInputRegisters(ushort startAddress, ushort quantityOfInputRegisters)
        {
            byte[] result = new byte[5];

            result[0] = (byte)FunctionCode.ReadInputRegisters;

            byte[] addressBytes = UtilsSnt.BitConverterSnt.GetBytes((ushort)(startAddress - 1));
            result[1] = addressBytes[0];
            result[2] = addressBytes[1];

            byte[] lenghtBytes = UtilsSnt.BitConverterSnt.GetBytes(quantityOfInputRegisters);
            result[3] = lenghtBytes[0];
            result[4] = lenghtBytes[1];

            return result;
        }

        //=====================================================================
        public static byte[] GenerateRequest_WriteSingleCoil(ushort outputAddress, bool outputValue)
        {
            byte[] result = new byte[5];

            ushort outputValueUshort = outputValue ? c_valueUshortTrueWriteSingleCoil : (ushort)0;

            result[0] = (byte)FunctionCode.WriteSingleCoil;

            byte[] addressBytes = UtilsSnt.BitConverterSnt.GetBytes((ushort)(outputAddress - 1));
            result[1] = addressBytes[0];
            result[2] = addressBytes[1];
            
            byte[] outputValueByte = UtilsSnt.BitConverterSnt.GetBytes(outputValueUshort);
            result[3] = outputValueByte[0];
            result[4] = outputValueByte[1];

            return result;
        }

        //=====================================================================
        public static byte[] GenerateRequest_WriteSingleRegister(ushort registerAddress, ushort registerValue)
        {
            byte[] result = new byte[5];

            result[0] = (byte)FunctionCode.WriteSingleRegister;

            byte[] addressBytes = UtilsSnt.BitConverterSnt.GetBytes((ushort)(registerAddress - 1));
            result[1] = addressBytes[0];
            result[2] = addressBytes[1];

            byte[] lenghtBytes = UtilsSnt.BitConverterSnt.GetBytes(registerValue);
            result[3] = lenghtBytes[0];
            result[4] = lenghtBytes[1];

            return result;
        }

        //=====================================================================
        public static byte[] GenerateRequest_WriteMultipleCoils(ushort startAddress, bool[] outputsValues) 
        {
            ushort quantityOfCoils = (ushort)outputsValues.Length;

            byte[] outputValuesByte = UtilsSnt.BoolConverterSnt.ToByte(outputsValues);            

            byte[] result = new byte[6 + outputValuesByte.Length];

            result[0] = (byte)FunctionCode.WriteMultipleCoils;

            byte[] addressBytes = UtilsSnt.BitConverterSnt.GetBytes((ushort)(startAddress - 1));
            result[1] = addressBytes[0];
            result[2] = addressBytes[1];

            byte[] quantityOfOutputsBytes = UtilsSnt.BitConverterSnt.GetBytes(quantityOfCoils);
            result[3] = quantityOfOutputsBytes[0];
            result[4] = quantityOfOutputsBytes[1];

            result[5] = (byte)outputValuesByte.Length;

            for (int i = 0; i < outputValuesByte.Length; i++)
                result[6 + i] = outputValuesByte[i];

            return result;
        }

        //=====================================================================
        public static byte[] GenerateRequest_WriteMultipleRegister(ushort startAddress, ushort[] registersValue)
        {
            byte[] result = new byte[6 + registersValue.Length * 2];

            result[0] = (byte)FunctionCode.WriteMultipleRegister;

            byte[] addressBytes = UtilsSnt.BitConverterSnt.GetBytes((ushort)(startAddress - 1));
            result[1] = addressBytes[0];
            result[2] = addressBytes[1];

            byte[] quantityOfRegisterBytes = UtilsSnt.BitConverterSnt.GetBytes((ushort)registersValue.Length);
            result[3] = quantityOfRegisterBytes[0];
            result[4] = quantityOfRegisterBytes[1];

            result[5] = (byte)(registersValue.Length * 2);

            for (int i = 0; i < registersValue.Length; i++)
            {
                byte[] registerByte = UtilsSnt.BitConverterSnt.GetBytes(registersValue[i]);
                result[6 + (2*i)] = registerByte[0];
                result[6 + (2*i) + 1] = registerByte[1];
            }

            return result;
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Response
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::  

        //=====================================================================
        public static bool[] GetResponse_ReadCoils(byte[] pdu, int quantyOfBytesWanted, out byte byteCount)
        {
            byte[] resultByte;

            byteCount = pdu[1];
            resultByte = new byte[byteCount];
            Array.Copy(pdu, 2, resultByte, 0, byteCount);

            return UtilsSnt.BoolConverterSnt.ToBool(resultByte, quantyOfBytesWanted);
        }

        //=====================================================================
        public static bool[] GetResponse_ReadDiscreteInputs(byte[] pdu, int quantyOfBytesWanted, out byte byteCount)
        {
            byte[] resultByte;

            byteCount = pdu[1];
            resultByte = new byte[byteCount];
            Array.Copy(pdu, 2, resultByte, 0, byteCount);

            return UtilsSnt.BoolConverterSnt.ToBool(resultByte, quantyOfBytesWanted);
        }

        //=====================================================================
        public static ushort[] GetResponse_ReadHoldingRegisters(byte[] pdu, out byte byteCount)
        {
            byte[] resultByte;

            byteCount = pdu[1];
            resultByte = new byte[byteCount];
            Array.Copy(pdu, 2, resultByte, 0, byteCount);

            ushort[] result = new ushort[byteCount / 2];
            for (int i = 0; i < result.Length; i++)
                result[i] = UtilsSnt.BitConverterSnt.ToUInt16(resultByte, i * 2);

            return result;
        }

        //=====================================================================
        public static ushort[] GetResponse_ReadInputRegisters(byte[] pdu, out byte byteCount)
        {
            byte[] resultByte;

            byteCount = pdu[1];
            resultByte = new byte[byteCount];
            Array.Copy(pdu, 2, resultByte, 0, byteCount);

            ushort[] result = new ushort[byteCount / 2];
            for(int i = 0; i < result.Length; i++)            
                result[i] = UtilsSnt.BitConverterSnt.ToUInt16(resultByte, i*2);         

            return result;
        }

        //=====================================================================
        public static void GetResponse_WriteSingleCoil(byte[] pdu, out ushort outputAddress, out bool outputValue)
        {
            outputAddress = (ushort)(UtilsSnt.BitConverterSnt.ToUInt16(pdu, 1) + 1);
            ushort outputValueUshort = UtilsSnt.BitConverterSnt.ToUInt16(pdu, 3);
            outputValue = outputValueUshort == c_valueUshortTrueWriteSingleCoil;
        }

        //=====================================================================
        public static void GetResponse_WriteSingleRegister(byte[] pdu, out ushort outputAddress, out ushort outputValue)
        {
            outputAddress = (ushort)(UtilsSnt.BitConverterSnt.ToUInt16(pdu, 1) + 1);
            outputValue = UtilsSnt.BitConverterSnt.ToUInt16(pdu, 3);
        }

        //=====================================================================
        public static void GetResponse_WriteMultipleCoils(byte[] pdu, out ushort startAddress, out ushort quantityOfOutputs)
        {
            startAddress = (ushort)(UtilsSnt.BitConverterSnt.ToUInt16(pdu, 1) + 1);
            quantityOfOutputs = UtilsSnt.BitConverterSnt.ToUInt16(pdu, 3);
        }

        //=====================================================================
        public static void GetResponse_WriteMultipleRegister(byte[] pdu, out ushort startAddress, out ushort quantityOfRegisters)
        {
            startAddress = (ushort)(UtilsSnt.BitConverterSnt.ToUInt16(pdu, 1) + 1);
            quantityOfRegisters = UtilsSnt.BitConverterSnt.ToUInt16(pdu, 3);
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Response Excepcion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::  

        //=====================================================================
        public static void GetResponseExcepcion(byte[] input, out FunctionCode function, out ExcepcionCode excepcion)
        {
            function = (FunctionCode)(input[0] - c_errorResponseOffset - 1);
            excepcion = (ExcepcionCode)input[1];
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Utils
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::  
        public static class Utils
        {
            //=====================================================================
            public static FunctionCode GetFunctionCode(byte[] pdu)
            {
                return (FunctionCode)pdu[0];
            }

            //=====================================================================
            public static bool IsExcepcionResponse(byte[] pdu)
            {
                return pdu[0] > c_errorResponseOffset;
            }

            //=====================================================================
            public static ushort ResponseLenghtWanted(byte[] pduRequest) 
            {

                FunctionCode functionCode = GetFunctionCode(pduRequest);

                ushort functionResponseLenght = 1;

                //usados en funciopnes de lectura
                ushort byteCounterResponseLenght = 1;
                ushort byteCounterResponseWanted;

                //usados en funciones de escritura
                ushort addressResponseLenght = 2;


                switch (functionCode)
                {
                    case FunctionCode.ReadCoils:
                        ushort quantityOfCoils = UtilsSnt.BitConverterSnt.ToUInt16(pduRequest, 3);
                        byteCounterResponseWanted = (ushort)((quantityOfCoils / 8) + ((quantityOfCoils % 8) > 0 ? 1 : 0));
                        return (ushort)(functionResponseLenght + byteCounterResponseLenght + byteCounterResponseWanted);

                    case FunctionCode.ReadDiscreteInputs:
                        ushort quantityOfInputs = UtilsSnt.BitConverterSnt.ToUInt16(pduRequest, 3);
                        byteCounterResponseWanted = (ushort)((quantityOfInputs / 8) + ((quantityOfInputs % 8) > 0 ? 1 : 0));
                        return (ushort)(functionResponseLenght + byteCounterResponseLenght + byteCounterResponseWanted);

                    case FunctionCode.ReadHoldingRegister:
                        ushort quantityOfRegister = UtilsSnt.BitConverterSnt.ToUInt16(pduRequest, 3);
                        byteCounterResponseWanted = (ushort)(2* quantityOfRegister);
                        return (ushort)(functionResponseLenght + byteCounterResponseLenght + byteCounterResponseWanted);

                    case FunctionCode.ReadInputRegisters:
                        ushort quantityOfInputsRegister = UtilsSnt.BitConverterSnt.ToUInt16(pduRequest, 3);
                        byteCounterResponseWanted = (ushort)(2 * quantityOfInputsRegister);
                        return (ushort)(functionResponseLenght + byteCounterResponseLenght + byteCounterResponseWanted);

                    case FunctionCode.WriteSingleCoil:
                        ushort outputValueLenght = 2;
                        return (ushort)(functionResponseLenght + addressResponseLenght + outputValueLenght);

                    case FunctionCode.WriteSingleRegister:
                        ushort registerValueLenght = 2;
                        return (ushort)(functionResponseLenght + addressResponseLenght + registerValueLenght);

                    case FunctionCode.WriteMultipleCoils:
                        ushort quantityOfOutputsLenght = 2;
                        return (ushort)(functionResponseLenght + addressResponseLenght + quantityOfOutputsLenght);

                    case FunctionCode.WriteMultipleRegister:
                        ushort quantityOfRegisterLenght = 2;
                        return (ushort)(functionResponseLenght + addressResponseLenght + quantityOfRegisterLenght);

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static class CheckErrors
        {
            //=====================================================================
            public static void CheckFunctionCode(byte[] pduRequest, byte[] pduResponse)
            {
                if (Utils.GetFunctionCode(pduRequest) != Utils.GetFunctionCode(pduResponse)) throw new ModbusPDUExcepcion("Function Code Request and Function Code Response are different");
            }

            //=====================================================================
            public static void CheckParameter(string name, ushort valueWanted , ushort valueResponse)
            {
                if (valueWanted != valueResponse) throw new ModbusPDUExcepcion(name + " Wanted and " + name + " Response are different");
            }

            //=====================================================================
            public static void CheckParameter(string name, bool valueWanted, bool valueResponse)
            {
                if (valueWanted != valueResponse) throw new ModbusPDUExcepcion(name + " Wanted and " + name + " Response are different");
            }

            //=====================================================================
            public static void CheckParameterLenght(int byteCounter, bool[] result)
            {
                CheckParameterLenght(byteCounter, result.Length / 8 + ((result.Length % 8) > 0 ? 1 : 0));
            }

            //=====================================================================
            public static void CheckParameterLenght(int byteCounter, ushort[] result)
            {
                CheckParameterLenght(byteCounter, result.Length * 2);
            }

            //=====================================================================
            private static void CheckParameterLenght(int byteCounter, int byteResult)
            {
                if (byteCounter != byteResult) throw new ModbusPDUExcepcion("Lenght Wanted and Lenght Response are different");
            }


            //=====================================================================
            public static void CheckExcepcionResponse(byte[] pduResponse)
            {
                if (Utils.IsExcepcionResponse(pduResponse))
                {
                    FunctionCode functionCode;
                    ExcepcionCode excepcionCode;
                    GetResponseExcepcion(pduResponse, out functionCode, out excepcionCode);
                    throw new ModbusPDUResponseExcepcion(functionCode, excepcionCode);
                }
            }
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Fields
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>
        /// Represent by a Byte. For 1 to 255. For 128 to 255 is reserved for Excepcion responses
        /// 1 to 64 Public Function Codes
        /// 65 to 72 and 100 to 110 User-Defined Function Codes
        /// 73 to 99 and 111 to 127 Reserved Function Codes  
        /// </summary>
        public enum FunctionCode
        {
            #region Data Access
            #region Bit Access
            #region Physical Discrete Inputs
            ReadDiscreteInputs = 02,
            #endregion
            #region Internal Bits or Physical Coils
            ReadCoils = 01,
            WriteSingleCoil = 05,
            WriteMultipleCoils = 15,
            #endregion
            #endregion

            #region 16 bits Access
            #region Physical Discrete Registers
            ReadInputRegisters = 04,
            #endregion
            #region Internal Registers or Physical Outputs Registers
            ReadHoldingRegister = 03,
            WriteSingleRegister = 06,
            WriteMultipleRegister = 16,
            ReadWriteMultipleRegisters = 23,
            MaskWriteRegister = 22,
            ReadFifoQueue = 24,
            #endregion
            #endregion

            #region File Record Access
            ReadFileRecord = 20,
            WriteFileRecord = 21,
            #endregion

            #endregion

            #region Diagnostics
            ReadExcepcionStatus = 07,
            Diagnostic = 08, //Subindice 00-18, 20
            GetComEventCounter = 11,
            GetComEventLog = 12,
            ReportServerID = 17,
            ReadDeviceIdentification = 43, //Subindice = 14
            #endregion

            #region Other
            EncapsulatedInterfaceTransport = 43, // Subcode = 13,14
            CANopenGeneralReference = 43, //Subcode 13
            #endregion
        }

        public enum ExcepcionCode
        {
            InvalidFunctionCode = 1,
            InvalidDataAddress = 2,
            InvalidDataValue = 3,
            ServerDeviceFailure = 4, //5, 6 to
            Acknowledge = 5,
            ServerDeviceBusy = 6,
            MemoryParityError = 8,
            GatewayPathUnavailable = 10,
            GatewayTargetDeviceFailedToRespond = 11,
        }

        const ushort c_valueUshortTrueWriteSingleCoil = 0xFF00;

        const ushort c_errorResponseOffset = 127;

        public class ModbusPDUExcepcion : ModbusExcepcion
        {
            public ModbusPDUExcepcion(string message) : base("ModbusPDU: " + message) { }
        }

        public class ModbusPDUResponseExcepcion : ModbusPDUExcepcion
        {
            public ModbusPDUResponseExcepcion(FunctionCode function, ExcepcionCode excepcion) : base("Excepcion Response: Function: " + function + " Excepcion: " + excepcion) { }
        }



        //big-endian
        //data is addressed from 0 to 65535
        //data numered X is addressed in the Modbus PDU X-1

        #endregion
    }
}
