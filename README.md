Code-Analyzer
=============
Designed and engineered a C# code dependency analyzer that determines the
package dependency among the C# files spanning over multiple clients and servers architecture 
[Visual C#.NET, WCF,WPF, LINQ].
WPF_GUI: Client GUI trying to connect to the server
WPF_GUI_Server2: server GUI
WCF_Peer_Comm: WCF protocol that forms a connection between client and the server
WCF_Peer_Comm1: WCF protocol that forms a connection between client and the another server
Parser: this package finds package dependency and type-analysis(Inheritance, Aggregation etc.,)
Analyzer: Calls on Parser and send the required data


