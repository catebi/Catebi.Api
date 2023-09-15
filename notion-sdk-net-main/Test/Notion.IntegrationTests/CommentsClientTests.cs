﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Notion.Client;
using Xunit;

namespace Notion.IntegrationTests;

public class CommentsClientTests : IntegrationTestBase, IDisposable
{
    private readonly Page _page;

    public CommentsClientTests()
    {
        _page = Client.Pages.CreateAsync(
            PagesCreateParametersBuilder.Create(
                new ParentPageInput { PageId = ParentPageId }
            ).Build()
        ).Result;
    }

    public void Dispose()
    {
        Client.Pages.UpdateAsync(_page.Id, new PagesUpdateParameters { Archived = true }).Wait();
    }

    [Fact]
    public async Task ShouldCreatePageComment()
    {
        // Arrange
        var parameters = CreateCommentParameters.CreatePageComment(
            new ParentPageInput { PageId = _page.Id },
            new List<RichTextBaseInput> { new RichTextTextInput { Text = new Text { Content = "This is a comment" } } }
        );

        // Act
        var response = await Client.Comments.CreateAsync(parameters);

        // Arrange

        Assert.NotNull(response.Parent);
        Assert.NotNull(response.Id);
        Assert.NotNull(response.DiscussionId);

        Assert.NotNull(response.RichText);
        Assert.Single(response.RichText);
        var richText = Assert.IsType<RichTextText>(response.RichText.First());
        Assert.Equal("This is a comment", richText.Text.Content);

        var pageParent = Assert.IsType<PageParent>(response.Parent);
        Assert.Equal(_page.Id, pageParent.PageId);
    }

    [Fact]
    public async Task ShouldCreateADiscussionComment()
    {
        // Arrange
        var comment = await Client.Comments.CreateAsync(
            CreateCommentParameters.CreatePageComment(
                new ParentPageInput { PageId = _page.Id },
                new List<RichTextBaseInput>
                {
                    new RichTextTextInput { Text = new Text { Content = "This is a comment" } }
                }
            )
        );

        // Act
        var response = await Client.Comments.CreateAsync(
            CreateCommentParameters.CreateDiscussionComment(
                comment.DiscussionId,
                new List<RichTextBaseInput>
                {
                    new RichTextTextInput { Text = new Text { Content = "This is a sub comment" } }
                }
            )
        );

        // Arrange
        Assert.Null(response.Parent);
        Assert.NotNull(response.Id);
        Assert.Equal(comment.DiscussionId, response.DiscussionId);

        Assert.NotNull(response.RichText);
        Assert.Single(response.RichText);
        var richText = Assert.IsType<RichTextText>(response.RichText.First());
        Assert.Equal("This is a sub comment", richText.Text.Content);

        var pageParent = Assert.IsType<PageParent>(response.Parent);
        Assert.Equal(_page.Id, pageParent.PageId);
    }
}
