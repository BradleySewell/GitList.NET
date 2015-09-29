using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GitList.Core.Entities.Serialization
{
    public class BinarySerializer
    {
        public static void SaveFile(object ItemToSerialize, string SaveFileLocation)
        {
            FileStream writeStream = null;
            if (File.Exists(SaveFileLocation))
            {
                writeStream = new FileStream(SaveFileLocation, FileMode.Truncate, FileAccess.Write, FileShare.Delete | FileShare.ReadWrite);
            }
            else
            {
                writeStream = new FileStream(SaveFileLocation, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Delete | FileShare.ReadWrite);
            }
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(writeStream, ItemToSerialize);
            writeStream.Close();

            File.SetAttributes(SaveFileLocation, FileAttributes.Hidden);

        }

        public static object LoadFile(string FilePathToDeserialize)
        {
            try
            {
                FileStream readStream = new FileStream(FilePathToDeserialize, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                var deserializeResponse = formatter.Deserialize(readStream);
                readStream.Close();
                return deserializeResponse;
            }
            catch
            {
                return null;
            }
        }
    }
}
