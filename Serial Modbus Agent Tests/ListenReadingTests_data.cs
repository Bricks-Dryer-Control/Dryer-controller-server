using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Serial_Modbus_Agent_Tests
{
    public partial class ListenReadingTests
    {
        
        public static readonly byte[][] requests = {
            new byte[] {0x12, 0x03, 0x01, 0x00, 0x00, 0x05, 0x86, 0x96},
            new byte[] {0x12, 0x03, 0x0a, 0x30, 0x81, 0x42, 0xbd, 0x14, 0x78, 0x40, 0x9e, 0xff, 0xff, 0x4d, 0x2e},
            new byte[] {0x13, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x47},
            new byte[] {0x13, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x1c, 0xd4},
            new byte[] {0x14, 0x03, 0x01, 0x00, 0x00, 0x05, 0x86, 0xf0},
            new byte[] {0x14, 0x03, 0x0a, 0xb6, 0x2a, 0x42, 0x85, 0x33, 0x20, 0x3f, 0xf3, 0x00, 0x00, 0x3f, 0x48},
            new byte[] {0x15, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x21},
            new byte[] {0x15, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x15, 0x12},
        };

        private readonly byte[] normalData = new byte[] {
            0x12, 0x03, 0x01, 0x00, 0x00, 0x05, 0x86, 0x96, 
            0x12, 0x03, 0x0a, 0x30, 0x81, 0x42, 0xbd, 0x14, 0x78, 0x40, 0x9e, 0xff, 0xff, 0x4d, 0x2e, 
            0x13, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x47, 
            0x13, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x1c, 0xd4, 
            0x14, 0x03, 0x01, 0x00, 0x00, 0x05, 0x86, 0xf0, 
            0x14, 0x03, 0x0a, 0xb6, 0x2a, 0x42, 0x85, 0x33, 0x20, 0x3f, 0xf3, 0x00, 0x00, 0x3f, 0x48, 
            0x15, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x21, 
            0x15, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x15, 0x12, 
            0x16, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x12, 
            0x16, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x10, 0xd1,

            0x01, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x35, 
            0x01, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x25, 0x06, 
            0x02, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x06, 
            0x02, 0x03, 0x0a, 0x3d, 0x0d, 0x42, 0x70, 0xf5, 0xc0, 0x41, 0x58, 0x00, 0x00, 0x9d, 0x95, 
            0x03, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0xd7, 
            0x03, 0x03, 0x0a, 0xe7, 0x11, 0x42, 0x80, 0x0a, 0x40, 0x40, 0x77, 0x00, 0x00, 0xf6, 0xb6, 
            0x04, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x60, 
            0x04, 0x03, 0x0a, 0xc0, 0xd5, 0x42, 0xaa, 0xe1, 0x48, 0x40, 0xea, 0x00, 0x00, 0xc3, 0x72, 
            0x05, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0xb1, 
            0x05, 0x03, 0x0a, 0xff, 0x14, 0x42, 0xa2, 0xc2, 0x90, 0x40, 0x55, 0x00, 0x00, 0x33, 0xcd, 
            0x06, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x82, 
            0x06, 0x03, 0x0a, 0x84, 0x3a, 0x42, 0x7b, 0xc2, 0x90, 0x40, 0x15, 0x00, 0x00, 0xd9, 0x78, 
            0x07, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x53, 
            0x07, 0x03, 0x0a, 0xf2, 0x89, 0x42, 0x8d, 0x51, 0xe0, 0x40, 0x38, 0x00, 0x00, 0x42, 0x54, 
            0x08, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0xac, 
            0x08, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x38, 0xcf, 
            0x09, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x7d,
            0x09, 0x03, 0x0a, 0x5c, 0x8f, 0x42, 0x7d, 0x99, 0x90, 0x40, 0x79, 0x00, 0x00, 0xe9, 0x95, 
            0x0a, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x4e,

            0x0b, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x9f, 
            0x08, 0x03, 0x02, 0x7d, 0xd2, 0x42, 0xa9, 0x66, 0x60, 0x40, 0x66, 0x00, 0x00, 0xeb, 0x3a, 
            0x0c, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x28, 
            0x0c, 0x03, 0x0a, 0x49, 0x55, 0x42, 0x76, 0x47, 0xa8, 0x40, 0xe1, 0x00, 0x00, 0x9f, 0x24, 
            0x0d, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0xf9, 
            0x0d, 0x03, 0x0a, 0xf8, 0x40, 0x42, 0x95, 0xeb, 0x80, 0x40, 0x31, 0x00, 0x00, 0x92, 0xba, 
            0x0e, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0xca, 
            0x0e, 0x03, 0x0a, 0x6c, 0xeb, 0x42, 0xa8, 0xf5, 0xc0, 0x40, 0x08, 0x00, 0x00, 0x65, 0x59, 
            0x0f, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x1b,

            0x10, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x74, 
            0x00, 0x03, 0x0a, 0x1f, 0x80, 0x42, 0x92, 0x1e, 0xb0, 0x40, 0x15, 0x00, 0x00, 0x68, 0x0b, 
            0x11, 0x03, 0x01, 0x00, 0x00, 0x05, 0x86, 0xa5, 
            0x11, 0x03, 0x0a, 0x39, 0x1c, 0x42, 0xa8, 0xeb, 0x80, 0x40, 0x11, 0x00, 0x00, 0xad, 0x15, 
            0x12, 0x03, 0x01, 0x00, 0x00, 0x05, 0x86, 0x96, 
            0x12, 0x03, 0x0a, 0x30, 0x81, 0x42, 0xbd, 0x14, 0x78, 0x40, 0x9e, 0xff, 0xff, 0x4d, 0x2e, 
            0x13, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x47, 
            0x13, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x1c, 0xd4, 
            0x14, 0x03, 0x01, 0x00, 0x00, 0x05, 0x86, 0xf0, 
            0x14, 0x03, 0x0a, 0xb4, 0x46, 0x42, 0x85, 0xa3, 0xc0, 0x3f, 0xf0, 0x00, 0x00, 0x0f, 0x06, 
            0x15, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x21, 
            0x15, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x15, 0x12, 
            0x16, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x12, 
            0x16, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x10, 0xd1,

            0x01, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x35, 0x01, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0xff, 0xff, 0x25, 0x06, 0x02, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x06, 0x02,
            0x03, 0x0a, 0x64, 0xac, 0x42, 0x70, 0xeb, 0x84, 0x41, 0x59, 0x00, 0x00, 0x19, 0x1d, 0x03, 0x03,
            0x01, 0x00, 0x00, 0x05, 0x85, 0xd7, 0x03, 0x03, 0x0a, 0xd7, 0x5b, 0x42, 0x80, 0xc2, 0x90, 0x40,
            0x75, 0x00, 0x00, 0xc9, 0x70, 0x04, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x60, 0x04, 0x03, 0x0a,
            0xa0, 0xbc, 0x42, 0xaa, 0xa3, 0xd8, 0x40, 0xe8, 0x00, 0x00, 0x6c, 0x61, 0x05, 0x03, 0x01, 0x00,
            0x00, 0x05, 0x85, 0xb1, 0x05, 0x03, 0x0a, 0xff, 0x14, 0x42, 0xa2, 0xc2, 0x90, 0x40, 0x55, 0x00,
            0x00, 0x33, 0xcd, 0x06, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x82, 0x06, 0x03, 0x0a, 0x84, 0x3a,
            0x42, 0x7b, 0xc2, 0x90, 0x40, 0x15, 0x00, 0x00, 0xd9, 0x78, 0x07, 0x03, 0x01, 0x00, 0x00, 0x05,
            0x84, 0x53, 0x07, 0x03, 0x0a, 0xf2, 0x89, 0x42, 0x8d, 0x51, 0xe0, 0x40, 0x38, 0x00, 0x00, 0x42,
            0x54, 0x08, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0xac, 0x08, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x38, 0xcf, 0x09, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x7d,
            0x09, 0x03, 0x0a, 0x5c, 0x8f, 0x42, 0x7d, 0x99, 0x90, 0x40, 0x79, 0x00, 0x00, 0xe9, 0x95, 0x0a,
            0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x4e,

            0x0b, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x9f, 0x0b, 0x06, 0x0a, 0x96, 0x05, 0x42, 0x29, 0x0a,
            0x40, 0x40, 0x67, 0x00, 0x00, 0x65, 0xd6, 0x0c, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x28, 0x0c,
            0x03, 0x0a, 0x2f, 0xf5, 0x42, 0x76, 0xeb, 0x80, 0x40, 0xe1, 0x00, 0x00, 0xfc, 0xe8, 0x0d, 0x03,
            0x01, 0x00, 0x00, 0x05, 0x84, 0xf9, 0x0d, 0x03, 0x0a, 0xe8, 0x5d, 0x42, 0x95, 0x00, 0x00, 0x40,
            0x30, 0x00, 0x00, 0x41, 0x7a, 0x0e, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0xca, 0x0e, 0x03, 0x0a,
            0x6f, 0xc3, 0x42, 0xa8, 0x7a, 0xe0, 0x40, 0x04, 0x00, 0x00, 0x35, 0xac, 0x0f, 0x03, 0x01, 0x00,
            0x00, 0x05, 0x85, 0x1b,

            0x10, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x74, 0x00, 0x03, 0x08, 0x42, 0xae, 0x42, 0x92, 0xae,
            0x10, 0x40, 0x17, 0x00, 0x00, 0x44, 0x77, 0x11, 0x03, 0x01, 0x00, 0x00, 0x05, 0x86, 0xa5, 0x11,
            0x03, 0x0a, 0x39, 0x1c, 0x42, 0xa8, 0xeb, 0x80, 0x40, 0x11, 0x00, 0x00, 0xad, 0x15, 0x12, 0x03,
            0x01, 0x00, 0x00, 0x05, 0x86, 0x96, 0x12, 0x03, 0x0a, 0x30, 0x81, 0x42, 0xbd, 0x14, 0x78, 0x40,
            0x9e, 0xff, 0xff, 0x4d, 0x2e, 0x13, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x47, 0x13, 0x03, 0x0a,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x1c, 0xd4, 0x14, 0x03, 0x01, 0x00,
            0x00, 0x05, 0x86, 0xf0, 0x14, 0x03, 0x0a, 0xb4, 0x46, 0x42, 0x85, 0xa3, 0xc0, 0x3f, 0xf0, 0x00,
            0x00, 0x0f, 0x06, 0x15, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x21, 0x15, 0x03, 0x0a, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x15, 0x12, 0x16, 0x03, 0x01, 0x00, 0x00, 0x05,
            0x87, 0x12, 0x16, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x10,
            0xd1,

            0x01, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x35, 0x01, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0xff, 0xff, 0x25, 0x06, 0x02, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x06, 0x02,
            0x03, 0x0a, 0x3e, 0xaf, 0x42, 0x70, 0x1e, 0xb8, 0x41, 0x59, 0x00, 0x00, 0x6a, 0x2c, 0x03, 0x03,
            0x01, 0x00, 0x00, 0x05, 0x85, 0xd7, 0x03, 0x03, 0x0a, 0xd7, 0x5b, 0x42, 0x80, 0xc2, 0x90, 0x40,
            0x75, 0x00, 0x00, 0xc9, 0x70, 0x04, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x60, 0x04, 0x03, 0x0a,
            0xb4, 0x7c, 0x42, 0xaa, 0xae, 0x10, 0x40, 0xe7, 0x00, 0x00, 0xac, 0xea, 0x05, 0x03, 0x01, 0x00,
            0x00, 0x05, 0x85, 0xb1, 0x05, 0x03, 0x0a, 0x00, 0x43, 0x42, 0xa3, 0x66, 0x60, 0x40, 0x56, 0x00,
            0x00, 0x65, 0xfc, 0x06, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x82, 0x06, 0x03, 0x0a, 0x84, 0x3a,
            0x42, 0x7b, 0xc2, 0x90, 0x40, 0x15, 0x00, 0x00, 0xd9, 0x78, 0x07, 0x03, 0x01, 0x00, 0x00, 0x05,
            0x84, 0x53, 0x07, 0x03, 0x0a, 0xe3, 0x63, 0x42, 0x8d, 0x0a, 0x40, 0x40, 0x37, 0x00, 0x00, 0x18,
            0x51, 0x08, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0xac, 0x08, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x38, 0xcf, 0x09, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x7d,
            0x09, 0x03, 0x0a, 0x7f, 0xb2, 0x42, 0x7d, 0x28, 0xf0, 0x40, 0x7c, 0x00, 0x00, 0x9f, 0x89, 0x0a,
            0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x4e,

            0x0b, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x9f, 0x03, 0x03, 0x0a, 0xb9, 0xfb, 0x42, 0xa9, 0xae,
            0x00, 0x40, 0x67, 0x00, 0x00, 0x5c, 0x01, 0x0c, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x28, 0x0c,
            0x03, 0x0a, 0x2f, 0xf5, 0x42, 0x76, 0xeb, 0x80, 0x40, 0xe1, 0x00, 0x00, 0xfc, 0xe8, 0x0d, 0x03,
            0x01, 0x00, 0x00, 0x05, 0x84, 0xf9, 0x0d, 0x03, 0x0a, 0xf3, 0xf4, 0x42, 0x95, 0x5c, 0x20, 0x40,
            0x2f, 0x00, 0x00, 0x1f, 0xc0, 0x0e, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0xca, 0x0e, 0x03, 0x0a,
            0x81, 0x71, 0x42, 0xa8, 0xae, 0x10, 0x40, 0x07, 0x00, 0x00, 0xde, 0xa9, 0x0f, 0x03, 0x01, 0x00,
            0x00, 0x05, 0x85, 0x1b,

            0x10, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x74, 0x00, 0x06, 0x0a, 0x43, 0xb9, 0x02, 0x92, 0x51,
            0xf0, 0x40, 0x18, 0x00, 0x00, 0x9a, 0x5b, 0x11, 0x03, 0x01, 0x00, 0x00, 0x05, 0x86, 0xa5, 0x11,
            0x03, 0x0a, 0x44, 0xa1, 0x42, 0xa8, 0xeb, 0x80, 0x40, 0x11, 0x00, 0x00, 0x53, 0x93, 0x12, 0x03,
            0x01, 0x00, 0x00, 0x05, 0x86, 0x96, 0x12, 0x03, 0x0a, 0x30, 0x81, 0x42, 0xbd, 0x14, 0x78, 0x40,
            0x9e, 0xff, 0xff, 0x4d, 0x2e, 0x13, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x47, 0x13, 0x03, 0x0a,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x1c, 0xd4, 0x14, 0x03, 0x01, 0x00,
            0x00, 0x05, 0x86, 0xf0, 0x14, 0x03, 0x0a, 0xb5, 0x38, 0x42, 0x85, 0xeb, 0x80, 0x3f, 0xf1, 0x00,
            0x00, 0x28, 0xe6, 0x15, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x21, 0x15, 0x03, 0x0a, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x15, 0x12, 0x16, 0x03, 0x01, 0x00, 0x00, 0x05,
            0x87, 0x12, 0x16, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x10,
            0xd1,

            0x01, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x35, 0x01, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0xff, 0xff, 0x25, 0x06, 0x02, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x06, 0x02,
            0x03, 0x0a, 0x25, 0xc2, 0x42, 0x70, 0x99, 0x98, 0x41, 0x59, 0x00, 0x00, 0x75, 0x7e, 0x03, 0x03,
            0x01, 0x00, 0x00, 0x05, 0x85, 0xd7, 0x03, 0x03, 0x0a, 0xd6, 0x73, 0x42, 0x80, 0x1e, 0xb0, 0x40,
            0x75, 0x00, 0x00, 0xf4, 0x2f, 0x04, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x60, 0x04, 0x03, 0x0a,
            0x98, 0xb3, 0x42, 0xaa, 0x99, 0x98, 0x40, 0xe9, 0x00, 0x00, 0xf9, 0xd1, 0x05, 0x03, 0x01, 0x00,
            0x00, 0x05, 0x85, 0xb1, 0x05, 0x03, 0x0a, 0xf5, 0x84, 0x42, 0xa2, 0x0a, 0x40, 0x40, 0x57, 0x00,
            0x00, 0xcf, 0xb1, 0x06, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x82, 0x06, 0x03, 0x0a, 0x82, 0x75,
            0x42, 0x7b, 0x1e, 0xb0, 0x40, 0x15, 0x00, 0x00, 0xdb, 0xcf, 0x07, 0x03, 0x01, 0x00, 0x00, 0x05,
            0x84, 0x53, 0x07, 0x03, 0x0a, 0xf2, 0x89, 0x42, 0x8d, 0x51, 0xe0, 0x40, 0x38, 0x00, 0x00, 0x42,
            0x54, 0x08, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0xac, 0x08, 0x03, 0x0a, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x38, 0xcf, 0x09, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x7d,
            0x09, 0x03, 0x0a, 0x7f, 0xb2, 0x42, 0x7d, 0x28, 0xf0, 0x40, 0x7c, 0x00, 0x00, 0x9f, 0x89, 0x0a,
            0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x4e,

            0x0b, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0x9f, 0x0b, 0x03, 0x0a, 0xd1, 0x07, 0x42, 0xa9, 0x8e,
            0x10, 0x40, 0x67, 0x00, 0x00, 0x8f, 0x51, 0x0c, 0x03, 0x01, 0x00, 0x00, 0x05, 0x85, 0x28, 0x0c,
            0x03, 0x0a, 0x2f, 0xf5, 0x42, 0x76, 0xeb, 0x80, 0x40, 0xe1, 0x00, 0x00, 0xfc, 0xe8, 0x0d, 0x03,
            0x01, 0x00, 0x00, 0x05, 0x84, 0xf9, 0x0d, 0x03, 0x0a, 0xef, 0xa9, 0x42, 0x95, 0xcc, 0xd0, 0x40,
            0x2c, 0x00, 0x00, 0xd7, 0xbc, 0x0e, 0x03, 0x01, 0x00, 0x00, 0x05, 0x84, 0xca, 0x0e, 0x03, 0x0a,
            0x6c, 0xeb, 0x42, 0xa8, 0xf5, 0xc0, 0x40, 0x08, 0x00, 0x00, 0x65, 0x59, 0x0f, 0x03, 0x01, 0x00,
            0x00, 0x05, 0x85, 0x1b,

            0x10, 0x03, 0x01, 0x00, 0x00, 0x05, 0x87, 0x74, 0x10, 0x03, 0x14, 0x03, 0xb9, 0x40, 0x92, 0x51,
            0xe0, 0x40, 0x18, 0x00, 0x00, 0x9a, 0x5b, 0x11, 0x03, 0x01, 0x00, 0x00, 0x05, 0x86, 0xa5, 0x11,
            0x03, 0x0a, 0x50, 0x28, 0x42, 0xa8, 0xeb, 0x80, 0x40, 0x11, 0x00, 0x00, 0x18, 0x45, 0x12, 0x03,
            0x01, 0x00, 0x00, 0x05, 0x86, 0x96, 0x12, 0x03, 0x0a, 0x30, 0x81, 0x42, 0xbd, 0x14, 0x78, 0x40,
            0x9e, 0xff, 0xff, 0x4d, 0x2e,
        };
    }
}
