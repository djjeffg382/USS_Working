// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "This will only be running on windows servers", Scope = "member", Target = "~M:MOO.DAL.Pi.Services.PiSnapshotSvc.Get(System.String)~MOO.DAL.Pi.Models.PiSnapshot")]
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "This will only be running on windows servers", Scope = "member", Target = "~M:MOO.DAL.Util.RegisterOLEDB")]
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "This will only be running on windows servers", Scope = "member", Target = "~M:MOO.DAL.Pi.Services.PiPointClassicSvc.SearchByTag(System.String)~System.Collections.Generic.List{MOO.DAL.Pi.Models.PiPointClassic}")]
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "This will only be running on windows servers", Scope = "member", Target = "~M:MOO.DAL.Pi.Services.PiPointClassicSvc.Insert(MOO.DAL.Pi.Models.PiPointClassic)~System.Int32")]
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>", Scope = "member", Target = "~M:MOO.DAL.Pi.Services.PiPointClassicSvc.Insert(MOO.DAL.Pi.Models.PiPointClassic,System.Data.OleDb.OleDbConnection)~System.Int32")]
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>", Scope = "member", Target = "~M:MOO.DAL.Pi.Services.PiPointClassicSvc.Get(System.String)~MOO.DAL.Pi.Models.PiPointClassic")]
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>", Scope = "member", Target = "~M:MOO.DAL.Pi.Services.PIArchiveSvc.RunPISQLCommand(System.Boolean,System.Data.OleDb.OleDbCommand)")]


[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "<Pending>", Scope = "namespace", Target = "~N:MOO.DAL.ToLive.Models")]
