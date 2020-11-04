using GZipTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GZipTest
{
    static class SerializationExtensions
    {
        public const int Magic = 0x41414556;
        public const int BlockInfoSize = sizeof(int) + sizeof(long) * 4;

        public static void ReserveHeader(this BinaryWriter binaryWriter, int numBlocks)
        {
            var headerSize = sizeof(int) + sizeof(int) + BlockInfoSize * numBlocks;
            binaryWriter.Seek(headerSize, SeekOrigin.Begin);
        }

        public static void WriteHeader(this BinaryWriter binaryWriter, IList<BlockInfo> blockInfos)
        {
            binaryWriter.Seek(0, SeekOrigin.Begin);
            binaryWriter.Write(Magic);
            binaryWriter.Write(blockInfos.Count);
            foreach (var blockInfo in blockInfos)
            {
                WriteBlockInfo(binaryWriter, blockInfo);
            }
        }

        public static IList<BlockInfo> ReadHeader(this BinaryReader binaryReader)
        {
            if (binaryReader.ReadInt32() != Magic) throw new InvalidDataException("Signature mismatch");
            var length = binaryReader.ReadInt32();
            return Enumerable.Range(0, length).Select(_ => ReadBlockInfo(binaryReader)).ToList();
        }

        static void WriteBlockInfo(BinaryWriter binaryWriter, BlockInfo blockInfo)
        {
            binaryWriter.Write(blockInfo.BlockIndex);
            binaryWriter.Write(blockInfo.BlockOffset);
            binaryWriter.Write(blockInfo.BlockSize);
            binaryWriter.Write(blockInfo.OriginalBlockOffset);
            binaryWriter.Write(blockInfo.OriginalBlockSize);
        }

        static BlockInfo ReadBlockInfo(BinaryReader binaryReader)
        {
            var blockIndex = binaryReader.ReadInt32();
            var blockOffset = binaryReader.ReadInt64();
            var blockSize = binaryReader.ReadInt64();
            var originalBlockOffset = binaryReader.ReadInt64();
            var originalBlockSize = binaryReader.ReadInt64();
            return new BlockInfo
            {
                BlockIndex = blockIndex,
                BlockOffset = blockOffset,
                BlockSize = blockSize,
                OriginalBlockOffset = originalBlockOffset,
                OriginalBlockSize = originalBlockSize
            };
        }
    }
}
