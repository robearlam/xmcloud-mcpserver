using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using GraphQL;
using ModelContextProtocol.Server;
using System.ComponentModel;
using XmCloud.McpServer.Models.Sites;

namespace XmCloud.McpServer.Tools.Sites;

[McpServerToolType]
public class SitesTool
{
    private IGraphQLClient client;

    public SitesTool()
    {
        string? contextId = Environment.GetEnvironmentVariable("CONTEXT_ID"); 
        var uri = new Uri($"https://edge-platform.sitecorecloud.io/v1/content/api/graphql/v1?sitecoreContextId={contextId}");
        client = new GraphQLHttpClient(uri, new SystemTextJsonSerializer());
    }

    [McpServerTool, Description("Gets information about all of the sites in a single XM Cloud instance.")]
    public async Task<List<SiteInfo>> GetAllSitesInfo()
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query SiteInfoCollectionQuery {
                    site {
                        siteInfoCollection {
                            name
                            hostname,
                            rootPath,
                            dictionary(language: ""en"", first:100) {
                                total,
                                results {
                                    key,
                                    value
                                }
                                pageInfo {
                                    endCursor,
                                    hasNext
                                }
                            }   
                        }
                    }
                }"
        };

        var response = await client.SendQueryAsync<SitesResponse>(query);
        return response.Data?.Site?.SiteInfoCollection ?? [];
    }

    [McpServerTool, Description("Gets information about a specific site in a single XM Cloud instance.")]
    public async Task<SiteInfo> GetSiteInfo(string siteName)
    {
        var query = new GraphQLRequest
        {
            Query = @"query($siteName: String!) {
                        site {
                            siteInfo(site: $siteName) {
                            name
                            hostname,
                            rootPath,
                            dictionary(language: ""en"", first:100) {
                                total,
                                results {
                                    key,
                                    value
                                }
                                pageInfo {
                                    endCursor,
                                    hasNext
                                }
                            }
                        }
                    }",
            Variables = new { siteName = siteName }
        };

        var response = await client.SendQueryAsync<SiteResponse>(query);
        return response.Data?.Site?.SiteInfo ?? new SiteInfo();
    }    
}