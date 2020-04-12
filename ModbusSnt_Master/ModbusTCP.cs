using System;
using System.Net.Sockets;

namespace ModbusSnt
{
    class ModbusTCP : ModbusADU
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Constructor
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 


        //=====================================================================
        public ModbusTCP(NetworkStream networkStream)
        {
            m_networkStream = networkStream;
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Implementation Abstract Class
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        public override byte[] SendReceiveMessage(ushort idTransaction, byte idUnit, byte[] pduRequest)
        {
            byte[] sendMessage = GenerateMessage(idTransaction, idUnit, pduRequest);

            //envio y recibo
            byte[] responseMessage = SendReceiveMessage(sendMessage);

            ushort idTransactionResponse, pduLenghtResponse;
            byte idUnitResponse;
            byte[] pduResponse = GetPdu(responseMessage, out idTransactionResponse, out pduLenghtResponse, out idUnitResponse);

            CheckPduLenght(pduResponse, pduLenghtResponse);

            CheckIdTransaction(idTransaction, idTransactionResponse);

            CheckIdUnit(idUnit, idUnitResponse);

            return pduResponse;
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Implementation 
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        byte[] SendReceiveMessage(byte[] sendMessage)
        {
            return SendReceiveMessage(m_networkStream, sendMessage, ModbusPDU.Utils.ResponseLenghtWanted(GetPdu(sendMessage)));
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Static Functions 
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        static byte[] SendReceiveMessage(NetworkStream networkSpeed, byte[] sendMessage, ushort responsePDUWanted)
        {
            networkSpeed.Write(sendMessage, 0, sendMessage.Length);

            ushort responseLenghtWanted = (ushort)(responsePDUWanted + 7);
            byte[] responseMessage = new byte[responseLenghtWanted];

            networkSpeed.Read(responseMessage, 0, responseMessage.Length);
            return responseMessage;
        }

        //=====================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTransaction"></param> It is used for transaction pairing, the MODBUS server copies in the response the transaction identifier of the request. 
        /// <param name="lenght"></param>  The  length  field  is  a  byte  count  of  the  following  fields,  including  the  Unit  Identifier and data fields.
        /// <param name="idUnit"></param>  This field is used for intra-system routing purpose
        /// <returns></returns>
        static byte[] Generate_MbapHeader(ushort idTransaction, ushort pduLenght, byte idUnit)
        {
            byte[] result = new byte[7];

            byte[] idTransactionByte = UtilsSnt.BitConverterSnt.GetBytes(idTransaction);
            result[0] = idTransactionByte[0];
            result[1] = idTransactionByte[1];

            byte[] idProtocolByte = UtilsSnt.BitConverterSnt.GetBytes(c_idProtocol);
            result[2] = idProtocolByte[0];
            result[3] = idProtocolByte[1];

            byte[] lenghtByte = UtilsSnt.BitConverterSnt.GetBytes((ushort)(pduLenght + 1)); //no se porque ostias el que tenemos hace +5  
            result[4] = lenghtByte[0];
            result[5] = lenghtByte[1];

            result[6] = idUnit;

            return result;
        }

        //=====================================================================
        static byte[] GenerateMessage(ushort idTransaction, byte idUnit, byte[] pdu)
        {
            byte[] mbap = Generate_MbapHeader(idTransaction, (ushort)pdu.Length, idUnit);

            return GenerateMessage(mbap, pdu);
        }

        //=====================================================================
        static byte[] GenerateMessage(byte[] mbap, byte[] pdu)
        {
            byte[] result = new byte[mbap.Length + pdu.Length];
            Array.Copy(mbap, 0, result, 0, mbap.Length);
            Array.Copy(pdu, 0, result, mbap.Length, pdu.Length);
            return result;
        }

        //=====================================================================
        static byte[] GetPdu(byte[] input, out byte [] mbap)
        {
            mbap = new byte[7];
            Array.Copy(input, 0, mbap, 0, mbap.Length);

            ushort pduLenght = (ushort)(UtilsSnt.BitConverterSnt.ToUInt16(mbap, 4) - 1);

            byte[] result = new byte[pduLenght];
            Array.Copy(input, mbap.Length, result, 0, result.Length);

            return result;
        }

        //=====================================================================
        static void GetMbapParameter(byte[] mbap, out ushort idTransaction, out ushort pduLenght, out byte idUnit)
        {
            idTransaction = UtilsSnt.BitConverterSnt.ToUInt16(mbap, 0);
            pduLenght = (ushort)(UtilsSnt.BitConverterSnt.ToUInt16(mbap, 4) - 1);
            idUnit = mbap[6];

        }

        //=====================================================================
        static byte[] GetPdu(byte[] input, out ushort idTransaction, out ushort pduLenght, out byte idUnit)
        {
            byte[] mbap;

            byte[] pdu = GetPdu(input, out mbap);

            GetMbapParameter(mbap, out idTransaction, out pduLenght, out idUnit);

            return pdu;
        }

        //=====================================================================
        static byte[] GetPdu(byte[] input)
        {
            byte[] mbap;
            return GetPdu(input, out mbap);
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region CheckErrors 
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        //=====================================================================
        static void CheckPduLenght(byte[] pduResponse, ushort pduLenghtResponse)
        {
            if (pduResponse.Length != pduLenghtResponse) throw new ModbusTCPExcepcion("PDU Response Lenght and PDU Lenght Response are Different");
        }

        //=====================================================================
        static void CheckIdTransaction(ushort idTransactionRequest, ushort idTransactionResponse)
        {
            if (idTransactionRequest != idTransactionResponse) throw new ModbusTCPExcepcion("Id Transaction Request and Id Transaction Response are different");
        }

        //=====================================================================
        static void CheckIdUnit(byte idUnitRequest, byte idUnitResponse)
        {
            if (idUnitRequest != idUnitResponse) throw new ModbusTCPExcepcion("Id Unit Request and Id Unit Response are different");
        }

        #endregion
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #region Fields 
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: 

        /// <summary>
        ///  It is used for intra-system multiplexing. The MODBUS protocol is identified by the value 0.
        /// </summary>
        const ushort c_idProtocol = 0;

        NetworkStream m_networkStream;

        class ModbusTCPExcepcion : ModbusADUExcepcion
        {
            public ModbusTCPExcepcion(string message) : base(message, "ModbusTCP") { }
        }

        //All MODBUS/TCP ADU are sent via TCP to registered port 502.

        #endregion
    }
}
