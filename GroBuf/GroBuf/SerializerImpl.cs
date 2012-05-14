using System.Runtime.InteropServices;

using GroBuf.DataMembersExtracters;

namespace GroBuf
{
    public class SerializerImpl
    {
        public SerializerImpl(IDataMembersExtracter dataMembersExtracter)
        {
            writer = new GroBufWriter(dataMembersExtracter);
            reader = new GroBufReader(dataMembersExtracter);
        }

        public int GetSize<T>(T obj)
        {
            return writer.GetSize(obj);
        }

        public void Serialize<T>(T obj, byte[] result, ref int index)
        {
            writer.Write(obj, result, ref index);
        }

        public byte[] Serialize<T>(T obj)
        {
            return writer.Write(obj);
        }

        public T Deserialize<T>(byte[] data)
        {
            return reader.Read<T>(data);
        }

        public T Deserialize<T>(byte[] data, ref int index)
        {
            return reader.Read<T>(data, ref index);
        }

        public void Merge<T>(T from, ref T to)
        {
            ChangeType(from, ref to);
        }

        public TTo ChangeType<TFrom, TTo>(TFrom obj)
        {
            TTo result = default(TTo);
            ChangeType(obj, ref result);
            return result;
        }

        public T Copy<T>(T obj)
        {
            return ChangeType<T, T>(obj);
        }

        private void ChangeType<TFrom, TTo>(TFrom obj, ref TTo result)
        {
            var size = writer.GetSize(obj);
            if(size <= 768)
            {
                var buf = new byte[size];
                int index = 0;
                writer.Write(obj, buf, ref index);
                reader.Read(buf, ref result);
            }
            else
            {
                var buf = Marshal.AllocHGlobal(size);
                try
                {
                    writer.Write(obj, buf);
                    int index = 0;
                    reader.Read(buf, ref index, size, ref result);
                }
                finally
                {
                    Marshal.FreeHGlobal(buf);
                }
            }
        }

        private readonly GroBufWriter writer;
        private readonly GroBufReader reader;
    }
}