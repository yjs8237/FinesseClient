using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;



namespace CTIFnClient
{
    class UTIL
    {
        public string AuthBase64_IDAndPw(String ID, String PWD)
        {
        //    MemoryStream ms = new MemoryStream(new byte[2048]);

            byte[] buffer = new byte[2048];
            //ms.Capacity = 2048;
            //ms.Position = 0;

            byte[] nullByte = new byte[1];
            nullByte[0] = 0x00;

            int nOffSet = 0;
            System.Array.Copy(nullByte, buffer, nullByte.Length);
            nOffSet += 1;
            
            byte[] idByte = Encoding.UTF8.GetBytes(ID);
            Buffer.BlockCopy(idByte, 0, buffer, nOffSet, idByte.Length);
            nOffSet += idByte.Length;


            Buffer.BlockCopy(nullByte, 0, buffer, nOffSet, nullByte.Length);
            nOffSet += 1;

            byte[] pwdByte = Encoding.UTF8.GetBytes(PWD);
            Buffer.BlockCopy(pwdByte, 0, buffer, nOffSet, pwdByte.Length);
            nOffSet += pwdByte.Length;

            return System.Convert.ToBase64String(buffer.Take(nOffSet).ToArray());
          
        }

        public String lpad(string str ,  string addStr,int len)
        {
            string returnStr = "";

            if (str == null)
            {
                return null;
            }

            int temp = len - str.Length;

            for (int i = 0; i < len - str.Length; i++)
            {
                returnStr += addStr;
            }

            str = returnStr + str;

            return str;
        }
    }
}
