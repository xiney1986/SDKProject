using System;

public class Crc32Utils
{
    protected static ulong[] Crc32Table;
    //生成CRC32码表
    public static void GetCRC32Table()
    {
        ulong Crc;
        Crc32Table = new ulong[256];
        int i, j;
        for (i = 0; i < 256; i++)
        {
            Crc = (ulong)i;
            for (j = 8; j > 0; j--)
            {
                if ((Crc & 1) == 1)
                    Crc = (Crc >> 1) ^ 0xEDB88320;
                else
                    Crc >>= 1;
            }
            Crc32Table[i] = Crc;
        }
    }

    //获取字符串的CRC32校验值
    public static ulong GetCRC32Str(byte[] bytes)
    {
        if (Crc32Table == null)
        {
            //生成码表
            GetCRC32Table();
        }
        ulong value = 0xffffffff;
        for (int i = 0; i < bytes.Length; i++)
        {
            value = (value >> 8) ^ Crc32Table[(value & 0xFF) ^ bytes[i]];
        }
        return value ^ 0xffffffff;
    }
}