/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements. See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

/*
 * This package is based on the work done by Keiron Liddle), Aftex Software
 * <keiron@aftexsw.com> to whom the Ant project is very grateful for his
 * great code.
 */

using System;

namespace Org.BouncyCastle.Apache.Bzip2
{
	/**
    * A simple class the hold and calculate the CRC for sanity checking
    * of the data.
    *
    * @author <a href="mailto:keiron@aftexsw.com">Keiron Liddle</a>
    */
    internal class CRC 
	{
        public static readonly int[] crc32Table = {
            unchecked((int)0x00000000), unchecked((int)0x04c11db7), unchecked((int)0x09823b6e), unchecked((int)0x0d4326d9),
            unchecked((int)0x130476dc), unchecked((int)0x17c56b6b), unchecked((int)0x1a864db2), unchecked((int)0x1e475005),
            unchecked((int)0x2608edb8), unchecked((int)0x22c9f00f), unchecked((int)0x2f8ad6d6), unchecked((int)0x2b4bcb61),
            unchecked((int)0x350c9b64), unchecked((int)0x31cd86d3), unchecked((int)0x3c8ea00a), unchecked((int)0x384fbdbd),
            unchecked((int)0x4c11db70), unchecked((int)0x48d0c6c7), unchecked((int)0x4593e01e), unchecked((int)0x4152fda9),
            unchecked((int)0x5f15adac), unchecked((int)0x5bd4b01b), unchecked((int)0x569796c2), unchecked((int)0x52568b75),
            unchecked((int)0x6a1936c8), unchecked((int)0x6ed82b7f), unchecked((int)0x639b0da6), unchecked((int)0x675a1011),
            unchecked((int)0x791d4014), unchecked((int)0x7ddc5da3), unchecked((int)0x709f7b7a), unchecked((int)0x745e66cd),
            unchecked((int)0x9823b6e0), unchecked((int)0x9ce2ab57), unchecked((int)0x91a18d8e), unchecked((int)0x95609039),
            unchecked((int)0x8b27c03c), unchecked((int)0x8fe6dd8b), unchecked((int)0x82a5fb52), unchecked((int)0x8664e6e5),
            unchecked((int)0xbe2b5b58), unchecked((int)0xbaea46ef), unchecked((int)0xb7a96036), unchecked((int)0xb3687d81),
            unchecked((int)0xad2f2d84), unchecked((int)0xa9ee3033), unchecked((int)0xa4ad16ea), unchecked((int)0xa06c0b5d),
            unchecked((int)0xd4326d90), unchecked((int)0xd0f37027), unchecked((int)0xddb056fe), unchecked((int)0xd9714b49),
            unchecked((int)0xc7361b4c), unchecked((int)0xc3f706fb), unchecked((int)0xceb42022), unchecked((int)0xca753d95),
            unchecked((int)0xf23a8028), unchecked((int)0xf6fb9d9f), unchecked((int)0xfbb8bb46), unchecked((int)0xff79a6f1),
            unchecked((int)0xe13ef6f4), unchecked((int)0xe5ffeb43), unchecked((int)0xe8bccd9a), unchecked((int)0xec7dd02d),
            unchecked((int)0x34867077), unchecked((int)0x30476dc0), unchecked((int)0x3d044b19), unchecked((int)0x39c556ae),
            unchecked((int)0x278206ab), unchecked((int)0x23431b1c), unchecked((int)0x2e003dc5), unchecked((int)0x2ac12072),
            unchecked((int)0x128e9dcf), unchecked((int)0x164f8078), unchecked((int)0x1b0ca6a1), unchecked((int)0x1fcdbb16),
            unchecked((int)0x018aeb13), unchecked((int)0x054bf6a4), unchecked((int)0x0808d07d), unchecked((int)0x0cc9cdca),
            unchecked((int)0x7897ab07), unchecked((int)0x7c56b6b0), unchecked((int)0x71159069), unchecked((int)0x75d48dde),
            unchecked((int)0x6b93dddb), unchecked((int)0x6f52c06c), unchecked((int)0x6211e6b5), unchecked((int)0x66d0fb02),
            unchecked((int)0x5e9f46bf), unchecked((int)0x5a5e5b08), unchecked((int)0x571d7dd1), unchecked((int)0x53dc6066),
            unchecked((int)0x4d9b3063), unchecked((int)0x495a2dd4), unchecked((int)0x44190b0d), unchecked((int)0x40d816ba),
            unchecked((int)0xaca5c697), unchecked((int)0xa864db20), unchecked((int)0xa527fdf9), unchecked((int)0xa1e6e04e),
            unchecked((int)0xbfa1b04b), unchecked((int)0xbb60adfc), unchecked((int)0xb6238b25), unchecked((int)0xb2e29692),
            unchecked((int)0x8aad2b2f), unchecked((int)0x8e6c3698), unchecked((int)0x832f1041), unchecked((int)0x87ee0df6),
            unchecked((int)0x99a95df3), unchecked((int)0x9d684044), unchecked((int)0x902b669d), unchecked((int)0x94ea7b2a),
            unchecked((int)0xe0b41de7), unchecked((int)0xe4750050), unchecked((int)0xe9362689), unchecked((int)0xedf73b3e),
            unchecked((int)0xf3b06b3b), unchecked((int)0xf771768c), unchecked((int)0xfa325055), unchecked((int)0xfef34de2),
            unchecked((int)0xc6bcf05f), unchecked((int)0xc27dede8), unchecked((int)0xcf3ecb31), unchecked((int)0xcbffd686),
            unchecked((int)0xd5b88683), unchecked((int)0xd1799b34), unchecked((int)0xdc3abded), unchecked((int)0xd8fba05a),
            unchecked((int)0x690ce0ee), unchecked((int)0x6dcdfd59), unchecked((int)0x608edb80), unchecked((int)0x644fc637),
            unchecked((int)0x7a089632), unchecked((int)0x7ec98b85), unchecked((int)0x738aad5c), unchecked((int)0x774bb0eb),
            unchecked((int)0x4f040d56), unchecked((int)0x4bc510e1), unchecked((int)0x46863638), unchecked((int)0x42472b8f),
            unchecked((int)0x5c007b8a), unchecked((int)0x58c1663d), unchecked((int)0x558240e4), unchecked((int)0x51435d53),
            unchecked((int)0x251d3b9e), unchecked((int)0x21dc2629), unchecked((int)0x2c9f00f0), unchecked((int)0x285e1d47),
            unchecked((int)0x36194d42), unchecked((int)0x32d850f5), unchecked((int)0x3f9b762c), unchecked((int)0x3b5a6b9b),
            unchecked((int)0x0315d626), unchecked((int)0x07d4cb91), unchecked((int)0x0a97ed48), unchecked((int)0x0e56f0ff),
            unchecked((int)0x1011a0fa), unchecked((int)0x14d0bd4d), unchecked((int)0x19939b94), unchecked((int)0x1d528623),
            unchecked((int)0xf12f560e), unchecked((int)0xf5ee4bb9), unchecked((int)0xf8ad6d60), unchecked((int)0xfc6c70d7),
            unchecked((int)0xe22b20d2), unchecked((int)0xe6ea3d65), unchecked((int)0xeba91bbc), unchecked((int)0xef68060b),
            unchecked((int)0xd727bbb6), unchecked((int)0xd3e6a601), unchecked((int)0xdea580d8), unchecked((int)0xda649d6f),
            unchecked((int)0xc423cd6a), unchecked((int)0xc0e2d0dd), unchecked((int)0xcda1f604), unchecked((int)0xc960ebb3),
            unchecked((int)0xbd3e8d7e), unchecked((int)0xb9ff90c9), unchecked((int)0xb4bcb610), unchecked((int)0xb07daba7),
            unchecked((int)0xae3afba2), unchecked((int)0xaafbe615), unchecked((int)0xa7b8c0cc), unchecked((int)0xa379dd7b),
            unchecked((int)0x9b3660c6), unchecked((int)0x9ff77d71), unchecked((int)0x92b45ba8), unchecked((int)0x9675461f),
            unchecked((int)0x8832161a), unchecked((int)0x8cf30bad), unchecked((int)0x81b02d74), unchecked((int)0x857130c3),
            unchecked((int)0x5d8a9099), unchecked((int)0x594b8d2e), unchecked((int)0x5408abf7), unchecked((int)0x50c9b640),
            unchecked((int)0x4e8ee645), unchecked((int)0x4a4ffbf2), unchecked((int)0x470cdd2b), unchecked((int)0x43cdc09c),
            unchecked((int)0x7b827d21), unchecked((int)0x7f436096), unchecked((int)0x7200464f), unchecked((int)0x76c15bf8),
            unchecked((int)0x68860bfd), unchecked((int)0x6c47164a), unchecked((int)0x61043093), unchecked((int)0x65c52d24),
            unchecked((int)0x119b4be9), unchecked((int)0x155a565e), unchecked((int)0x18197087), unchecked((int)0x1cd86d30),
            unchecked((int)0x029f3d35), unchecked((int)0x065e2082), unchecked((int)0x0b1d065b), unchecked((int)0x0fdc1bec),
            unchecked((int)0x3793a651), unchecked((int)0x3352bbe6), unchecked((int)0x3e119d3f), unchecked((int)0x3ad08088),
            unchecked((int)0x2497d08d), unchecked((int)0x2056cd3a), unchecked((int)0x2d15ebe3), unchecked((int)0x29d4f654),
            unchecked((int)0xc5a92679), unchecked((int)0xc1683bce), unchecked((int)0xcc2b1d17), unchecked((int)0xc8ea00a0),
            unchecked((int)0xd6ad50a5), unchecked((int)0xd26c4d12), unchecked((int)0xdf2f6bcb), unchecked((int)0xdbee767c),
            unchecked((int)0xe3a1cbc1), unchecked((int)0xe760d676), unchecked((int)0xea23f0af), unchecked((int)0xeee2ed18),
            unchecked((int)0xf0a5bd1d), unchecked((int)0xf464a0aa), unchecked((int)0xf9278673), unchecked((int)0xfde69bc4),
            unchecked((int)0x89b8fd09), unchecked((int)0x8d79e0be), unchecked((int)0x803ac667), unchecked((int)0x84fbdbd0),
            unchecked((int)0x9abc8bd5), unchecked((int)0x9e7d9662), unchecked((int)0x933eb0bb), unchecked((int)0x97ffad0c),
            unchecked((int)0xafb010b1), unchecked((int)0xab710d06), unchecked((int)0xa6322bdf), unchecked((int)0xa2f33668),
            unchecked((int)0xbcb4666d), unchecked((int)0xb8757bda), unchecked((int)0xb5365d03), unchecked((int)0xb1f740b4)
        };

        public CRC() {
            InitialiseCRC();
        }

        internal void InitialiseCRC() {
            globalCrc = unchecked((int)0xffffffff);
        }

        internal int GetFinalCRC() {
            return ~globalCrc;
        }

        internal int GetGlobalCRC() {
            return globalCrc;
        }

        internal void SetGlobalCRC(int newCrc) {
            globalCrc = newCrc;
        }

        internal void UpdateCRC(int inCh) {
            int temp = (globalCrc >> 24) ^ inCh;
            if (temp < 0) {
                temp = 256 + temp;
            }
            globalCrc = (globalCrc << 8) ^ CRC.crc32Table[temp];
        }

        internal int globalCrc;
    }
}