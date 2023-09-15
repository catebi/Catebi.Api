﻿using System.Diagnostics.CodeAnalysis;

namespace Notion.Client
{
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface INotionClient
    {
        IUsersClient Users { get; }

        IDatabasesClient Databases { get; }

        IPagesClient Pages { get; }

        ISearchClient Search { get; }

        IBlocksClient Blocks { get; }

        ICommentsClient Comments { get; }

        IRestClient RestClient { get; }
    }

    public class NotionClient : INotionClient
    {
        public NotionClient(
            IRestClient restClient,
            IUsersClient users,
            IDatabasesClient databases,
            IPagesClient pages,
            ISearchClient search,
            ICommentsClient comments,
            IBlocksClient blocks)
        {
            RestClient = restClient;
            Users = users;
            Databases = databases;
            Pages = pages;
            Search = search;
            Comments = comments;
            Blocks = blocks;
        }

        public IUsersClient Users { get; }

        public IDatabasesClient Databases { get; }

        public IPagesClient Pages { get; }

        public ISearchClient Search { get; }

        public IBlocksClient Blocks { get; }

        public ICommentsClient Comments { get; }

        public IRestClient RestClient { get; }
    }
}
