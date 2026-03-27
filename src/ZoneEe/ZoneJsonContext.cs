using System.Text.Json.Serialization;
using ZoneEe.Models;

namespace ZoneEe;

[JsonSerializable(typeof(List<DnsRecord>))]
[JsonSerializable(typeof(DnsRecord))]
[JsonSerializable(typeof(DnsRecordCreate))]
[JsonSerializable(typeof(DnsRecordUpdate))]
[JsonSerializable(typeof(List<Domain>))]
[JsonSerializable(typeof(Domain))]
[JsonSerializable(typeof(DomainRenew))]
[JsonSerializable(typeof(List<DomainRenew>))]
[JsonSerializable(typeof(DomainLinks))]
public partial class ZoneJsonContext : JsonSerializerContext;
