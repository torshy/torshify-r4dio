using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Torshify.Radio.Framework
{
    public class TrackLink
    {
        #region Fields

        private TrackLinkBuilder _builder;

        private string _uri;
        private const string Passphrase = "Isn't it";
        private const string SaltValue = "Standard";
        private const string InitVector = "@1B2c3D4e5F6g7H8";

        #endregion Fields

        #region Constructors

        public TrackLink(string trackSourceName)
        {
            _builder = new TrackLinkBuilder();
            TrackSource = trackSourceName;
        }

        private TrackLink()
            : this(null)
        {
        }

        #endregion Constructors

        #region Properties

        public string TrackSource
        {
            get;
            private set;
        }

        public string Uri
        {
            get
            {
                if (string.IsNullOrEmpty(_uri))
                {
                    _uri = Encrypt();
                }

                return _uri;
            }
            private set
            {
                _uri = value;
            }
        }

        #endregion Properties

        #region Indexers

        public string this[string key]
        {
            get { return _builder[key]; }
            set { _builder.Add(key, value); }
        }

        #endregion Indexers

        #region Methods

        public static TrackLink FromUri(string trackLinkUri)
        {
            TrackLink link = new TrackLink();
            link.Decrypt(trackLinkUri);
            return link;
        }

        protected string Encrypt()
        {
            var plainText = _builder.ToString();
            string encrypted = RijndaelSimple.Encrypt(plainText, Passphrase, SaltValue, "SHA1", 1, InitVector, 256);

            string log;
            Huffman h = new Huffman(plainText);
            var hash = h.Encode(out log);
            string e = string.Empty;
            foreach (var b in hash)
            {
                e += b.ToString("x2").ToLower();
            }
            return "r4dio:" + TrackSource + ":" + e;
        }

        protected void Decrypt(string trackLinkUri)
        {
            Uri = trackLinkUri;

            string[] parts = trackLinkUri.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3)
            {
                throw new InvalidOperationException("Unable to parse link");
            }

            TrackSource = parts[1];

            string encrypted = parts[2];
            string decrypted = RijndaelSimple.Decrypt(encrypted, Passphrase, SaltValue, "SHA1", 1, InitVector, 256);

            var result = HttpUtility.ParseQueryString(decrypted);

            foreach (string key in result.AllKeys)
            {
                _builder.Add(key, result[key]);
            }
        }

        #endregion Methods

        #region Nested Types

        public class Crc32 : HashAlgorithm
        {
            public const UInt32 DefaultPolynomial = 0xedb88320;
            public const UInt32 DefaultSeed = 0xffffffff;

            private UInt32 hash;
            private UInt32 seed;
            private UInt32[] table;
            private static UInt32[] defaultTable;

            public Crc32()
            {
                table = InitializeTable(DefaultPolynomial);
                seed = DefaultSeed;
                Initialize();
            }

            public Crc32(UInt32 polynomial, UInt32 seed)
            {
                table = InitializeTable(polynomial);
                this.seed = seed;
                Initialize();
            }

            public override void Initialize()
            {
                hash = seed;
            }

            protected override void HashCore(byte[] buffer, int start, int length)
            {
                hash = CalculateHash(table, hash, buffer, start, length);
            }

            protected override byte[] HashFinal()
            {
                byte[] hashBuffer = UInt32ToBigEndianBytes(~hash);
                this.HashValue = hashBuffer;
                return hashBuffer;
            }

            public override int HashSize
            {
                get
                {
                    return 32;
                }
            }

            public static UInt32 Compute(byte[] buffer)
            {
                return ~CalculateHash(InitializeTable(DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
            }

            public static UInt32 Compute(UInt32 seed, byte[] buffer)
            {
                return ~CalculateHash(InitializeTable(DefaultPolynomial), seed, buffer, 0, buffer.Length);
            }

            public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
            {
                return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
            }

            private static UInt32[] InitializeTable(UInt32 polynomial)
            {
                if (polynomial == DefaultPolynomial && defaultTable != null)
                    return defaultTable;

                UInt32[] createTable = new UInt32[256];
                for (int i = 0; i < 256; i++)
                {
                    UInt32 entry = (UInt32)i;
                    for (int j = 0; j < 8; j++)
                        if ((entry & 1) == 1)
                            entry = (entry >> 1) ^ polynomial;
                        else
                            entry = entry >> 1;
                    createTable[i] = entry;
                }

                if (polynomial == DefaultPolynomial)
                    defaultTable = createTable;

                return createTable;
            }

            private static UInt32 CalculateHash(UInt32[] table, UInt32 seed, byte[] buffer, int start, int size)
            {
                UInt32 crc = seed;
                for (int i = start; i < size; i++)
                    unchecked
                    {
                        crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
                    }
                return crc;
            }

            private byte[] UInt32ToBigEndianBytes(UInt32 x)
            {
                return new byte[] {
            (byte)((x >> 24) & 0xff),
            (byte)((x >> 16) & 0xff),
            (byte)((x >> 8) & 0xff),
            (byte)(x & 0xff)
        };
            }
        }

        /// <summary>
        /// This class uses a symmetric key algorithm (Rijndael/AES) to encrypt and 
        /// decrypt data. As long as encryption and decryption routines use the same
        /// parameters to generate the keys, the keys are guaranteed to be the same.
        /// The class uses static functions with duplicate code to make it easier to
        /// demonstrate encryption and decryption logic. In a real-life application, 
        /// this may not be the most efficient way of handling encryption, so - as
        /// soon as you feel comfortable with it - you may want to redesign this class.
        /// </summary>
        private class RijndaelSimple
        {
            #region Methods

            /// <summary>
            /// Decrypts specified ciphertext using Rijndael symmetric key algorithm.
            /// </summary>
            /// <param name="cipherText">
            /// Base64-formatted ciphertext value.
            /// </param>
            /// <param name="passPhrase">
            /// Passphrase from which a pseudo-random password will be derived. The
            /// derived password will be used to generate the encryption key.
            /// Passphrase can be any string. In this example we assume that this
            /// passphrase is an ASCII string.
            /// </param>
            /// <param name="saltValue">
            /// Salt value used along with passphrase to generate password. Salt can
            /// be any string. In this example we assume that salt is an ASCII string.
            /// </param>
            /// <param name="hashAlgorithm">
            /// Hash algorithm used to generate password. Allowed values are: "MD5" and
            /// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
            /// </param>
            /// <param name="passwordIterations">
            /// Number of iterations used to generate password. One or two iterations
            /// should be enough.
            /// </param>
            /// <param name="initVector">
            /// Initialization vector (or IV). This value is required to encrypt the
            /// first block of plaintext data. For RijndaelManaged class IV must be
            /// exactly 16 ASCII characters long.
            /// </param>
            /// <param name="keySize">
            /// Size of encryption key in bits. Allowed values are: 128, 192, and 256.
            /// Longer keys are more secure than shorter keys.
            /// </param>
            /// <returns>
            /// Decrypted string value.
            /// </returns>
            /// <remarks>
            /// Most of the logic in this function is similar to the Encrypt
            /// logic. In order for decryption to work, all parameters of this function
            /// - except cipherText value - must match the corresponding parameters of
            /// the Encrypt function which was called to generate the
            /// ciphertext.
            /// </remarks>
            public static string Decrypt(string cipherText,
                string passPhrase,
                string saltValue,
                string hashAlgorithm,
                int passwordIterations,
                string initVector,
                int keySize)
            {
                // Convert strings defining encryption key characteristics into byte
                // arrays. Let us assume that strings only contain ASCII codes.
                // If strings include Unicode characters, use Unicode, UTF7, or UTF8
                // encoding.
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

                // Convert our ciphertext into a byte array.
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

                // First, we must create a password, from which the key will be
                // derived. This password will be generated from the specified
                // passphrase and salt value. The password will be created using
                // the specified hash algorithm. Password creation can be done in
                // several iterations.
                PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                                passPhrase,
                                                                saltValueBytes,
                                                                hashAlgorithm,
                                                                passwordIterations);

                // Use the password to generate pseudo-random bytes for the encryption
                // key. Specify the size of the key in bytes (instead of bits).
                byte[] keyBytes = password.GetBytes(keySize / 8);

                // Create uninitialized Rijndael encryption object.
                RijndaelManaged symmetricKey = new RijndaelManaged();

                // It is reasonable to set encryption mode to Cipher Block Chaining
                // (CBC). Use default options for other symmetric key parameters.
                symmetricKey.Mode = CipherMode.CBC;

                // Generate decryptor from the existing key bytes and initialization
                // vector. Key size will be defined based on the number of the key
                // bytes.
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                                                                 keyBytes,
                                                                 initVectorBytes);

                // Define memory stream which will be used to hold encrypted data.
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

                // Define cryptographic stream (always use Read mode for encryption).
                CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                              decryptor,
                                                              CryptoStreamMode.Read);

                // Since at this point we don't know what the size of decrypted data
                // will be, allocate the buffer long enough to hold ciphertext;
                // plaintext is never longer than ciphertext.
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                // Start decrypting.
                int decryptedByteCount = cryptoStream.Read(plainTextBytes,
                                                           0,
                                                           plainTextBytes.Length);

                // Close both streams.
                memoryStream.Close();
                cryptoStream.Close();

                // Convert decrypted data into a string.
                // Let us assume that the original plaintext string was UTF8-encoded.
                string plainText = Encoding.UTF8.GetString(plainTextBytes,
                                                           0,
                                                           decryptedByteCount);

                // Return decrypted string.
                return plainText;
            }

            /// <summary>
            /// Encrypts specified plaintext using Rijndael symmetric key algorithm
            /// and returns a base64-encoded result.
            /// </summary>
            /// <param name="plainText">
            /// Plaintext value to be encrypted.
            /// </param>
            /// <param name="passPhrase">
            /// Passphrase from which a pseudo-random password will be derived. The
            /// derived password will be used to generate the encryption key.
            /// Passphrase can be any string. In this example we assume that this
            /// passphrase is an ASCII string.
            /// </param>
            /// <param name="saltValue">
            /// Salt value used along with passphrase to generate password. Salt can
            /// be any string. In this example we assume that salt is an ASCII string.
            /// </param>
            /// <param name="hashAlgorithm">
            /// Hash algorithm used to generate password. Allowed values are: "MD5" and
            /// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
            /// </param>
            /// <param name="passwordIterations">
            /// Number of iterations used to generate password. One or two iterations
            /// should be enough.
            /// </param>
            /// <param name="initVector">
            /// Initialization vector (or IV). This value is required to encrypt the
            /// first block of plaintext data. For RijndaelManaged class IV must be 
            /// exactly 16 ASCII characters long.
            /// </param>
            /// <param name="keySize">
            /// Size of encryption key in bits. Allowed values are: 128, 192, and 256. 
            /// Longer keys are more secure than shorter keys.
            /// </param>
            /// <returns>
            /// Encrypted value formatted as a base64-encoded string.
            /// </returns>
            public static string Encrypt(string plainText,
                string passPhrase,
                string saltValue,
                string hashAlgorithm,
                int passwordIterations,
                string initVector,
                int keySize)
            {
                // Convert strings into byte arrays.
                // Let us assume that strings only contain ASCII codes.
                // If strings include Unicode characters, use Unicode, UTF7, or UTF8
                // encoding.
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

                // Convert our plaintext into a byte array.
                // Let us assume that plaintext contains UTF8-encoded characters.
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                // First, we must create a password, from which the key will be derived.
                // This password will be generated from the specified passphrase and
                // salt value. The password will be created using the specified hash
                // algorithm. Password creation can be done in several iterations.
                PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                                passPhrase,
                                                                saltValueBytes,
                                                                hashAlgorithm,
                                                                passwordIterations);

                // Use the password to generate pseudo-random bytes for the encryption
                // key. Specify the size of the key in bytes (instead of bits).
                byte[] keyBytes = password.GetBytes(keySize / 8);

                // Create uninitialized Rijndael encryption object.
                RijndaelManaged symmetricKey = new RijndaelManaged();

                // It is reasonable to set encryption mode to Cipher Block Chaining
                // (CBC). Use default options for other symmetric key parameters.
                symmetricKey.Mode = CipherMode.CBC;

                // Generate encryptor from the existing key bytes and initialization
                // vector. Key size will be defined based on the number of the key
                // bytes.
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
                                                                 keyBytes,
                                                                 initVectorBytes);

                // Define memory stream which will be used to hold encrypted data.
                MemoryStream memoryStream = new MemoryStream();

                // Define cryptographic stream (always use Write mode for encryption).
                CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                             encryptor,
                                                             CryptoStreamMode.Write);
                // Start encrypting.
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

                // Finish encrypting.
                cryptoStream.FlushFinalBlock();

                // Convert our encrypted data from a memory stream into a byte array.
                byte[] cipherTextBytes = memoryStream.ToArray();

                // Close both streams.
                memoryStream.Close();
                cryptoStream.Close();

                // Convert encrypted data into a base64-encoded string.
                string cipherText = Convert.ToBase64String(cipherTextBytes);

                // Return encrypted string.
                return cipherText;
            }

            #endregion Methods
        }
        /// <summary>
        ///     Author: Eric Nusbaum
        ///     E-Mail: eric@enusbaum.com
        ///     Version: 1.0
        ///     Date: 05/22/2009
        /// </summary>
        public sealed class Huffman : IDisposable
        {
            #region --[ Members ]--
            /*
         * Members contains all the private and internal variables
         * used by the Huffman class to both Encode and Decode data
         */
            private List<Leaf> HuffmanTree = new List<Leaf>();
            private MemoryStream msInputbytes = new MemoryStream();
            private StringBuilder Log = new StringBuilder();
            private bool LogEnabled = false;

            /// <summary>
            ///     Container for Leafs and Nodes within the Huffman Tree
            /// </summary>
            internal class Leaf
            {
                public Guid ID
                {
                    get;
                    set;
                }
                public Guid ParentID
                {
                    get;
                    set;
                }
                public bool IsNode
                {
                    get;
                    set;
                }
                public bool Left
                {
                    get;
                    set;
                }
                public bool Right
                {
                    get;
                    set;
                }

                public byte ByteValue
                {
                    get;
                    set;
                }
                public int BitValue
                {
                    get;
                    set;
                }
                public int BitCount
                {
                    get;
                    set;
                }
                public long FrequencyValue
                {
                    get;
                    set;
                }

                public Leaf()
                {
                    ID = Guid.NewGuid();
                }
            }
            #endregion

            #region --[ IDisposable Members ]--
            /*
         * Because the Huffman Class implements IDisposable, we need to
         * implement the Dispose method.
         */

            public void Dispose()
            {
                msInputbytes.Dispose();
                HuffmanTree = null;
                Log = null;
            }

            #endregion

            #region --[ Constructors ]--
            /*
         * The Huffman Class has several constructors to make encoding
         * data easier
         */

            /// <summary>
            ///     Constructor -- Nothing added to Frequency Table
            /// </summary>
            public Huffman()
            {
                Init();
            }

            /// <summary>
            ///     Constructor -- byte[] added to Frequency Table
            /// </summary>
            /// <param name="bInput"></param>
            public Huffman(byte[] bInput)
            {
                Init();

                //Add to Freqency Table Input Buffer
                foreach (byte b in bInput)
                {
                    HuffmanTree[b].FrequencyValue++;
                    msInputbytes.WriteByte(b);
                }
            }

            public Huffman(string sInput)
            {
                Init();

                //Add to Input Buffer & Frequency Table
                foreach (char c in sInput)
                {
                    HuffmanTree[c].FrequencyValue++;
                    msInputbytes.WriteByte((byte)c);
                }

            }
            #endregion

            #region --[ Public Methods ]--
            /*
         * Public Methods exposed in the Huffman Class
         */

            /// <summary>
            ///     Adds a Byte to the Frequency Table
            /// </summary>
            /// <param name="b"></param>
            public void Add(byte b)
            {
                HuffmanTree[b].FrequencyValue++;
                BuildTree();
            }

            /// <summary>
            ///     Adds a String to the Frequency Table
            /// </summary>
            /// <param name="s"></param>
            public void Add(string s)
            {
                foreach (char c in s)
                {
                    HuffmanTree[c].FrequencyValue++;
                }
            }

            /// <summary>
            ///     Clears data entered for encoding
            /// </summary>
            public void Clear()
            {
                msInputbytes.Dispose();
                msInputbytes = new MemoryStream();
            }

            /// <summary>
            ///     Applies Huffman Encoding to the data entered
            /// </summary>
            /// <param name="OutputLog">string -- Log of Encoding, which returns the Huffman Tree Information</param>
            /// <returns>byte[] -- byte[] containing the encoded data package</returns>
            public byte[] Encode(out string OutputLog)
            {
                //Enable Logging
                LogEnabled = true;

                //Build the Tree
                BuildTree();

                //Encode the Tree
                EncodeTree();

                //Encode the Data
                byte[] bEncodedOutput = Encode();

                //Setup output params and return
                OutputLog = Log.ToString();
                return bEncodedOutput;
            }

            /// <summary>
            ///     Applies Huffman Encoding to the data entered
            /// </summary>
            /// <returns>byte[] -- byte[] containing the encoded data package</returns>
            public byte[] Encode()
            {
                //Local Variables
                Int64 iBuffer = 0;
                int iBufferCount = 0;
                int iBytesEncoded = 0;

                MemoryStream msEncodedOutput = new MemoryStream();

                //Build the Tree
                BuildTree();

                //Encode the Tree
                EncodeTree();

                //Remove Nodes (since theyre not needed once the tree is built)
                List<Leaf> OptimizedTree = new List<Leaf>(HuffmanTree);
                OptimizedTree.RemoveAll(delegate(Leaf leaf)
                {
                    return leaf.IsNode;
                });

                //Generation Dictionary to Add to Header
                MemoryStream msEncodedHeader = new MemoryStream();
                foreach (Leaf l in OptimizedTree)
                {

                    if (!l.IsNode & l.FrequencyValue > 0)
                    {
                        iBuffer = l.ByteValue;
                        iBuffer <<= 8;
                        iBuffer ^= l.BitCount;
                        iBuffer <<= 48;
                        iBuffer ^= l.BitValue;
                        msEncodedHeader.Write(BitConverter.GetBytes(iBuffer), 0, 8);
                        iBytesEncoded++;
                        iBuffer = 0;
                    }
                }

                //Write Final Output Size 1st
                msEncodedOutput.Write(BitConverter.GetBytes(msInputbytes.Length), 0, 8);

                //Then Write Dictionary Word Count
                msEncodedOutput.WriteByte((byte)(iBytesEncoded - 1));

                //Then Write Dictionary
                msEncodedOutput.Write(msEncodedHeader.ToArray(), 0, Convert.ToInt16(msEncodedHeader.Length));

                //Pad with 3 Null

                msEncodedOutput.Write(new byte[] { 66, 67, 68 }, 0, 3);

                //Begin Writing Encoded Data Stream
                iBuffer = 0;
                iBufferCount = 0;
                foreach (byte b in msInputbytes.ToArray())
                {
                    Leaf FoundLeaf = OptimizedTree[b];

                    //How many bits are we adding?
                    iBufferCount += FoundLeaf.BitCount;

                    //Shift the buffer if it's not == 0x00
                    if (iBuffer != 0)
                    {
                        iBuffer <<= FoundLeaf.BitCount;
                        iBuffer ^= FoundLeaf.BitValue;
                    }
                    else
                    {
                        iBuffer = FoundLeaf.BitValue;
                    }

                    //Are there at least 8 bits in the buffer?
                    while (iBufferCount > 7)
                    {
                        //Write to output
                        int iBufferOutput = (int)(iBuffer >> (iBufferCount - 8));
                        msEncodedOutput.WriteByte((byte)iBufferOutput);
                        iBufferCount = iBufferCount - 8;
                        iBufferOutput <<= iBufferCount;
                        iBuffer ^= iBufferOutput;
                    }
                }

                //Write remaining bits in buffer
                if (iBufferCount > 0)
                {
                    iBuffer = iBuffer << (8 - iBufferCount);
                    msEncodedOutput.WriteByte((byte)iBuffer);
                }
                return msEncodedOutput.ToArray();
            }

            public byte[] Decode(byte[] bInput)
            {
                //Local Variables
                List<Leaf> DecodeDictionary = new List<Leaf>(255);
                Leaf DecodedLeaf = null;
                long iInputBuffer = 0;
                long iStreamLength = 0;
                int iInputBufferSize = 0;
                int iOutputBuffer = 0;
                int iDictionaryRecords = 0;
                int iDictionaryEndByte = 0;
                int iBytesWritten = 0;

                //Populate Decode Dictionary with 256 Leafs
                for (int i = 0; i < 256; i++)
                {
                    DecodeDictionary.Add(new Leaf());
                }

                //Retrieve Stream Length
                iStreamLength = BitConverter.ToInt64(bInput, 0);

                //Establish Output Buffer to write unencoded data to
                byte[] bDecodedOutput = new byte[iStreamLength];

                //Retrieve Records in Dictionary
                iDictionaryRecords = bInput[8];

                //Calculate Ending Byte of Dictionary
                iDictionaryEndByte = (((iDictionaryRecords + 1) * 8) + 8);

                //Begin Decoding Dictionary (4 Bytes Per Entry)
                for (int i = 9; i <= iDictionaryEndByte; i += 8)
                {
                    iInputBuffer = BitConverter.ToInt64(bInput, i);

                    DecodedLeaf = new Leaf();

                    //Get Byte Value
                    DecodedLeaf.ByteValue = (byte)(iInputBuffer >> 56);
                    if (DecodedLeaf.ByteValue != 0)
                        iInputBuffer ^= (((Int64)DecodedLeaf.ByteValue) << 56);

                    //Get Bit Count
                    DecodedLeaf.BitCount = (int)(iInputBuffer >> 48);
                    iInputBuffer ^= (((Int64)DecodedLeaf.BitCount) << 48);

                    //Get Bit Value
                    DecodedLeaf.BitValue = (int)(iInputBuffer);

                    //Add Decoded Leaf to Dictionary
                    DecodeDictionary[DecodedLeaf.ByteValue] = DecodedLeaf;
                }

                //Begin Looping through Input and Decoding
                iInputBuffer = 0;
                for (int i = (iDictionaryEndByte + 4); i < bInput.Length; i++)
                {
                    //Increment the Buffer
                    iInputBufferSize += 8;

                    if (iInputBuffer != 0)
                    {
                        iInputBuffer <<= 8;
                        iInputBuffer ^= bInput[i];
                    }
                    else
                    {
                        iInputBuffer = bInput[i];
                    }

                    //Loop through the Current Buffer until it's exhausted
                    for (int j = (iInputBufferSize - 1); j >= 0; j--)
                    {
                        iOutputBuffer = (int)(iInputBuffer >> j);

                        //Leading 0;
                        if (iOutputBuffer == 0)
                            continue;
                        int iBitCount = iInputBufferSize - j;
                        //Try and find a byte in the dictionary that matches what's currently in the buffer
                        for (int k = 0; k < 256; k++)
                        {
                            if (DecodeDictionary[k].BitValue == iOutputBuffer && DecodeDictionary[k].BitCount == iBitCount)
                            {
                                //Byte Found, Write it to the Output Buffer and XOR it from the current Input Buffer
                                bDecodedOutput[iBytesWritten] = DecodeDictionary[k].ByteValue;
                                iOutputBuffer <<= j;
                                iInputBuffer ^= iOutputBuffer;
                                iInputBufferSize = j;
                                iBytesWritten++;
                                break;
                            }
                        }
                    }
                }
                return bDecodedOutput;
            }
            #endregion

            #region --[ Private Method ]--
            /*
         * Private Methods protected in the Huffman Class
         */

            /// <summary>
            ///     Populate the Leaf Table (Frequency Table), the Leafs will be turned into Nodes
            /// </summary>
            private void Init()
            {
                //Setup Freqency Table with Leafs
                for (short i = 0; i <= 255; i++)
                {
                    HuffmanTree.Add(new Leaf()
                    {
                        ByteValue = (byte)i
                    });
                }
            }

            /// <summary>
            ///     Walks up the tree from each Leaf, encoding as it goes.
            /// </summary>
            /// <returns></returns>
            private bool EncodeTree()
            {
                StringBuilder sbOutput = new StringBuilder();
                int iBinaryValue = 0;
                int iBitCount = 0;
                int iLeadingZeros = 0;

                //Go through the Frequency Table and create Leafs from it
                foreach (Leaf Node in HuffmanTree)
                {
                    //Only process the byte if it actually occurs
                    if (!Node.IsNode && Node.FrequencyValue != 0)
                    {
                        iBinaryValue = 0;
                        iBitCount = 0;
                        iLeadingZeros = 0;

                        if (Node.Left || Node.Right)
                        {
                            //Left Node == 0, Right Node == 1
                            if (Node.Left)
                                iBitCount++;
                            else if (Node.Right)
                            {
                                iBinaryValue ^= ((int)1 << iBitCount);
                                iBitCount++;
                            }

                            //Process up the tree through the parent nodes
                            Leaf ParentNode = HuffmanTree.Find(delegate(Leaf leaf)
                            {
                                return leaf.ID == Node.ParentID;
                            });
                            while (ParentNode.ParentID != new Guid())
                            {
                                //Left Node == 0, Right Node == 1
                                if (ParentNode.Left)
                                    iBitCount++;
                                else if (ParentNode.Right)
                                {
                                    iBinaryValue ^= ((int)1 << iBitCount);
                                    iBitCount++;
                                }

                                //Continue up the tree to the parent nodes
                                ParentNode = HuffmanTree.Find(delegate(Leaf leaf)
                                {
                                    return leaf.ID == ParentNode.ParentID;
                                });
                            }
                        }

                        //Account for Leading Zeros
                        //Total C# cheater way to do it, but whatever... it works :P
                        if (iBinaryValue != 0)
                            iLeadingZeros = iBitCount - Convert.ToString(iBinaryValue, 2).Length;
                        else
                            iLeadingZeros = iBitCount;

                        //Assign the Encoded value to the Node
                        Node.BitValue = iBinaryValue;
                        Node.BitCount = iBitCount;

                        //Make sure it's not all 0's, as this wouldn't decode correctly
                        if (Node.BitValue == 0)
                        {
                            Node.BitValue = 1;
                            Node.BitCount++;
                        }

                        //Appent it to the output log
                        if (LogEnabled)
                            Log.AppendLine(string.Format("{0} = {1} (Bits: {2})", (int)Node.ByteValue, Convert.ToString(Node.BitValue, 2), Node.BitCount));
                    }
                }
                return true;
            }

            /// <summary>
            ///     Takes Frequency Value and Establishes Parent/Child relationship with Tree Nodes & Leafs
            /// </summary>
            /// <returns></returns>
            private bool BuildTree()
            {
                //Local Variables
                int iParentIndex = 0;

                List<Leaf> OptimizedTree = new List<Leaf>(HuffmanTree);
                List<Leaf> WorkingTree;
                Leaf NewParent;

                //Remove anything with a 0 Frequency Value
                OptimizedTree.RemoveAll(delegate(Leaf leaf)
                {
                    return leaf.FrequencyValue == 0;
                });

                //Order with highest frequency at 'end', lowest at 'beginning'
                OptimizedTree.Sort(delegate(Leaf L1, Leaf L2)
                {
                    return L1.FrequencyValue.CompareTo(L2.FrequencyValue);
                });

                WorkingTree = new List<Leaf>(OptimizedTree);
                while (WorkingTree.Count > 1)
                {
                    //Sort by Frequency
                    //Order with highest frequency at 'end', lowest at 'beginning'
                    WorkingTree.Sort(delegate(Leaf L1, Leaf L2)
                    {
                        return L1.FrequencyValue.CompareTo(L2.FrequencyValue);
                    });

                    //Take 'First Two' and join them with a new node
                    NewParent = new Leaf()
                    {
                        FrequencyValue = WorkingTree[0].FrequencyValue + WorkingTree[1].FrequencyValue,
                        IsNode = true
                    };

                    HuffmanTree.Add(NewParent);

                    //Assign Parent to Left Node
                    iParentIndex = HuffmanTree.FindIndex(delegate(Leaf L1)
                    {
                        return L1.Equals(WorkingTree[0]);
                    });
                    HuffmanTree[iParentIndex].Left = true;
                    HuffmanTree[iParentIndex].ParentID = NewParent.ID;

                    //Assign Parent to Right Node
                    iParentIndex = HuffmanTree.FindIndex(delegate(Leaf L1)
                    {
                        return L1.Equals(WorkingTree[1]);
                    });
                    HuffmanTree[iParentIndex].Right = true;
                    HuffmanTree[iParentIndex].ParentID = NewParent.ID;

                    OptimizedTree = new List<Leaf>(HuffmanTree);

                    //Remove anything with a 0 Frequency Value
                    OptimizedTree.RemoveAll(delegate(Leaf leaf)
                    {
                        return leaf.FrequencyValue == 0;
                    });

                    //Order with highest frequency at 'end', lowest at 'beginning'
                    OptimizedTree.Sort(delegate(Leaf L1, Leaf L2)
                    {
                        return L1.FrequencyValue.CompareTo(L2.FrequencyValue);
                    });

                    WorkingTree = new List<Leaf>(OptimizedTree);

                    //Remove anything with a parent
                    WorkingTree.RemoveAll(delegate(Leaf leaf)
                    {
                        return leaf.ParentID != new Guid();
                    });
                }

                return true;
            }
            #endregion
        }
        #endregion Nested Types
    }
}