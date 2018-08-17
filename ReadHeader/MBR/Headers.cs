using System;
using System.Collections.Generic;
using System.Text;

namespace DiskHeader.MBR
{
    public readonly struct Headers : IDiskHeaders
    {
        Header IDiskHeaders.Mbr => Mbr;
        public readonly Header Mbr;
        private Headers(Header Mbr) => this.Mbr = Mbr;
        public static implicit operator Header(in Headers headers) => headers.Mbr;
        public static explicit operator Headers(in Header header) => new Headers(header);
    }
}
