﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Torshify.Radio.Spotify.QueryService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://schemas.torshify/v1", ConfigurationName="QueryService.QueryService")]
    public interface QueryService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.torshify/v1/QueryService/Query", ReplyAction="http://schemas.torshify/v1/QueryService/QueryResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Torshify.Origo.Contracts.V1.NotLoggedInFault), Action="http://schemas.torshify/v1/QueryService/QueryNotLoggedInFaultFault", Name="NotLoggedInFault", Namespace="http://schemas.datacontract.org/2004/07/Torshify.Origo.Contracts.V1")]
        Torshify.Origo.Contracts.V1.Query.QueryResult Query([System.ServiceModel.MessageParameterAttribute(Name="query")] string query1, int trackOffset, int trackCount, int albumOffset, int albumCount, int artistOffset, int artistCount);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.torshify/v1/QueryService/AlbumBrowse", ReplyAction="http://schemas.torshify/v1/QueryService/AlbumBrowseResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Torshify.Origo.Contracts.V1.NotLoggedInFault), Action="http://schemas.torshify/v1/QueryService/AlbumBrowseNotLoggedInFaultFault", Name="NotLoggedInFault", Namespace="http://schemas.datacontract.org/2004/07/Torshify.Origo.Contracts.V1")]
        Torshify.Origo.Contracts.V1.Query.AlbumBrowseResult AlbumBrowse(string albumId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.torshify/v1/QueryService/ArtistBrowse", ReplyAction="http://schemas.torshify/v1/QueryService/ArtistBrowseResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Torshify.Origo.Contracts.V1.NotLoggedInFault), Action="http://schemas.torshify/v1/QueryService/ArtistBrowseNotLoggedInFaultFault", Name="NotLoggedInFault", Namespace="http://schemas.datacontract.org/2004/07/Torshify.Origo.Contracts.V1")]
        Torshify.Origo.Contracts.V1.Query.ArtistBrowseResult ArtistBrowse(string artistId, Torshify.Origo.Contracts.V1.Query.ArtistBrowsingType type);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.torshify/v1/QueryService/GetPlaylist", ReplyAction="http://schemas.torshify/v1/QueryService/GetPlaylistResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Torshify.Origo.Contracts.V1.NotLoggedInFault), Action="http://schemas.torshify/v1/QueryService/GetPlaylistNotLoggedInFaultFault", Name="NotLoggedInFault", Namespace="http://schemas.datacontract.org/2004/07/Torshify.Origo.Contracts.V1")]
        Torshify.Origo.Contracts.V1.Playlist GetPlaylist(string link);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface QueryServiceChannel : Torshify.Radio.Spotify.QueryService.QueryService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class QueryServiceClient : System.ServiceModel.ClientBase<Torshify.Radio.Spotify.QueryService.QueryService>, Torshify.Radio.Spotify.QueryService.QueryService {
        
        public QueryServiceClient() {
        }
        
        public QueryServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public QueryServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public QueryServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public QueryServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Torshify.Origo.Contracts.V1.Query.QueryResult Query(string query1, int trackOffset, int trackCount, int albumOffset, int albumCount, int artistOffset, int artistCount) {
            return base.Channel.Query(query1, trackOffset, trackCount, albumOffset, albumCount, artistOffset, artistCount);
        }
        
        public Torshify.Origo.Contracts.V1.Query.AlbumBrowseResult AlbumBrowse(string albumId) {
            return base.Channel.AlbumBrowse(albumId);
        }
        
        public Torshify.Origo.Contracts.V1.Query.ArtistBrowseResult ArtistBrowse(string artistId, Torshify.Origo.Contracts.V1.Query.ArtistBrowsingType type) {
            return base.Channel.ArtistBrowse(artistId, type);
        }
        
        public Torshify.Origo.Contracts.V1.Playlist GetPlaylist(string link) {
            return base.Channel.GetPlaylist(link);
        }
    }
}
