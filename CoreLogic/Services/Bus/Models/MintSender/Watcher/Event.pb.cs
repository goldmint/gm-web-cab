// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: proto/mintsender/watcher/event.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace MintSender.Watcher.Event {

  /// <summary>Holder for reflection information generated from proto/mintsender/watcher/event.proto</summary>
  public static partial class EventReflection {

    #region Descriptor
    /// <summary>File descriptor for proto/mintsender/watcher/event.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static EventReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CiRwcm90by9taW50c2VuZGVyL3dhdGNoZXIvZXZlbnQucHJvdG8SBWV2ZW50",
            "Im4KBlJlZmlsbBIPCgdzZXJ2aWNlGAEgASgJEhEKCXB1YmxpY0tleRgCIAEo",
            "CRIMCgRmcm9tGAMgASgJEg0KBXRva2VuGAQgASgJEg4KBmFtb3VudBgFIAEo",
            "CRITCgt0cmFuc2FjdGlvbhgGIAEoCSIrCglSZWZpbGxBY2sSDwoHc3VjY2Vz",
            "cxgBIAEoCBINCgVlcnJvchgCIAEoCUIkWgd3YXRjaGVyqgIYTWludFNlbmRl",
            "ci5XYXRjaGVyLkV2ZW50YgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::MintSender.Watcher.Event.Refill), global::MintSender.Watcher.Event.Refill.Parser, new[]{ "Service", "PublicKey", "From", "Token", "Amount", "Transaction" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::MintSender.Watcher.Event.RefillAck), global::MintSender.Watcher.Event.RefillAck.Parser, new[]{ "Success", "Error" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  /// <summary>
  /// Refill is an event from the service notifying about a wallet refilling transaction
  /// </summary>
  public sealed partial class Refill : pb::IMessage<Refill> {
    private static readonly pb::MessageParser<Refill> _parser = new pb::MessageParser<Refill>(() => new Refill());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Refill> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::MintSender.Watcher.Event.EventReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Refill() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Refill(Refill other) : this() {
      service_ = other.service_;
      publicKey_ = other.publicKey_;
      from_ = other.from_;
      token_ = other.token_;
      amount_ = other.amount_;
      transaction_ = other.transaction_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Refill Clone() {
      return new Refill(this);
    }

    /// <summary>Field number for the "service" field.</summary>
    public const int ServiceFieldNumber = 1;
    private string service_ = "";
    /// <summary>
    /// Service name (to differentiate multiple requestors): 1..64
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Service {
      get { return service_; }
      set {
        service_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "publicKey" field.</summary>
    public const int PublicKeyFieldNumber = 2;
    private string publicKey_ = "";
    /// <summary>
    /// Destination (watching) wallet address in Base58
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string PublicKey {
      get { return publicKey_; }
      set {
        publicKey_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "from" field.</summary>
    public const int FromFieldNumber = 3;
    private string from_ = "";
    /// <summary>
    /// Source wallet address in Base58
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string From {
      get { return from_; }
      set {
        from_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "token" field.</summary>
    public const int TokenFieldNumber = 4;
    private string token_ = "";
    /// <summary>
    /// GOLD or MNT
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Token {
      get { return token_; }
      set {
        token_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "amount" field.</summary>
    public const int AmountFieldNumber = 5;
    private string amount_ = "";
    /// <summary>
    /// Token amount in major units: 1.234 (18 decimal places)
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Amount {
      get { return amount_; }
      set {
        amount_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "transaction" field.</summary>
    public const int TransactionFieldNumber = 6;
    private string transaction_ = "";
    /// <summary>
    /// Digest of the refilling tx in Base58
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Transaction {
      get { return transaction_; }
      set {
        transaction_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Refill);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Refill other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Service != other.Service) return false;
      if (PublicKey != other.PublicKey) return false;
      if (From != other.From) return false;
      if (Token != other.Token) return false;
      if (Amount != other.Amount) return false;
      if (Transaction != other.Transaction) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Service.Length != 0) hash ^= Service.GetHashCode();
      if (PublicKey.Length != 0) hash ^= PublicKey.GetHashCode();
      if (From.Length != 0) hash ^= From.GetHashCode();
      if (Token.Length != 0) hash ^= Token.GetHashCode();
      if (Amount.Length != 0) hash ^= Amount.GetHashCode();
      if (Transaction.Length != 0) hash ^= Transaction.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Service.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Service);
      }
      if (PublicKey.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(PublicKey);
      }
      if (From.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(From);
      }
      if (Token.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(Token);
      }
      if (Amount.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(Amount);
      }
      if (Transaction.Length != 0) {
        output.WriteRawTag(50);
        output.WriteString(Transaction);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Service.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Service);
      }
      if (PublicKey.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(PublicKey);
      }
      if (From.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(From);
      }
      if (Token.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Token);
      }
      if (Amount.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Amount);
      }
      if (Transaction.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Transaction);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Refill other) {
      if (other == null) {
        return;
      }
      if (other.Service.Length != 0) {
        Service = other.Service;
      }
      if (other.PublicKey.Length != 0) {
        PublicKey = other.PublicKey;
      }
      if (other.From.Length != 0) {
        From = other.From;
      }
      if (other.Token.Length != 0) {
        Token = other.Token;
      }
      if (other.Amount.Length != 0) {
        Amount = other.Amount;
      }
      if (other.Transaction.Length != 0) {
        Transaction = other.Transaction;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Service = input.ReadString();
            break;
          }
          case 18: {
            PublicKey = input.ReadString();
            break;
          }
          case 26: {
            From = input.ReadString();
            break;
          }
          case 34: {
            Token = input.ReadString();
            break;
          }
          case 42: {
            Amount = input.ReadString();
            break;
          }
          case 50: {
            Transaction = input.ReadString();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// RefillAck is a reply for Refill
  /// </summary>
  public sealed partial class RefillAck : pb::IMessage<RefillAck> {
    private static readonly pb::MessageParser<RefillAck> _parser = new pb::MessageParser<RefillAck>(() => new RefillAck());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RefillAck> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::MintSender.Watcher.Event.EventReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RefillAck() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RefillAck(RefillAck other) : this() {
      success_ = other.success_;
      error_ = other.error_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RefillAck Clone() {
      return new RefillAck(this);
    }

    /// <summary>Field number for the "success" field.</summary>
    public const int SuccessFieldNumber = 1;
    private bool success_;
    /// <summary>
    /// Success is true in case of success
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Success {
      get { return success_; }
      set {
        success_ = value;
      }
    }

    /// <summary>Field number for the "error" field.</summary>
    public const int ErrorFieldNumber = 2;
    private string error_ = "";
    /// <summary>
    /// Error contains error descrition in case of failure
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Error {
      get { return error_; }
      set {
        error_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as RefillAck);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(RefillAck other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Success != other.Success) return false;
      if (Error != other.Error) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Success != false) hash ^= Success.GetHashCode();
      if (Error.Length != 0) hash ^= Error.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Success != false) {
        output.WriteRawTag(8);
        output.WriteBool(Success);
      }
      if (Error.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Error);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Success != false) {
        size += 1 + 1;
      }
      if (Error.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Error);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(RefillAck other) {
      if (other == null) {
        return;
      }
      if (other.Success != false) {
        Success = other.Success;
      }
      if (other.Error.Length != 0) {
        Error = other.Error;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Success = input.ReadBool();
            break;
          }
          case 18: {
            Error = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
