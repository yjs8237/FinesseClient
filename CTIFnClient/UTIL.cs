using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace CTIFnClient
{
    class UTIL
    {
        public String AuthBase64_IDAndPw(String ID, String PWD)
        {
            int nul = 0x00;
            string strResult = Convert.ToString(nul) + ID + Convert.ToString(nul) + PWD;

            System.Text.Encoding encoding = System.Text.Encoding.UTF8;

            byte[] arr = encoding.GetBytes(strResult);

            return System.Convert.ToBase64String(arr);
          
        }
    }
}
