using System.Text.Json.Serialization;
using ZoneEe.Models;

namespace ZoneEe.Cli.Infrastructure;

[JsonSerializable(typeof(List<DnsRecord>))]
[JsonSerializable(typeof(DnsRecord))]
[JsonSerializable(typeof(DnsRecordCreate))]
[JsonSerializable(typeof(DnsRecordUpdate))]
[JsonSerializable(typeof(List<Domain>))]
[JsonSerializable(typeof(Domain))]
[JsonSerializable(typeof(DomainDetail))]
[JsonSerializable(typeof(DomainRegister))]
internal partial class AppJsonContext : JsonSerializerContext;
