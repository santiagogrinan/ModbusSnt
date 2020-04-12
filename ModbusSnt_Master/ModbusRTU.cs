using System;
using System.Net.Sockets;
using System.IO.Ports;

namespace ModbusSnt
{
    class ModbusRTU : ModbusADU
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Constructor
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        public ModbusRTU(NetworkStream networkStream)
        {
            m_physicalLayer = PhysicalLayer.TCP;
            m_networkStream = networkStream;
        }


        //=====================================================================
        public ModbusRTU(SerialPort serialPort)
        {
            m_physicalLayer = PhysicalLayer.Serial;
            m_serialPort = serialPort;            
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Implementation Abstract Class
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        public override byte[] SendReceiveMessage(ushort idTransaction, byte idUnit, byte[] pduRequest)
        {
            //idTranscation is not used in this ADU
            byte[] sendMessage = GenerateMessage(idUnit, pduRequest);

            //send and recieve
            byte[] responseMessage = SendReceiveMessage(sendMessage);

            byte idUnitResponse;
            byte[] crcResponse;
            byte[] pduResponse = GetPdu(responseMessage, out idUnitResponse, out crcResponse);
            
            CheckIdUnit(idUnit, idUnitResponse);

            CheckCRC(idUnitResponse, pduResponse, crcResponse);

            return pduResponse;
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Implementation 
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        byte[] SendReceiveMessage(byte[] sendMessage)
        {
            switch (m_physicalLayer)
            {
                case PhysicalLayer.Serial: return SendReceiveMessage(m_serialPort, sendMessage, (ushort)(ModbusPDU.Utils.ResponseLenghtWanted(GetPdu(sendMessage)) + 3));
                case PhysicalLayer.TCP: return SendReceiveMessage(m_networkStream, sendMessage, (ushort)(ModbusPDU.Utils.ResponseLenghtWanted(GetPdu(sendMessage)) + 3));
                default: throw new NotImplementedException();
            }
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Static Function 
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        static byte[] SendReceiveMessage(SerialPort serialPort, byte[] sendMessage, ushort responseMessageLenghtWanted)
        {
            //Santi
            while (serialPort.BytesToRead > 0)
            {
                int despreciate = serialPort.ReadByte();
            }
            serialPort.Write(sendMessage, 0, sendMessage.Length);
            byte[] responseMessage = new byte[responseMessageLenghtWanted];

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < serialPort.ReadTimeout && serialPort.BytesToRead < responseMessageLenghtWanted) ;
            serialPort.Read(responseMessage, 0, responseMessageLenghtWanted);
            return responseMessage;
        }


        //=====================================================================
        static byte[] SendReceiveMessage(NetworkStream networkSpeed, byte[] sendMessage, ushort responseMessageLenghtWanted)
        {
            while (networkSpeed.DataAvailable)
            {
                int despreciate = networkSpeed.ReadByte();
            }
            networkSpeed.Write(sendMessage, 0, sendMessage.Length);
            byte[] responseMessage = new byte[responseMessageLenghtWanted];
            networkSpeed.Read(responseMessage, 0, responseMessageLenghtWanted);
            return responseMessage;
        }


        //=====================================================================
        static byte[] GenerateMessage (byte idUnit, byte[] pdu)
        {
            byte[] crc = Generate_CRC(idUnit, pdu);

            byte[] result = new byte[1 + pdu.Length + crc.Length];

            result[0] = idUnit;
            Array.Copy(pdu, 0, result, 1, pdu.Length);
            Array.Copy(crc, 0, result, pdu.Length + 1, crc.Length);

            return result;
        }


        //=====================================================================
        static byte[] Generate_CRC (byte idUnit, byte[] pdu)
        {
            byte[] messageWithoutCRC = new byte[pdu.Length + 1];
            messageWithoutCRC[0] = idUnit;
            Array.Copy(pdu, 0, messageWithoutCRC, 1, pdu.Length);

            return Generate_CRC(messageWithoutCRC);
        }


        //=====================================================================
        static byte[] Generate_CRC (byte[] messageWithoutCRC)
        {
            // 1. Load a 16–bit register with FFFF hex (all 1’s). Call this the CRC register.
            byte[] result = BitConverter.GetBytes((ushort)0XFFFF);


            for (int i = 0; i < messageWithoutCRC.Length; i++)
            {
                //2. Exclusive OR the first 8–bit byte of the message with the low–order byte of the 16–bit CRC register, putting the result in theCRC register.
                result[0] = (byte)(messageWithoutCRC[i] ^ result[0]);

                for (int j = 0; j < 8; j++)
                {
                    //3. Shift the CRC register one bit to the right (toward the LSB), zero–filling the MSB. Extract and examine the LSB
                    bool lsb = ByteToBool(result[0], 0);
                    ushort temRegister = BitConverter.ToUInt16(result, 0);
                    temRegister = (ushort)(temRegister >> 1);
                    result = BitConverter.GetBytes(temRegister);

                    // 4. (If the LSB was 0): Repeat Step 3(another shift).(If the LSB was 1): Exclusive OR the CRC register with the polynomial value 0xA001(1010 0000 0000 0001).
                    if (lsb)
                    {
                        ushort polynomialValue = 0xA001;
                        temRegister = BitConverter.ToUInt16(result, 0);
                        temRegister = (ushort)(temRegister ^ polynomialValue);
                        result = BitConverter.GetBytes(temRegister);
                    }

                    // 5. Repeat  Steps  3  and  4  until  8  shifts  have  been  performed.  When  this  is  done,  a  complete  8–bit  byte  will  have  beenprocessed.
                }
                // 6. Repeat Steps 2 through 5 for the next 8–bit byte of the message.Continue doing this until all bytes have been processed.
            }

            // 7. The final content of the CRC register is the CRC value.
            // 8. When the CRC is placed into the message, its upper and lower bytes must be swapped as described below.
            //byte high = result[0];
            //byte low = result[1];
            //result[0] = low;
            //result[1] = high;

            return result;
        }


        //===========================================================================
        static bool ByteToBool (byte register, byte index)
        {
            byte aux = (byte)((register >> index) & 1);

            if (aux > 0) return true;
            else return false;
        }


        //===========================================================================
        static byte[] GetPdu (byte[] input, out byte idUnit, out byte[] crc)
        {
            idUnit = input[0];
            byte[] result = null;

            result = new byte[input.Length - 3];
            Array.Copy(input, 1, result, 0, result.Length);

            crc = new byte[2];
            Array.Copy(input, input.Length - 2, crc, 0, crc.Length);
            return result;
        }


        //===========================================================================
        static byte[] GetPdu (byte[] input)
        {
            byte idUnit;
            byte[] crc;

            return GetPdu(input, out idUnit, out crc);
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region CheckErrors 
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        static void CheckIdUnit(byte idUnitRequest, byte idUnitResponse)
        {
            if (idUnitRequest != idUnitResponse) throw new ModbusRTUExcepcion("IdUnit Request is different to IdUnit Response");
        }


        //===========================================================================
        static void CheckCRC(byte idUnit, byte[] pdu, byte[] crcResponse)
        {
            byte[] crcWanted = Generate_CRC(idUnit, pdu);

            bool result = crcWanted[0] == crcResponse[0] && crcWanted[1] == crcResponse[1];

            if (!result)
                throw new ModbusRTUExcepcion("CRC Wanted and CRC Response are different");
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Fields 
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        SerialPort m_serialPort;
        NetworkStream m_networkStream;
        PhysicalLayer m_physicalLayer;
        enum PhysicalLayer
        {
            TCP,
            Serial,
        }

        class ModbusRTUExcepcion : ModbusADUExcepcion
        {
            public ModbusRTUExcepcion(string message) : base(message, "ModbusRTU") { }
        }

        //broadcast mode: don´t wait response, idUnit (address devie) is 0
        //0 Broadcast Address / 1 to 247 Slave Individual Addresses / 248 to 255 Reserved

        //Modbus Serial Line PDU: AddressField ModbusPDU CRC(or LRC)

        #endregion
    }
}
