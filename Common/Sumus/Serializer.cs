﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Goldmint.Common.Sumus {

	public class Serializer : IDisposable {

		private MemoryStream _buffer;

		public Serializer() {
			if (!BitConverter.IsLittleEndian) throw new Exception("Big-endian is not supported");
			_buffer = new MemoryStream();
		}

		public void Dispose() {
			_buffer?.Dispose();
		}

		public byte[] Data() {
			return _buffer.ToArray();
		}

		public string Hex() {
			var sb = new StringBuilder();
			var data = this.Data();
			foreach (var t in data) {
				sb.Append(t.ToString("x2"));
			}
			return sb.ToString().ToLower();
		}

		public void Write(byte v) {
			_buffer.WriteByte(v);
		}

		public void Write(byte[] v) {
			if (v == null) throw new ArgumentException("Array is null");
			_buffer.Write(v, 0, v.Length);
		}

		public void Write(ushort v) {
			this.Write(BitConverter.GetBytes(v));
		}

		public void Write(uint v) {
			this.Write(BitConverter.GetBytes(v));
		}

		public void Write(ulong v) {
			this.Write(BitConverter.GetBytes(v));
		}

		public void Write(string v) {
			const int max = 64;
			if (v == null) throw new ArgumentException("String is null");
			if (Encoding.UTF8.GetByteCount(v) > max) throw new ArgumentException($"String have to be {max} bytes length (utf-8)");

			var b = new byte[max];
			Encoding.UTF8.GetBytes(v, 0, v.Length, b, 0);
			this.Write(b);
		}

		public void Write(Amount a) {
			const int imax = 10;
			const int fmax = 18;

			_buffer.WriteByte(a.Value < 0 ? (byte)1 : (byte)0);

			var v = a.ToString().TrimStart('-');
			if (!v.Contains(".")) {
				this.Write(FlipAmountString("".PadRight(fmax, '0')));
				this.Write(FlipAmountString(v.PadLeft(imax, '0')));
			}
			else {
				var p = v.Split('.', 2);
				this.Write(FlipAmountString(p[1].PadRight(fmax, '0')));
				this.Write(FlipAmountString(p[0].PadLeft(imax, '0')));
			}
		}

		// ---

		// "1234...5678" => [0x78 0x56 .. 0x34 0x12]
		private static byte[] FlipAmountString(string s) {
			if (s == null || s.Length % 2 != 0) {
				throw new ArgumentException("Invalid string");
			}
			byte[] ret = new byte[s.Length / 2];
			for (var i = 0; i < ret.Length; i++) {
				ret[ret.Length - i - 1] = Convert.ToByte(s.Substring(i * 2, 2), 16);
			}
			return ret;
		}
	}

	public class Deserializer : IDisposable {

		private MemoryStream _buffer;

		public Deserializer(byte[] data) {
			if (!BitConverter.IsLittleEndian) throw new Exception("Big-endian is not supported");
			_buffer = new MemoryStream(data);
			_buffer.Position = 0;
		}

		public void Dispose() {
			_buffer?.Dispose();
		}

		public bool ReadByte(out byte v) {
			v = 0;
			var b = _buffer.ReadByte();
			if (b == -1) return false;
			v = (byte) b;
			return true;
		}

		public bool ReadBytes(out byte[] v, int length) {
			if (length <= 0) throw new ArgumentException("Length should be greater than 0");
			v = null;
			var b = new byte[length];
			if (_buffer.Read(b, 0, length) == length) {
				v = b;
				return true;
			}
			return false;
		}

		public bool ReadUint16(out ushort v) {
			v = 0;
			if (this.ReadBytes(out var b, 2)) {
				v = BitConverter.ToUInt16(b, 0);
				return true;
			}
			return false;
		}

		public bool ReadUint32(out uint v) {
			v = 0;
			if (this.ReadBytes(out var b, 4)) {
				v = BitConverter.ToUInt32(b, 0);
				return true;
			}
			return false;
		}

		public bool ReadUint64(out ulong v) {
			v = 0;
			if (this.ReadBytes(out var b, 8)) {
				v = BitConverter.ToUInt64(b, 0);
				return true;
			}
			return false;
		}

		public bool ReadString(out string v) {
			const int max = 64;
			v = null;

			if (this.ReadBytes(out var b, max)) {
				var to = -1;
				for (int i = 0; i < max; i++) {
					if (b[i] == 0x0) {
						to = i;
						break;
					}
				}
				v = to != -1 ? Encoding.UTF8.GetString(b, 0, to) : Encoding.UTF8.GetString(b);
				return true;
			}
			return false;
		}

		public bool ReadAmount(out Amount a) {
			const int imax = 10;
			const int fmax = 18;

			a = null;

			if (!this.ReadByte(out var sign) || sign > 1) {
				return false;
			}
			if (!this.ReadBytes(out var fracBytes, fmax/2)) {
				return false;
			}
			if (!this.ReadBytes(out var intBytes, imax / 2)) {
				return false;
			}

			var fracp = FlipAmountString(fracBytes);
			var intp = FlipAmountString(intBytes);

			a = new Amount(string.Format("{0}.{1}", intp, fracp));
			if (sign == 1) {
				a.Value *= BigInteger.MinusOne;
			}

			return true;
		}

		// ---

		// [0x78 0x56 .. 0x34 0x12] => "1234...5678"
		private static string FlipAmountString(byte[] b) {
			if (b == null || b.Length == 0) {
				throw new ArgumentException("Invalid bytes");
			}
			var sb = new StringBuilder(b.Length * 2);
			for (var i = b.Length - 1; i >= 0; i--) {
				sb.Append(b[i].ToString("x2"));
			}
			return sb.ToString();
		}
	}
}
